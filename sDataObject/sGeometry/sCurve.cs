using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sGeometry
{
    public class sCurve : sGeometryBase
    {
        public double length { get; set; }
        public bool isClosed { get; set; }
        public eCurveType curveType { get; set; }

        public sCurve DuplicatesCurve()
        {
            if(this.curveType == eCurveType.LINE)
            {
                sLine sc = this as sLine;
                return sc.DuplicatesLine();
            }
            else if(this.curveType == eCurveType.POLYLINE)
            {
                sPolyLine thispl = this as sPolyLine;
                return thispl.DuplicatesPolyline();
            }
            else if(this.curveType == eCurveType.NURBSCURVE)
            {
                sNurbsCurve nc = this as sNurbsCurve;
                return nc.DuplicatesNurbsCurve();
            }
            else
            {
                return null;
            }
        }

        public bool Intersect(sCurve c, double tol, out sGeometryBase intGeo)
        {
            bool doesIntersect = false;
            sGeometryBase intGeometry = null;

            if(this.curveType == eCurveType.LINE && c.curveType == eCurveType.LINE)
            {
                sLine l1 = this as sLine;
                sLine l2 = c as sLine;
                doesIntersect = l1.GetIntersection(l2, tol, out intGeometry);
            }
            else if (this.curveType == eCurveType.LINE && c.curveType == eCurveType.POLYLINE)
            {

            }
            else if (this.curveType == eCurveType.POLYLINE && c.curveType == eCurveType.LINE)
            {

            }
            else if (this.curveType == eCurveType.POLYLINE && c.curveType == eCurveType.POLYLINE)
            {

            }

            intGeo = intGeometry;
            return doesIntersect;
        }
    }

    public enum eCurveType
    {
        LINE,
        POLYLINE,
        NURBSCURVE
    }
}
