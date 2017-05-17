using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sGeometry;
using Newtonsoft.Json;

namespace sDataObject.sElement
{
    public class sSystemSetting
    {
        public string systemName { get; set; }
        public string currentCase { get; set; }
        public eSystemCheckType currentCheckType { get; set; }
        public eASKSGHType asksghType { get; set; }
        public double currentStressThreshold_pascal { get; set; }
        public double currentDeflectionThreshold_mm { get; set; }
        public string systemConnectionID { get; set; }

        public string systemOriUnit { get; set; }
        public double mergeTolerance_m { get; set; }
        public double meshDensity_m { get; set; }
        public sBoundingBox systemBoundingBox { get; set; }

        public sSystemSetting()
        {

        }

        public sSystemSetting DuplicatesSystemSetting()
        {
            sSystemSetting ns = new sSystemSetting();
            ns.systemName = this.systemName;
            ns.currentCase = this.currentCase;
            ns.currentCheckType = this.currentCheckType;
            ns.asksghType = this.asksghType;
            ns.currentDeflectionThreshold_mm = this.currentDeflectionThreshold_mm;
            ns.currentStressThreshold_pascal = this.currentStressThreshold_pascal;
            if(this.systemConnectionID != null) ns.systemConnectionID = this.systemConnectionID;

            ns.systemOriUnit = this.systemOriUnit;
            ns.mergeTolerance_m = this.mergeTolerance_m;
            ns.meshDensity_m = this.meshDensity_m;
            if(this.systemBoundingBox != null) ns.systemBoundingBox = this.systemBoundingBox.DuplicatesBoundingBox();
            return ns;
        }

    }

    public enum eSystemCheckType
    {
        StrengthCheck = 0,
        StiffnessCheck = 13
    }

    public enum eASKSGHType
    {
        ASKSGH_Colorify_Dyanmic=0,
        ASKSGH_Colorify_Static=1,
        ASKSGH_Webify=2,
        ASKSGH_Gridify=3
    }


}
