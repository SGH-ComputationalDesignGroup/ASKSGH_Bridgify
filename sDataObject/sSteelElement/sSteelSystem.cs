using sDataObject.ElementBase;
using sDataObject.IElement;
using sDataObject.sElement;
using sDataObject.sGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sSteelElement
{
    public class sSteelSystem : SystemBase
    {
        public bool IsComposite { get; set; }

        public sSteelSystem()
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
            ISystem nsys = new sSteelSystem();

            nsys.TransfersSystemBasis(this);
            nsys.TransfersSystemIFrameSetElements(this);

            return nsys;
        }

        //more functions
        private void toggleMinuteDensityStatus(eSteelFrameSetType type, bool toggle)
        {
            foreach (sSteelFrameSet fs in this.frameSets.Cast<sSteelFrameSet>().Where(f => f.frameStructureType == type))
            {
                fs.AsMinuteDensity = toggle;
            }
        }

        private int applyDesignedCrossSections(eSteelFrameSetType type, int index = 0)
        {
            int count = 0;
            foreach (sSteelFrameSet fs in this.frameSets.Cast<sSteelFrameSet>().Where(f => f.frameStructureType == type))
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

        public override void ToggleMinuteDensityStatus(object frameSetFilter, bool toggle)
        {
            if(frameSetFilter is string)
            {
                foreach (IFrameSet fs in this.frameSets.Where(f => f.frameSetName.Equals((string)frameSetFilter)))
                {
                    fs.AsMinuteDensity = toggle;
                }
            }
            else if(frameSetFilter is eSteelFrameSetType)
            {
                eSteelFrameSetType type = (eSteelFrameSetType)frameSetFilter;
                this.toggleMinuteDensityStatus(type, toggle);
            }
        }

        public override int ApplyDesignedCrossSections(object frameSetFilter, int index = 0)
        {
            int count = 0;

            
            if (frameSetFilter is eSteelFrameSetType)
            {
                eSteelFrameSetType type = (eSteelFrameSetType)frameSetFilter;
                count = this.applyDesignedCrossSections(type, index);
            }
            else if(frameSetFilter is string)
            {
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
            }
            return count;
        }
    }
}
