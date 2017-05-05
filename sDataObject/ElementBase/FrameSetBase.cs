using sDataObject.IElement;
using sDataObject.sElement;
using sDataObject.sGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.ElementBase
{
    public abstract class FrameSetBase : IFrameSet
    {
        public Guid objectGUID { get; set; }
        public string frameSetName { get; set; }
        public int setId { get; set; }
        public sCurve parentCrv { get; set; }
        public List<sCurve> parentSegments { get; set; }

        public bool AsMinuteDensity { get; set; }
        public sCrossSection crossSection { get; set; }
        public List<sCrossSection> designedCrossSections { get; set; }
        public List<sLineLoad> lineLoads { get; set; }

        public sFixity parentFixityAtStart { get; set; }
        public List<sFixity> segmentFixitiesAtStart { get; set; }
        public sFixity parentFixityAtEnd { get; set; }
        public List<sFixity> segmentFixitiesAtEnd { get; set; }
        public List<sXYZ> associatedLocations { get; set; }

        public List<sFrame> frames { get; set; }
        public sResultRange results_Max { get; set; }
        

        public double GetFrameSetDemand(eColorMode forceType, int round = -2)
        {
            double maxDemand = 0.0;
            sRange dataRange = this.GetFrameSetResultRange(forceType);
            maxDemand = Math.Max(Math.Abs(dataRange.min), Math.Abs(dataRange.max));
            //Here everything should be SI Unit!!!

            //if ((int)forceType > 6)
            //{
            //    if (forceType.ToString().Contains("Moment"))
            //    {
            //        //N.m > kip.in
            //        maxDemand *= 0.28476439306;
            //    }
            //    else if (forceType.ToString().Contains("Force"))
            //    {
            //
            //    }
            //    else if (forceType.ToString().Contains("Deflection"))
            //    {
            //        //mm to in
            //        maxDemand *= 0.0393701;
            //    }
            //}

            if (round > -1)
            {
                maxDemand = Math.Round(maxDemand, round);
            }

            return maxDemand;
        }

        public sRange GetFrameSetResultRange(eColorMode colMode)
        {
            ///
            // Deflection_Local is Z only??????????????
            ///
            sRange ran = new sRange();
            if (colMode == eColorMode.Deflection_Local)
            {
                ran.min = 0.0;
                ran.max = this.results_Max.deflectionLocalMax_Abs_mm.Z;
            }
            else if (colMode == eColorMode.Deflection)
            {
                ran.min = 0.0;
                ran.max = this.results_Max.deflectionMax_Abs_mm.GetLength();
            }
            else if (colMode == eColorMode.Force_X)
            {
                ran.min = this.results_Max.forceMax_Negative.X;
                ran.max = this.results_Max.forceMax_Positive.X;
            }
            else if (colMode == eColorMode.Force_Y)
            {
                ran.min = this.results_Max.forceMax_Negative.Y;
                ran.max = this.results_Max.forceMax_Positive.Y;
            }
            else if (colMode == eColorMode.Force_Z)
            {
                ran.min = this.results_Max.forceMax_Negative.Z;
                ran.max = this.results_Max.forceMax_Positive.Z;
            }
            else if (colMode == eColorMode.Moment_X)
            {
                ran.min = this.results_Max.momentMax_Negative.X;
                ran.max = this.results_Max.momentMax_Positive.X;
            }
            else if (colMode == eColorMode.Moment_Y)
            {
                ran.min = this.results_Max.momentMax_Negative.Y;
                ran.max = this.results_Max.momentMax_Positive.Y;
            }
            else if (colMode == eColorMode.Moment_Z)
            {
                ran.min = this.results_Max.momentMax_Negative.Z;
                ran.max = this.results_Max.momentMax_Positive.Z;
            }
            else if (colMode == eColorMode.Stress_Combined_Absolute)
            {
                ran.min = 0.0;
                ran.max = this.results_Max.stressCombinedAbs;
            }
            else
            {
                throw new NotImplementedException();
            }

            if (ran.min == double.MaxValue)
            {
                ran.min = 0.0;
            }
            if (ran.max == double.MinValue)
            {
                ran.max = 0.0;
            }

            return ran;
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

        public void UpdatesFrameCrossSections()
        {
            foreach (sFrame f in this.frames)
            {
                f.crossSection = null;
                f.crossSection = this.crossSection.DuplicatesCrosssection();
            }
        }

        public void AddBeamElement(sLine sln, sXYZ upvec, int id)
        {
            sFrame sb = new sFrame(sln, upvec.DuplicatesXYZ());
            sb.parentGuid = this.objectGUID;
            sb.frameName = this.frameSetName + "_" + this.setId + "_" + id;
            sb.frameID = id;

            sb.crossSection = this.crossSection;

            //load
            if (this.lineLoads != null)
            {
                sb.lineLoads = new List<sLineLoad>();
                foreach (sLineLoad ll in this.lineLoads)
                {
                    sb.lineLoads.Add(ll.DuplicatesLineLoad());
                }
            }


            this.frames.Add(sb);
        }

        public void EnsureBeamElement()
        {
            if (this.frames.Count == 0)
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
            else if (this.frames.Count == 1)
            {
                sFrame sTemp = this.frames[0].DuplicatesFrame();
                this.frames.Clear();

                this.AddBeamElement(new sLine(sTemp.axis.startPoint, sTemp.axis.PointAt(0.5)), sTemp.upVector, 0);
                this.AddBeamElement(new sLine(sTemp.axis.PointAt(0.5), sTemp.axis.endPoint), sTemp.upVector, 1);
            }
        }

        public void ResetFixities()
        {
            this.parentFixityAtStart = null;
            this.parentFixityAtEnd = null;

            if (this.segmentFixitiesAtStart != null) this.segmentFixitiesAtStart.Clear();
            if (this.segmentFixitiesAtEnd != null) this.segmentFixitiesAtEnd.Clear();

            foreach (sFrame sb in this.frames)
            {
                sb.fixityAtStart = null;
                sb.fixityAtEnd = null;
            }
        }

        public void AwareElementsFixitiesByParentFixity(double tol)
        {
            if (this.frames.Count == 0) return;
            foreach (sFrame sb in this.frames)
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
            if (this.frames.Count == 0) return;
            foreach (sFrame sb in this.frames)
            {
                if (this.segmentFixitiesAtStart != null && this.segmentFixitiesAtStart.Count > 0)
                {
                    foreach (sFixity fx in this.segmentFixitiesAtStart)
                    {
                        if (sb.axis.startPoint.DistanceTo(fx.location) < tol)
                        {
                            if (this.parentFixityAtStart != null)
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
                                if (this.parentFixityAtEnd.location.DistanceTo(fx.location) > tol)
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

        public abstract IFrameSet DuplicatesFrameSet();
    }
}
