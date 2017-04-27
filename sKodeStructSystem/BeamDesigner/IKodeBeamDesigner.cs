using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sBuildingElement;
using sDataObject.sElement;
using sDataObject.sGeometry;

namespace sKodeStructSystem.BeamDesigner
{
    public interface IKodeBeamDesigner : IsKodeDesigner
    {
        void CheckFlexuralYieldingDCR(sbBeam sbeam);
    }
}
