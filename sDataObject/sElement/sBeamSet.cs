using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using sDataObject.sGeometry;

namespace sDataObject.sElement
{
    public class sBeamSet : sElementBase
    {
        public string beamSetName { get; set; }
        public int setId { get; set; }
        public sCurve parentCrv { get; set; }
        public List<sCurve> parentSegments { get; set; }

        public sCrossSection crossSection { get; set; }
        public List<sLineLoad> lineLoads { get; set; }

        public sFixity parentFixityAtStart { get; set; }
        public List<sFixity> segmentFixitiesAtStart { get; set; }
        public sFixity parentFixityAtEnd { get; set; }
        public List<sFixity> segmentFixitiesAtEnd { get; set; }
        public List<sXYZ> associatedLocations { get; set; }

        public List<sBeam> beams { get; set; }
        public sBeamSetResult results_Max { get; set; }

        public sBeamSet DuplicatesBeamSet()
        {
            sBeamSet bs = new sBeamSet();
            bs.beamSetName = this.beamSetName;

            bs.objectGUID = this.objectGUID;

            bs.setId = this.setId;
            bs.parentCrv = this.parentCrv.DuplicatesCurve();
            if(this.parentSegments != null)
            {
                bs.parentSegments = new List<sCurve>();
                foreach(sCurve sg in this.parentSegments)
                {
                    bs.parentSegments.Add(sg.DuplicatesCurve());
                }
            }
            bs.crossSection = this.crossSection.DuplicatesCrosssection();
            if(this.lineLoads != null)
            {
                bs.lineLoads = new List<sLineLoad>();
                foreach(sLineLoad ll in this.lineLoads)
                {
                    bs.lineLoads.Add(ll.DuplicatesLineLoad());
                }
            }
            if (this.parentFixityAtStart != null) bs.parentFixityAtStart = this.parentFixityAtStart.DuplicatesFixity();
            if (this.parentFixityAtEnd != null) bs.parentFixityAtEnd = this.parentFixityAtEnd.DuplicatesFixity();

            if(this.segmentFixitiesAtStart != null)
            {
                bs.segmentFixitiesAtStart = new List<sFixity>();
                foreach(sFixity f in this.segmentFixitiesAtStart)
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

            if(this.associatedLocations != null)
            {
                bs.associatedLocations = new List<sXYZ>();
                foreach(sXYZ lc in this.associatedLocations)
                {
                    bs.associatedLocations.Add(lc.DuplicatesXYZ());
                }
            }
            

            if(this.beams != null)
            {
                bs.beams = new List<sBeam>();
                foreach(sBeam sb in this.beams)
                {
                    bs.beams.Add(sb.DuplicatesBeam());
                }
            }

            if(this.results_Max != null)
            { 
                bs.results_Max = this.results_Max.DuplicatesBeamSetResult();
            }

            return bs;
        }

        public sBeamSet()
        {
            this.beams = new List<sBeam>();
            this.lineLoads = new List<sLineLoad>();

            this.parentSegments = new List<sCurve>();
        }

        public sBeamSet(sCurve pCrv)
        {
            this.beams = new List<sBeam>();
            this.lineLoads = new List<sLineLoad>();

            this.parentSegments = new List<sCurve>();
            this.parentCrv = pCrv;
        }

        public void UpdateLineLoad(sLineLoad lload)
        {
            if (this.lineLoads != null)
            {
                bool isThere = false;
                foreach (sLineLoad ll in this.lineLoads)
                {
                    if (ll.loadPatternName == lload.loadPatternName)
                    {
                        ll.ApplyUpdatedLineLoad(lload);
                        isThere = true;
                    }
                }
                if (isThere == false)
                {
                    this.lineLoads.Add(lload);
                }
            }
            else
            {
                this.lineLoads = new List<sLineLoad>();
                this.lineLoads.Add(lload);
            }
        }

        public void AddBeamElement(sLine sln, sXYZ upvec, int id)
        {
            sBeam sb = new sBeam(sln, upvec.DuplicatesXYZ());
            sb.parentGuid = this.objectGUID;
            sb.beamName = this.beamSetName + "_" + this.setId + "_" + id;
            sb.beamID = id;

            sb.crossSection = this.crossSection;

            //load
            if(this.lineLoads != null)
            {
                sb.lineLoads = new List<sLineLoad>();
                foreach(sLineLoad ll in this.lineLoads)
                {
                    sb.lineLoads.Add(ll.DuplicatesLineLoad());
                }
            }


            this.beams.Add(sb);
        }

        public void EnsureBeamElement()
        {
            if (this.beams.Count == 0)
            {
                sCurve bc = this.parentCrv;
                if (bc.curveType == eCurveType.LINE)
                {
                    sLine ln = bc as sLine;
                    //??? UPVECTOR???
                    this.AddBeamElement(new sLine(ln.startPoint, ln.PointAt(0.5)), sXYZ.Zaxis(), 0);
                    this.AddBeamElement(new sLine(ln.PointAt(0.5), ln.endPoint), sXYZ.Zaxis(), 1);
                }
            }
            else if (this.beams.Count == 1)
            {
                sBeam sTemp = this.beams[0].DuplicatesBeam();
                this.beams.Clear();

                this.AddBeamElement(new sLine(sTemp.axis.startPoint, sTemp.axis.PointAt(0.5)), sTemp.upVector, 0);
                this.AddBeamElement(new sLine(sTemp.axis.PointAt(0.5), sTemp.axis.endPoint), sTemp.upVector, 1);
            }
        }

        public void ResetFixities()
        {
            this.parentFixityAtStart = null;
            this.parentFixityAtEnd = null;

            if(this.segmentFixitiesAtStart != null) this.segmentFixitiesAtStart.Clear();
            if (this.segmentFixitiesAtEnd != null) this.segmentFixitiesAtEnd.Clear();

            foreach(sBeam sb in this.beams)
            {
                sb.fixityAtStart = null;
                sb.fixityAtEnd = null;
            }
        }

        public void AwareElementsFixitiesByParentFixity(double tol)
        {
            if (this.beams.Count == 0) return;
            foreach (sBeam sb in this.beams)
            {
                if (this.parentFixityAtStart != null)
                {
                    if (sb.axis.startPoint.DistanceTo(this.parentFixityAtStart.location) < tol)
                    {
                        sb.fixityAtStart = this.parentFixityAtStart.DuplicatesFixity();
                    }
                }

                if (this.parentFixityAtEnd != null)
                {
                    if (sb.axis.endPoint.DistanceTo(this.parentFixityAtEnd.location) < tol)
                    {
                        sb.fixityAtEnd = this.parentFixityAtEnd.DuplicatesFixity();
                    }
                }
            }
        }

        public void AwareElementFixitiesBySegementFixities(double tol)
        {
            if (this.beams.Count == 0) return;
            foreach (sBeam sb in this.beams)
            {
                if (this.segmentFixitiesAtStart != null && this.segmentFixitiesAtStart.Count > 0)
                {
                    foreach (sFixity fx in this.segmentFixitiesAtStart)
                    {
                        if (sb.axis.startPoint.DistanceTo(fx.location) < tol)
                        {
                            if(this.parentFixityAtStart != null)
                            {
                                if (this.parentFixityAtStart.location.DistanceTo(fx.location) > tol)
                                {
                                    sb.fixityAtStart = fx.DuplicatesFixity();
                                }
                            }
                            else
                            {
                                sb.fixityAtStart = fx.DuplicatesFixity();
                            }
                        }
                    }
                }

                if (this.segmentFixitiesAtEnd != null && this.segmentFixitiesAtEnd.Count > 0)
                {
                    foreach (sFixity fx in this.segmentFixitiesAtEnd)
                    {
                        if (sb.axis.endPoint.DistanceTo(fx.location) < tol)
                        {
                            if (this.parentFixityAtEnd != null)
                            {
                                if(this.parentFixityAtEnd.location.DistanceTo(fx.location) > tol)
                                {
                                    sb.fixityAtEnd = fx.DuplicatesFixity();
                                }
                            }
                            else
                            {
                                sb.fixityAtEnd = fx.DuplicatesFixity();
                            }
                        }
                    }
                }

            }
        }
    
    }
}
