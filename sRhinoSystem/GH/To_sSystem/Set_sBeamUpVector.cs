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
    public class Set_sBeamUpVector : GH_Component
    {
        public Set_sBeamUpVector()
            : base("Set sBeamSet UpVector", "Set sBeamSet UpVector", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSet", "sBeamSet", "...", GH_ParamAccess.list);
            pManager.AddVectorParameter("upvectors", "upvectors", "...", GH_ParamAccess.list);
            // Params.Input[0].Optional = true; 
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSet", "sBeamSet", "sBeamSet", GH_ParamAccess.list);
            pManager.AddPointParameter("locations", "locations", "locations", GH_ParamAccess.list);
            pManager.AddVectorParameter("upVectors", "upVectors", "upVectors", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<sBeamSet> beamsets = new List<sBeamSet>();
            List<Vector3d> upvectors = new List<Vector3d>();
            
            if (!DA.GetDataList(0, beamsets)) return;
            if (!DA.GetDataList(1, upvectors)) return;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");
            sRhinoConverter rhcon_ToRhinoModel = new sRhinoConverter("Meters", modelUnit);

            List<sBeamSet> duplicated = new List<sBeamSet>();
            List<Point3d> pts = new List<Point3d>();
            List<Vector3d> vecs = new List<Vector3d>();

            this.Message = "";
            int nonSegCount = 0;
            if (upvectors.Count == beamsets.Count)
            {
                for (int i = 0; i < beamsets.Count; ++i)
                {
                    if (beamsets[i].beams.Count > 0)
                    {
                        sXYZ upvecThis = rhcon.TosXYZ(upvectors[i]);
                        sBeamSet dubs = beamsets[i].DuplicatesBeamSet();
                        dubs.EnsureBeamElement();

                        foreach (sBeam sb in dubs.beams)
                        {
                            sb.AwareLocalPlane(upvecThis);

                            pts.Add(rhcon_ToRhinoModel.EnsureUnit(rhcon.ToRhinoPoint3d(sb.axis.PointAt(0.5))));
                            vecs.Add(rhcon.ToRhinoVector3d(sb.upVector));
                        }
                        duplicated.Add(dubs);
                    }
                    else
                    {
                        nonSegCount++;
                    }

                }
            }
            else if(upvectors.Count == 1)
            {
                foreach (sBeamSet bs in beamsets)
                {
                    if(bs.beams.Count > 0)
                    {
                        sBeamSet dubs = bs.DuplicatesBeamSet();
                        dubs.EnsureBeamElement();

                        foreach (sBeam sb in dubs.beams)
                        {
                            sb.AwareLocalPlane(rhcon.TosXYZ(upvectors[0]));

                            pts.Add(rhcon_ToRhinoModel.EnsureUnit(rhcon.ToRhinoPoint3d(sb.axis.PointAt(0.5))));
                            vecs.Add(rhcon.ToRhinoVector3d(sb.upVector));
                        }
                        duplicated.Add(dubs);
                    }
                    else
                    {
                        nonSegCount++;
                    }
                }
            }
            else
            {
                this.Message = "";
                return;
            }

            if (nonSegCount == 0)
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
            get { return new Guid("e779724c-3ac3-432b-ba29-4e8512c10c48"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.upvector;
            }
        }

    }
}
