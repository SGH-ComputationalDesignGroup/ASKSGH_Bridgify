using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sGeometry
{
    public class sBoundingBox : sGeometryBase
    {
        public sRange xSize { get; set; }
        public sRange ySize { get; set; }
        public sRange zSize { get; set; }
        
        public sXYZ min { get; set; }
        public sXYZ max { get; set; }
        public sXYZ diagonal { get; set; }
        public sXYZ center { get; set; }

        public sBoundingBox()
        {

        }

        public sBoundingBox DuplicatesBoundingBox()
        {
            sBoundingBox nb = new sBoundingBox();
            nb.xSize = this.xSize.DuplicatesRange();
            nb.ySize = this.ySize.DuplicatesRange();
            nb.zSize = this.zSize.DuplicatesRange();

            nb.min = this.min.DuplicatesXYZ();
            nb.max = this.max.DuplicatesXYZ();
            nb.diagonal = this.diagonal.DuplicatesXYZ();
            nb.center = this.center.DuplicatesXYZ();
            return nb;
        }

    }
}
