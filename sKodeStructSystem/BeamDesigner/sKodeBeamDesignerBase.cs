using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sElement;
using sDataObject.IElement;

namespace sKodeStructSystem.BeamDesigner
{
    public class sKodeBeamDesignerBase
    {
        public double GetDemand_Flexural_Vertical(sCrossSection c, IFrameSet fs_Full, IFrameSet fs_Minute)
        {
            //strength check
            double momentY_full = fs_Full.GetFrameSetDemand(eColorMode.Moment_Y);
            double momentY_min = fs_Minute.GetFrameSetDemand(eColorMode.Moment_Y);
            double momentY_self = (momentY_full - momentY_min);

            double selfWeightRatio = (c.weight / fs_Full.crossSection.weight);

            double momentY_adjusted = ((momentY_self * selfWeightRatio) + momentY_min);
            momentY_adjusted *= 0.28476439306;//N.m > kip.in

            return momentY_adjusted;
        }

        public double GetDemand_LocalDeflection_Vertical(sCrossSection c, IFrameSet fs_Live)
        {
            double def_local = fs_Live.GetFrameSetDemand(eColorMode.Deflection_Local);

            double IyyRatio = (fs_Live.crossSection.I_StrongAxis / c.I_StrongAxis);

            double def_adjusted = IyyRatio * def_local;
            def_adjusted *= 0.0393701;//mm to in
            return def_adjusted;
        }

        public double GetDemand_LocalDeflection_Vertical(double adjusted_Iyy, IFrameSet fs_Live)
        {
            double def_local = fs_Live.GetFrameSetDemand(eColorMode.Deflection_Local);

            double IyyRatio = (fs_Live.crossSection.I_StrongAxis / adjusted_Iyy);

            double def_adjusted = IyyRatio * def_local;
            def_adjusted *= 0.0393701;//mm to in
            return def_adjusted;
        }

    }
}
