using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sGeometry;

namespace sDataObject.sElement
{
    public class sNode : sElementBase
    {
        public string nodeName { get; set; }
        public int nodeID { get; set; }
        public sXYZ location { get; set; }

        public object extraData { get; set; }

        public List<sPointLoad> pointLoads { get; set; }
        public sPointSupport boundaryCondition { get; set; }


        public sNode()
        {
            
        }

        public sNode(sXYZ loc)
        {
            this.location = loc;
        }

        public sNode DuplicatesNode()
        {
            sNode nn = new sNode(this.location);
            nn.nodeID = this.nodeID;
            if(this.boundaryCondition != null) nn.boundaryCondition = this.boundaryCondition.DuplicatesPointSupport();
            
            if(this.pointLoads != null && this.pointLoads.Count > 0)
            {
                nn.pointLoads = new List<sPointLoad>();
                foreach (sPointLoad pl in this.pointLoads)
                {
                    nn.pointLoads.Add(pl.DuplicatePointLoad());
                }
            }

            return nn;
        }

        public void UpdatePointElement(sPointLoad pl)
        {
            if (this.pointLoads == null && this.pointLoads.Count == 0) this.pointLoads = new List<sPointLoad>();

            int count = 0;
            foreach(sPointLoad epl in this.pointLoads)
            {
                if(epl.loadPatternName == pl.loadPatternName)
                {
                    count++;
                    epl.forceVector += pl.forceVector;

                    //moment?...
                }
            }
            if(count == 0)
            {
                this.pointLoads.Add(pl);
            }
        }

        public void UpdatePointElement(sPointSupport sp)
        {
            this.boundaryCondition = sp;
        }

        public void UpdatePointLoadByPatternFactor_LinearAdditive(string pattern, double factor, ref sPointLoad comboLoad)
        {
            foreach(sPointLoad pl in this.pointLoads)
            {
                if(pl.loadPatternName == pattern)
                {
                    if(pl.forceVector != null && pl.forceVector.GetLength() > 0)
                    {
                        if (comboLoad.forceVector == null) comboLoad.forceVector = sXYZ.Zero();
                        comboLoad.forceVector += pl.forceVector * factor;
                    }
                    if (pl.momentVector != null && pl.momentVector.GetLength() > 0)
                    {
                        if (comboLoad.momentVector == null) comboLoad.momentVector = sXYZ.Zero();
                        comboLoad.momentVector += pl.momentVector * factor;
                    }
                }
            }
        }

        /*
        public void DistributeLoadToRelaventBeams_AsLineLoad(sSystem jsys)
        {
            //for SAP cal, I don't need to merge loads for load combination!! 
            //for Millipede cal, I need to merge loads for load combination!!
            //In this function, I just need to convert point load to relavent beams as line load
            //no need to consider load combination
            if (this.pointLoads != null)
            {
                foreach (sPointLoad pl in this.pointLoads)
                {
                    pl.DistributePointLoadToRelaventBeams(this.location, jsys);
                }
            }
        }
        */

        public double DistanceTo(sNode n)
        {
            return this.location.DistanceTo(n.location);
        }
    }


}
