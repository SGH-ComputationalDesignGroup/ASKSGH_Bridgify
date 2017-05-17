using sDataObject.sElement;
using sDataObject.ElementBase;
using sDataObject.sGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.IElement
{
    public interface IFrameSet : IsObject
    {
        string frameSetName { get; set; }
        int setId { get; set; }
        sCurve parentCrv { get; set; }
        List<sCurve> parentSegments { get; set; }
        bool AsMinuteDensity { get; set; }
        sCrossSection crossSection { get; set; }
        List<sCrossSection> designedCrossSections { get; set; }
        List<sLineLoad> lineLoads { get; set; }

        sFixity parentFixityAtStart { get; set; }
        List<sFixity> segmentFixitiesAtStart { get; set; }
        sFixity parentFixityAtEnd { get; set; }
        List<sFixity> segmentFixitiesAtEnd { get; set; }
        List<sXYZ> associatedLocations { get; set; }

        List<sFrame> frames { get; set; }
        sResultRange results_Max { get; set; }

        IFrameSet DuplicatesFrameSet();

        double GetFrameSetDemand(eColorMode forceType, int round = -2);
        sRange GetFrameSetResultRange(eColorMode colMode);
        void UpdateLineLoad(sLineLoad lload);
        void UpdatesFrameCrossSections();
        void AddBeamElement(sLine sln, sXYZ upvec, int id);
        void EnsureBeamElement();
        void ResetFixities();
        void AwareElementsFixitiesByParentFixity(double tol);
        void AwareElementFixitiesBySegementFixities(double tol);


    }
}
