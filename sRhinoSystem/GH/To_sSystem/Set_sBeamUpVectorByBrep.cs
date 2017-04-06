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
using Grasshopper.Kernel.Types;

namespace sRhinoSystem.GH.To_sSystem
{
    public class Set_sBeamUpVectorByBrep : GH_Component
    {
        public Set_sBeamUpVectorByBrep()
            : base("Set sBeamElements UpVector ByBrep", "Set sBeamElements UpVector ByBrep", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamElements", "sBeamElements", "...", GH_ParamAccess.list);
            pManager.AddBrepParameter("upvectorGuideBrep", "upvectorGuideBrep", "...", GH_ParamAccess.item);
            pManager.AddNumberParameter("tolerance", "tolerance", "...", GH_ParamAccess.item, 0.05);

            Params.Input[2].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamElements", "sBeamElements", "sBeamElements", GH_ParamAccess.list);
            pManager.AddPointParameter("locations", "locations", "locations", GH_ParamAccess.list);
            pManager.AddVectorParameter("upVectors", "upVectors", "upVectors", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<object> sBeamObjs = new List<object>();
            Brep b = null;
            double tol = 0.05;
            
            if (!DA.GetDataList(0, sBeamObjs)) return;
            if (!DA.GetData(1, ref b)) return;
            if (!DA.GetData(2, ref tol)) return;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");
            sRhinoConverter rhcon_ToRhinoModel = new sRhinoConverter("Meters", modelUnit);

            Brep scaleB = rhcon.EnsureUnit(b);
            
            List<Point3d> pts = new List<Point3d>();
            List<Vector3d> vecs = new List<Vector3d>();

            List<object> duplicated = new List<object>();
            int nonSegmentizedCount = 0;

            this.Message = "";
            foreach (object ob in sBeamObjs)
            {
                GH_ObjectWrapper wap = new GH_ObjectWrapper(ob);
                sBeamSet bs = wap.Value as sBeamSet;
                if (bs != null)
                {
                    sBeamSet dubs = bs.DuplicatesBeamSet();
                    if (dubs.beams.Count > 0)
                    {
                        dubs.EnsureBeamElement();
                        rhcon.AwareBeamUpVectorsOnBrep(ref dubs, scaleB, tol);
                        foreach (sBeam ssb in dubs.beams)
                        {
                            pts.Add(rhcon_ToRhinoModel.EnsureUnit(rhcon.ToRhinoPoint3d(ssb.axis.PointAt(0.5))));
                            vecs.Add(rhcon.ToRhinoVector3d(ssb.upVector));
                        }
                        duplicated.Add(dubs);
                    }
                    else
                    {
                        nonSegmentizedCount++;
                    }
                }
                /*
                sBeam sb = wap.Value as sBeam;
                if(sb != null)
                {
                    sBeam dusb = sb.DuplicatesBeam();
                    Vector3d nv = rhcon.GetNormalVectorAtPointOnBrep(rhcon.ToRhinoPoint3d(dusb.axis.PointAt(0.5)), scaleB, tol);
                    dusb.AwareLocalPlane(rhcon.TosXYZ(nv));
                    duplicated.Add(dusb);
                }
                */
            }
          
            if(nonSegmentizedCount == 0)
            {
                DA.SetDataList(0, duplicated);
                DA.SetDataList(1, pts);
                DA.SetDataList(2, vecs);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Segmentize Beam Set First To Assign Upvectors");
                //this.Message = "Segmentize Beam Set First To Assign Upvectors";
            }
            
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("bd69c78b-e1c2-45ee-ad72-1032e9481898"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.upvector_ByBrep;
            }
        }

    }
}
