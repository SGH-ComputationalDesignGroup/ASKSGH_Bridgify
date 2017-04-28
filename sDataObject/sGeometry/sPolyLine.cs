using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sGeometry
{
    public class sPolyLine : sCurve
    {
        public List<sXYZ> vertice { get; set; }
        public List<sLine> segments { get; set; }

        public sPolyLine()
        {
            
        }
        
        public sPolyLine(List<sXYZ> verticeIn, bool close)
        {
            this.isClosed = close;

            this.vertice = new List<sXYZ>();
            this.vertice = verticeIn.ToList();

            this.segments = new List<sLine>();
            for(int i = 0; i < this.vertice.Count-1; ++i)
            {
                this.segments.Add(new sLine(this.vertice[i], this.vertice[i+1]));
            }
            if (this.isClosed)
            {
                this.segments.Add(new sLine(this.vertice[this.vertice.Count-1], this.vertice[0]));
            }

            foreach(sLine seg in this.segments)
            {
                this.length += seg.length;
            }
            this.curveType = eCurveType.POLYLINE;
        }

        public sPolyLine DuplicatesPolyline()
        {
            List<sXYZ> vts = new List<sXYZ>();
            foreach(sXYZ v in this.vertice)
            {
                vts.Add(v.DuplicatesXYZ());
            }
            return new sPolyLine(vts, this.isClosed);
        }

        public bool GetIntersection(sCurve pl, double tolerance, out List<sXYZ> intPoints, out List<sCurve> intCurves)
        {
            bool doesIntersect = false;
            List<sXYZ> intPts = new List<sXYZ>();
            List<sCurve> intCrvs = new List<sCurve>();
            if (pl.curveType == eCurveType.LINE)
            {
                for(int i = 0; i < this.segments.Count; ++i)
                {
                    sGeometryBase igeo;
                    if(this.segments[i].GetIntersection(pl as sLine, tolerance, out igeo))
                    {
                        if(igeo is sLine)
                        {
                            intCrvs.Add(igeo as sCurve);
                        }
                        if(igeo is sXYZ)
                        {
                            intPts.Add(igeo as sXYZ);
                        }
                    }
                }
            }
            else if(pl.curveType == eCurveType.POLYLINE)
            {

            }
            else if(pl.curveType == eCurveType.NURBSCURVE)
            {

            }

            //cull if intpoint is on intcurve?

            intPoints = intPts;
            intCurves = intCrvs;
            return doesIntersect;
        }
    }
}
