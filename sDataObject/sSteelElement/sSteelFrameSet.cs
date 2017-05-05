
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
    public class sSteelFrameSet : FrameSetBase
    {
        public eSteelFrameSetType frameStructureType { get; set; }
        public bool AsCantilever { get; set; }
        public bool AsComposite { get; set; }
        public sEffectiveSlabWidth effectiveSlabEdges { get; set; }

        public sSteelFrameSet()
        {
            SetDefaults();
        }

        public sSteelFrameSet(sCurve pCrv, bool asComposite = false)
        {
            this.parentCrv = pCrv;
            SetDefaults();
            this.AsComposite = asComposite;
        }

        void SetDefaults()
        {
            this.objectGUID = Guid.NewGuid();
            this.frames = new List<sFrame>();
            this.lineLoads = new List<sLineLoad>();

            this.parentSegments = new List<sCurve>();

            this.AsMinuteDensity = false;
            this.AsCantilever = false;
            this.AsComposite = false;
            this.frameStructureType = eSteelFrameSetType.AsNominal;
        }
        
        public override IFrameSet DuplicatesFrameSet()
        {
            sSteelFrameSet bs = new sSteelFrameSet();
            bs.frameSetName = this.frameSetName;
            bs.objectGUID = this.objectGUID;

            bs.frameStructureType = this.frameStructureType;
            bs.AsCantilever = this.AsCantilever;
            bs.AsComposite = this.AsComposite;

            bs.AsMinuteDensity = this.AsMinuteDensity;
            if (this.effectiveSlabEdges != null)
            {
                bs.effectiveSlabEdges = this.effectiveSlabEdges.DuplicatesEffectiveSlabWidth();
            }

            bs.setId = this.setId;
            bs.parentCrv = this.parentCrv.DuplicatesCurve();
            if (this.parentSegments != null)
            {
                bs.parentSegments = new List<sCurve>();
                foreach (sCurve sg in this.parentSegments)
                {
                    bs.parentSegments.Add(sg.DuplicatesCurve());
                }
            }
            bs.crossSection = this.crossSection.DuplicatesCrosssection();

            if (this.designedCrossSections != null)
            {
                bs.designedCrossSections = new List<sCrossSection>();
                foreach (sCrossSection cs in this.designedCrossSections)
                {
                    bs.designedCrossSections.Add(cs.DuplicatesCrosssection());
                }
            }

            if (this.lineLoads != null)
            {
                bs.lineLoads = new List<sLineLoad>();
                foreach (sLineLoad ll in this.lineLoads)
                {
                    bs.lineLoads.Add(ll.DuplicatesLineLoad());
                }
            }
            if (this.parentFixityAtStart != null) bs.parentFixityAtStart = this.parentFixityAtStart.DuplicatesFixity();
            if (this.parentFixityAtEnd != null) bs.parentFixityAtEnd = this.parentFixityAtEnd.DuplicatesFixity();

            if (this.segmentFixitiesAtStart != null)
            {
                bs.segmentFixitiesAtStart = new List<sFixity>();
                foreach (sFixity f in this.segmentFixitiesAtStart)
                {
                    bs.segmentFixitiesAtStart.Add(f.DuplicatesFixity());
                }
            }

            if (this.segmentFixitiesAtEnd != null)
            {
                bs.segmentFixitiesAtEnd = new List<sFixity>();
                foreach (sFixity f in this.segmentFixitiesAtEnd)
                {
                    bs.segmentFixitiesAtEnd.Add(f.DuplicatesFixity());
                }
            }

            if (this.associatedLocations != null)
            {
                bs.associatedLocations = new List<sXYZ>();
                foreach (sXYZ lc in this.associatedLocations)
                {
                    bs.associatedLocations.Add(lc.DuplicatesXYZ());
                }
            }


            if (this.frames != null)
            {
                bs.frames = new List<sFrame>();
                foreach (sFrame sb in this.frames)
                {
                    bs.frames.Add(sb.DuplicatesFrame());
                }
            }

            if (this.results_Max != null)
            {
                bs.results_Max = this.results_Max.DuplicatesResultRange();
            }

            return bs;
        }
    }

    public enum eSteelFrameSetType
    {
        AsBeam = 0,
        AsGirder = 1,
        AsColumn = 2,
        AsNominal = 3
    }
}
