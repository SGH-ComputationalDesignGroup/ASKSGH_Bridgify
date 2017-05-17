
using sDataObject.ElementBase;
using sDataObject.sElement;
using sDataObject.sGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.IElement
{
    public interface ISystem : IsObject
    {
        List<sNode> nodes { get; set; }
        List<IFrameSet> frameSets { get; set; }
        List<sMesh> meshes { get; set; }
        List<string> loadPatterns { get; set; }
        List<sLoadCombination> loadCombinations { get; set; }
        double estimatedWeight { get; set; }
        double estimatedMaxD { get; set; }
        sSystemSetting systemSettings { get; set; }
        sResultRange systemResults { get; set; }

        //interface functions
        void AwaresSystemResult();
        IFrameSet GetsFrameSetByGUID(Guid gid);
        int ApplyDesignedCrossSections(int index = 0);
        void AddsBeamSet(IFrameSet bset);
        void ResetBeamsInBeamSet();
        bool UpdateNodeFromPointElement(sPointSupport ps, int id);
        bool UpdateNodeFromPointElement(sPointLoad pl, int id);
        bool AwareExistingNode(sXYZ location, out sNode existingNode);
        void ConstructBeamResultMesh(eColorMode colorMode, ref List<sMesh> meshes, out sRange dataRange, sRange threshold = null, double du = 0.0);
        sRange GetSystemBeamResultRange(eColorMode colorMode);
        void SetLoadCombination(sLoadCombination com);
        void AwarePatternNames();
        void AwarePatternNames(string patt);
        void TransfersSystemBasis(ISystem ssys);

        //need to test these.... json & objectify
        //should these be abstract???????????????
        //ISystem Objectify(string jsonFile, bool isForWeb = false);
        string Jsonify(bool isForWeb = false);

        //abstract functions
        void TransfersSystemIFrameSetElements(ISystem ssys);
        ISystem DuplicatesSystem();
        void DuplicateFromsSystem(ISystem ssys);

        void ToggleMinuteDensityStatus(object frameSetFilter, bool toggle);
        int ApplyDesignedCrossSections(object frameSetFilter, int index = 0);
    }

    public enum eColorMode
    {
        Stress_Combined_Absolute = 0,
        Stress_Moment_X = 1,
        Stress_Moment_Y = 2,
        Stress_Moment_Z = 3,
        Stress_Axial_X = 4,
        Stress_Axial_Y = 5,
        Stress_Axial_Z = 6,
        Moment_X = 7,
        Moment_Y = 8,
        Moment_Z = 9,
        Force_X = 10,
        Force_Y = 11,
        Force_Z = 12,
        Deflection = 13,
        Deflection_Local = 15,
        NONE = 14

    }
}
