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
    public class To_sLineLoad : GH_Component
    {
        public To_sLineLoad()
            : base("sLineLoad", "sLineLoad", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("patternName", "patternName", "...", GH_ParamAccess.item);
            pManager.AddVectorParameter("forceVectors(N/m|lbf/ft)", "forceVectors(N/m|lbf/ft)", "...", GH_ParamAccess.list);
            
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sLineLoadElement", "sLineLoadElement", "sLineLoadElement", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string patternName = "";
            List<Vector3d> forceVectors = new List<Vector3d>();

            if (!DA.GetData(0, ref patternName)) return;
            if (!DA.GetDataList(1, forceVectors)) return;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");

            object outobj = null;
            if (forceVectors.Count == 1)
            {
                sLineLoad l_before = new sLineLoad(patternName, eLoadType.DistributedLoad, true, rhcon.TosXYZ(forceVectors[0]));
                outobj = rhcon.EnsureUnit(l_before);
            }
            else if (forceVectors.Count > 1)
            {
                sLineLoadGroup lg = new sLineLoadGroup();
                lg.loads = new List<sLineLoad>();
                foreach (Vector3d lv in forceVectors)
                {
                    sLineLoad sl = new sLineLoad(patternName, eLoadType.DistributedLoad, true, rhcon.TosXYZ(lv));
                    lg.loads.Add(rhcon.EnsureUnit(sl));
                }
                outobj = lg;
            }
            
            DA.SetData(0, outobj);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("ab08b777-3858-4585-b6a4-476de3bd6e65"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.lineLoad;
            }
        }

    }
}
