using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject;
using sDataObject.sGeometry;
using sDataObject.sElement;
using Dyn = Autodesk.DesignScript.Geometry;

using Autodesk.DesignScript.Runtime;

using System.Drawing;

namespace ASKSGH_Bridgify
{
    internal class sDynamoConverter
    {
        internal string baseUnit { get; set; }
        internal string targetUnit { get; set; }

        public sDynamoConverter()
        {
            
        }
        public sDynamoConverter(string baseU, string targetU)
        {
            
            this.baseUnit = baseU;
            this.targetUnit = targetU;
        }

        public void DisposeGeometries(List<Dyn.Geometry> disposeGeo)
        {
            if (disposeGeo == null) return;
            foreach(Dyn.Geometry g in disposeGeo)
            {
                g.Dispose();
            }
        }
        
        internal Dyn.Geometry EnsureUnit(Dyn.Geometry geo, bool disposeInput = false)
        {
            Dyn.Geometry scgeo = null;
            Dyn.Plane xy = Dyn.Plane.XY();
            if (targetUnit == baseUnit)
            {
                scgeo = geo.Scale(xy, 1, 1, 1);
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    double factor = 3.280841666667;
                    scgeo = geo.Scale(xy, factor, factor, factor);

                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    double factor = 0.304800164592;
                    scgeo = geo.Scale(xy, factor, factor, factor);
                }
                else
                {
                 
                }
            }
            if(disposeInput)geo.Dispose();
            xy.Dispose();
            return scgeo;
        }
        internal sPointLoad EnsureUnit(sPointLoad pl)
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
                    //pl.location = TosXYZ((Dyn.Point) EnsureUnit(ToDynamoPoint(pl.location)));
                    return pl;
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    pl.ScaleByFactor(4.44822);
                    //pl.location = TosXYZ((Dyn.Point)EnsureUnit(ToDynamoPoint(pl.location)));
                    return pl;
                }
                else
                {
                    return null;
                }
            }
        }
        internal sLineLoad EnsureUnit(sLineLoad ll)
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
        internal double EnsureUnit_Dimension(double dim)
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
        internal List<double> EnsureUnit(List<double> dims)
        {
            List<double> united = new List<double>();
            foreach(double d in dims)
            {
                united.Add(EnsureUnit_Dimension(d));
            }
            return united;
        }
        internal double EnsureUnit_Deflection(double val)
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
        internal Dyn.Vector EnsureUnit_Deflection(Dyn.Vector val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return val.Scale(0.03937011);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return val.Scale(25.4);
                }
                else
                {
                    return null;
                }
            }
        }
        internal double EnsureUnit_Stress(double val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
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
        internal double EnsureUnit_Force(double val)
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
        internal Dyn.Vector EnsureUnit_Force(Dyn.Vector val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return val.Scale(0.224809);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return val.Scale(4.44822);
                }
                else
                {
                    return null;
                }
            }
        }
        internal double EnsureUnit_Moment(double val)
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
        internal Dyn.Vector EnsureUnit_Moment(Dyn.Vector val)
        {
            if (targetUnit == baseUnit)
            {
                return val;
            }
            else
            {
                if (baseUnit == "Meters" && targetUnit == "Feet")
                {
                    return val.Scale(0.7375610332);
                }
                else if (baseUnit == "Feet" && targetUnit == "Meters")
                {
                    return val.Scale(1.35582);
                }
                else
                {
                    return null;
                }
            }
        }
        internal double EnsureUnit_Volume(double val)
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
        internal double EnsureUnit_Area(double val)
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

        private Dyn.BoundingBox ToDynamoBoundingBox(List<IsObject> sobjs)
        {
            List<Dyn.Geometry> dygeos = new List<Autodesk.DesignScript.Geometry.Geometry>();
            foreach(IsObject so in sobjs)
            {
                if(so is sFrame)
                {
                    sFrame sb = so as sFrame;
                    dygeos.Add(ToDynamoLine(sb.axis));
                }
            }
            Dyn.BoundingBox dbox = Dyn.BoundingBox.ByGeometryCoordinateSystem(dygeos, Dyn.CoordinateSystem.ByOrigin(0, 0, 0));
            dygeos.Clear();
            return dbox;
        }
        internal sBoundingBox TosBoundingBox(List<IsObject> sobjs)
        {
            return TosBoundingBox(ToDynamoBoundingBox(sobjs));
        }
        internal sBoundingBox TosBoundingBox(Dyn.BoundingBox dybx)
        {
            sBoundingBox sbox = new sBoundingBox();
            sbox.center = TosXYZ((Dyn.Point)EnsureUnit(Dyn.Line.ByStartPointEndPoint(dybx.MinPoint, dybx.MaxPoint).PointAtParameter(0.5)));
            sbox.min = TosXYZ((Dyn.Point)EnsureUnit(dybx.MinPoint));
            sbox.max = TosXYZ((Dyn.Point)EnsureUnit(dybx.MaxPoint));

            sbox.diagonal = sbox.max - sbox.min;
            sbox.xSize = new sRange(sbox.min.X, sbox.max.X);
            sbox.ySize = new sRange(sbox.min.Y, sbox.max.Y);
            sbox.zSize = new sRange(sbox.min.Z, sbox.max.Z);
            return sbox;
        }

        internal Dyn.Point ToDynamoPoint(sXYZ sxyz)
        {
            return Dyn.Point.ByCoordinates(sxyz.X, sxyz.Y, sxyz.Z);
        }
        internal Dyn.Vector ToDynamoVector(sXYZ sxyz)
        {
            return Dyn.Vector.ByCoordinates(sxyz.X, sxyz.Y, sxyz.Z);
        }
        internal sXYZ TosXYZ(Dyn.Point dynp)
        {
            return new sXYZ(dynp.X, dynp.Y, dynp.Z);
        }
        internal sXYZ TosXYZ(Dyn.Vector dyv)
        {
            return new sXYZ(dyv.X, dyv.Y, dyv.Z);
        }

        internal Dyn.Line ToDynamoLine(sLine sln)
        {
            return Dyn.Line.ByStartPointEndPoint(ToDynamoPoint(sln.startPoint), ToDynamoPoint(sln.endPoint));
        }
        internal sLine TosLine(Dyn.Line dyln)
        {
            return new sLine(TosXYZ(dyln.StartPoint), TosXYZ(dyln.EndPoint));
        }

        internal Dyn.PolyCurve ToDynamoPolyCurve(sPolyLine spl)
        {
            List<Dyn.Point> cpts = new List<Dyn.Point>();
            for (int i = 0; i < spl.vertice.Count; ++i)
            {
                cpts.Add(ToDynamoPoint(spl.vertice[i]));
            }

            return Dyn.PolyCurve.ByPoints(cpts, spl.isClosed);
        }
        internal sPolyLine TosPolyLine(Dyn.PolyCurve dpc)
        {

            List<sXYZ> vertice = new List<sXYZ>();
            for(int i = 0; i < dpc.NumberOfCurves; ++i)
            {
                vertice.Add(TosXYZ(dpc.CurveAtIndex(i).StartPoint));
                if(i == dpc.NumberOfCurves - 1)
                {
                    vertice.Add(TosXYZ(dpc.CurveAtIndex(0).EndPoint));
                }
            }

            sPolyLine spl = new sPolyLine(vertice, dpc.IsClosed);

            return spl;
            
        }
        internal Dyn.Curve ToDynamoCurve(sCurve sc)
        {
            Dyn.Curve rc = null;
            if (sc.curveType == eCurveType.LINE)
            {
                rc = ToDynamoLine(sc as sLine) as Dyn.Curve;
            }
            else if (sc.curveType == eCurveType.POLYLINE)
            {
                rc = ToDynamoPolyCurve(sc as sPolyLine) as Dyn.Curve;
            }
            else if (sc.curveType == eCurveType.NURBSCURVE)
            {
                //rc = ToRhinoNurbsCurve(sc as sNurbsCurve);
            }
            return rc;
        }
        internal sCurve TosCurve(Dyn.Curve dc)
        {
            sCurve sc = null;
            
            Dyn.Line dl = dc as Dyn.Line;
            if(dl != null)
            {
                sc = TosLine(dl);
                sc.curveType = eCurveType.LINE;
                dl.Dispose();
            }
            Dyn.PolyCurve pc = dc as Dyn.PolyCurve;
            if(pc != null)
            {
                if(pc.NumberOfCurves == 1)
                {
                    ////
                    //what if this segement is nurbsCurve??????
                    //PolyCurve can be joined nurbsCurve!!!!!!!!!

                    ///
                    Dyn.Line dtl = Dyn.Line.ByStartPointEndPoint(pc.StartPoint, pc.EndPoint);
                    sc = TosLine(dtl);
                    sc.curveType = eCurveType.LINE;
                    dtl.Dispose();
                }
                else
                {
                    sc = TosPolyLine(pc);
                    sc.curveType = eCurveType.POLYLINE;
                }
                pc.Dispose();
            }
           // Dyn.NurbsCurve nc = dc as Dyn.NurbsCurve;
           // if(nc != null)
           // {
           //     
           // }

            return sc;
        }

        internal sMesh TosMesh(List<double> triVerticeInfo, sColor scol)
        {
            //nine conseq
            sMesh sm = new sMesh();
            int faceCount = (int)(triVerticeInfo.Count / 9);

            int verID = 0;
            int infoID = 0;
            for (int i = 0; i < faceCount; ++i)
            {
                sm.SetVertex(verID + 0, new sXYZ(triVerticeInfo[infoID + 0], triVerticeInfo[infoID + 1], triVerticeInfo[infoID + 2]), scol);
                sm.SetVertex(verID + 1, new sXYZ(triVerticeInfo[infoID + 3], triVerticeInfo[infoID + 4], triVerticeInfo[infoID + 5]), scol);
                sm.SetVertex(verID + 2, new sXYZ(triVerticeInfo[infoID + 6], triVerticeInfo[infoID + 7], triVerticeInfo[infoID + 8]), scol);

                sm.SetFace(i, verID + 0, verID + 1, verID + 2);

                verID += 3;
                infoID += 9;
            }
            sm.ComputeNormals();
            
            return sm;
        }
        internal List<double> ToDynamoMeshTriVerticeInfo(sMesh sm)
        {
            List<double> vs = new List<double>();
            
            foreach(sFace sf in sm.faces)
            {
                vs.Add(sm.vertices[sf.A].location.X);
                vs.Add(sm.vertices[sf.A].location.Y);
                vs.Add(sm.vertices[sf.A].location.Z);

                vs.Add(sm.vertices[sf.B].location.X);
                vs.Add(sm.vertices[sf.B].location.Y);
                vs.Add(sm.vertices[sf.B].location.Z);

                vs.Add(sm.vertices[sf.C].location.X);
                vs.Add(sm.vertices[sf.C].location.Y);
                vs.Add(sm.vertices[sf.C].location.Z);
            }
            return vs;
        }
        
        internal List<Dyn.Mesh> ToDynamoMesh(sSystem ssys, eColorMode colorMode, out List<List<Color>> verticeColor ,double du = 0.0, sRange th = null)
        {
            List<sMesh> sms = new List<sMesh>();
            sRange dataRange;
            ssys.ConstructBeamResultMesh(colorMode, ref sms, out dataRange ,th, du);

            List<List<Color>> allcols = new List<List<Color>>();
            List<Dyn.Mesh> dms = new List<Autodesk.DesignScript.Geometry.Mesh>();
            foreach(sMesh sm in sms)
            {
                List<Color> cols;
                dms.Add(ToDynamoMesh(sm, out cols));
                allcols.Add(cols);
            }
            verticeColor = allcols;
            return dms;
        }
        internal void ToDynamoToolKitMeshData(sSystem ssys, eColorMode colorMode,out List<List<Dyn.Point>>vpts, out List<List<int>> findice, out List<List<int>> colorsR, out List<List<int>> colorsG, out List<List<int>> colorsB, double du = 0.0, sRange th = null)
        {
            List<sMesh> sms = new List<sMesh>();
            sRange dataRange;
            ssys.ConstructBeamResultMesh(colorMode, ref sms, out dataRange,th, du);

            List<List<Dyn.Point>> ppp = new List<List<Dyn.Point>>();
            List<List<int>> iii = new List<List<int>>();
            List<List<int>> rrr = new List<List<int>>();
            List<List<int>> ggg = new List<List<int>>();
            List<List<int>> bbb = new List<List<int>>();

            foreach (sMesh sm in sms)
            {
                
                List<int> ii = new List<int>();
                List<int> rr = new List<int>();
                List<int> gg = new List<int>();
                List<int> bb = new List<int>();
                List<Dyn.Point> pp = new List<Autodesk.DesignScript.Geometry.Point>();

                ToDynamoToolKitMeshData(sm, ref pp, ref ii, ref rr, ref gg ,ref bb);

                ppp.Add(pp);
                iii.Add(ii);
                rrr.Add(rr);
                ggg.Add(gg);
                bbb.Add(bb);
            }
            vpts = ppp;
            findice = iii;
            colorsR = rrr;
            colorsG = ggg;
            colorsB = bbb;
        }
        internal Dyn.Mesh ToDynamoMesh(sMesh sm, out List<Color> vcols)
        {
            List<Dyn.Point> pts = new List<Autodesk.DesignScript.Geometry.Point>();
            List<Dyn.IndexGroup> indice = new List<Autodesk.DesignScript.Geometry.IndexGroup>();
            List<Color> cols = new List<Color>();

            for(int i = 0; i < sm.vertices.Count; ++i)
            {
                pts.Add(ToDynamoPoint(sm.vertices[i].location));
                if(sm.vertices[i].color != null)
                {
                    cols.Add(sm.vertices[i].color.ToWinColor());
                }
            }

            for(int i = 0; i < sm.faces.Count; ++i)
            {
                Dyn.IndexGroup faceIDs = Dyn.IndexGroup.ByIndices((uint)sm.faces[i].A, (uint)sm.faces[i].B, (uint)sm.faces[i].C);
                indice.Add(faceIDs);
            }

            vcols = cols;

            return Dyn.Mesh.ByPointsFaceIndices(pts, indice);
        }
        internal void ToDynamoToolKitMeshData(sMesh sm, ref List<Dyn.Point> vpts, ref List<int> findice, ref List<int> rr, ref List<int> gg, ref List<int> bb)
        {
            for (int i = 0; i < sm.vertices.Count; ++i)
            {
                vpts.Add(ToDynamoPoint(sm.vertices[i].location));
               // if (sm.vertices[i].color != null)
               // {
                    Color vc = sm.vertices[i].color.ToWinColor();
                    rr.Add(vc.R);
                    gg.Add(vc.G);
                    bb.Add(vc.B);
               // }
            }

            for (int i = 0; i < sm.faces.Count; ++i)
            {
                findice.Add(sm.faces[i].A);
                findice.Add(sm.faces[i].B);
                findice.Add(sm.faces[i].C);
            }
        }
        internal sMesh TosMesh(Dyn.Mesh dym)
        {
            sMesh sm = new sMesh();

            for (int i = 0; i < dym.VertexPositions.Length; ++i)
            {
                sm.SetVertex(i, new sXYZ(dym.VertexPositions[i].X, dym.VertexPositions[i].Y, dym.VertexPositions[i].Z));
              // if (rm.VertexColors[i] != null)
              // {
              //     sm.vertices[i].color = sColor.FromWinColor(rm.VertexColors[i]);
              // }
            }

            for (int i = 0; i < dym.FaceIndices.Length; ++i)
            {
                sm.SetFace(i, (int)dym.FaceIndices[i].A, (int)dym.FaceIndices[i].B, (int)dym.FaceIndices[i].C);
            }

            return sm;
        }

        internal Dyn.Solid ToDynamoBeamPreview(sFrame sb)
        {
            return Dyn.Solid.ByLoft(new Dyn.Curve[2] { ToRhinoCrosssectionProfile(sb, 0.0), ToRhinoCrosssectionProfile(sb, 1.0) });
        }

        internal Dyn.PolyCurve ToRhinoCrosssectionProfile(sFrame sb, double t)
        {
            sCrossSection scs = sb.crossSection;
            List<sXYZ> vertice = new List<sXYZ>();
            sPlane secPlane = new sPlane(sb.axis.PointAt(t), sb.localPlane.Xaxis, sb.localPlane.Yaxis);

            if (scs.sectionType == eSectionType.AISC_I_BEAM)
            {
                vertice = scs.GetWbeamFaceVertices(secPlane).ToList();
            }
            else if (scs.sectionType == eSectionType.HSS_REC)
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

            List<Dyn.Point> dpts = new List<Autodesk.DesignScript.Geometry.Point>();
            for (int i = 0; i < vertice.Count; ++i)
            {
                dpts.Add(this.ToDynamoPoint(vertice[i]));
            }
            return Dyn.PolyCurve.ByPoints(dpts, true);
        }

        internal void SplitSegmentizesBeamSet(ref List<sFrameSet> beamset, double intTol, double segTol, List<object> pointEle = null)
        {
            List<Dyn.Geometry> disposeGeo = new List<Dyn.Geometry>();
            List<Dyn.Curve> allCrvs = new List<Dyn.Curve>();
            foreach(sFrameSet bs in beamset)
            {
                bs.frames.Clear();
                Dyn.Curve c = ToDynamoCurve(bs.parentCrv);
                allCrvs.Add(c);
                disposeGeo.Add(c);
            }

            List<Dyn.Point> pts = new List<Dyn.Point>();
            if (pointEle != null)
            {
                foreach (object eb in pointEle)
                {
                    sPointLoad pl = eb as sPointLoad;
                    if (pl != null)
                    {
                        Dyn.Point p = ToDynamoPoint(pl.location);
                        pts.Add(p);
                        disposeGeo.Add(p);
                    }
                    sPointSupport ps = eb as sPointSupport;
                    if (ps != null)
                    {
                        Dyn.Point p = ToDynamoPoint(ps.location);
                        pts.Add(p);
                        disposeGeo.Add(p);
                    }
                }
            }
            if (pts.Count == 0) pts = null;
            
            foreach (sFrameSet bs in beamset)
            {
                Dyn.Curve dc = ToDynamoCurve(bs.parentCrv);
                int id = 0;
                List<Dyn.PolyCurve> segCrvs;
                List<Dyn.Point> assPts;
                List<Dyn.PolyCurve> segPlns = SplitSegmentizeCurveByCurves(dc, allCrvs, intTol, segTol, out segCrvs, out assPts, pts);

                if (segPlns.Count > 0)
                {
                    for (int i = 0; i < segPlns.Count; ++i)
                    {
                        if (segCrvs.Count > 1)
                        {
                            bs.parentSegments.Add(TosCurve(segCrvs[i]));
                            disposeGeo.Add(segCrvs[i]);
                        }

                        Dyn.Curve [] segs = segPlns[i].Curves();
                        for(int j = 0; j < segs.Length; ++j)
                        {
                            if(segs[j].Length > 0.005)
                            {
                                Dyn.Line dl = Dyn.Line.ByStartPointEndPoint(segs[j].StartPoint, segs[j].EndPoint);
                                bs.AddBeamElement(TosLine(dl), sXYZ.Zaxis(), id);
                                id++;
                                disposeGeo.Add(dl);
                            }
                            disposeGeo.Add(segs[j]);
                        }
                        disposeGeo.Add(segPlns[i]);
                    }
                    if (assPts != null)
                    {
                        bs.associatedLocations = new List<sXYZ>();
                        foreach (Dyn.Point ap in assPts)
                        {
                            bs.associatedLocations.Add(TosXYZ(ap));
                            disposeGeo.Add(ap);
                        }
                    }
                }
                else
                {
                    Dyn.Line dl = Dyn.Line.ByStartPointEndPoint(dc.StartPoint, dc.EndPoint);
                    if(dl.Length > 0.005)
                    {
                        bs.AddBeamElement(TosLine(dl), sXYZ.Zaxis(), id);
                        bs.EnsureBeamElement();
                        id++;
                    }
                    disposeGeo.Add(dl);
                }

                bs.AwareElementsFixitiesByParentFixity(intTol);

                disposeGeo.Add(dc);
            }

            this.DisposeGeometries(disposeGeo);

        }

        internal List<Dyn.PolyCurve> SplitSegmentizeCurveByCurves(Dyn.Curve c, List<Dyn.Curve> crvs, double intTol, double segTol, out List<Dyn.PolyCurve> segCrvs, out List<Dyn.Point> associatedPts, List<Dyn.Point> pts = null)
        {
            List<Dyn.Geometry> disposeGeo = new List<Dyn.Geometry>();
            List<Dyn.PolyCurve> pls = new List<Dyn.PolyCurve>();
            List<Dyn.PolyCurve> segcrvs = new List<Dyn.PolyCurve>();
            List<Dyn.Point> assPts = new List<Dyn.Point>();
            foreach (Dyn.Curve spc in SplitCurveByCurves(c, crvs, intTol, out assPts, pts))
            {
                int segCount = (int)(spc.Length / segTol);
                ////////////
                //Do not segmentize if spc is straight line.... How????
                //How to deal with nurbsCurve?
                ////////////
                if (IsLineCurve(spc)) segCount = 0;

                List<Dyn.Point> segPts = new List<Dyn.Point>();
                if (segCount < 1)
                {
                    segPts.Add(spc.StartPoint);
                    segPts.Add(spc.EndPoint);
                }
                else
                {
                    segPts.Add(spc.StartPoint);
                    foreach (Dyn.Point sp in spc.PointsAtEqualSegmentLength(segCount)) {
                        segPts.Add(sp);
                        disposeGeo.Add(sp);
                    }
                    segPts.Add(spc.EndPoint);
                }

                segcrvs.Add(Dyn.PolyCurve.ByJoinedCurves(new Dyn.Curve[]{spc}));

                Dyn.PolyCurve pl = Dyn.PolyCurve.ByPoints(segPts);
                pls.Add(pl);
                
            }
            segCrvs = segcrvs;
            associatedPts = assPts;

            DisposeGeometries(disposeGeo);
            return pls;
        }

        internal List<Dyn.Curve> SplitCurveByCurves(Dyn.Curve c, List<Dyn.Curve> crvs, double intTol, out List<Dyn.Point>assPts, List<Dyn.Point> pts = null)
        {
            List<Dyn.Geometry> disposeGeo = new List<Dyn.Geometry>();
            List<Dyn.Point> paras = new List<Dyn.Point>();
            List<Dyn.Point> apts = new List<Dyn.Point>();

            foreach (Dyn.Curve dc in crvs)
            {
                Dyn.Geometry [] intg = c.Intersect(dc);
                if(intg != null)
                {
                    for(int i = 0; i < intg.Length; ++i)
                    {
                        if(intg[i] is Dyn.Point)
                        {
                            paras.Add(intg[i] as Dyn.Point);
                        }
                        else
                        {
                            disposeGeo.Add(intg[i]);
                        }
                    }
                }
            }
            if(pts != null)
            {
                foreach (Dyn.Point p in pts)
                {
                    if (c.DistanceTo(p) < intTol)
                    {
                        paras.Add(Dyn.Point.ByCoordinates(p.X, p.Y, p.Z));
                        apts.Add(Dyn.Point.ByCoordinates(p.X, p.Y, p.Z));
                    }
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

            List<Dyn.Curve> splitedCulled = new List<Dyn.Curve>();
            foreach(Dyn.Curve sc in c.SplitByPoints(paras))
            {
                if(sc.Length > 0.005)
                {
                    splitedCulled.Add(sc);
                }
                else
                {
                    disposeGeo.Add(sc);
                }
            }
            foreach(Dyn.Point p in paras)
            {
                disposeGeo.Add(p);
            }
            this.DisposeGeometries(disposeGeo);

            return splitedCulled.ToList();
        }

        internal bool IsLineCurve(Dyn.Curve c)
        {
            double lenC = c.Length;
            Dyn.Line ln = Dyn.Line.ByStartPointEndPoint(c.StartPoint, c.EndPoint);
            double lenL = ln.Length;
            ln.Dispose();
            if(Math.Abs(lenC - lenL) < 0.001)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
