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
    public class To_RhinoCurve : GH_Component
    {

        public To_RhinoCurve()
            : base("sBeamSet to RhinoCurve", "sBeamSet to RhinoCurve", "...", "ASKSGH.Bridgify", "To Rhino")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; } // | GH_Exposure.obscure;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSets", "sBeamSets", "...", GH_ParamAccess.item);
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("sBeamSetName", "sBeamSetName", "sBeamSetName", GH_ParamAccess.item);
            pManager.AddCurveParameter("sBeamSetCurve", "sBeamSetCurve", "sBeamSetCurve", GH_ParamAccess.item);
            pManager.AddGenericParameter("sCrossSection", "sCrossSection", "sCrossSection", GH_ParamAccess.item);
            pManager.AddGenericParameter("sBeams", "sBeams", "sBeams", GH_ParamAccess.list);
            pManager.AddGenericParameter("sLineLoads", "sLineLoads", "sLineLoads", GH_ParamAccess.list);
            pManager.AddGenericParameter("sFixity", "sFixity", "sFixity", GH_ParamAccess.list);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            sBeamSet bs = null;

            if (!DA.GetData(0, ref bs)) return;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter("Meters", modelUnit);

            List<sFixity> fixs = new List<sFixity>();
            //fixs.Add(bs.fixityAtStart);
            //fixs.Add(bs.fixityAtEnd);

            List<sLineLoad> lls = new List<sLineLoad>();
            if(bs.lineLoads != null && bs.lineLoads.Count > 0)
            {
                lls = rhcon.EnsureUnit(bs.lineLoads).ToList();
            }

            DA.SetData(0, bs.beamSetName);
            DA.SetData(1, rhcon.EnsureUnit( rhcon.ToRhinoCurve(bs.parentCrv)));
            DA.SetData(2, bs.crossSection);
            DA.SetDataList(3, bs.beams);
            DA.SetDataList(4, lls);
            DA.SetDataList(5, fixs);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("1ddae607-4825-44c9-a920-19f99c43550c"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.ToRhinoCurve;
            }
        }


    }
}
