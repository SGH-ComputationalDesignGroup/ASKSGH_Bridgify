using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sGeometry;
using Newtonsoft.Json;

namespace sDataObject.sElement
{
    public class sSystem
    {
        public List<sNode> nodes { get; set; }
        public List<sBeamSet> beamSets { get; set; }
        public List<sMesh> meshes { get; set; }
        public List<string> loadPatterns { get; set; }
        public List<sLoadCombination> loadCombinations { get; set; }
        public double estimatedWeight { get; set; }
        public double estimatedMaxD { get; set; }
        
        public sSystemSetting systemSettings { get; set; }


        public sSystem()
        {
            this.nodes = new List<sNode>();
            this.beamSets = new List<sBeamSet>();
            this.meshes = new List<sMesh>();
            this.loadPatterns = new List<string>();
            //this.loadPatterns.Add("DEAD"); // as default
            this.loadCombinations = new List<sLoadCombination>();
        }

        public sBeamSet GetBeamSetByGUID(Guid gid)
        {
            sBeamSet bs = null;
            foreach (sBeamSet b in this.beamSets)
            {
                if (b.objectGUID.Equals(gid))
                {
                    bs = b;
                    break;
                }
            }
            return bs;
        }

        public void ResetBeamsInBeamSet()
        {
            foreach(sBeamSet bs in this.beamSets)
            {
                bs.beams.Clear();
                bs.beams = new List<sBeam>();
            }
        }
        //node functions
        public bool UpdateNodeFromPointElement(sPointSupport ps, int id)
        {
            sNode exNode;
            if (AwareExistingNode(ps.location, out exNode))
            {
                exNode.UpdatePointElement(ps);
                return false;
            }
            else
            {
                sNode newN = ps.TosNode();
                newN.nodeID = id;
                //newN.elementGUID = Guid.NewGuid();
                this.nodes.Add(newN);
                return true;
            }
        }

        public bool UpdateNodeFromPointElement(sPointLoad pl, int id)
        {
            sNode exNode;
            if (AwareExistingNode(pl.location, out exNode))
            {
                exNode.UpdatePointElement(pl);
                return false;
            }
            else
            {
                sNode newN = pl.TosNode();
                newN.nodeID = id;
                //newN.elementGUID = Guid.NewGuid();
                this.nodes.Add(newN);
                return true;
            }
        }

        public bool AwareExistingNode(sXYZ location, out sNode existingNode)
        {
            sNode exn = null;
            if (this.nodes != null && this.nodes.Count > 0)
            {
                foreach (sNode sn in this.nodes)
                {
                    double dis = location.DistanceTo(sn.location);
                    if (dis < 0.001)
                    {
                        exn = sn;
                        break;
                    }
                }
            }
            if (exn != null)
            {
                existingNode = exn;
                return true;
            }
            else
            {
                existingNode = null;
                return false;
            }
        }

        public void AddsBeamSet(sBeamSet bset)
        {
            bset.EnsureBeamElement();
            this.beamSets.Add(bset);
        }

        public List<sPointLoad> GetPointLoadsByCombo(sLoadCombination combo)
        {
            List<sPointLoad> selectedLoads = new List<sPointLoad>();

            foreach(sNode sn in this.nodes)
            {
                if (sn.pointLoads != null && sn.pointLoads.Count > 0)
                {
                    sPointLoad comboLoad = new sPointLoad();
                    if (combo.combinationType == eCombinationType.LinearAdditive)
                    {
                        for (int i = 0; i < combo.patterns.Count; ++i)
                        {
                            string pattern = combo.patterns[i];
                            double factor = combo.factors[i];
                            
                           sn.UpdatePointLoadByPatternFactor_LinearAdditive(pattern, factor, ref comboLoad);
                        }
                    }
                    if(comboLoad.forceVector != null || comboLoad.momentVector != null)
                    {
                        comboLoad.location = sn.location;
                        selectedLoads.Add(comboLoad);
                    }
                }
            }
            return selectedLoads;
        }
        
        public List<sPointLoad> GetPointLoadsByPattern(string patternName)//, out List<sNode> selectedNode)
        {
            //List<sNode> nodesSelected = new List<sNode>();
            List<sPointLoad> loadsSelected = new List<sPointLoad>();

            foreach (sNode sn in this.nodes)
            {
                int count = 0;
                if (sn.pointLoads != null && sn.pointLoads.Count > 0)
                {
                    foreach (sPointLoad pl in sn.pointLoads)
                    {
                        if (pl.loadPatternName == patternName)
                        {
                            count++;
                            loadsSelected.Add(pl);
                        }
                    }
                    if (count > 0)
                    {
                        //    nodesSelected.Add(sn);
                    }
                }
            }
            //selectedNode = nodesSelected;
            return loadsSelected;
        }
        

        

        public void ConstructBeamResultMesh(eColorMode colorMode,  ref List<sMesh> meshes, out sRange dataRange, sRange threshold = null, double du = 0.0)
        {
            if (colorMode != eColorMode.NONE)
            {
                sRange resultRange = GetSystemBeamResultRange(colorMode);

                foreach(sBeamSet bs in this.beamSets)
                {
                    foreach (sBeam b in bs.beams)
                    {
                        sMesh sm = b.ConstructBeamColorMesh(resultRange, colorMode, threshold, du);
                        meshes.Add(sm);
                    }
                }

                dataRange = resultRange;
            }
            else
            {
                foreach (sBeamSet bs in this.beamSets)
                {
                    foreach (sBeam b in bs.beams)
                    {
                        sMesh sm = b.ConstructBeamColorMesh(new sRange(0.0, 0.0), colorMode, new sRange(0.0, 0.0), 0.0);
                        meshes.Add(sm);
                    }
                }
                dataRange = null;
            }
            
        }

        public sRange GetSystemBeamResultRange(eColorMode colorMode)
        {
            double minV = double.MaxValue;
            double maxV = double.MinValue;

            foreach (sBeamSet bs in this.beamSets)
            {
                foreach (sBeam b in bs.beams)
                {
                    sRange bRange = b.GetBeamResultRange(colorMode);
                    if (bRange.min < minV)
                    {
                        minV = bRange.min;
                    }
                    if (bRange.max > maxV)
                    {
                        maxV = bRange.max;
                    }
                }
            }
            return new sRange(minV, maxV);
        }

        //system functions
        public void SetLoadCombination(sLoadCombination com)
        {
            if(this.loadCombinations.Exists(c => c.Equals(com)) == false)
            {
                this.loadCombinations.Add(com);
            }
            this.AwarePatternNames();
        }

        public void AwarePatternNames()
        {
            foreach(sLoadCombination com in this.loadCombinations)
            {
                foreach(string pattName in com.patterns)
                {
                    if (pattName != "DEAD")
                    {
                        if (this.loadPatterns.Exists(p => string.Equals(p, pattName, StringComparison.OrdinalIgnoreCase)) == false)
                        {
                            this.loadPatterns.Add(pattName);
                        }
                    }
                }
            }
        }

        public void AwarePatternNames(string patt)
        {
            if(patt != "DEAD")
            {
                if (this.loadPatterns.Exists(p => string.Equals(p, patt, StringComparison.OrdinalIgnoreCase)) == false)
                {
                    this.loadPatterns.Add(patt);
                }
            }
        }

        public static sSystem Objectify(string jsonFile)
        {
            return JsonConvert.DeserializeObject<sSystem>(jsonFile, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        //this guy doesn't know other system type.. override function?
        public static T Objectify<T>(string jsonFile)
        {
            return JsonConvert.DeserializeObject<T>(jsonFile, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        public string Jsonify()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            });
        }

        
/*
        //need updates
        public void UpdatePointLoads(sSystem pointLoadSystem)
        {
            foreach (sNode jn in pointLoadSystem.nodes)
            {
                jn.DistributeLoadToRelaventBeams_AsLineLoad(this);

            }
        }
        //need updates
        public void UpdatePointLoads(List<sNode> pointLoads)
        {
            foreach (sNode jn in pointLoads)
            {
                jn.DistributeLoadToRelaventBeams_AsLineLoad(this);
            }
        }
        */

    }

    public enum eColorMode
    {
        Stress_Combined_Absolute = 0,
        Stress_Moment_X = 1,
        Stress_Moment_Y = 2,
        Stress_Moment_Z = 3,
        Stress_Axial_X = 4,
        Stress_Axial_Y = 5,
        Stress_Axial_Z = 6,
        Moment_X = 7,
        Moment_Y = 8,
        Moment_Z = 9,
        Force_X = 10,
        Force_Y = 11,
        Force_Z = 12,
        Deflection = 13,
        NONE = 14

    }

}
