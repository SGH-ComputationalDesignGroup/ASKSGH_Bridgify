using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using sDataObject.sGeometry;

namespace sDataObject.sElement
{
    public class sFrame : sElementBase
    {
        public int frameID { get; set; }
        public Guid parentGuid { get; set; }
        public string frameName { get; set; }
        public sNode node0 { get; set; }
        public sNode node1 { get; set; }
        public sLine axis { get; set; }
        public sXYZ upVector { get; set; }
        public sXYZ sideVector { get; set; }
        public sPlane localPlane { get; set; }
        public sCrossSection crossSection { get; set; }

        public object extraData { get; set; }

        public sFixity fixityAtStart { get; set; }
        public sFixity fixityAtEnd { get; set; }

        public List<sTributaryArea> tributaryAreas { get; set; }
        public List<sLineLoad> lineLoads { get; set; }

        public double beamWeight { get; set; }
        public List<sFrameResult> results { get; set; }

        public sFrame DuplicatesFrame()
        {
            sFrame newb = new sFrame();

            newb.objectGUID = this.objectGUID;

            newb.frameID = this.frameID;
            newb.parentGuid = this.parentGuid;
            newb.frameName = this.frameName;
            newb.node0 = this.node0.DuplicatesNode();
            newb.node1 = this.node1.DuplicatesNode();
            newb.axis = this.axis.DuplicatesLine();
            if (this.upVector != null) newb.upVector = this.upVector.DuplicatesXYZ();
            if (this.sideVector != null) newb.sideVector = this.sideVector.DuplicatesXYZ();
            if (this.localPlane != null) newb.localPlane = this.localPlane.DuplicatesPlane();
            newb.crossSection = this.crossSection.DuplicatesCrosssection();

            if (this.fixityAtStart != null) newb.fixityAtStart = this.fixityAtStart.DuplicatesFixity();
            if (this.fixityAtEnd != null) newb.fixityAtEnd = this.fixityAtEnd.DuplicatesFixity();

            if(this.tributaryAreas != null)
            {
                newb.tributaryAreas = new List<sTributaryArea>();
                foreach(sTributaryArea ta in this.tributaryAreas)
                {
                    newb.tributaryAreas.Add(ta.DuplicatesTributaryArea());
                }
            }
            if(this.lineLoads != null)
            {
                newb.lineLoads = new List<sLineLoad>();
                foreach(sLineLoad ll in this.lineLoads)
                {
                    newb.lineLoads.Add(ll.DuplicatesLineLoad());
                }
            }

            newb.beamWeight = this.beamWeight;
            if(this.results != null)
            {
                newb.results = new List<sFrameResult>();
                foreach(sFrameResult re in this.results)
                {
                    newb.results.Add(re.DuplicatesFrameResult());
                }
            }
            return newb;
        }


        public sFrame()
        {
            //this.upVector = sXYZ.Zaxis();
        }

        public sFrame(sLine axln, sXYZ upVec)
        {
            this.node0 = new sNode();
            this.node0.location = axln.startPoint;
            this.node1 = new sNode();
            this.node1.location = axln.endPoint;

            this.axis = axln;

            this.AwareLocalPlane(upVec);
        }

        public sFrame(sLine axln)
        {
            this.node0 = new sNode();
            this.node0.location = axln.startPoint;
            this.node1 = new sNode();
            this.node1.location = axln.endPoint;

            this.axis = axln;
        }

        public sFrame(sNode n0, sNode n1)
        {
            this.node0 = n0;
            this.node1 = n1;

            this.axis = new sLine(n0.location, n1.location);
        }

        public sFrame(sNode n0, sNode n1, sXYZ upVec)
        {
            this.node0 = n0;
            this.node1 = n1;

            this.axis = new sLine(n0.location, n1.location);

            this.AwareLocalPlane(upVec);
        }



        public void AwareLocalPlane(sXYZ upVec)
        {
            sXYZ beamDir = this.axis.direction;
            beamDir.Unitize();
            double dot = Math.Abs(beamDir * sXYZ.Zaxis());

            bool isVertical = false;
            if (dot > 0.9)
            {
                isVertical = true;
            }
            this.upVector = upVec;
            if (isVertical) this.upVector = sXYZ.Xaxis();

            sXYZ lx = this.axis.direction;
            lx.Unitize();
            sXYZ lz = this.upVector;
            lz.Unitize();
            sXYZ ly = sXYZ.CrossProduct(lx, lz);
            ly.Unitize();

            this.localPlane = new sPlane(this.node0.location, lx, ly);
        }

        public void UpdateLineLoad(sLineLoad lload)
        {
            if(this.lineLoads != null)
            {
                bool isThere = false;
                foreach (sLineLoad ll in this.lineLoads)
                {
                    if(ll.loadPatternName == lload.loadPatternName)
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

        public void UpdateLineLoadByPatternFactor_LinearAdditive(string pattern, double factor, ref sLineLoad comboLoad)
      {
          foreach(sLineLoad ll in this.lineLoads)
          {
              if(ll.loadPatternName == pattern)
              {
                  if(ll.load_Force != null && ll.load_Force.GetLength() > 0)
                  {
                      if (comboLoad.load_Force == null) comboLoad.load_Force = sXYZ.Zero();
                      comboLoad.load_Force += ll.load_Force * factor;
                  }
                  if(ll.load_Moment != null && ll.load_Moment.GetLength() > 0)
                  {
                      if (comboLoad.load_Moment == null) comboLoad.load_Moment = sXYZ.Zero();
                      comboLoad.load_Moment += ll.load_Moment * factor;
                  }
                  if(ll.load_Scalar > 0.0)
                  {
                      comboLoad.load_Scalar += ll.load_Scalar * factor;
                  }
              }
          }
      }

        public sMesh ConstructBeamColorMesh(sRange dataRange, eColorMode colorMode, sRange threshold, double du)
        {
            sMesh sm = new sMesh();
            
            int vertexID = 0;
            for (int i = 0; i < this.results.Count; ++i)
            {
                sFrameResult br = this.results[i];

                for (int j = 0; j < br.sectionResults.Count; ++j)
                {
                    sFrameSectionResult sr = br.sectionResults[j];
                    sColor vcol = this.GetBeamResultColor(dataRange, colorMode, i, j, 255, threshold);
                    sXYZ deflectionVec = du * (sr.deflection_mm * 0.001);
                    sXYZ deflectedPt = sr.location + deflectionVec;
                    sm.SetVertex(vertexID, deflectedPt, vcol);
                    //or
                    //sm.SetVertex(vertexID, sr.point, vcol);

                    sr.ID = vertexID;

                    vertexID++;
                }

            }

            int vertexCountPerFace = this.results[0].sectionResults.Count;
            int faceIndex = 0;
            for (int i = 0; i < this.results.Count-1; ++i)
            {
                sFrameResult br_this = this.results[i];
                sFrameResult br_next = this.results[i + 1];

                for(int j = 0; j < br_this.sectionResults.Count; ++j)
                {
                    int id0 = 0;
                    int id1 = 0;
                    int id2 = 0;
                    int id3 = 0;
                    if (j < br_this.sectionResults.Count - 1)
                    {
                        id0 = br_this.sectionResults[j].ID;
                        id1 = br_next.sectionResults[j].ID;
                        id2 = br_next.sectionResults[j+1].ID;
                        id3 = br_this.sectionResults[j+1].ID;
                    }
                    else
                    {
                        id0 = br_this.sectionResults[j].ID;
                        id1 = br_next.sectionResults[j].ID;
                        id2 = br_next.sectionResults[0].ID;
                        id3 = br_this.sectionResults[0].ID;
                    }

                    sm.SetFace(faceIndex, faceIndex + 1, id0, id1, id2, id3);
                    faceIndex += 2;
                } 
            }

            sm.ComputeNormals();
            return sm;
        }

        public sRange GetBeamResultRange(eColorMode colorMode)
        {
            double minV = double.MaxValue;
            double maxV = double.MinValue;

            foreach (sFrameResult br in this.results)
            {
                if (colorMode == eColorMode.Force_X)
                {
                    if (br.force.X < minV)
                    {
                        minV = br.force.X;
                    }
                    if (br.force.X > maxV)
                    {
                        maxV = br.force.X;
                    }
                }
                else if (colorMode == eColorMode.Force_Y)
                {
                    if (br.force.Y < minV)
                    {
                        minV = br.force.Y;
                    }
                    if (br.force.Y > maxV)
                    {
                        maxV = br.force.Y;
                    }
                }
                else if (colorMode == eColorMode.Force_Z)
                {
                    if (br.force.Z < minV)
                    {
                        minV = br.force.Z;
                    }
                    if (br.force.Z > maxV)
                    {
                        maxV = br.force.Z;
                    }
                }
                else if (colorMode == eColorMode.Moment_X)
                {
                    if (br.moment.X < minV)
                    {
                        minV = br.moment.X;
                    }
                    if (br.moment.X > maxV)
                    {
                        maxV = br.moment.X;
                    }
                }
                else if (colorMode == eColorMode.Moment_Y)
                {
                    if (br.moment.Y < minV)
                    {
                        minV = br.moment.Y;
                    }
                    if (br.moment.Y > maxV)
                    {
                        maxV = br.moment.Y;
                    }
                }
                else if (colorMode == eColorMode.Moment_Z)
                {
                    if (br.moment.Z < minV)
                    {
                        minV = br.moment.Z;
                    }
                    if (br.moment.Z > maxV)
                    {
                        maxV = br.moment.Z;
                    }
                }
                else if (colorMode == eColorMode.Stress_Combined_Absolute)
                {
                    foreach (sFrameSectionResult secRe in br.sectionResults)
                    {
                        double absStress = Math.Abs(secRe.stress_Combined);
                        if (absStress < minV)
                        {
                            minV = absStress;
                        }
                        if (absStress > maxV)
                        {
                            maxV = absStress;
                        }
                    }
                }
                else if (colorMode == eColorMode.Stress_Axial_X)
                {
                    foreach (sFrameSectionResult secRe in br.sectionResults)
                    {
                        if (secRe.stress_Axial_X < minV)
                        {
                            minV = secRe.stress_Axial_X;
                        }
                        if (secRe.stress_Axial_X > maxV)
                        {
                            maxV = secRe.stress_Axial_X;
                        }
                    }
                }
                else if (colorMode == eColorMode.Stress_Moment_Y)
                {
                    foreach (sFrameSectionResult secRe in br.sectionResults)
                    {
                        if (secRe.stress_Moment_Y < minV)
                        {
                            minV = secRe.stress_Moment_Y;
                        }
                        if (secRe.stress_Moment_Y > maxV)
                        {
                            maxV = secRe.stress_Moment_Y;
                        }
                    }
                }
                else if (colorMode == eColorMode.Stress_Moment_Z)
                {
                    foreach (sFrameSectionResult secRe in br.sectionResults)
                    {
                        if (secRe.stress_Moment_Z < minV)
                        {
                            minV = secRe.stress_Moment_Z;
                        }
                        if (secRe.stress_Moment_Z > maxV)
                        {
                            maxV = secRe.stress_Moment_Z;
                        }
                    }
                }
                else if(colorMode == eColorMode.Deflection)
                {
                    foreach(sFrameSectionResult secRe in br.sectionResults)
                    {
                        double def = secRe.deflection_mm.GetLength();
                        if (secRe.stress_Moment_Z < minV)
                        {
                            minV = def;
                        }
                        if (secRe.stress_Moment_Z > maxV)
                        {
                            maxV = def;
                        }
                    }
                }
            }
            sRange ran = new sRange(minV, maxV);
            return ran;

        }

        public sColor GetBeamResultColor(sRange dataRange, eColorMode colorMode, int resultIndex, int sectionIndex,  int alpha, sRange threshold = null)
        {
            sColor col = new sColor();
            sColorGradient cg = null;

            if (colorMode == eColorMode.Force_X)
            {
                double valThis = this.results[resultIndex].force.X;
                cg = sColorGradient.GetCyanRedGradient(dataRange, threshold);
                double remapped = dataRange.GetOriginBasedNormalizedAt(valThis);
                col = cg.ColorAt(remapped);
            }
            else if (colorMode == eColorMode.Force_Y)
            {
                double valThis = this.results[resultIndex].force.Y;
                cg = sColorGradient.GetCyanRedGradient(dataRange, threshold);
                double remapped = dataRange.GetOriginBasedNormalizedAt(valThis);
                col = cg.ColorAt(remapped);
            }
            else if (colorMode == eColorMode.Force_Z)
            {
                double valThis = this.results[resultIndex].force.Z;
                cg = sColorGradient.GetCyanRedGradient(dataRange, threshold);
                double remapped = dataRange.GetOriginBasedNormalizedAt(valThis);
                col = cg.ColorAt(remapped);
            }
            else if (colorMode == eColorMode.Moment_X)
            {
                double valThis = this.results[resultIndex].moment.X;
                cg = sColorGradient.GetCyanRedGradient(dataRange, threshold);
                double remapped = dataRange.GetOriginBasedNormalizedAt(valThis);
                col = cg.ColorAt(remapped);
            }
            else if (colorMode == eColorMode.Moment_Y)
            {
                double valThis = this.results[resultIndex].moment.Y;
                cg = sColorGradient.GetCyanRedGradient( dataRange, threshold);
                double remapped = dataRange.GetOriginBasedNormalizedAt(valThis);
                col = cg.ColorAt(remapped);
            }
            else if (colorMode == eColorMode.Moment_Z)
            {
                double valThis = this.results[resultIndex].moment.Z;
                cg = sColorGradient.GetCyanRedGradient( dataRange, threshold);
                double remapped = dataRange.GetOriginBasedNormalizedAt(valThis);
                col = cg.ColorAt(remapped);
            }
            else if (colorMode == eColorMode.Stress_Combined_Absolute)
            {
                double valThis =  this.results[resultIndex].sectionResults[sectionIndex].stress_Combined;
                cg = sColorGradient.GetRainbowLikeGradient( dataRange, threshold);
                double remapped = dataRange.GetOriginBasedNormalizedAt(valThis);
                
                col = cg.ColorAt(remapped);
            }
            else if (colorMode == eColorMode.Stress_Axial_X)
            {
                
            }
            else if (colorMode == eColorMode.Stress_Moment_Y)
            {
                double valThis = this.results[resultIndex].sectionResults[sectionIndex].stress_Moment_Y;
                cg = sColorGradient.GetCyanRedGradient( dataRange, threshold);
                double remapped = dataRange.GetOriginBasedNormalizedAt(valThis);
                //double remapped = dataRange.GetNormalizedAt(valThis);
                col = cg.ColorAt(remapped);
            }
            else if (colorMode == eColorMode.Stress_Moment_Z)
            {
                double valThis = this.results[resultIndex].sectionResults[sectionIndex].stress_Moment_Z;
                cg = sColorGradient.GetCyanRedGradient(dataRange, threshold);
                double remapped = dataRange.GetOriginBasedNormalizedAt(valThis);
                //double remapped = dataRange.GetNormalizedAt(valThis);
                col = cg.ColorAt(remapped);
            }
            else if(colorMode == eColorMode.Deflection)
            {
                double valThis = this.results[resultIndex].sectionResults[sectionIndex].deflection_mm.GetLength();
                cg = sColorGradient.GetRainbowLikeGradient(dataRange, threshold);
                double remapped = dataRange.GetOriginBasedNormalizedAt(valThis);

                col = cg.ColorAt(remapped);
            }
            else if(colorMode == eColorMode.NONE)
            {
                col = new sColor(200, 200, 200);
            }
            return col;
        }

        public bool AwareFixity(sFixity fix, double tol = 0.005)
        {
            if(fix.location.DistanceTo(this.axis.startPoint) < tol)
            {
                this.fixityAtStart = fix.DuplicatesFixity();
            }

            if(fix.location.DistanceTo(this.axis.endPoint) < tol)
            {
                this.fixityAtEnd = fix.DuplicatesFixity();
            }

            if(this.fixityAtStart != null || this.fixityAtEnd != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
