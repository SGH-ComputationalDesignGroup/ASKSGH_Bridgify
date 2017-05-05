using Newtonsoft.Json;
using sDataObject.IElement;
using sDataObject.sElement;
using sDataObject.sGeometry;
using sDataObject.sSteelElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.ElementBase
{
    public abstract class SystemBase : ISystem
    {
        public Guid objectGUID { get; set; }
        public List<sNode> nodes { get; set; }
        public List<IFrameSet> frameSets { get; set; }
        public List<sMesh> meshes { get; set; }
        public List<string> loadPatterns { get; set; }
        public List<sLoadCombination> loadCombinations { get; set; }
        public double estimatedWeight { get; set; }
        public double estimatedMaxD { get; set; }

        public sSystemSetting systemSettings { get; set; }
        public sResultRange systemResults { get; set; }

        public SystemBase()
        {
            this.objectGUID = Guid.NewGuid();
        }

        public void AwaresSystemResult()
        {
            this.systemResults = new sResultRange();

            foreach (IFrameSet fs in this.frameSets)
            {
                this.systemResults.UpdateMaxValues(fs.results_Max);
            }
        }
        public IFrameSet GetsFrameSetByGUID(Guid gid)
        {
            IFrameSet bs = null;
            foreach (IFrameSet b in this.frameSets)
            {
                if (b.objectGUID.Equals(gid))
                {
                    bs = b;
                    break;
                }
            }
            return bs;
        }
        public int ApplyDesignedCrossSections(int index = 0)
        {
            int count = 0;
            foreach (IFrameSet fs in this.frameSets)
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
        public void AddsBeamSet(IFrameSet bset)
        {
            bset.EnsureBeamElement();
            this.frameSets.Add(bset);
        }
        public void ResetBeamsInBeamSet()
        {
            foreach (IFrameSet bs in this.frameSets)
            {
                bs.frames.Clear();
                bs.frames = new List<sFrame>();
            }
        }
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
        public void ConstructBeamResultMesh(eColorMode colorMode, ref List<sMesh> meshes, out sRange dataRange, sRange threshold = null, double du = 0.0)
        {
            if (colorMode != eColorMode.NONE)
            {
                sRange resultRange = GetSystemBeamResultRange(colorMode);

                foreach (IFrameSet bs in this.frameSets)
                {
                    foreach (sFrame b in bs.frames)
                    {
                        sMesh sm = b.ConstructBeamColorMesh(resultRange, colorMode, threshold, du);
                        meshes.Add(sm);
                    }
                }

                dataRange = resultRange;
            }
            else
            {
                foreach (IFrameSet bs in this.frameSets)
                {
                    foreach (sFrame b in bs.frames)
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

            foreach (IFrameSet bs in this.frameSets)
            {
                foreach (sFrame b in bs.frames)
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
        public void SetLoadCombination(sLoadCombination com)
        {
            if (this.loadCombinations.Exists(c => c.Equals(com)) == false)
            {
                this.loadCombinations.Add(com);
            }
            this.AwarePatternNames();
        }
        public void AwarePatternNames()
        {
            foreach (sLoadCombination com in this.loadCombinations)
            {
                foreach (string pattName in com.patterns)
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
            if (patt != "DEAD")
            {
                if (this.loadPatterns.Exists(p => string.Equals(p, patt, StringComparison.OrdinalIgnoreCase)) == false)
                {
                    this.loadPatterns.Add(patt);
                }
            }
        }
        public void DuplicateFromsSystem(ISystem ssys)
        {
            this.TransfersSystemBasis(ssys);
            this.TransfersSystemIFrameSetElements(ssys);
        }
        public void TransfersSystemIFrameSetElements(ISystem ssys)
        {
            if (ssys.frameSets != null)
            {
                this.frameSets = new List<IFrameSet>();
                foreach (IFrameSet bs in ssys.frameSets)
                {
                    this.frameSets.Add(bs.DuplicatesFrameSet());
                }
            }
        }
        public void TransfersSystemBasis(ISystem ssys)
        {
            this.systemSettings = ssys.systemSettings.DuplicatesSystemSetting();
            if (ssys.meshes != null && ssys.meshes.Count > 0)
            {
                this.meshes = new List<sMesh>();
                foreach (sMesh m in ssys.meshes)
                {
                    this.meshes.Add(m.DuplicatesMesh());
                }
            }

            //sElement Part
            if (ssys.systemResults != null)
            {
                this.systemResults = ssys.systemResults.DuplicatesResultRange();
            }

            if (ssys.nodes != null)
            {
                this.nodes = new List<sNode>();
                foreach (sNode ns in ssys.nodes)
                {
                    this.nodes.Add(ns.DuplicatesNode());
                }
            }

            if (ssys.loadPatterns != null)
            {
                this.loadPatterns = ssys.loadPatterns.ToList();
            }

            if (ssys.loadCombinations != null)
            {
                this.loadCombinations = new List<sLoadCombination>();
                foreach (sLoadCombination com in ssys.loadCombinations)
                {
                    this.loadCombinations.Add(com.DuplicatesLoadCombination());
                }
            }

            this.estimatedMaxD = ssys.estimatedMaxD;
            this.estimatedWeight = ssys.estimatedWeight;
        }


        public static ISystem Objectify(string jsonFile, bool isForWeb = false)
        {
            if (isForWeb)
            {
                return JsonConvert.DeserializeObject<ISystem>(jsonFile, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
            }
            else
            {
                return JsonConvert.DeserializeObject<ISystem>(jsonFile, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });
            }

        }
        public string Jsonify(bool isForWeb = false)
        {
            if (isForWeb)
            {
                return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            else
            {
                return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.All,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });
            }

        }

        //abstract functions
        public abstract ISystem DuplicatesSystem();
        public abstract void ToggleMinuteDensityStatus(object frameSetFilter, bool toggle);
        public abstract int ApplyDesignedCrossSections(object frameSetFilter, int index = 0);


    }
}
