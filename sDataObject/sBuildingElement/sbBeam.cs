using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sElement;
using sDataObject.sGeometry;

namespace sDataObject.sBuildingElement
{
    public class sbBeam : sBeamSet
    {
        public Guid sbBeamID { get; set; }
        
        public bool asGirder { get; set; }
        public bool atPerimeter { get; set; }

        public sbBeam()
        {
            this.sbBeamID = Guid.NewGuid();
        }

        public enum eStructureType
        {
            STEEL_SYSTEM,
            COMPOSITE_SYSTEM,
            CONCRETE_SYSTEM,
            WOOD_SYSTEM
        }
    }
}
