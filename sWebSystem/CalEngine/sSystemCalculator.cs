using sDataObject.IElement;
using sDataObject.sElement;
using sDataObject.sSteelElement;
using sKodeStructSystem.BuildingDesigner;
using sNStatSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sWebSystem.CalEngine
{
    public class sSystemCalculator
    {
        public ISystem CalculateSystem(ISystem sys)
        {
            return null;
        }

        public ISystem CalculatesSystem(ISystem sys, string caseName = "")
        {
            string caseToCal = caseName;
            if(caseToCal.Length == 0)
            {
                caseToCal = sys.systemSettings.currentCase;
            }
            return sStatSystem.GetCalculatedSystem(sys, caseToCal);
        }

        public ISystem CalculateSteelSystem_Test(ISystem sys, bool asComposite)
        {
            sKodeSteelBuildingDesigner kde = new sKodeSteelBuildingDesigner();

            ISystem sys_LRFD_Full = sStatSystem.GetCalculatedSystem(sys, "1.2D+1.6L");

            sys.ToggleMinuteDensityStatus(eSteelFrameSetType.AsBeam, true);
            ISystem sys_LRFD_BeamMin = sStatSystem.GetCalculatedSystem(sys, "1.2D+1.6L");

            sys.ToggleMinuteDensityStatus(eSteelFrameSetType.AsBeam, false);
            ISystem sys_Live = sStatSystem.GetCalculatedSystem(sys, "L");

            double maxDepth_Beams;
            kde.DesignSteelBeams_Gravity(asComposite, eSteelFrameSetType.AsBeam, sys_LRFD_Full, ref sys_LRFD_BeamMin, sys_Live, out maxDepth_Beams, 2);

            //Apply CrossSection doesn't work...
            sys_LRFD_BeamMin.ApplyDesignedCrossSections(eSteelFrameSetType.AsBeam, 0);
            sys_LRFD_BeamMin.ToggleMinuteDensityStatus(eSteelFrameSetType.AsBeam, false);

            sys = sys_LRFD_BeamMin;

            //recal for girder
            sys_LRFD_Full = sStatSystem.GetCalculatedSystem(sys, "1.2D+1.6L");

            sys.ToggleMinuteDensityStatus(eSteelFrameSetType.AsGirder, true);
            sys_LRFD_BeamMin = sStatSystem.GetCalculatedSystem(sys, "1.2D+1.6L");

            sys.ToggleMinuteDensityStatus(eSteelFrameSetType.AsGirder, false);
            sys_Live = sStatSystem.GetCalculatedSystem(sys, "L");

            //girderDesign
            double maxDepth_Girder;
            kde.DesignSteelBeams_Gravity(asComposite, eSteelFrameSetType.AsGirder, sys_LRFD_Full, ref sys_LRFD_BeamMin, sys_Live, out maxDepth_Girder, 2, -1.0, maxDepth_Beams * 1.01);

            //update designed shapes & reset systems
            sys_LRFD_BeamMin.ApplyDesignedCrossSections(eSteelFrameSetType.AsGirder, 0);
            sys_LRFD_BeamMin.ToggleMinuteDensityStatus(eSteelFrameSetType.AsGirder, false);

            return sStatSystem.GetCalculatedSystem(sys_LRFD_BeamMin, "1.2D+1.6L");
        }
    }
}