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
    public class To_RhinoPointsPL : GH_Component
    {

        public To_RhinoPointsPL()
            : base("sPointLoad to RhinoPoint", "sPointLoad to RhinoPoint", "...", "ASKSGH.Bridgify", "To Rhino")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; } // | GH_Exposure.obscure;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sPointLoad", "sPointLoad", "...", GH_ParamAccess.item);
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Point", "Point", GH_ParamAccess.item);
            pManager.AddTextParameter("LoadPattern", "LoadPattern", "LoadPattern", GH_ParamAccess.item);
            pManager.AddVectorParameter("LoadForce", "LoadForce", "LoadForce", GH_ParamAccess.item);
            pManager.AddVectorParameter("LoadMoment", "LoadMoment", "LoadMoment", GH_ParamAccess.item);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            sPointLoad sn = null;

            if (!DA.GetData(0, ref sn)) return;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter("Meters", modelUnit);

            
            DA.SetData(0, rhcon.EnsureUnit(rhcon.ToRhinoPoint3d(sn.location)));
            DA.SetData(1, sn.loadPatternName);
            if(sn.forceVector != null)
            {
                DA.SetData(2, rhcon.EnsureUnit_Force(rhcon.ToRhinoVector3d(sn.forceVector)));
            }
            if(sn.momentVector != null)
            {
                DA.SetData(3, rhcon.EnsureUnit_Moment(rhcon.ToRhinoVector3d(sn.momentVector)));
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("62756a57-85d3-423c-9999-e645b0f80863"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.ToRhinoPoint;
            }
        }


    }
}
