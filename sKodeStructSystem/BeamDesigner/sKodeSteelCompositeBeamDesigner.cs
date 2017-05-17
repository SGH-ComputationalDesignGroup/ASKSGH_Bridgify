using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sElement;
using sDataObject.sGeometry;

using Kodestruct.Common;
using Kodestruct.Steel;
using Kodestruct.Common.Entities;
using Kodestruct.Common.Section.Interfaces;
using Kodestruct.Steel.AISC.SteelEntities.Materials;
using Kodestruct.Steel.AISC.AISC360v10.Flexure;
using Kodestruct.Steel.AISC.Interfaces;
using Kodestruct.Steel.AISC.Steel.Entities;
using Kodestruct.Steel.AISC.AISC360v10.Composite;
using sDataObject.IElement;
using sDataObject.sSteelElement;

namespace sKodeStructSystem.BeamDesigner
{
    
    public class sKodeSteelCompositeBeamDesigner : sKodeBeamDesignerBase, IKodeBeamDesigner
    {
        public sKodeSteelCompositeBeamDesigner()
        {

        }

        public List<sCrossSection> Design_Beams_Gravity(IFrameSet fs_Full, IFrameSet fs_Minute, IFrameSet fs_Live, List<sCrossSection> sortedShapes, int count = 3, double depthLimitMax_in = -1, double depthLimitMin_in = -1)
        {
            List<sCrossSection> selected = new List<sCrossSection>();
            int selectedCount = 0;

            foreach(sCrossSection c in sortedShapes)
            {
                //depth check
                if (depthLimitMax_in > 0.0 && c.depth > depthLimitMax_in) continue;
                if (depthLimitMin_in > 0.0 && c.depth < depthLimitMin_in) continue;

                //strength check
                double momentY_adjusted = this.GetDemand_Flexural_Vertical(c, fs_Full, fs_Minute);
                //?? put minute frame???
                double momentY_capacity = this.GetFlexuralStrength_Vertical(c, eColorMode.Moment_Y, fs_Minute);
                double momentY_DCR = (momentY_adjusted / momentY_capacity);

                //stiffness check
                //units??
                double I_LB = this.GetLowerBoundMomentOfInertia(c, fs_Live);
                double def_adjusted = this.GetDemand_LocalDeflection_Vertical(I_LB, fs_Live);
                double def_capacity = (fs_Live.parentCrv.length / 360.0);

                sSteelFrameSet sfs_Live = fs_Live as sSteelFrameSet;
                if (sfs_Live.AsCantilever) def_capacity *= 2;
                def_capacity *= 39.3701;//m to in
                double def_DCR = (def_adjusted / def_capacity);

                //select
                if (momentY_DCR > 0 && momentY_DCR < 0.9999 && def_DCR > 0 && def_DCR < 0.9999)
                {
                    selected.Add(c);
                    selectedCount++;
                }

                if (selectedCount == count)
                {
                    break;
                }
            }
            return selected;
        }
        
        public double GetLowerBoundMomentOfInertia(sCrossSection cCheck, IFrameSet fs)
        {
            sKodeStructConverter kcon = new sKodeStructConverter();
            SteelMaterial mat = kcon.ToKodeStructMaterial_Steel(fs.crossSection);
            double I_LB = 0;
            double b_eff = this.GetBeamEffectiveSlabWidth(fs);
            double SumQ_n = this.GetSumOfStudsStrength(fs);
            // assuming, 
            // Shear Stud Anchor
            // Light Weight Concrete : 4ksi
            // solid concrete thickness = 2.5"
            // rib thickness = 3"
            // 
            double h_solid = 2.5;
            double h_rib = 3.0;
            double F_y = mat.YieldStress; //?? unit ?? F_y of what??
            double fc_prime = 4.0;//?? unit ??

            //check lowerBound Moment of Inertia by check section???
            ISection shape = kcon.ToKodeStructCrossSection(cCheck);

            if (shape is ISliceableShapeProvider)
            {
                ISliceableShapeProvider prov = shape as ISliceableShapeProvider;
                ISliceableSection sec = prov.GetSliceableShape();
                CompositeBeamSection cs = new CompositeBeamSection(sec, b_eff, h_solid, h_rib, F_y, fc_prime);
                I_LB = cs.GetLowerBoundMomentOfInertia(SumQ_n);
            }
            else
            {
                if (shape is ISliceableSection)
                {
                    ISliceableSection sec = shape as ISliceableSection;
                    CompositeBeamSection cs = new CompositeBeamSection(sec, b_eff, h_solid, h_rib, F_y, fc_prime);
                    I_LB = cs.GetLowerBoundMomentOfInertia(SumQ_n);
                }
                else
                {
                    throw new Exception("Shape type not supported. Please provide a shape object of standard geometry");
                }

            }



            return I_LB;
        }

        public double GetFlexuralStrength_Vertical(sCrossSection section, eColorMode forceType, IFrameSet fs)
        {
            sKodeStructConverter kcon = new sKodeStructConverter();
            SteelMaterial mat = kcon.ToKodeStructMaterial_Steel(section);
            
            string Code = "AISC360-10";
            double phiM_n = 0;

            double b_eff = this.GetBeamEffectiveSlabWidth(fs);
            double SumQ_n = this.GetSumOfStudsStrength(fs);
            // assuming, 
            // Shear Stud Anchor
            // Light Weight Concrete : 4ksi
            // solid concrete thickness = 2.5"
            // rib thickness = 3"
            // 
            double h_solid = 2.5;
            double h_rib = 3.0;
            double F_y = mat.YieldStress; //?? unit ?? F_y of what??
            double fc_prime = 4.0;//?? unit ??
            
            MomentAxis Axis = kcon.ToKodeStructMomentAxis(forceType);

            //?? just this for composite?
            ISection shape = kcon.ToKodeStructCrossSection(section);

            if (shape is ISliceableShapeProvider)
            {
                ISliceableShapeProvider prov = shape as ISliceableShapeProvider;
                ISliceableSection sec = prov.GetSliceableShape();
                CompositeBeamSection cs = new CompositeBeamSection(sec, b_eff, h_solid, h_rib, F_y, fc_prime);
                phiM_n = cs.GetFlexuralStrength(SumQ_n);
            }
            else
            {
                if (shape is ISliceableSection)
                {
                    ISliceableSection sec = shape as ISliceableSection;
                    CompositeBeamSection cs = new CompositeBeamSection(sec, b_eff, h_solid, h_rib, F_y, fc_prime);
                    phiM_n = cs.GetFlexuralStrength(SumQ_n);
                }
                else
                {
                    throw new Exception("Shape type not supported. Please provide a shape object of standard geometry");
                }
            }
            return phiM_n;
        }

        public double GetSumOfStudsStrength(IFrameSet fs)
        {
            double len_ft = fs.parentCrv.length * 3.280841666667;
            int studCount = (int)Math.Round((len_ft / 1) , 0);
            // assuming, 
            // Shear Stud Anchor
            // Light Weight Concrete : 4ksi
            // Deck Perpendicular
            // 1 Weak Studs per rib
            // 3/4" in diameter
            return studCount * 17.2;
        }

        public double GetBeamEffectiveSlabWidth(IFrameSet ifs)
        {
            sSteelFrameSet fs = ifs as sSteelFrameSet;
            CompositeBeamSection cs = new CompositeBeamSection();
            double L = fs.parentCrv.length * 39.3701;//m to in
            return  cs.GetEffectiveSlabWidth(L, fs.effectiveSlabEdges.L_centerLeft_in, fs.effectiveSlabEdges.L_centerRight_in, fs.effectiveSlabEdges.L_edgeLeft_in, fs.effectiveSlabEdges.L_edgeRight_in);
        }
    }
    
}
