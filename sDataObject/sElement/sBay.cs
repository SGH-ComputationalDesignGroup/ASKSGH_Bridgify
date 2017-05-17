using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using sDataObject.sGeometry;

namespace sDataObject.sElement
{
    /*
    public class sBay : sElementBase
    {
        public int bayID { get; set; }
        public eBayType bayType { get; set; }
        public sPolyLine bayBoundary { get; set; }
        public List<int> girderIndices { get; set; }
        public List<int> beamIndices { get; set; }
        
        public sBay()
        {
            this.bayType = eBayType.Default;
        }

        public sBay DuplicatesBay()
        {
            sBay nb = new sBay();
            nb.objectGUID = this.objectGUID;
            nb.bayID = this.bayID;
            nb.bayType = this.bayType;

            if(this.bayBoundary != null)
            {
                nb.bayBoundary = this.bayBoundary.DuplicatesPolyline();
            }

            if(this.girderIndices != null)
            {
                nb.girderIndices = new List<int>();
                foreach(int gi in this.girderIndices)
                {
                    nb.girderIndices.Add(gi);
                }
            }

            if (this.beamIndices != null)
            {
                nb.beamIndices = new List<int>();
                foreach (int gi in this.beamIndices)
                {
                    nb.beamIndices.Add(gi);
                }
            }
            return nb;
        }

    }
    */
    public enum eBayType
    {
        Inside = 0,
        OnEdge = 1,
        OnEdge_Cantilever = 2,
        Corner = 3,
        Unidentified = 4,
        Default = 5
    }
}
