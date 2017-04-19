using sDataObject.sGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sElement
{
    public class sPointLoad
    {
        public string loadPatternName { get; set; }

        public sXYZ location { get; set; }

        public sXYZ momentVector { get; set; }
        public sXYZ forceVector { get; set; }

        //public sTributaryArea tributaryArea { get; set; }

        public string loadingBeamName { get; set; }
        public sXYZ loadingDirection { get; set; }

        public sPointLoad()
        {

        }

        public sNode TosNode()
        {
            sNode newNode = new sNode();
            newNode.location = this.location;
            newNode.pointLoads = new List<sPointLoad>();
            newNode.pointLoads.Add(this);
            return newNode;
        }

        public sPointLoad DuplicatePointLoad()
        {
            sPointLoad newload = new sPointLoad();
            newload.location = this.location;
            newload.loadingBeamName = this.loadingBeamName;
            if(this.forceVector != null) newload.forceVector = this.forceVector;
            if(this.momentVector != null) newload.momentVector = this.momentVector;
            newload.loadingDirection = this.loadingDirection;
            newload.loadPatternName = this.loadPatternName;
            return newload;
        }

        public void ScaleByFactor(double fac)
        {
            if (this.forceVector != null) this.forceVector *= fac;
            if (this.momentVector != null) this.momentVector *= fac;
        }

        public void GetRelaventBeams(sXYZ point, List<sFrame> beams, out sFrame beam0, out double cldis0, out sFrame beam1, out double cldis1)
        {
            sFrame rb0 = null;
            double cdis0 = double.MaxValue;
            sFrame rb1 = null;
            double cdis1 = double.MaxValue;

            sLine l0 = new sLine(point, point + (this.loadingDirection * 1000));
            sLine l1 = new sLine(point, point - (this.loadingDirection * 1000));

            foreach (sFrame jb in beams)
            {
                if (jb.frameName.Contains(this.loadingBeamName))
                {
                    sLine bax = jb.axis;

                    sXYZ ip0;
                    if (bax.GetIntersection(l0, 0.005, out ip0))
                    {
                        double dis0 = ip0.DistanceTo(point);
                        if (dis0 < cdis0)
                        {
                            cdis0 = dis0;
                            rb0 = jb;
                        }
                    }
                    sXYZ ip1;
                    if (bax.GetIntersection(l1, 0.005, out ip1))
                    {
                        double dis1 = ip1.DistanceTo(point);
                        if (dis1 < cdis1)
                        {
                            cdis1 = dis1;
                            rb1 = jb;
                        }
                    }
                }
            }
            beam0 = rb0;
            beam1 = rb1;
            cldis0 = cdis0;
            cldis1 = cdis1;
        }

        /*
        public void DistributePointLoadToRelaventBeams(sXYZ point, sSystem jsys)
        {
            sBeam rb0;
            sBeam rb1;
            double cdis0;
            double cdis1;
            GetRelaventBeams(point, jsys.beams, out rb0, out cdis0, out rb1, out cdis1);

            if (rb0 != null && rb1 != null)
            {
                if (cdis1 == 0 && cdis0 == 0)
                {
                    sXYZ lload = (this.forceVector / rb0.axis.length);
                    sLineLoad sl = new sLineLoad(this.loadPatternName, eLoadType.DistributedLoad, true, lload);
                    rb0.UpdateLineLoad(sl);
                }
                else
                {
                    double ratio0 = (cdis1 / (cdis0 + cdis1));
                    double ratio1 = (cdis0 / (cdis0 + cdis1));

                    sLine rb0Ln = rb0.axis;
                    sLine rb1Ln = rb1.axis;

                    sXYZ lload0 = ((ratio0 * this.forceVector) / rb0Ln.length);
                    sXYZ lload1 = ((ratio1 * this.forceVector) / rb1Ln.length);

                    sLineLoad sl0 = new sLineLoad(this.loadPatternName, eLoadType.DistributedLoad, true, lload0);
                    rb0.UpdateLineLoad(sl0);
                    sLineLoad sl1 = new sLineLoad(this.loadPatternName, eLoadType.DistributedLoad, true, lload1);
                    rb1.UpdateLineLoad(sl1);
                }
            }
            else if (rb0 != null && rb1 == null)
            {
                sXYZ lload = (this.forceVector / rb0.axis.length);
                sLineLoad sl = new sLineLoad(this.loadPatternName, eLoadType.DistributedLoad, true, lload);
                rb0.UpdateLineLoad(sl);
            }
            else if (rb0 == null && rb1 != null)
            {
                sXYZ lload = (this.forceVector / rb1.axis.length);
                sLineLoad sl = new sLineLoad(this.loadPatternName, eLoadType.DistributedLoad, true, lload);
                rb0.UpdateLineLoad(sl);
            }
        }
        */
    }
}
