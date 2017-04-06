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
    public class To_RhinoLines : GH_Component
    {

        public To_RhinoLines()
            : base("sBeam to RhinoLine", "sBeam to RhinoLine", "...", "ASKSGH.Bridgify", "To Rhino")
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
            pManager.AddLineParameter("sBeamLine", "sBeamLine", "sBeamLine", GH_ParamAccess.item);
            pManager.AddGenericParameter("sCrossSection", "sCrossSection", "sCrossSection", GH_ParamAccess.item);
            pManager.AddGenericParameter("sBeamUpvector", "sBeamUpvector", "sBeamUpvector", GH_ParamAccess.item);
            pManager.AddGenericParameter("sLineLoads", "sLineLoads", "sLineLoads", GH_ParamAccess.list);
            pManager.AddGenericParameter("sFixity", "sFixity", "sFixity", GH_ParamAccess.list);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            sBeam sb = null;

            if (!DA.GetData(0, ref sb)) return;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter("Meters", modelUnit);

            List<sFixity> fixs = new List<sFixity>();
            fixs.Add(sb.fixityAtStart);
            fixs.Add(sb.fixityAtEnd);

            List<sLineLoad> lls = new List<sLineLoad>();
            if(sb.lineLoads != null && sb.lineLoads.Count > 0)
            {
                lls = rhcon.EnsureUnit(sb.lineLoads).ToList();
            }

            DA.SetData(0, sb.beamName);
            DA.SetData(1, sb.beamID);
            DA.SetData(2, (Line) rhcon.EnsureUnit( rhcon.ToRhinoLine(sb.axis)));
            DA.SetData(3, sb.crossSection);
            DA.SetData(4, rhcon.ToRhinoVector3d(sb.upVector));
            DA.SetDataList(5, lls);
            DA.SetDataList(6, fixs);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("8629a5f6-7ecb-4a41-9cac-57db0081f414"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.ToRhinoLine;
            }
        }


    }
}
