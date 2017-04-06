using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using sDataObject.sGeometry;

namespace sDataObject.sElement
{
    public class sBeamSetResult
    {
        public sXYZ forceMax_Positive { get; set; }
        public sXYZ forceMax_Negative { get; set; }
        public sXYZ momentMax_Positive { get; set; }
        public sXYZ momentMax_Negative { get; set; }
        public sXYZ deflectionMax_Abs_mm { get; set; }
        
        public sBeamSetResult()
        {
            this.forceMax_Positive = new sXYZ(double.MinValue, double.MinValue, double.MinValue);
            this.forceMax_Negative = new sXYZ(double.MaxValue, double.MaxValue, double.MaxValue);
            this.momentMax_Positive = new sXYZ(double.MinValue, double.MinValue, double.MinValue);
            this.momentMax_Negative = new sXYZ(double.MaxValue, double.MaxValue, double.MaxValue);
            this.deflectionMax_Abs_mm = new sXYZ(double.MinValue, double.MinValue, double.MinValue);
        }

        public void UpdateMaxValues(sBeamResult bre)
        {
            if(bre.force.X < 0.0)
            {
                if (bre.force.X < this.forceMax_Negative.X) this.forceMax_Negative.X = bre.force.X;
                if (bre.force.Y < this.forceMax_Negative.Y) this.forceMax_Negative.Y = bre.force.Y;
                if (bre.force.Z < this.forceMax_Negative.Z) this.forceMax_Negative.Z = bre.force.Z;
            }
            else
            {
                if (bre.force.X > this.forceMax_Positive.X) this.forceMax_Positive.X = bre.force.X;
                if (bre.force.Y > this.forceMax_Positive.Y) this.forceMax_Positive.Y = bre.force.Y;
                if (bre.force.Z > this.forceMax_Positive.Z) this.forceMax_Positive.Z = bre.force.Z;
            }

            if(bre.moment.X < 0.0)
            {
                if (bre.moment.X < this.momentMax_Negative.X) this.momentMax_Negative.X = bre.moment.X;
                if (bre.moment.Y < this.momentMax_Negative.Y) this.momentMax_Negative.Y = bre.moment.Y;
                if (bre.moment.Z < this.momentMax_Negative.Z) this.momentMax_Negative.Z = bre.moment.Z;
            }
            else
            {
                if (bre.moment.X > this.momentMax_Positive.X) this.momentMax_Positive.X = bre.moment.X;
                if (bre.moment.Y > this.momentMax_Positive.Y) this.momentMax_Positive.Y = bre.moment.Y;
                if (bre.moment.Z > this.momentMax_Positive.Z) this.momentMax_Positive.Z = bre.moment.Z;
            }

            double defx = Math.Abs(bre.deflection_mm.X);
            if (defx > this.deflectionMax_Abs_mm.X) this.deflectionMax_Abs_mm.X = defx;
            double defy = Math.Abs(bre.deflection_mm.Y);
            if (defy > this.deflectionMax_Abs_mm.Y) this.deflectionMax_Abs_mm.Y = defy;
            double defz = Math.Abs(bre.deflection_mm.Z);
            if (defz > this.deflectionMax_Abs_mm.Z) this.deflectionMax_Abs_mm.Z = defz;
        }

        public sBeamSetResult DuplicatesBeamSetResult()
        {
            sBeamSetResult newre = new sBeamSetResult();

            newre.forceMax_Positive = this.forceMax_Positive.DuplicatesXYZ();
            newre.forceMax_Negative = this.forceMax_Negative.DuplicatesXYZ();
            newre.momentMax_Positive = this.momentMax_Positive.DuplicatesXYZ();
            newre.momentMax_Negative = this.momentMax_Negative.DuplicatesXYZ();
            newre.deflectionMax_Abs_mm = this.deflectionMax_Abs_mm.DuplicatesXYZ();

            return newre;
        }

    }
}
