using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sGeometry
{
    public class sFace : sGeometryBase
    {
        public int ID { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public sXYZ normal { get; set; }
        
        public sFace()
        {

        }

        public sFace DuplicatesFace()
        {
            sFace nf = new sFace();
            nf.ID = this.ID;
            nf.A = this.A;
            nf.B = this.B;
            nf.C = this.C;
            nf.normal = this.normal.DuplicatesXYZ();
            return nf;
        }

        public void ComputeFaceNormal(sMesh m)
        {
            sLine l0 = new sLine(m.vertices[this.A].location , m.vertices[this.B].location);
            sLine l1 = new sLine(m.vertices[this.A].location, m.vertices[this.C].location);

            this.normal = sXYZ.CrossProduct(l0.direction, l1.direction);
        }
    }
}
