using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject;
using sDataObject.sGeometry;
using sDataObject.sElement;
using Rhino.Geometry;
using System.Drawing;
using sDataObject.IElement;

namespace sRhinoSystem
{
    public class sRhinoConverter
    {
        public string baseUnit { get; set; } 
        public string targetUnit { get; set; } 

        public sRhinoConverter()
        {
            this.baseUnit = "";
            this.targetUnit = "";
        }
        public sRhinoConverter(string baseU, string targetU)
        {
            this.baseUnit = baseU;
            this.targetUnit = targetU;
        }

        private BoundingBox GetRhinoBoundingBox(List<IsObject> objs)
        {
            List<Point3d> pts = new List<Point3d>();

            foreach(IsObject so in objs)
            {
                if(so is sFrameSet)
                {
                    sFrameSet sb = so as sFrameSet;
                    Curve rc = ToRhinoCurve(sb.parentCrv);
                    Point3d[] dpts;
                    rc.DivideByCount(3, true, out dpts);
                    for(int i = 0; i < dpts.Length; ++i)
                    {
                        pts.Add(dpts[i]);
                    }
                }
                if (so is sMesh)
                {
                    sMesh sm = so as sMesh;
                    foreach (sVertex sv in sm.vertices)
                    {
                        pts.Add(ToRhinoPoint3d(sv.location));
                    }
                }
            }
            return new BoundingBox(pts);
        }
        public sBoundingBox TosBoundingBox(List<IsObject> objs)
        {
            return TosBoundingBox(GetRhinoBoundingBox(objs));
        }
        public sBoundingBox TosBoundingBox(BoundingBox rbox)
        {
            sBoundingBox sbox = new sBoundingBox();

            sbox.min = TosXYZ(EnsureUnit(rbox.Min));
            sbox.max = TosXYZ(EnsureUnit( rbox.Max));
            sbox.diagonal = TosXYZ(EnsureUnit(rbox.Diagonal));
            sbox.center = TosXYZ(EnsureUnit(rbox.Center));

            sbox.xSize = new sRange(sbox.min.X , sbox.max.X);
            sbox.ySize = new sRange(sbox.min.Y, sbox.max.Y);
            sbox.zSize = new sRange(sbox.min.Z, sbox.max.Z);

            return sbox;
        }


        ////////////////////////////////////////////////////////////////////////////////////
        ///Need to Clean up here............................................................
        ////////////////////////////////////////////////////////////////////////////////////
        public T EnsureUnit_test<T>(object rhGeo)
        {
            GeometryBase geo = rhGeo as GeometryBase;
            if (targetUnit == baseUnit)
            {
                return (T)Convert.ChangeType(geo, typeof(T));
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 3.280841666667));
                    return (T)Convert.ChangeType(rhGeo, typeof(T));
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 0.304800164592));
                    return (T)Convert.ChangeType(rhGeo, typeof(T));
                }
                else
                {
                    return default(T);
                }
            }
        }
        public object EnsureUnit_test(object rhGeo)
        {
            GeometryBase geo = rhGeo as GeometryBase;
            if (geo == null) return null;

            if (targetUnit != baseUnit)
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 3.280841666667));
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 0.304800164592));
                }
            }
            return geo;
        }

        public Point3d EnsureUnit(Point3d geo)
        {
            if(targetUnit == baseUnit)
            {
                return geo;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin,3.280841666667));
                    return geo;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 0.304800164592));
                    return geo;
                }
                else
                {
                    return Point3d.Unset;
                }
            }
        }
        public Vector3d EnsureUnit(Vector3d geo)
        {
            if (targetUnit == baseUnit)
            {
                return geo;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 3.280841666667));
                    return geo;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 0.304800164592));
                    return geo;
                }
                else
                {
                    return Vector3d.Unset;
                }
            }
        }
        public Brep EnsureUnit(Brep geo)
        {
            if (targetUnit == baseUnit)
            {
                return geo;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 3.280841666667));
                    return geo;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 0.304800164592));
                    return geo;
                }
                else
                {
                    return null;
                }
            }
        }
        public Mesh EnsureUnit(Mesh geo)
        {
            if (targetUnit == baseUnit)
            {
                return geo;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 3.280841666667));
                    return geo;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 0.304800164592));
                    return geo;
                }
                else
                {
                    return null;
                }
            }
        }
        public Line EnsureUnit(Line geo)
        {
            if (targetUnit == baseUnit)
            {
                return geo;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 3.280841666667));
                    return geo;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 0.304800164592));
                    return geo;
                }
                else
                {
                    return Line.Unset;
                }
            }
        }
        public Polyline EnsureUnit(Polyline geo)
        {
            if (targetUnit == baseUnit)
            {
                return geo;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 3.280841666667));
                    return geo;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 0.304800164592));
                    return geo;
                }
                else
                {
                    return null;
                }
            }
        }
        public NurbsCurve EnsureUnit(NurbsCurve geo)
        {
            if (targetUnit == baseUnit)
            {
                return geo;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 3.280841666667));
                    return geo;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 0.304800164592));
                    return geo;
                }
                else
                {
                    return null;
                }
            }
        }
        public Curve EnsureUnit(Curve geo)
        {
            if (targetUnit == baseUnit)
            {
                return geo;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 3.280841666667));
                    return geo;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    geo.Transform(Transform.Scale(Point3d.Origin, 0.304800164592));
                    return geo;
                }
                else
                {
                    return null;
                }
            }
        }
        public sPointLoad EnsureUnit(sPointLoad pl)
        {
            if (targetUnit == baseUnit)
            {
                return pl;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    pl.ScaleByFactor(0.224809);
                    return pl;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    pl.ScaleByFactor(4.44822);
                    return pl;
                }
                else
                {
                    return null;
                }
            }
        }
        public List<sPointLoad> EnsureUnit(List<sPointLoad> pls)
        {
            if (targetUnit == baseUnit)
            {
                return pls;
            }
            else
            {
                List<sPointLoad> plsU = new List<sPointLoad>();
                foreach(sPointLoad l in pls)
                {
                    plsU.Add(EnsureUnit(l));
                }
                return plsU;
            }
        }
        public sLineLoad EnsureUnit(sLineLoad ll)
        {
            if (targetUnit == baseUnit)
            {
                return ll;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    ll.ScaleByFactor(0.06852);
                    return ll;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    ll.ScaleByFactor(14.59383);
                    return ll;
                }
                else
                {
                    return null;
                }
            }
        }
        public List<sLineLoad> EnsureUnit(List<sLineLoad> lls)
        {
            if (targetUnit == baseUnit)
            {
                return lls;
            }
            else
            {
                List<sLineLoad> llsU = new List<sLineLoad>();
                foreach(sLineLoad l in lls)
                {
                    llsU.Add(EnsureUnit(l));
                }
                return llsU;
            }
        }
        public double EnsureUnit(double val, eColorMode dataType, bool MPaConvert = false)
        {
            double valEnsured = val;
            switch (dataType)
            {
                case eColorMode.Deflection:
                    valEnsured = EnsureUnit_Deflection(val);
                    break;
                case eColorMode.Force_X:
                    valEnsured = EnsureUnit_Force(val);
                    break;
                case eColorMode.Force_Y:
                    valEnsured = EnsureUnit_Force(val);
                    break;
                case eColorMode.Force_Z:
                    valEnsured = EnsureUnit_Force(val);
                    break;
                case eColorMode.Moment_X:
                    valEnsured = EnsureUnit_Moment(val);
                    break;
                case eColorMode.Moment_Y:
                    valEnsured = EnsureUnit_Moment(val);
                    break;
                case eColorMode.Moment_Z:
                    valEnsured = EnsureUnit_Moment(val);
                    break;
                case eColorMode.Stress_Axial_X:
                    valEnsured = EnsureUnit_Stress(val, MPaConvert);
                    break;
                case eColorMode.Stress_Axial_Y:
                    valEnsured = EnsureUnit_Stress(val, MPaConvert);
                    break;
                case eColorMode.Stress_Axial_Z:
                    valEnsured = EnsureUnit_Stress(val, MPaConvert);
                    break;
                case eColorMode.Stress_Moment_X:
                    valEnsured = EnsureUnit_Stress(val, MPaConvert);
                    break;
                case eColorMode.Stress_Moment_Y:
                    valEnsured = EnsureUnit_Stress(val, MPaConvert);
                    break;
                case eColorMode.Stress_Moment_Z:
                    valEnsured = EnsureUnit_Stress(val, MPaConvert);
                    break;
                case eColorMode.Stress_Combined_Absolute:
                    valEnsured = EnsureUnit_Stress(val, MPaConvert);
                    break;
            }
            
            return valEnsured;
        }
        public double EnsureUnit(double dim)
        {
            if (targetUnit == baseUnit)
            {
                return dim;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return (dim * 3.28084);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return (dim * 0.3048);
                }
                else
                {
                    return 0.0;
                }
            }
        }
        //mm <> in
        public double EnsureUnit_Deflection(double val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return (val * 0.03937011);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return (val * 25.4);
                }
                else
                {
                    return 0.0;
                }
            }
            
        }
        
        /// MPa <> Ksi
        public double EnsureUnit_Stress(double val, bool MPaConvert = false)
        {
            if (targetUnit == baseUnit)
            {
                if(targetUnit == "Meters" && MPaConvert)
                {
                    return val / 1.0E6;
                }
                else
                {
                    return val;
                }
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return (val * 1.45037738007E-7);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return (val * 6894757.280010371);
                }
                else
                {
                    return 0.0;
                }
            }
        }
        //N <> lbs
        public double EnsureUnit_Force(double val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return (val * 0.224809);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return (val * 4.44822);
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public Vector3d EnsureUnit_Force(Vector3d val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return (val * 0.224809);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return (val * 4.44822);
                }
                else
                {
                    return Vector3d.Zero;
                }
            }
        }
        // N.m <> lb.ft
        public double EnsureUnit_Moment(double val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return (val * 0.7375610332);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return (val * 1.35582);
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public Vector3d EnsureUnit_Moment(Vector3d val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return (val * 0.7375610332);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return (val * 1.35582);
                }
                else
                {
                    return Vector3d.Zero;
                }
            }
        }
        public double EnsureUnit_Volume(double val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return (val * 35.3147);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return (val * 0.0283168);
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public double EnsureUnit_Area(double val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return (val * 10.7639);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return (val * 0.092903);
                }
                else
                {
                    return 0.0;
                }
            }
        }
        public Interval EnsureUnit_DataRange(Interval ran, eColorMode colMode, bool MPaConvert = false)
        {
            Interval urn = new Interval(EnsureUnit(ran.Min, colMode, MPaConvert), EnsureUnit(ran.Max, colMode, MPaConvert));
            
           //if(colMode == eColorMode.Deflection)
           //{
           //    urn = new Interval(EnsureUnit_Deflection(ran.Min), EnsureUnit_Deflection(ran.Max));
           //}
           //else if(colMode == eColorMode.Force_X)
           //{
           //    urn = new Interval(EnsureUnit_Force(ran.Min), EnsureUnit_Force(ran.Max));
           //}
           //else if (colMode == eColorMode.Force_Y)
           //{
           //    urn = new Interval(EnsureUnit_Force(ran.Min), EnsureUnit_Force(ran.Max));
           //}
           //else if (colMode == eColorMode.Force_Z)
           //{
           //    urn = new Interval(EnsureUnit_Force(ran.Min), EnsureUnit_Force(ran.Max));
           //}
           //else if (colMode == eColorMode.Moment_X)
           //{
           //    urn = new Interval(EnsureUnit_Moment(ran.Min), EnsureUnit_Moment(ran.Max));
           //}
           //else if (colMode == eColorMode.Moment_Y)
           //{
           //    urn = new Interval(EnsureUnit_Moment(ran.Min), EnsureUnit_Moment(ran.Max));
           //}
           //else if (colMode == eColorMode.Moment_Z)
           //{
           //    urn = new Interval(EnsureUnit_Moment(ran.Min), EnsureUnit_Moment(ran.Max));
           //}
           //
           //else if (colMode == eColorMode.Stress_Axial_X)
           //{
           //    urn = new Interval(EnsureUnit_Stress(ran.Min), EnsureUnit_Stress(ran.Max));
           //}
           //else if (colMode == eColorMode.Stress_Axial_Y)
           //{
           //    urn = new Interval(EnsureUnit_Stress(ran.Min), EnsureUnit_Stress(ran.Max));
           //}
           //else if (colMode == eColorMode.Stress_Axial_Z)
           //{
           //    urn = new Interval(EnsureUnit_Stress(ran.Min), EnsureUnit_Stress(ran.Max));
           //}
           //else if (colMode == eColorMode.Stress_Moment_X)
           //{
           //    urn = new Interval(EnsureUnit_Stress(ran.Min), EnsureUnit_Stress(ran.Max));
           //}
           //else if (colMode == eColorMode.Stress_Moment_Y)
           //{
           //    urn = new Interval(EnsureUnit_Stress(ran.Min), EnsureUnit_Stress(ran.Max));
           //}
           //else if (colMode == eColorMode.Stress_Moment_Z)
           //{
           //    urn = new Interval(EnsureUnit_Stress(ran.Min), EnsureUnit_Stress(ran.Max));
           //}
           //else if (colMode == eColorMode.Stress_Combined_Absolute)
           //{
           //    urn = new Interval(EnsureUnit_Stress(ran.Min), EnsureUnit_Stress(ran.Max));
           //}
            return urn;
        }

        public Brep ToRhinoBeamPreview(sFrame sb)
        {
            Curve profileS = ToRhinoCrosssectionProfile(sb, 0.0);
            Curve profileE = ToRhinoCrosssectionProfile(sb, 1.0);

            Curve[] profiles = new Curve[2] { profileS, profileE };
            return Brep.CreateFromLoft(profiles, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0];
        }

        //?????????????
        public List<Curve> ToRhinoCrosssectionProfile(sFrameSet bs, double lenTol)
        {
            List<Curve> profiles = new List<Curve>();

            for(int i = 0; i < bs.frames.Count; ++i)
            {

            }

            return profiles;
        }
        public Curve ToRhinoCrosssectionProfile(sFrame sb, double t)
        {
            sCrossSection scs = sb.crossSection;
            List<sXYZ> vertice = new List<sXYZ>();
            sPlane secPlane = new sPlane(sb.axis.PointAt(t), sb.localPlane.Xaxis, sb.localPlane.Yaxis);

            if (scs.sectionType == eSectionType.AISC_I_BEAM)
            {
                vertice = scs.GetWbeamFaceVertices(secPlane).ToList();
            }
            else if(scs.sectionType == eSectionType.HSS_REC)
            {
                vertice = scs.GetHSSRecFaceVertices_Simple(secPlane);
            }
            else if (scs.sectionType == eSectionType.HSS_ROUND)
            {
                vertice = scs.GetHSSRoundFaceVertices_Simple(secPlane);
            }
            else if (scs.sectionType == eSectionType.RECTANGLAR)
            {
                vertice = scs.GetRecFaceVertices_Simple(secPlane);
            }
            else if (scs.sectionType == eSectionType.SQUARE)
            {
                vertice = scs.GetSquareFaceVertices_Simple(secPlane);
            }
            else if (scs.sectionType == eSectionType.ROUND)
            {
                vertice = scs.GetRoundFaceVertices_Simple(secPlane);
            }

            Polyline pl = new Polyline();
            for(int i = 0; i < vertice.Count; ++i)
            {
                pl.Add(this.ToRhinoPoint3d(vertice[i]));
            }
            pl.Add(this.ToRhinoPoint3d(vertice[0]));
            Curve profile = pl.ToNurbsCurve();

            return profile;
        }

        public Interval ToRhinoInterval(sRange sran)
        {
            return new Interval(sran.min, sran.max);
        }
        public sRange TosRange(Interval rhintv)
        {
            return new sRange(rhintv.Min, rhintv.Max);
        }

        public Point3d ToRhinoPoint3d(sXYZ sxyz)
        {
          return new Point3d(sxyz.X, sxyz.Y, sxyz.Z);
        }
        public sXYZ TosXYZ(Point3d rhp)
        {
            return new sXYZ(rhp.X, rhp.Y, rhp.Z);
        }

        public Vector3d ToRhinoVector3d(sXYZ sxyz)
        {
            return new Vector3d(sxyz.X, sxyz.Y, sxyz.Z);
        }
        public sXYZ TosXYZ(Vector3d rhv)
        {
            return new sXYZ(rhv.X, rhv.Y, rhv.Z);
        }

        public Plane ToRhinoPlane(sPlane spl)
        {
            return new Plane(ToRhinoPoint3d(spl.origin), ToRhinoVector3d(spl.Xaxis), ToRhinoVector3d(spl.Yaxis));
        }
        public sPlane TosPlane(Plane rhpl)
        {
            return new sPlane(TosXYZ(rhpl.Origin), TosXYZ(rhpl.XAxis), TosXYZ(rhpl.YAxis));
        }

        public Line ToRhinoLine(sLine sl)
        {
            return new Line(ToRhinoPoint3d(sl.startPoint), ToRhinoPoint3d(sl.endPoint));
        }
        public sLine TosLine(Line rhln)
        {
            return new sLine(TosXYZ(rhln.From), TosXYZ(rhln.To));
        }
        public List<sLine> TosLines(Polyline rhpl)
        {
            List<sLine> lns = new List<sLine>();
            for(int i = 0; i < rhpl.SegmentCount; ++i)
            {
                lns.Add(TosLine(rhpl.SegmentAt(i)));
            }
            return lns;
        }
        public List<sLine> TosLines(Curve c, double maxLenTol)
        {
            List<sLine> lns = new List<sLine>();
            PolylineCurve pc = c.ToPolyline(0, 0, 0.0, 0.0, 0.0, 0.0, 0.0, maxLenTol, true);
            Curve [] segs = pc.DuplicateSegments();
            for(int i = 0; i < segs.Length; ++i)
            {
                lns.Add(TosLine(new Line(segs[i].PointAtStart, segs[i].PointAtEnd)));
            }
            return lns;
        }
        public NurbsCurve ToRhinoNurbsCurve(sNurbsCurve snc)
        {
            NurbsCurve nnc = new NurbsCurve(snc.degree, snc.controlPoints.Count);
            for (int i = 0; i < snc.knots.Count; ++i)
            {
                nnc.Knots[i] = snc.knots[i];
            }
            for (int i = 0; i < snc.controlPoints.Count; ++i)
            {
                Point3d cloc = new Point3d(snc.controlPoints[i].X, snc.controlPoints[i].Y, snc.controlPoints[i].Z);
                nnc.Points[i] = new ControlPoint(cloc, snc.weights[i]);
            }
            return nnc;
        }
        public sNurbsCurve TosNurbsCurve(NurbsCurve rnc)
        {
            List<sXYZ> cpts = new List<sXYZ>();
            List<double> wes = new List<double>();
            List<double> ks = new List<double>();
            foreach(ControlPoint cp in rnc.Points)
            {
                cpts.Add(this.TosXYZ(cp.Location));
                wes.Add(cp.Weight);
            }
            foreach(double k in rnc.Knots)
            {
                ks.Add(k);
            }
            return new sNurbsCurve(cpts, wes, ks, rnc.GetLength(), rnc.Degree);
        }
        public Polyline ToRhinoPolyline(sPolyLine spl)
        {
            Polyline rpl = new Polyline();
            foreach(sXYZ v in spl.vertice)
            {
                rpl.Add(ToRhinoPoint3d(v));
            }
            if (spl.isClosed)
            {
                rpl.Add(ToRhinoPoint3d(spl.vertice[0]));
            }
            return rpl;
        }
        public sPolyLine TosPolyline(Polyline rpl)
        {
            List<sXYZ> svs = new List<sXYZ>();
            for(int i = 0; i < rpl.Count-1; ++i)
            {
                svs.Add(TosXYZ(rpl[i]));
            }
            if(rpl.IsClosed == false)
            {
                svs.Add(TosXYZ(rpl[rpl.Count-1]));
            }

            sPolyLine spl = new sPolyLine(svs, rpl.IsClosed);
            return spl;
        }
        public Curve ToRhinoCurve(sCurve sc)
        {
            Curve rc = null;
            if(sc.curveType == eCurveType.LINE)
            {
                rc = ToRhinoLine(sc as sLine).ToNurbsCurve();
            }
            else if(sc.curveType == eCurveType.POLYLINE)
            {
                rc = ToRhinoPolyline(sc as sPolyLine).ToNurbsCurve();
            }
            else if(sc.curveType == eCurveType.NURBSCURVE)
            {
                rc = ToRhinoNurbsCurve(sc as sNurbsCurve);
            }
            return rc;
        }
        public sCurve TosCurve(object rc)
        {
            sCurve sc = null;
            Curve c = rc as Curve;
            if (c != null)
            {
                if (c.Degree == 3 || c.Degree == 2)
                {
                    sc = TosNurbsCurve(c as NurbsCurve);
                    sc.curveType = eCurveType.NURBSCURVE;
                }
                else
                {
                    Curve[] segs = c.DuplicateSegments();
                    if (segs.Length > 0)
                    {
                        Polyline rpl;
                        c.TryGetPolyline(out rpl);
                        sc = TosPolyline(rpl);
                        sc.curveType = eCurveType.POLYLINE;
                    }
                    else
                    {
                        Line rl = new Line(c.PointAtStart, c.PointAtEnd);
                        sc = TosLine(rl);
                        sc.curveType = eCurveType.LINE;
                    }
                }
            }

            return sc;
        }

        public Mesh ToRhinoMesh(sMesh sm)
        {
            Mesh m = new Mesh();
            for (int i = 0; i < sm.vertices.Count; ++i)
            {
                m.Vertices.SetVertex(i, sm.vertices[i].location.X, sm.vertices[i].location.Y, sm.vertices[i].location.Z);
                if (sm.vertices[i].color != null)
                {
                    Color vcol = sm.vertices[i].color.ToWinColor();
                    m.VertexColors.SetColor(i, vcol);
                }
            }
            for (int i = 0; i < sm.faces.Count; ++i)
            {
                m.Faces.SetFace(i, sm.faces[i].A, sm.faces[i].B, sm.faces[i].C);
            }
            m.FaceNormals.ComputeFaceNormals();
            m.Normals.ComputeNormals();
            return m;
        }
        public sMesh TosMesh(Mesh rhm)
        {
            Mesh rm = EnsureTriangulated(rhm);
            rm.FaceNormals.ComputeFaceNormals();
            rm.Normals.ComputeNormals();

            sMesh sm = new sMesh();

            for (int i = 0; i < rm.Vertices.Count; ++i)
            {
                sm.SetVertex(i, new sXYZ(rm.Vertices[i].X, rm.Vertices[i].Y, rm.Vertices[i].Z));
                sm.vertices[i].normal = this.TosXYZ( rm.Normals[i]);
                if (rm.VertexColors[i] != null)
                {
                    sm.vertices[i].color = sColor.FromWinColor(rm.VertexColors[i]);
                }
            }

            for (int i = 0; i < rm.Faces.Count; ++i)
            {
                sm.SetFace(i, rm.Faces[i].A, rm.Faces[i].B, rm.Faces[i].C);
                sm.faces[i].normal = this.TosXYZ(rm.FaceNormals[i]);
            }

            return sm;
        }
        public Mesh EnsureTriangulated(Mesh rhm)
        {
            int facecount = rhm.Faces.Count;
            for (int i = 0; i < facecount; i++)
            {
                var mf = rhm.Faces[i];
                if (mf.IsQuad)
                {
                    double dist1 = rhm.Vertices[mf.A].DistanceTo(rhm.Vertices[mf.C]);
                    double dist2 = rhm.Vertices[mf.B].DistanceTo(rhm.Vertices[mf.D]);
                    if (dist1 > dist2)
                    {
                        rhm.Faces.AddFace(mf.A, mf.B, mf.D);
                        rhm.Faces.AddFace(mf.B, mf.C, mf.D);
                    }
                    else
                    {
                        rhm.Faces.AddFace(mf.A, mf.B, mf.C);
                        rhm.Faces.AddFace(mf.A, mf.C, mf.D);
                    }
                }
            }

            var newfaces = new List<MeshFace>();
            foreach (var mf in rhm.Faces)
            {
                if (mf.IsTriangle) newfaces.Add(mf);
            }

            rhm.Faces.Clear();
            rhm.Faces.AddFaces(newfaces);
            return rhm;
        }

        public List<Grasshopper.Kernel.Geometry.Voronoi.Cell2> GetVoronoiCells(List<Point3d> pts, Brep b)
        {

            Surface fa = b.Faces[0].ToNurbsSurface();

            List<Point2d> uvs = new List<Point2d>();
            foreach (Point3d p in pts)
            {
                double u;
                double v;
                fa.ClosestPoint(p, out u, out v);
                uvs.Add(new Point2d(u, v));
            }
            var nodes = new Grasshopper.Kernel.Geometry.Node2List();
            foreach (Point2d p in uvs)
            {
                nodes.Append(new Grasshopper.Kernel.Geometry.Node2(p.X, p.Y));
            }

            var outline = new Grasshopper.Kernel.Geometry.Node2List();
            for (int i = 0; i < b.Vertices.Count; ++i)
            {
                double u;
                double v;
                fa.ClosestPoint(b.Vertices[i].Location, out u, out v);
                outline.Append(new Grasshopper.Kernel.Geometry.Node2(u, v));
            }

            return Grasshopper.Kernel.Geometry.Voronoi.Solver.Solve_BruteForce(nodes, outline);
        }

        public List<Polyline> SplitSegmentizeCurveByCurves(Curve c, List<Curve> crvs, double intTol, double segTol, out List<Curve> segCrvs, out List<Point3d> associatedPts ,List<Point3d> pts = null)
        {
            List<Polyline> pls = new List<Polyline>();
            List<Curve> segcrvs = new List<Curve>();
            List<Point3d> assPts = new List<Point3d>();
            foreach (Curve spc in SplitCurveByCurves(c, crvs, intTol, out assPts, pts))
            {
                PolylineCurve pc = spc.ToPolyline(0, 0, 0, 0, 0, segTol, 0, 0, true);
                segcrvs.Add(spc);

                Polyline pl;
                pc.TryGetPolyline(out pl);
                pls.Add(pl);
            }
            segCrvs = segcrvs;
            associatedPts = assPts;
            return pls;
        }
        public List<Polyline> SplitSegmentizeCurves(List<Curve> crvs ,double intTol, double segTol, out List<List<Point3d>> associatedPts ,List<Point3d> pts = null)
        {
            List<Polyline> pls = new List<Polyline>();
            List<List<Point3d>> assPts = new List<List<Point3d>>();

            List<Point3d> apts = new List<Point3d>();
            foreach (Curve spc in SplitCurves(crvs, intTol, out apts , pts))
            {
                PolylineCurve pc = spc.ToPolyline(0, 0, 0, 0, 0, segTol, 0, 0, true);

                Polyline pl;
                pc.TryGetPolyline(out pl);
                pls.Add(pl);
                assPts.Add(apts);
            }

            associatedPts = assPts;
            return pls;
        }
        public List<Curve> SplitCurves(List<Curve> crvs, double tol, out List<Point3d> assPts, List<Point3d> pts = null)
        {
            List<Curve> splited = new List<Curve>();
            List<Point3d> apts = new List<Point3d>();
            for(int i = 0; i < crvs.Count; ++i)
            {
                foreach(Curve spc in SplitCurveByCurves(crvs[i], crvs, tol,out apts ,pts))
                {
                    splited.Add(spc);
                }
            }
            if (apts.Count == 0)
            {
                assPts = null;
            }
            else
            {
                assPts = apts;
            }

            return splited;
        }
        public List<Curve> SplitCurveByCurves(Curve c, List<Curve> crvs, double tol,out List<Point3d> assPts ,List<Point3d> pts = null)
        {
            List<double> paras = new List<double>();

            foreach(Curve cc in crvs)
            {
                Rhino.Geometry.Intersect.CurveIntersections cin = Rhino.Geometry.Intersect.Intersection.CurveCurve(c, cc, tol, tol);
                if(cin != null)
                {
                    for(int i = 0; i < cin.Count; ++i)
                    {
                        if (cin[i].IsPoint)
                        {
                            paras.Add(cin[i].ParameterA);
                        }
                    }
                }
            }
            List<Point3d> apts = new List<Point3d>();
            if (pts != null)
            {
                foreach(Point3d p in pts)
                {
                    double t;
                    if(c.ClosestPoint(p, out t, tol))
                    {
                        paras.Add(t);
                        apts.Add(p);
                    }
                }
            }
            if(apts.Count == 0)
            {
                assPts = null;
            }
            else
            {
                assPts = apts;
            }
            
            return c.Split(paras).ToList();
        }
        public Vector3d GetNormalVectorAtPointOnBrep(Point3d p, Brep b, double tol)
        {
            Vector3d nv = Vector3d.Zero;
            for(int i = 0; i < b.Faces.Count; ++i)
            {
                Surface s = b.Faces[i].ToNurbsSurface();

                nv += this.GetNormalVectorAtPointOnSurface(p, s, tol);
            }
            nv.Unitize();
            return nv;
        }
        public Vector3d GetNormalVectorAtPointOnSurface(Point3d p, Surface srf, double tol)
        {
            double u;
            double v;
            srf.ClosestPoint(p, out u, out v);
            Point3d sp = srf.PointAt(u, v);

            Vector3d nv = Vector3d.Zero;
            double dis = sp.DistanceTo(p);
            if(dis < tol)
            {
                nv = srf.NormalAt(u, v);
            }
            nv.Unitize();
            return nv;
        }
        
        //.....hate to put this here though....
        public void SplitSegmentizesBeamSet(ref List<IFrameSet> beamset, double intTol, double segTol, List<object> pointEle = null)
        {
            List<Curve> allCrvs = new List<Curve>();
            foreach (IFrameSet bs in beamset)
            {
                bs.frames.Clear(); //??
                allCrvs.Add(ToRhinoCurve(bs.parentCrv));
            }

            List<Point3d> pts = new List<Point3d>();
            if (pointEle != null)
            {
                foreach (object eb in pointEle)
                {
                    sPointLoad pl = eb as sPointLoad;
                    if (pl != null)
                    {
                        pts.Add(ToRhinoPoint3d(pl.location));
                    }
                    sPointSupport ps = eb as sPointSupport;
                    if (ps != null)
                    {
                        pts.Add(ToRhinoPoint3d(ps.location));
                    }
                }
            }
            if (pts.Count == 0) pts = null;

            foreach (IFrameSet bs in beamset)
            {
                Curve rc = ToRhinoCurve(bs.parentCrv);
                int id = 0;
                List<Curve> segCrvs;
                List<Point3d> assPts;
                List<Polyline> segPlns = SplitSegmentizeCurveByCurves(rc, allCrvs, intTol, segTol, out segCrvs, out assPts ,pts);

                if (segPlns.Count > 0)
                {
                    for (int i = 0; i < segPlns.Count; ++i)
                    {
                        if (segCrvs.Count > 1)
                        {
                            bs.parentSegments.Add(this.TosCurve(segCrvs[i]));
                        }

                        for (int j = 0; j < segPlns[i].SegmentCount; ++j)
                        {
                            Line segln = segPlns[i].SegmentAt(j);
                            /////////////////////////
                            //??????????????????????/
                            /////////////////////////
                            if(segln.Length > 0.005)
                            {
                                bs.AddBeamElement(TosLine(segln), sXYZ.Zaxis(), id);
                                bs.EnsureBeamElement();
                                id++;
                            }
                            
                        }
                    }
                    if (assPts != null)
                    {
                        bs.associatedLocations = new List<sXYZ>();
                        foreach (Point3d ap in assPts)
                        {
                            bs.associatedLocations.Add(TosXYZ(ap));
                        }
                    }
                }
                else
                {
                    Line segln = new Line(rc.PointAtStart, rc.PointAtEnd);
                    /////////////////////////
                    //??????????????????????/
                    /////////////////////////
                    if(segln.Length > 0.005)
                    {
                        bs.AddBeamElement(TosLine(segln), sXYZ.Zaxis(), id);
                        bs.EnsureBeamElement();
                        id++;
                    }
                    
                }
                
                bs.AwareElementsFixitiesByParentFixity(intTol);
            }
        }

        public void AwareBeamUpVectorsOnBrep(ref IFrameSet bSet, Brep guideBrep, double tol)
        {
            if (bSet.frames.Count > 0)
            {
                foreach (sFrame sb in bSet.frames)
                {
                    Point3d cp = ToRhinoPoint3d(sb.axis.PointAt(0.5));
                    Vector3d nv = GetNormalVectorAtPointOnBrep(cp, guideBrep, tol);
                    if(nv.Length > 0.001)
                    {
                        sb.AwareLocalPlane(TosXYZ(nv));
                    }
                }
            }
        }
    }
}
