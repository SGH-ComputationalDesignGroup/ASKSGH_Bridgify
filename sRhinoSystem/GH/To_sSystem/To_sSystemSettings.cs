using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel;
using sDataObject;
using sDataObject.sElement;
using sDataObject.sGeometry;
using Grasshopper.Kernel.Types;
using sRhinoSystem.Properties;

namespace sRhinoSystem.GH.To_sSystem
{
    public class To_sSystemSettings : GH_Component
    {
        public To_sSystemSettings()
            : base("sSystem Settings", "sSystem Settings", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("systemName", "systemName", "systemName", GH_ParamAccess.item);

            pManager.AddTextParameter("defaultLoadCase", "defaultLoadCase", "defaultLoadCase", GH_ParamAccess.item, "DEAD");
            pManager.AddIntegerParameter("defaultCheckType", "defaultCheckType", "defaultCheckType", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("StressThreshold(MPa|ksi)", "StressThreshold(MPa|ksi)", "StressThreshold(MPa|ksi)", GH_ParamAccess.item, -1);
            pManager.AddNumberParameter("DeflectionThreshold(mm|in)", "DeflectionThreshold(mm|in)", "DeflectionThreshold(mm|in)", GH_ParamAccess.item, -1);

            pManager.AddNumberParameter("mergeTolerance(mm|in)", "mergeTolerance(mm|in)", "mergeTolerance(mm|in)", GH_ParamAccess.item, -1);
            pManager.AddNumberParameter("colorMeshSegmentSize(m|ft)", "colorMeshSegmentSize(m|ft)", "colorMeshSegmentSize(m|ft)", GH_ParamAccess.item, -1);

            Params.Input[1].Optional = true;
            Params.Input[2].Optional = true;
            Params.Input[3].Optional = true;
            Params.Input[4].Optional = true;
            Params.Input[5].Optional = true;
            Params.Input[6].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sSystemSettings", "sSystemSettings", "sSystemSettings", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string name = "";
            string loadcase = "";
            int type = 0;
            double st = 0.0;
            double def = 0.0;
            double merge = 0.0;
            double mSeg = 0.0;

            if (!DA.GetData(0, ref name)) return;
            if (!DA.GetData(1, ref loadcase)) return;
            if (!DA.GetData(2, ref type)) return;
            if (!DA.GetData(3, ref st)) return;
            if (!DA.GetData(4, ref def)) return;
            if (!DA.GetData(5, ref merge)) return;
            if (!DA.GetData(6, ref mSeg)) return;

            sSystemSetting set = new sSystemSetting();
            set.systemName = name;
            set.currentCase = loadcase;
            
            if(type == 1)
            {
                set.currentCheckType = eSystemCheckType.StiffnessCheck;
            }
            else
            {
                set.currentCheckType = eSystemCheckType.StrengthCheck;
            }

            string currentUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            set.systemOriUnit = currentUnit;

            if(st > 0)
            {
                if(currentUnit == "Meters")
                {
                    //MPa to Pa
                    set.currentStressThreshold_pascal = st * 1000000;
                }
                else
                {
                    //ksi to Pa
                    set.currentStressThreshold_pascal = st * 6894757.28;
                }   
            }
            else
            {
                set.currentStressThreshold_pascal = 25 * 6894757.28;
            }
            
            if(def > 0)
            {
                if (currentUnit == "Meters")
                {
                    set.currentDeflectionThreshold_mm = def;
                }
                else
                {
                    set.currentDeflectionThreshold_mm = def * 25.4;
                }
            }
            else
            {
                set.currentDeflectionThreshold_mm = 100;
            }

            if(merge > 0)
            {
                if (currentUnit == "Meters")
                {
                    //mm to m
                    set.mergeTolerance_m = merge * 0.001;
                }
                else
                {
                    //in to m
                    set.mergeTolerance_m = merge * 0.0254;
                }
            }
            else
            {
                set.mergeTolerance_m = 0.005;
            }

            if (mSeg > 0)
            {
                if (currentUnit == "Meters")
                {
                    //m to m
                    set.meshDensity_m = mSeg;
                }
                else
                {
                    //ft to m
                    set.meshDensity_m = mSeg * 0.3048;
                }
            }
            else
            {
                set.meshDensity_m = 0.5;
            }

            DA.SetData(0, set);
            
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("f87d0037-d0f5-4fc4-872f-237ac7b343c5"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.systemSetting;
            }
        }

    }
}
