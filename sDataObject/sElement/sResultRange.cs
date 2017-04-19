using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using sDataObject.sGeometry;

namespace sDataObject.sElement
{
    public class sResultRange
    {
        public sXYZ forceMax_Positive { get; set; }
        public sXYZ forceMax_Negative { get; set; }
        public sXYZ momentMax_Positive { get; set; }
        public sXYZ momentMax_Negative { get; set; }
        public sXYZ deflectionMax_Abs_mm { get; set; }
        public double stressCombinedAbs { get; set; }

        public sResultRange()
        {
            this.forceMax_Positive = new sXYZ(double.MinValue, double.MinValue, double.MinValue);
            this.forceMax_Negative = new sXYZ(double.MaxValue, double.MaxValue, double.MaxValue);
            this.momentMax_Positive = new sXYZ(double.MinValue, double.MinValue, double.MinValue);
            this.momentMax_Negative = new sXYZ(double.MaxValue, double.MaxValue, double.MaxValue);
            this.deflectionMax_Abs_mm = new sXYZ(double.MinValue, double.MinValue, double.MinValue);
            this.stressCombinedAbs = double.MinValue;
        }

        public void UpdateMaxValues(sResultRange rer)
        {
            if (rer.forceMax_Negative.X < this.forceMax_Negative.X) this.forceMax_Negative.X = rer.forceMax_Negative.X;
            if (rer.forceMax_Negative.Y < this.forceMax_Negative.Y) this.forceMax_Negative.Y = rer.forceMax_Negative.Y;
            if (rer.forceMax_Negative.Z < this.forceMax_Negative.Z) this.forceMax_Negative.Z = rer.forceMax_Negative.Z;

            if (rer.forceMax_Positive.X > this.forceMax_Positive.X) this.forceMax_Positive.X = rer.forceMax_Positive.X;
            if (rer.forceMax_Positive.Y > this.forceMax_Positive.Y) this.forceMax_Positive.Y = rer.forceMax_Positive.Y;
            if (rer.forceMax_Positive.Z > this.forceMax_Positive.Z) this.forceMax_Positive.Z = rer.forceMax_Positive.Z;

            if (rer.momentMax_Negative.X < this.momentMax_Negative.X) this.momentMax_Negative.X = rer.momentMax_Negative.X;
            if (rer.momentMax_Negative.Y < this.momentMax_Negative.Y) this.momentMax_Negative.Y = rer.momentMax_Negative.Y;
            if (rer.momentMax_Negative.Z < this.momentMax_Negative.Z) this.momentMax_Negative.Z = rer.momentMax_Negative.Z;

            if (rer.momentMax_Positive.X > this.momentMax_Positive.X) this.momentMax_Positive.X = rer.momentMax_Positive.X;
            if (rer.momentMax_Positive.Y > this.momentMax_Positive.Y) this.momentMax_Positive.Y = rer.momentMax_Positive.Y;
            if (rer.momentMax_Positive.Z > this.momentMax_Positive.Z) this.momentMax_Positive.Z = rer.momentMax_Positive.Z;

            if (rer.deflectionMax_Abs_mm.X > this.deflectionMax_Abs_mm.X) this.deflectionMax_Abs_mm.X = rer.deflectionMax_Abs_mm.X;
            if (rer.deflectionMax_Abs_mm.Y > this.deflectionMax_Abs_mm.Y) this.deflectionMax_Abs_mm.Y = rer.deflectionMax_Abs_mm.Y;
            if (rer.deflectionMax_Abs_mm.Z > this.deflectionMax_Abs_mm.Z) this.deflectionMax_Abs_mm.Z = rer.deflectionMax_Abs_mm.Z;

            if (rer.stressCombinedAbs > this.stressCombinedAbs) this.stressCombinedAbs = rer.stressCombinedAbs;
        }

        public void UpdateMaxValues(sFrameResult bre)
        {
            if(bre.force.X < 0.0)
            {
                if (bre.force.X < this.forceMax_Negative.X) this.forceMax_Negative.X = bre.force.X;
            }
            else
            {
                if (bre.force.X > this.forceMax_Positive.X) this.forceMax_Positive.X = bre.force.X;
            }
            if(bre.force.Y < 0.0)
            {
                if (bre.force.Y < this.forceMax_Negative.Y) this.forceMax_Negative.Y = bre.force.Y;
            }
            else
            {
                if (bre.force.Y > this.forceMax_Positive.Y) this.forceMax_Positive.Y = bre.force.Y;
            }
            if (bre.force.Z < 0.0)
            {
                if (bre.force.Z < this.forceMax_Negative.Z) this.forceMax_Negative.Z = bre.force.Z;
            }
            else
            {
                if (bre.force.Z > this.forceMax_Positive.Z) this.forceMax_Positive.Z = bre.force.Z;
            }

            if (bre.moment.X < 0.0)
            {
                if (bre.moment.X < this.momentMax_Negative.X) this.momentMax_Negative.X = bre.moment.X;
            }
            else
            {
                if (bre.moment.X > this.momentMax_Positive.X) this.momentMax_Positive.X = bre.moment.X;
            }
            if(bre.moment.Y < 0.0)
            {
                if (bre.moment.Y < this.momentMax_Negative.Y) this.momentMax_Negative.Y = bre.moment.Y;
            }
            else
            {
                if (bre.moment.Y > this.momentMax_Positive.Y) this.momentMax_Positive.Y = bre.moment.Y;
            }
            if (bre.moment.Z < 0.0)
            {
                if (bre.moment.Z < this.momentMax_Negative.Z) this.momentMax_Negative.Z = bre.moment.Z;
            }
            else
            {
                if (bre.moment.Z > this.momentMax_Positive.Z) this.momentMax_Positive.Z = bre.moment.Z;
            }

            double defx = Math.Abs(bre.deflection_mm.X);
            if (defx > this.deflectionMax_Abs_mm.X) this.deflectionMax_Abs_mm.X = defx;
            double defy = Math.Abs(bre.deflection_mm.Y);
            if (defy > this.deflectionMax_Abs_mm.Y) this.deflectionMax_Abs_mm.Y = defy;
            double defz = Math.Abs(bre.deflection_mm.Z);
            if (defz > this.deflectionMax_Abs_mm.Z) this.deflectionMax_Abs_mm.Z = defz;

            foreach(sFrameSectionResult vr in bre.sectionResults)
            {
                if(vr.stress_Combined > this.stressCombinedAbs)
                {
                    this.stressCombinedAbs = vr.stress_Combined;
                }
            }

            
        }

        public sResultRange DuplicatesResultRange()
        {
            sResultRange newre = new sResultRange();

            newre.forceMax_Positive = this.forceMax_Positive.DuplicatesXYZ();
            newre.forceMax_Negative = this.forceMax_Negative.DuplicatesXYZ();
            newre.momentMax_Positive = this.momentMax_Positive.DuplicatesXYZ();
            newre.momentMax_Negative = this.momentMax_Negative.DuplicatesXYZ();
            newre.deflectionMax_Abs_mm = this.deflectionMax_Abs_mm.DuplicatesXYZ();

            return newre;
        }

    }
}
