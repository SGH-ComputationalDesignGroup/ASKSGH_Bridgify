using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sElement;
using sDataObject.sGeometry;
using sDataObject.IElement;

namespace sKodeStructSystem.BeamDesigner
{
    public interface IKodeBeamDesigner : IsKodeDesigner
    {
        List<sCrossSection> Design_Beams_Gravity(IFrameSet fs_Full, IFrameSet fs_Minute, IFrameSet fs_Live, List<sCrossSection> sortedShapes, int count = 3, double depthLimitMax_in = -1.0, double depthLimitMin_in = -1.0);


    }
}
