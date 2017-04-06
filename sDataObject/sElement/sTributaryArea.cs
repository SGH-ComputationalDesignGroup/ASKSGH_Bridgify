using sDataObject.sGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sElement
{
    public class sTributaryArea
    {
        public double area { get; set; }
        public sCurve areaBoundary { get; set; }

        public sTributaryArea()
        {

        }
       
        public sTributaryArea DuplicatesTributaryArea()
        {
            sTributaryArea newta = new sTributaryArea();
            newta.area = this.area;
            newta.areaBoundary = this.areaBoundary.DuplicatesCurve();//????
            return newta;
        }
    }

}
