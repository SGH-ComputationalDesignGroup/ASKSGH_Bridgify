using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sGeometry
{
    public class sXYZ : sGeometryBase
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public sXYZ()
        {

        }

        public sXYZ(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public sXYZ DuplicatesXYZ()
        {
            return new sXYZ(this.X, this.Y, this.Z);
        }

        public static sXYZ Zero()
        {
            return new sXYZ(0, 0, 0);
        }

        public static sXYZ Xaxis()
        {
            return new sXYZ(1, 0, 0);
        }

        public static sXYZ Yaxis()
        {
            return new sXYZ(0, 1, 0);
        }

        public static sXYZ Zaxis()
        {
            return new sXYZ(0, 0, 1);
        }

        public static sXYZ operator +(sXYZ v0, sXYZ v1)
        {
            return new sXYZ(v0.X + v1.X, v0.Y + v1.Y, v0.Z + v1.Z);
        }

        public static sXYZ operator -(sXYZ v0, sXYZ v1)
        {
            return new sXYZ(v0.X - v1.X, v0.Y - v1.Y, v0.Z - v1.Z);
        }

        public static sXYZ operator * (double scale, sXYZ v1)
        {
            return new sXYZ(v1.X * scale, v1.Y * scale, v1.Z * scale);
        }

        public static sXYZ operator *(sXYZ v1, double scale)
        {
            return new sXYZ(v1.X * scale, v1.Y * scale, v1.Z * scale);
        }

        public static double operator *(sXYZ v0, sXYZ v1)
        {
            return (v0.X * v1.X) + (v0.Y * v1.Y) + (v0.Z * v1.Z);
        }

        public static sXYZ operator /(sXYZ v1, double scale)
        {
            return new sXYZ(v1.X / scale, v1.Y / scale, v1.Z / scale);
        }

        public static sXYZ operator /(double scale, sXYZ v1)
        {
            return new sXYZ(v1.X / scale, v1.Y / scale, v1.Z / scale);
        }

        public static sXYZ CrossProduct(sXYZ v1, sXYZ v2)
        {
            v1.Unitize();
            v2.Unitize();

            double x, y, z;
            x = v1.Y * v2.Z - v2.Y * v1.Z;
            y = (v1.X * v2.Z - v2.X * v1.Z) * -1;
            z = v1.X * v2.Y - v2.X * v1.Y;

            var rtnvector = new sXYZ(x, y, z);
            rtnvector.Unitize(); 
            return rtnvector;
        }

        public sXYZ ProjectTo(sXYZ to)
        {
            return (this * to) * (to * to) * to;
        }

        public sXYZ RejectTo(sXYZ to)
        {
            return this - ProjectTo(to);
        }
        
        public double DistanceTo(sXYZ vec)
        {
            double xx = (double)(this.X - vec.X);
            double yy = (double)(this.Y - vec.Y);
            double zz = (double)(this.Z - vec.Z);

            return Math.Sqrt(xx * xx + yy * yy + zz * zz);
        }

        public double GetLength()
        {
            return DistanceTo(new sXYZ(0, 0, 0));
        }

        public void Unitize()
        {
            double distance = this.GetLength();

            this.X = this.X / distance;
            this.Y = this.Y / distance;
            this.Z = this.Z / distance;
        }

        public static sXYZ Rotate(sXYZ v, sXYZ axis, double angle)
        {
            sXYZ result = new sXYZ();

            double tr = t(angle);
            double cos = c(angle);
            double sin = s(angle);

            result.X = a1(angle, axis, tr, cos) * v.X + a2(angle, axis, tr, sin) * v.Y + a3(angle, axis, tr, sin) * v.Z;
            result.Y = b1(angle, axis, tr, sin) * v.X + b2(angle, axis, tr, cos) * v.Y + b3(angle, axis, tr, sin) * v.Z;
            result.Z = c1(angle, axis, tr, sin) * v.X + c2(angle, axis, tr, sin) * v.Y + c3(angle, axis, tr, cos) * v.Z;

            return result;
        }

        private static double t(double angle)
        {
            return 1 - (double)Math.Cos((double)angle);
        }

        private static double c(double angle)
        {
            return (double)Math.Cos((double)angle);
        }

        private static double s(double angle)
        {
            return (double)Math.Sin((double)angle);
        }

        private static double a1(double angle, sXYZ axis, double tr, double cos)
        {
            return (tr * axis.X * axis.X) + cos;
        }

        private static double a2(double angle, sXYZ axis, double tr, double sin)
        {
            return (tr * axis.X * axis.Y) - (sin * axis.Z);
        }

        private static double a3(double angle, sXYZ axis, double tr, double sin)
        {
            return (tr * axis.X * axis.Z) + (sin * axis.Y);
        }

        private static double b1(double angle, sXYZ axis, double tr, double sin)
        {
            return (tr * axis.X * axis.Y) + (sin * axis.Z);
        }

        private static double b2(double angle, sXYZ axis, double tr, double cos)
        {
            return (tr * axis.Y * axis.Y) + cos;
        }

        private static double b3(double angle, sXYZ axis, double tr, double sin)
        {
            return (tr * axis.Y * axis.Z) - (sin * axis.X);
        }

        private static double c1(double angle, sXYZ axis, double tr, double sin)
        {
            return (tr * axis.X * axis.Z) - (sin * axis.Y);
        }

        private static double c2(double angle, sXYZ axis, double tr, double sin)
        {
            return (tr * axis.Y * axis.Z) + (sin * axis.X);
        }

        private static double c3(double angle, sXYZ axis, double tr, double cos)
        {
            return (tr * axis.Z * axis.Z) + cos;
        }
    }
}
