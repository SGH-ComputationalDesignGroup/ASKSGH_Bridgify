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
using System.IO;
using sRhinoSystem.Properties;

namespace sRhinoSystem.GH.ToRhinoSystem
{
    public class To_RhinoBrep : GH_Component
    {

        public To_RhinoBrep()
            : base("sBeam to Brep", "sBeam to Brep", "...", "ASKSGH.Bridgify", "To Rhino")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; } // | GH_Exposure.obscure;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeams", "sBeams", "...", GH_ParamAccess.item);
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("sBeamName", "sBeamName", "sBeamName", GH_ParamAccess.item);
            pManager.AddIntegerParameter("sBeamID", "sBeamID", "sBeamID", GH_ParamAccess.item);
            pManager.AddBrepParameter("sBeamBrep", "sBeamBrep", "sBeamBrep", GH_ParamAccess.item);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            sBeam sb = null;

            if (!DA.GetData(0, ref sb)) return;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter("Meters", modelUnit);

            Brep bb = rhcon.EnsureUnit(rhcon.ToRhinoBeamPreview(sb)) as Brep;

            DA.SetData(0, sb.beamName);
            DA.SetData(1, sb.beamID);
            DA.SetData(2, bb);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("01fd5707-1d50-4c81-9118-6576188eaf1e"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.ToRhinoBrep;
            }
        }


    }
}
