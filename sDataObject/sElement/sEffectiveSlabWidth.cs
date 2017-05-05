using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sElement
{
    public class sEffectiveSlabWidth
    {
        public double L_centerLeft_in { get; set; }
        public double L_centerRight_in { get; set; }
        public double L_edgeLeft_in { get; set; }
        public double L_edgeRight_in { get; set; }

        public sEffectiveSlabWidth()
        {
            this.L_edgeLeft_in = -1;
            this.L_edgeRight_in = -1;
        }

        public sEffectiveSlabWidth DuplicatesEffectiveSlabWidth()
        {
            sEffectiveSlabWidth ne = new sEffectiveSlabWidth();
            ne.L_centerLeft_in = this.L_centerLeft_in;
            ne.L_centerRight_in = this.L_centerRight_in;
            ne.L_edgeLeft_in = this.L_edgeLeft_in;
            ne.L_edgeRight_in = this.L_edgeRight_in;
            return ne;
        }
    }
}
