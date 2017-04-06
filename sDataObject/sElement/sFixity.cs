using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sGeometry;

namespace sDataObject.sElement
{
    public class sFixity
    {
        public sXYZ location { get; set; }
        public bool [] release { get; set; }
        public double [] partialRelease { get; set; }

        public sFixity()
        {

        }

        public sFixity DuplicatesFixity()
        {
            sFixity newfix = new sFixity();
            newfix.location = this.location.DuplicatesXYZ();
            if(this.release != null)
            {
                newfix.release = this.release.ToArray();
            }
            if(this.partialRelease != null)
            {
                newfix.partialRelease = this.partialRelease.ToArray();
            }
            return newfix;
        }

        public bool IsOnLocation(List<sXYZ> locs, double tol)
        {
            bool isOn = false;

            if (locs != null)
            {
                foreach (sXYZ x in locs)
                {
                    if (this.location.DistanceTo(x) < tol)
                    {
                        isOn = true;
                        break;
                    }
                }
            }

            return isOn;
        }

        public bool IsOnLocation(sXYZ loc, double tol)
        {
            bool isOn = false;

            if (loc != null)
            {
                if (this.location.DistanceTo(loc) < tol)
                {
                    isOn = true;
                }
            }

            return isOn;
        }
    }

    public enum eFixityType
    {
        FULLY_FIXED,
        MOMENTREALESED_START,
        MOMENTREALESED_END,
        MOMENTREALESED_BOTH,
        FIXITIES_BY_DOF,
        FIXITIES_BY_PARTIALRELEASE
    }
}
