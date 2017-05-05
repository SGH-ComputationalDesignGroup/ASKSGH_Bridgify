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

        public bool Intersect(sCurve c, double tol, out List<sXYZ> intPts, out List<sCurve> intCrvs)
        {
            bool doesIntersect = false;
            List<sXYZ> ips = new List<sXYZ>();
            List<sCurve> ics = new List<sCurve>();

            if(this.curveType == eCurveType.LINE && c.curveType == eCurveType.LINE)
            {
                sLine l1 = this as sLine;
                sLine l2 = c as sLine;
                sGeometryBase igeo;
                doesIntersect = l1.GetIntersection(l2, tol, out igeo);
                if(igeo is sLine)
                {
                    ics.Add(igeo as sCurve);
                }
                else if(igeo is sXYZ)
                {
                    ips.Add(igeo as sXYZ);
                }
            }
            else if (this.curveType == eCurveType.LINE && c.curveType == eCurveType.POLYLINE)
            {
                sPolyLine pl = c as sPolyLine;
                pl.GetIntersection(this, tol, out ips, out ics);
            }
            else if (this.curveType == eCurveType.POLYLINE && c.curveType == eCurveType.LINE)
            {
                sPolyLine pl = this as sPolyLine;
                pl.GetIntersection(c, tol, out ips, out ics);
            }
            else if (this.curveType == eCurveType.POLYLINE && c.curveType == eCurveType.POLYLINE)
            {
                sPolyLine pl = this as sPolyLine;
                pl.GetIntersection(c, tol, out ips, out ics);
            }
            intCrvs = ics;
            intPts = ips;
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
