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
using sDataObject.IElement;
using sDataObject.sSteelElement;

namespace sKodeStructSystem.BeamDesigner
{
    
    public class sKodeSteelBeamDesigner : sKodeBeamDesignerBase, IKodeBeamDesigner
    {
        public sKodeSteelBeamDesigner()
        {

        }

        public List<sCrossSection> Design_Beams_Gravity(IFrameSet fs_Full, IFrameSet fs_Minute, IFrameSet fs_Live, List<sCrossSection> sortedShapes, int count = 3, double depthLimitMax_in = -1.0, double depthLimitMin_in = -1.0)
        {
            List<sCrossSection> selected = new List<sCrossSection>();
            int selectedCount = 0;
            foreach (sCrossSection c in sortedShapes)
            {
                //depth check
                if (depthLimitMax_in > 0.0 && c.depth > depthLimitMax_in) continue;
                if (depthLimitMin_in > 0.0 && c.depth < depthLimitMin_in) continue;

                double momentY_adjusted = this.GetDemand_Flexural_Vertical(c, fs_Full, fs_Minute);
                double momentY_capacity = this.GetFlexuralStrength_Vertical(c, eColorMode.Moment_Y);
                double momentY_DCR = (momentY_adjusted / momentY_capacity);

                //stiffness check
                double def_adjusted = this.GetDemand_LocalDeflection_Vertical(c, fs_Live);
                double def_capacity = (fs_Live.parentCrv.length / 360.0);
                sSteelFrameSet sfs_Live = fs_Live as sSteelFrameSet;
                if (sfs_Live.AsCantilever) def_capacity *= 2;
                def_capacity *= 39.3701;//m to in

                double def_DCR = (def_adjusted/def_capacity);

                //select
                if (momentY_DCR > 0 && momentY_DCR < 0.9999 && def_DCR > 0 && def_DCR < 0.9999)
                {
                    selected.Add(c);
                    selectedCount++;
                }

                if(selectedCount == count)
                {
                    break;
                }
            }
            return selected;
        }

        public double GetFlexuralStrength_Vertical(sCrossSection section, eColorMode forceType)
        {
            sKodeStructConverter kcon = new sKodeStructConverter();
            SteelMaterial mat = kcon.ToKodeStructMaterial_Steel(section);

            string flexuralCompressional = "Top";
            string Code = "AISC360-10";
            bool IsRolledMember = true;
            double phiM_n = 0;

            MomentAxis Axis = kcon.ToKodeStructMomentAxis(forceType);

            FlexuralCompressionFiberPosition FlexuralCompression;
            bool IsValidStringCompressionLoc = Enum.TryParse(flexuralCompressional, true, out FlexuralCompression);
            if (IsValidStringCompressionLoc == false)
            {
                throw new Exception("Flexural compression location selection not recognized. Check input string.");
            }

            ISection shape = kcon.ToKodeStructCrossSection(section);

            FlexuralMemberFactory factory = new FlexuralMemberFactory();
            ISteelBeamFlexure beam = factory.GetBeam(shape, mat, null, Axis, FlexuralCompression, IsRolledMember);

            SteelLimitStateValue Y = beam.GetFlexuralYieldingStrength(FlexuralCompression);
            phiM_n = Y.Value;

            if (Y.IsApplicable)
            {
                return phiM_n;
            }
            else
            {
                return -1;
            }
        }

    }

}
