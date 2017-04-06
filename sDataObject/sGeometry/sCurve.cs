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
    }

    public enum eCurveType
    {
        LINE,
        POLYLINE,
        NURBSCURVE
    }
}
