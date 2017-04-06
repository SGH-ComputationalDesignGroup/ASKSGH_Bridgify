using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sGeometry
{
    public class sNurbsCurve : sCurve
    {
        public List<sXYZ> controlPoints { get; set; }
        public List<double> weights { get; set; }
        public List<double> knots { get; set; }
        public int degree = 3;

        public sNurbsCurve()
        {

        }

        public sNurbsCurve(List<sXYZ> cPts, List<double> wes, List<double> ks, double len, int deg)
        {
            this.controlPoints = cPts.ToList();
            this.weights = wes.ToList();
            this.knots = ks.ToList();

            this.length = len;
            this.degree = deg;

            this.curveType = eCurveType.NURBSCURVE;
        }

        public sNurbsCurve DuplicatesNurbsCurve()
        {
            List<sXYZ> cpts = new List<sXYZ>();
            foreach(sXYZ cp in this.controlPoints)
            {
                cpts.Add(cp.DuplicatesXYZ());
            }
            return new sNurbsCurve(cpts, this.weights.ToList(), this.knots.ToList(), this.length, this.degree);
        }
    }
}
