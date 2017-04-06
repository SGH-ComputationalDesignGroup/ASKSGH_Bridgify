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
    public class TributaryArea_Points : GH_Component
    {

        public TributaryArea_Points()
            : base("AwareTributaryArea_ByPoints", "AwareTributaryArea_ByPoints", "...", "ASKSGH.Bridgify", "sUtility")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; } // | GH_Exposure.obscure;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("boundaryBrep", "boundaryBrep", "Non planar Brep or extreme UV Brep will cause inaccuracy, Currently limited only for a single face planar Brep", GH_ParamAccess.item);
            pManager.AddPointParameter("pointsForLoad", "pointsForLoad", "...", GH_ParamAccess.list);
            pManager.AddVectorParameter("projectTo", "projectTo", "projectTo", GH_ParamAccess.item, Vector3d.Unset);

            Params.Input[2].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("LoadPoints", "LoadPoints", "LoadPoints", GH_ParamAccess.list);
            pManager.AddCurveParameter("TributaryAreaCrvs", "TributaryAreaCrvs", "TributaryAreaCrvs", GH_ParamAccess.list);
            pManager.AddNumberParameter("TributaryArea", "TributaryArea", "TributaryArea", GH_ParamAccess.list);
            pManager.AddVectorParameter("LoadDirection", "LoadDirection", "LoadDirection", GH_ParamAccess.item);
        }

        public static string result = "";
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep b = null;
            List<Point3d> lpts = new List<Point3d>();
            Vector3d projection = Vector3d.Unset;

            if (!DA.GetData(0, ref b)) return;
            if (!DA.GetDataList(1, lpts)) return;
            if (!DA.GetData(2, ref projection)) return;

            List<Curve> voronoi = new List<Curve>();
            List<Point3d> lpps = new List<Point3d>();
            List<double> areas = new List<double>();
            
            projection.Unitize();
            sRhinoConverter rhcon = new sRhinoConverter();
            Vector3d loadDirection = Vector3d.ZAxis * -1;
            if(b.Faces.Count == 1)
            {
                Surface srf = b.Faces[0].ToNurbsSurface();
                Vector3d orinv = srf.NormalAt(srf.Domain(0).Mid, srf.Domain(1).Mid);

                if (srf.IsPlanar())
                {
                    List<Grasshopper.Kernel.Geometry.Voronoi.Cell2> cells = rhcon.GetVoronoiCells(lpts, b);

                    for (int i = 0; i < cells.Count; ++i)
                    {
                        if (cells[i] == null) continue;

                        Curve cellcrv = cells[i].ToPolyline().ToNurbsCurve();
                        Curve pushed = srf.Pushup(cellcrv, 0.001);

                        Point3d selected = Point3d.Unset;
                        //Brep cb = Brep.CreatePlanarBreps(pushed)[0];

                        foreach (Point3d lp in lpts)
                        {
                            Plane ppl = new Plane(lp, orinv);
                            if(pushed.Contains(lp, ppl) == PointContainment.Inside || pushed.Contains(lp, ppl) == PointContainment.Coincident)
                            //if (cb.ClosestPoint(lp).DistanceTo(lp) < 0.005)
                            {
                                selected = lp;
                                break;
                            }
                        }

                        if (selected != Point3d.Unset)
                        {
                            lpps.Add(selected);
                            AreaMassProperties ampOri = AreaMassProperties.Compute(pushed);

                            if (projection != Vector3d.Unset)
                            {
                                Plane plTo = new Plane(ampOri.Centroid, projection);
                                Curve projected = pushed.DuplicateCurve();
                                projected.Transform(Transform.PlanarProjection(plTo));

                                areas.Add(AreaMassProperties.Compute(projected).Area);

                                voronoi.Add(projected);
                                loadDirection = projection;
                            }
                            else
                            {
                                areas.Add(ampOri.Area);

                                voronoi.Add(pushed);
                                loadDirection = orinv;
                            }

                        }

                    }
                    this.Message = "";
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please use a planar brep");
                    this.Message = "Please use a planar brep";
                }
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please use a single face brep");
                this.Message = "Please use a single face brep";
            }


            DA.SetDataList(0, lpps);
            DA.SetDataList(1, voronoi);
            DA.SetDataList(2, areas);
            DA.SetData(3, loadDirection);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("1df3a9b7-0da1-475c-a5ad-45ebb324e813"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.tributary_point;
            }
        }


    }
}
