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
using Grasshopper.Kernel.Data;

namespace sRhinoSystem.GH.ToRhinoSystem
{
    public class TributaryArea_Lines : GH_Component
    {

        public TributaryArea_Lines()
            : base("AwareTributaryArea_ByLines", "AwareTributaryArea_ByLines", "...", "ASKSGH.Bridgify", "sUtility")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; } // | GH_Exposure.obscure;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("boundaryBrep", "boundaryBrep", "Non planar Brep or extreme UV Brep will cause inaccuracy, Currently limited only for a single face planar Brep", GH_ParamAccess.item);
            pManager.AddLineParameter("linesForLoad", "linesForLoad", "...", GH_ParamAccess.list);
            pManager.AddNumberParameter("segmentLength", "segmentLength", "segmentLength", GH_ParamAccess.item, -1.0);
            pManager.AddVectorParameter("projectTo", "projectTo", "projectTo", GH_ParamAccess.item, Vector3d.Unset);

            Params.Input[2].Optional = true;
            Params.Input[3].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("LoadLines", "LoadLines", "LoadLines", GH_ParamAccess.list);
            pManager.AddCurveParameter("TributaryAreaCrvs", "TributaryAreaCrvs", "TributaryAreaCrvs", GH_ParamAccess.tree);
            pManager.AddNumberParameter("TributaryArea", "TributaryArea", "TributaryArea", GH_ParamAccess.tree);
            pManager.AddIntervalParameter("TributaryParam", "TributaryParam", "TributaryParam", GH_ParamAccess.tree);
            pManager.AddVectorParameter("LoadDirection", "LoadDirection", "LoadDirection", GH_ParamAccess.item);
        }

        public static string result = "";
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Brep b = null;
            List<Line> lns = new List<Line>();
            double segLength = -1.0;
            Vector3d projection = Vector3d.Unset;

            if (!DA.GetData(0, ref b)) return;
            if (!DA.GetDataList(1, lns)) return;
            if (!DA.GetData(2, ref segLength)) return;
            if (!DA.GetData(3, ref projection)) return;
            
            projection.Unitize();
            sRhinoConverter rhcon = new sRhinoConverter();
            Vector3d loadDirection = Vector3d.ZAxis * -1;

            List<Line> beamlns = new List<Line>();
            DataTree<Curve> voronois = new DataTree<Curve>();
            DataTree<double> triAreas = new DataTree<double>();
            DataTree<Interval> parameters = new DataTree<Interval>();

            if (b.Faces.Count == 1)
            {
                Surface srf = b.Faces[0].ToNurbsSurface();
                Vector3d orinv = srf.NormalAt(srf.Domain(0).Mid, srf.Domain(1).Mid);

                if (srf.IsPlanar())
                {
                    List<Point3d> lpts = new List<Point3d>();
                    foreach(Line l in lns)
                    {
                        int den = 0;
                        Curve bc = l.ToNurbsCurve();
                        if(segLength > 0.0)
                        {
                            den = (int)(bc.GetLength() / segLength);
                        }
                        if (den < 3) den = 3;

                        Point3d[] segPts;
                        bc.DivideByCount(den, false, out segPts);
                        for(int i = 0; i < segPts.Length; ++i)
                        {
                            lpts.Add(segPts[i]);
                        }
                        lpts.Add(bc.PointAtLength(0.05));
                        lpts.Add(bc.PointAtLength(bc.GetLength() - 0.05));
                    }

                    List<Grasshopper.Kernel.Geometry.Voronoi.Cell2> cells = rhcon.GetVoronoiCells(lpts, b);

                    int branchID = 0;
                    foreach(Line ll in lns)
                    {
                        GH_Path bpth = new GH_Path(branchID);
                        beamlns.Add(ll);

                        Curve checkCrv = GetShortenBeamAxis(ll, 0.05).ToNurbsCurve();
                        checkCrv.Domain = new Interval(0.0, 1.0);
                        for (int i = 0; i < cells.Count; ++i)
                        {
                            if (cells[i] == null) continue;

                            Curve cellcrv = cells[i].ToPolyline().ToNurbsCurve();
                            Curve pushedCell = srf.Pushup(cellcrv, 0.001);

                            Rhino.Geometry.Intersect.CurveIntersections cin = Rhino.Geometry.Intersect.Intersection.CurveCurve(checkCrv, pushedCell, 0.005, 0.005);
                            if (cin != null && cin.Count > 0)
                            {
                                AreaMassProperties amp = AreaMassProperties.Compute(pushedCell);

                                double areaThis = amp.Area;

                                if(projection != Vector3d.Unset)
                                {
                                    Plane plTo = new Plane(amp.Centroid, projection);
                                    Curve projected = pushedCell.DuplicateCurve();
                                    projected.Transform(Transform.PlanarProjection(plTo));

                                    AreaMassProperties ampProjected = AreaMassProperties.Compute(projected);
                                    
                                    voronois.Add(projected, bpth);
                                    triAreas.Add(ampProjected.Area, bpth);
                                    loadDirection = projection;
                                }
                                else
                                {
                                    voronois.Add(pushedCell, bpth);
                                    triAreas.Add(areaThis, bpth);
                                    loadDirection = orinv;
                                }

                                Interval inv = Interval.Unset;
                                if (cin.Count == 1)
                                {
                                    if(cin[0].ParameterA < 0.5)
                                    {
                                        inv = new Interval(0.0, cin[0].ParameterA);
                                    }
                                    else
                                    {
                                        inv = new Interval(cin[0].ParameterA, 1.0);
                                    }
                                }
                                else if(cin.Count == 2)
                                {
                                     inv = new Interval(cin[0].ParameterA, cin[1].ParameterA);
                                }
                                parameters.Add(inv, bpth);
                            }
                        }

                        branchID++;
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


            DA.SetDataList(0, beamlns);
            DA.SetDataTree(1, voronois);
            DA.SetDataTree(2, triAreas);
            DA.SetDataTree(3, parameters);
            DA.SetData(4, loadDirection);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("cb9001d3-85e9-49b2-97bb-1a6b60f12bae"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.tributary_line;
            }
        }

        internal Line GetShortenBeamAxis(Line bln, double lenFactor)
        {
            Vector3d dir = bln.Direction;
            dir.Unitize();
            dir *= (lenFactor * 1.5);

            return new Line(bln.From + dir, bln.To - dir);
        }
    }
}
