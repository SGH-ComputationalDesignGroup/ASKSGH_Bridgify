using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sGeometry
{
    public class sGeometryBase : sObject
    {
        public Guid objectGUID { get; set; }

        public sGeometryBase()
        {
            this.objectGUID = Guid.NewGuid();
        }
    }
}
