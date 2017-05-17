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

using sKodeStructSystem.BeamDesigner;
using sDataObject.sSteelElement;
using sDataObject.IElement;

namespace sKodeStructSystem.BuildingDesigner
{

    public class sKodeSteelBuildingDesigner : IKodeBuildingDesigner
    {
        public sKodeSteelBuildingDesigner()
        {

        }

        public void DesignSteelBeams_Gravity(bool isComposite ,eSteelFrameSetType beamType, ISystem sysLRFD_Full, ref ISystem sysLRFD_BeamMinute, ISystem sysLIVE, out double maxDepth_Designed, int count = 3, double depthLimitMax_in = -1.0, double depthLimitMin_in = -1.0)
        {
            List<sCrossSection> wShapes = sCrossSection.GetAllWShapes().OrderBy(c => c.weight).ThenBy(c => c.depth).ToList();

            IKodeBeamDesigner beamDesign = null;
            if (isComposite)
            {
                beamDesign = new sKodeSteelCompositeBeamDesigner();
            }
            else
            {
                beamDesign = new sKodeSteelBeamDesigner();
            }
            try
            {
                var beamDesignGroup = sysLRFD_BeamMinute.frameSets.Cast<sSteelFrameSet>().Where(f => f.frameStructureType == beamType).GroupBy(f => f.GetFrameSetDemand(eColorMode.Moment_Y, 0));
                //
                //this should be per bay???????????????????
                double maxDepth = 0.0;
                //
                //
                foreach (var bg in beamDesignGroup)
                {
                    sSteelFrameSet f_LRFD_beamMinute = bg.ElementAt(0);
                    sSteelFrameSet f_LRFD_Full = sysLRFD_Full.GetsFrameSetByGUID(f_LRFD_beamMinute.objectGUID) as sSteelFrameSet;
                    sSteelFrameSet f_Live = sysLIVE.GetsFrameSetByGUID(f_LRFD_beamMinute.objectGUID) as sSteelFrameSet;

                    List<sCrossSection> designedSections = beamDesign.Design_Beams_Gravity(f_LRFD_Full, f_LRFD_beamMinute, f_Live, wShapes, count, depthLimitMax_in, depthLimitMin_in);
                    if (designedSections != null && designedSections.Count > 0)
                    {
                        foreach (sSteelFrameSet b in bg)
                        {
                            b.designedCrossSections = new List<sCrossSection>();
                            foreach (sCrossSection cs in designedSections)
                            {
                                b.designedCrossSections.Add(cs.DuplicatesCrosssection());
                            }
                        }
                        foreach (sCrossSection cs in designedSections)
                        {
                            if (cs.depth > maxDepth) maxDepth = cs.depth;
                        }
                    }
                }

                maxDepth_Designed = maxDepth;
            }
            catch
            {
                throw new NotImplementedException("Use Steel System");
            }
        }
        
        public void DesignSteelColumns_Gravity()
        {
            //return column shapes only!! add function
            List<sCrossSection> wShapes = sCrossSection.GetAllWShapes().OrderBy(c => c.weight).ThenBy(c => c.depth).ToList();


        }
        
    }

    
}
