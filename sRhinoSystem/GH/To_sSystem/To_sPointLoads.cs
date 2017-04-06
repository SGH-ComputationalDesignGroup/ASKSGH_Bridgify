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
using sRhinoSystem.Properties;

namespace sRhinoSystem.GH.To_sSystem
{
    public class To_sPointLoad : GH_Component
    {
        public To_sPointLoad()
            : base("sPointLoad", "sPointLoad", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("point", "point", "...", GH_ParamAccess.item);
            pManager.AddTextParameter("patternName", "patternName", "...", GH_ParamAccess.item);
            pManager.AddVectorParameter("forceVector(N|lbf)", "forceVector(N|lbf)", "...", GH_ParamAccess.item, Vector3d.Unset);
            pManager.AddVectorParameter("momentVector(N|lbf)", "momentVector(N|lbf)", "...", GH_ParamAccess.item, Vector3d.Unset);

            Params.Input[2].Optional = true;
            Params.Input[3].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sPointLoad", "sPointLoad", "sPointLoad", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d point = Point3d.Unset;
            string pattern = "";
            Vector3d force = Vector3d.Unset;
            Vector3d moment = Vector3d.Unset;

            if (!DA.GetData(0, ref point)) return;
            if (!DA.GetData(1, ref pattern)) return;
            DA.GetData(2, ref force);
            DA.GetData(3, ref moment);

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");

            int count = 0;

            sPointLoad pl = new sPointLoad();
            pl.location = rhcon.TosXYZ(rhcon.EnsureUnit(point));

            pl.loadPatternName = pattern;
            if (force != Vector3d.Unset)
            {
                pl.forceVector = rhcon.TosXYZ(force);
                count++;
            }
            
            if(moment != Vector3d.Unset)
            {
                pl.momentVector = rhcon.TosXYZ(moment);
                count++;
            }

            if(count > 0)
            {
                DA.SetData(0, rhcon.EnsureUnit(pl));
            }
            else
            {
                DA.SetData(0, null);
            }

        }

        public override Guid ComponentGuid
        {
            get { return new Guid("bf9de78d-851f-4b55-a2b6-136f16a30d09"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.PointLoad;
            }
        }

    }
}
