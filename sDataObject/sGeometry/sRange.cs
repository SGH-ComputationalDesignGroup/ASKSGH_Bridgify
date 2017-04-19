using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sGeometry
{
    public class sRange : sGeometryBase
    {
        public double min { get; set; }
        public double max { get; set; }
        public double length { get; set; }

        public sRange()
        {

        }

        public sRange(double minVal, double maxVal)
        {
            if(minVal > maxVal)
            {
                this.min = maxVal;
                this.max = minVal;
            }
            else
            {
                this.min = minVal;
                this.max = maxVal;
            }
            this.length = Math.Abs(this.max - this.min);
        }

        public sRange DuplicatesRange()
        {
            sRange nr = new sRange(this.min, this.max);
            return nr;
        }

        public double GetNormalizedAt(double value)
        {
            //if (value >= this.max) return 1.0;
            //if (value <= this.min) return 0.0;

            return (value - this.min) / (this.length);
        }

        public double GetRemappedAt(double value, sRange targetRn)
        {
            if (value >= targetRn.max) return targetRn.max;
            if (value <= targetRn.min) return targetRn.min;

            return targetRn.min + ((value - this.min) * (targetRn.length) / (this.length));
        }

        public double GetOriginBasedNormalizedAt(double value)
        {
            if(this.min < 0.0 && this.max < 0.0)
            {
                sRange newRn = new sRange(this.min, Math.Abs(this.min));
                return newRn.GetNormalizedAt(value);
            }
            else if(this.min < 0.0 && this.max > 0.0)
            {
                double absMax = Math.Max(Math.Abs(this.min), Math.Abs(this.max));
                sRange newRn = new sRange(-absMax, absMax);
                return newRn.GetNormalizedAt(value);
            }
            else if(this.min > 0.0 && this.max > 0.0)
            {
                sRange newRn = new sRange(-1 * Math.Abs(this.max), this.max);
                return newRn.GetNormalizedAt(value);
            }
            else
            {
                return this.GetNormalizedAt(value);
            }
        }

        public static double GetEnsureValue(double valThis, sRange range = null)
        {
            if(range != null)
            {
                if(valThis <= range.min)
                {
                    return range.min;
                }
                else if(valThis >= range.max)
                {
                    return range.max;
                }
                else
                {
                    return valThis;
                }
            }
            else
            {
                return valThis;
            }
        }

        public bool Includes(double val)
        {
            bool inc = false;

            if(val >= this.min && val <= this.max)
            {
                inc = true;
            }

            return inc;
        }
    }
}
