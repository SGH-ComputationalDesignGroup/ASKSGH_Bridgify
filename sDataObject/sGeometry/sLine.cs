using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sGeometry
{
    public class sLine : sCurve
    {
        public sXYZ startPoint { get; set; }
        public sXYZ endPoint { get; set; }
        public sXYZ direction { get; set; }

        public sLine()
        {

        }

        public sLine(sXYZ sp, sXYZ ep)
        {
            this.startPoint = sp;
            this.endPoint = ep;
            this.length = sp.DistanceTo(ep);
            this.direction = ep - sp;
            this.direction.Unitize();

            this.curveType = eCurveType.LINE;
        }

        public sLine DuplicatesLine()
        {
            sLine nl = new sLine(this.startPoint.DuplicatesXYZ(), this.endPoint.DuplicatesXYZ());
            return nl;
        }

        public sXYZ PointAt(double normalizedParam)
        {
            sXYZ dir = this.direction * (this.length * normalizedParam);
            return this.startPoint + dir;
        }

        public sXYZ PointAtLength(double lengthParam)
        {
            sXYZ dir = this.direction * lengthParam;
            return dir;
        }

        public bool GetIntersection(sLine ln, double tolerance, out sGeometryBase intGeo)
        {
            double t0;
            double t1;
            double dis = this.GetClosestDistanceBetween(ln, out t0, out t1);

            if(dis < tolerance)
            {
                sXYZ ip0 = this.PointAt(t0);
                sXYZ ip1 = this.PointAt(t1);
                if(ip0.DistanceTo(ip1) < 0.001)
                {
                    intGeo = this.PointAt(t0);
                    return true;
                }
                else
                {
                    intGeo = new sLine(ip0, ip1);
                    return true;
                }
            }
            else
            {
                intGeo = null;
                return false;
            }
        }

        public double GetClosestDistanceTo(sXYZ p, out double t)
        {
            sXYZ minuteVec = new sXYZ(0.00001, 0,0);
            sLine ln = new sLine(p, p + minuteVec);

            double t0;
            double t1;
            double dis = this.GetClosestDistanceBetween(ln, out t0, out t1);

            t = t0;
            return dis;
        }

        public double GetClosestDistanceBetween(sLine ln, out double t0, out double t1)
        {
            sXYZ u = this.endPoint - this.startPoint;
            sXYZ v = ln.endPoint - ln.startPoint;
            sXYZ w = this.startPoint - ln.startPoint;

            double a = u * u;// sXYZ.Dot(u, u);
            double b = u * v;// sXYZ.Dot(u, v);
            double c = v * v;// sXYZ.Dot(v, v);
            double d = u * w;// sXYZ.Dot(u, w);
            double e = v * w;// sXYZ.Dot(v, w);

            double D = (a * c) - (b * b);
            double sc = 0.0;
            double sN = 0.0;
            double sD = D;
            double tc = 0.0;
            double tN = 0.0;
            double tD = D;


            if (D < 0.00001)
            {
                //parallel
                sN = 0.0;         
                sD = 1.0;         
                tN = e;                                                                          
                tD = c;
            }
            else
            {
                sN = (b * e - c * d);
                tN = (a * e - b * d);
                if (sN < 0.0)
                {       
                    sN = 0.0;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {  
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0.0)
            {       
                tN = 0.0;
                if (-d < 0.0)
                    sN = 0.0;
                else if (-d > a)
                    sN = sD;
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {
                tN = tD;
                if ((-d + b) < 0.0)
                    sN = 0;
                else if ((-d + b) > a)
                    sN = sD;
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }

            if (Math.Abs(sN) < 0.0001)
            {
                sc = 0.0;
            }
            else
            {
                sc = (sN / sD);
            }
            if (Math.Abs(tN) < 0.0001)
            {
                tc = 0.0;
            }
            else
            {
                tc = (tN / tD);
            }


            sXYZ cp = w + (sc * u) - (tc * v);

            t0 = sc;
            t1 = tc;
            return cp.GetLength();
        }
    }
}
