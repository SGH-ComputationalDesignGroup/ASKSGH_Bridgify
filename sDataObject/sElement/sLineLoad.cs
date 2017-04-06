using sDataObject.sGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sDataObject.sElement
{
    public class sLineLoadGroup
    {
        public List<sLineLoad> loads { get; set; }

        public sLineLoadGroup()
        {

        }

       
    }

    public class sLineLoad
    {
        public string loadPatternName { get; set; }
        public eLoadType loadType { get; set; }
        public bool AsGlobalCorSys { get; set; }

        public double load_Scalar { get; set; } = 0.0;
        public sXYZ load_Force { get; set; }
        public sXYZ load_Moment { get; set; }

        //for un-even distributed load
        public List<sXYZ> uneven_loads_Force { get; set; }
        public List<sRange> parameters_loads_Force { get; set; }
        public List<sXYZ> uneven_loads_Moment { get; set; }
        public List<sRange> parameters_loads_Moment { get; set; }
        //need update for strin loads...

         

        public sLineLoad()
        {
        }

        public sLineLoad DuplicatesLineLoad()
        {
            sLineLoad newll = new sLineLoad();
            newll.loadPatternName = this.loadPatternName;
            newll.loadType = this.loadType;
            newll.AsGlobalCorSys = this.AsGlobalCorSys;

            newll.load_Scalar = this.load_Scalar;
            if (this.load_Force != null) newll.load_Force = this.load_Force.DuplicatesXYZ();
            if (this.load_Moment != null) newll.load_Moment = this.load_Moment.DuplicatesXYZ();

            if(this.uneven_loads_Force != null)
            {
                newll.uneven_loads_Force = new List<sXYZ>();
                foreach(sXYZ lf in this.uneven_loads_Force)
                {
                    newll.uneven_loads_Force.Add(lf.DuplicatesXYZ());
                }
            }
            if (this.parameters_loads_Force != null)
            {
                newll.parameters_loads_Force = this.parameters_loads_Force.ToList();
            }
            if (this.uneven_loads_Moment != null)
            {
                newll.uneven_loads_Moment = new List<sXYZ>();
                foreach (sXYZ lf in this.uneven_loads_Moment)
                {
                    newll.uneven_loads_Moment.Add(lf.DuplicatesXYZ());
                }
            }
            if (this.parameters_loads_Moment != null)
            {
                newll.parameters_loads_Moment = this.parameters_loads_Moment.ToList();
            }
            return newll;
        }

        public sLineLoad(string pattern, eLoadType type, bool IsGlobal ,sXYZ forceVec = null, sXYZ momentVec = null, double loadScalar = 0.0)
        {
            this.loadPatternName = pattern;
            this.loadType = type;
            this.AsGlobalCorSys = IsGlobal;
            if(loadScalar > 0.0)
            {
                this.load_Scalar = loadScalar;
            }
            if(forceVec != null)
            {
                this.load_Force = forceVec;
            }
            if(momentVec != null)
            {
                this.load_Moment = momentVec;
            }
        }

        public void ScaleByFactor(double fac)
        {
            if (this.load_Force != null) this.load_Force *= fac;
            if (this.load_Moment != null) this.load_Moment *= fac;
            if (this.load_Scalar > 0.0) this.load_Scalar *= fac;

            if(this.uneven_loads_Force != null && this.uneven_loads_Force.Count > 0)
            {
                for(int i = 0; i < this.uneven_loads_Force.Count; ++i)
                {
                    this.uneven_loads_Force[i] *= fac;
                }
            }

            if (this.uneven_loads_Moment != null && this.uneven_loads_Moment.Count > 0)
            {
                for (int i = 0; i < this.uneven_loads_Moment.Count; ++i)
                {
                    this.uneven_loads_Moment[i] *= fac;
                }
            }
        }

        public void ApplyUpdatedLineLoad(sLineLoad lload)
        {
            if(lload.load_Scalar > 0.0)
            {
                this.load_Scalar += lload.load_Scalar;
            }
            if (lload.load_Force != null)
            {
                if (this.load_Force == null) this.load_Force = sXYZ.Zero();
                this.load_Force += lload.load_Force;
            }
            if (lload.load_Moment != null)
            {
                if (this.load_Moment == null) this.load_Moment = sXYZ.Zero();
                this.load_Moment += lload.load_Moment;
            }

            //????
            if(lload.uneven_loads_Force != null && lload.uneven_loads_Force.Count == this.uneven_loads_Force.Count)
            {
                for (int i = 0; i < this.uneven_loads_Force.Count; ++i)
                {
                    this.uneven_loads_Force[i] += lload.uneven_loads_Force[i];
                }
            }

            if (lload.uneven_loads_Moment != null && lload.uneven_loads_Moment.Count == this.uneven_loads_Moment.Count)
            {
                for (int i = 0; i < this.uneven_loads_Moment.Count; ++i)
                {
                    this.uneven_loads_Moment[i] += lload.uneven_loads_Moment[i];
                }
            }
        }
        
    }

    public enum eLoadType
    {
        GravityLoad,
        ConcentratedLoad,
        DistributedLoad,
        DistributedLoad_Projected,
        TemperatureLoad,
        TemperatureLoad_Gradient_22,
        TemperatureLoad_Gradient_33,
        StrainLoad,
        DeformationLoad,
        TargetForce,
    }
}
