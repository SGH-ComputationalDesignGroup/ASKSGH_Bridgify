using sDataObject.sGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sElement
{
    public class sPointSupport
    {
        public sXYZ location { get; set; }
        public eSupportType supportType { get; set; }
        public bool[] constraints { get; set; }

        public sXYZ reaction_force { get; set; }
        public sXYZ reaction_moment { get; set; }

        public sPointSupport()
        {
            this.supportType = eSupportType.NONE;
        }

        public sNode TosNode()
        {
            sNode newNode = new sNode();
            newNode.location = this.location;
            newNode.boundaryCondition = this;
            return newNode;
        }

        public sPointSupport DuplicatesPointSupport()
        {
            sPointSupport ns = new sPointSupport();
            ns.location = this.location;
            ns.supportType = this.supportType;
            if(this.constraints != null) ns.constraints = this.constraints.ToArray();
            if(this.reaction_force != null) ns.reaction_force = this.reaction_force;
            if(this.reaction_moment != null) ns.reaction_moment = this.reaction_moment;
            return ns;
        }

    }

    public enum eSupportType
    {
        NONE = 0,
        PINNED = 1,
        FIXED = 2,
        CUSTOM = 3
    }
}
