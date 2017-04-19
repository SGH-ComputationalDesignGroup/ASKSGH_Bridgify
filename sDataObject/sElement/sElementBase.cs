using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sElement
{
    public class sElementBase : IsObject
    {
        public Guid objectGUID { get; set; }

        public sElementBase()
        {
            this.objectGUID = Guid.NewGuid();
        }
    }
}
