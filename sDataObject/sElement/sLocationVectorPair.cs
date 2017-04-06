using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sGeometry;

namespace sDataObject.sElement
{
    public class sLocationVectorPair
    {
        public sXYZ location { get; set; }
        public sXYZ vector { get; set; }


        public sLocationVectorPair()
        {

        }

        public sLocationVectorPair(sXYZ loc, sXYZ vec)
        {
            this.location = loc;
            this.vector = vec;
        }
    }
}
