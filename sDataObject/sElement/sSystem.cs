using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sGeometry;
using Newtonsoft.Json;
using sDataObject.ElementBase;
using sDataObject.IElement;

namespace sDataObject.sElement
{
    public class sSystem : SystemBase
    {
        public sSystem()
        {
            this.nodes = new List<sNode>();
            this.frameSets = new List<IFrameSet>();
            this.meshes = new List<sMesh>();
            this.loadPatterns = new List<string>();
            this.loadCombinations = new List<sLoadCombination>();
        }

        //abstract override
        public override ISystem DuplicatesSystem()
        {
            ISystem nsys = new sSystem();

            nsys.TransfersSystemBasis(this);
            nsys.TransfersSystemIFrameSetElements(this);
            //any other things only for system???
            
            return nsys;
        }

        public override void ToggleMinuteDensityStatus(object frameSetFilter, bool toggle)
        {
            foreach (IFrameSet fs in this.frameSets.Where(f => f.frameSetName.Equals((string)frameSetFilter)))
            {
                fs.AsMinuteDensity = toggle;
            }
        }
        public override int ApplyDesignedCrossSections(object frameSetFilter, int index = 0)
        {
            int count = 0;
            foreach (IFrameSet fs in this.frameSets.Where(f => f.frameSetName.Equals((string)frameSetFilter)))
            {
                if (fs.designedCrossSections != null && fs.designedCrossSections.Count > 0)
                {
                    fs.crossSection = null;
                    if (index > fs.designedCrossSections.Count - 1) index = fs.designedCrossSections.Count - 1;
                    fs.crossSection = fs.designedCrossSections[index].DuplicatesCrosssection();

                    fs.UpdatesFrameCrossSections();

                    count++;
                }
            }
            return count;
        }


    }
}
