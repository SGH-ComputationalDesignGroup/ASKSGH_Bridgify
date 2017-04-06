using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace sDataObject.sGeometry
{
    public class sVertex : sGeometryBase
    {
        public int ID { get; set; }
        public sXYZ location { get; set; }
        public sXYZ normal { get; set; }
        public sColor color { get; set; }
        public List<int> faceIndices { get; set; }
        public object data { get; set; }

        public sVertex()
        {
            this.faceIndices = new List<int>();
        }

        public sVertex(int id, sXYZ p)
        {
            this.ID = id;
            this.location = p;
            this.faceIndices = new List<int>();
        }

        public sVertex DuplicatesVertex()
        {
            sVertex nv = new sVertex();
            nv.color = this.color.DuplicatesColor();
            nv.ID = this.ID;
            nv.location = this.location.DuplicatesXYZ();
            nv.normal = this.normal.DuplicatesXYZ();
            nv.faceIndices = this.faceIndices.ToList();
            nv.data = this.data;

            return nv;
        }

        public void ComputeNormal(sMesh m)
        {
            sXYZ nv = sXYZ.Zero();
            foreach(int f in this.faceIndices)
            {
                nv += m.faces[f].normal;
            }
            this.normal = nv;
        }
    }
}
