using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sGeometry
{
    public class sPlane : sGeometryBase
    {
        public sXYZ origin { get; set; }
        public sXYZ Xaxis { get; set; }
        public sXYZ Yaxis { get; set; }
        public sXYZ Zaxis { get; set; }

        public sPlane()
        {

        }

        public sPlane(sXYZ ori, sXYZ x, sXYZ y)
        {
            this.origin = ori;
            this.Xaxis = x;
            this.Yaxis = y;
            this.Zaxis = sXYZ.CrossProduct(x, y);
            this.Zaxis *= -1;

            this.Xaxis.Unitize();
            this.Yaxis.Unitize();
            this.Zaxis.Unitize();
        }

        public sPlane DuplicatesPlane()
        {
            sPlane sp = new sPlane(this.origin.DuplicatesXYZ(), this.Xaxis.DuplicatesXYZ(), this.Yaxis.DuplicatesXYZ());
            return sp;
        }
    }
}
