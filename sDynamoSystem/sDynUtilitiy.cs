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
using Tessellation;

namespace ASKSGH_Bridgify
{
    public static class sUtilitiy
    {
        [MultiReturn(new[] { "LoadPoints", "TributaryAreaSrfs", "TributaryArea" })]
        public static object AwareTributaryArea_ByPoints(Dyn.Surface boundarySrf, List<Dyn.Point> pointsForLoad, bool flipDirection)
        {
            sDynamoConverter dycon = new sDynamoConverter("Feet", "Meters");

            List<Dyn.UV> uvs = new List<Dyn.UV>();
            

           //double fac = 10;
           //if (flipDirection) fac *= -1;
           //foreach (Dyn.Curve seg in Dyn.PolyCurve.ByJoinedCurves(boundarySrf.PerimeterCurves()).Offset(fac).Explode())
           //{
           //    uvs.Add(boundarySrf.UVParameterAtPoint(seg.PointAtParameter(0.5)));
           //}
            foreach (Dyn.Point lp in pointsForLoad)
            {
                uvs.Add(boundarySrf.UVParameterAtPoint(lp));
            }
            List<Dyn.Surface> vorocut = new List<Dyn.Surface>();
            foreach (var vc in Voronoi.ByParametersOnSurface(uvs, boundarySrf))
            {
                Dyn.Curve vcc = vc as Dyn.Curve;

                Dyn.Geometry[] ints = boundarySrf.Intersect(vcc);
                if (ints != null && ints.Length > 0)
                {
                    for (int i = 0; i < ints.Length; ++i)
                    {
                        if (ints[i] is Dyn.Curve)
                        {
                            Dyn.Curve ic = ints[i] as Dyn.Curve;
                            Dyn.Surface isrf = ic.Extrude(Dyn.Vector.ZAxis().Scale(5));
                            vorocut.Add(isrf);
                        }
                        else
                        {
                            object t = ints[i];
                        }
                    }
                }
            }
            List<Dyn.Surface> voroPattern = new List<Dyn.Surface>();
            List<double> voroArea = new List<double>();
            List<Dyn.Point> lpts = new List<Dyn.Point>();

            Dyn.PolySurface vorocutPoly = Dyn.PolySurface.ByJoinedSurfaces(vorocut);
            Dyn.Geometry[] splited = boundarySrf.Split(vorocutPoly);
            for(int i = 0; i < splited.Length; ++i)
            {
                Dyn.Surface vsrf = splited[i] as Dyn.Surface;
                if(vsrf != null)
                {
                    //voroPattern.Add(Dyn.PolyCurve.ByJoinedCurves(vsrf.PerimeterCurves()));
                    voroPattern.Add(vsrf);
                    voroArea.Add(vsrf.Area);

                    foreach (Dyn.Point lp in pointsForLoad)
                    {
                        if(lp.DistanceTo(vsrf) < 0.005)
                        {
                            lpts.Add(lp);
                            break;
                        }
                    }
                }
            }

          
            //vorocutPoly.Dispose();

            return new Dictionary<string, object>
            {
                { "LoadPoints", lpts },
                { "TributaryAreaSrfs", voroPattern },
                { "TributaryArea", voroArea }
            };
        }

        [MultiReturn(new[] { "LoadPoints", "TributarySolids", "TributaryVolumes_ft3" })]
        public static object AwareTributaryVolumeByAreaSolid(List<Dyn.Point> pointsForLoad, List<Dyn.Surface> tributaryAreaSrfs, Dyn.Solid volume)
        {
            List<Dyn.Point> lpts = new List<Dyn.Point> ();
            List<Dyn.Geometry> tsols = new List<Dyn.Geometry>();
            List<double> vols = new List<double>();
            foreach (Dyn.Surface ts in tributaryAreaSrfs)
            {
                Dyn.Solid trisol = Dyn.PolyCurve.ByJoinedCurves(ts.PerimeterCurves()).ExtrudeAsSolid(Dyn.Vector.ZAxis().Scale(50));

                Dyn.Solid intSelected = null;

                Dyn.Geometry[] ints = trisol.Intersect(volume);
                if(ints != null && ints.Length > 0)
                {
                    for(int i = 0; i < ints.Length; ++i)
                    {
                        Dyn.Solid ins = ints[i] as Dyn.Solid;
                        if(ins != null)
                        {
                            intSelected = ins;
                        }
                    }
                }

                if(intSelected != null)
                {
                    tsols.Add(intSelected.Scale(Dyn.Plane.ByOriginNormal(intSelected.Centroid(), Dyn.Vector.ZAxis()), 0.95, 0.95, 0.95));
                    vols.Add(intSelected.Volume);

                    foreach (Dyn.Point lp in pointsForLoad)
                    {
                        if (ts.DistanceTo(lp) < 0.005)
                        {
                            lpts.Add(lp);
                            break;
                        }
                    }
                }
            }



            return new Dictionary<string, object>
            {
                { "LoadPoints", lpts },
                { "TributarySolids", tsols },
                { "TributaryVolumes_ft3", vols }
            };
        }

        [MultiReturn(new[] { "DesignLength_ft","DesignHeight_ft","DesignMaxLoad_psf", "DesignDensity_pcf", "DrfitVolumeSolid" })]
        public static object SnowDrift_ByEdgeHeight(Dyn.Curve edgeCurve, bool flipDirectioin, double snowLoad_ground_psf, double snowLoad_flatRoof_psf, double roofLength_lower_ft, double roofLength_upper_ft, double roofHeight_delta_ft, double adjustmentFactor_ft = 10.0)
        {
            sDynamoUtilities dynutil = new sDynamoUtilities();

            double designLen;
            double designHeight;
            double designMaxLoad;
            double designSnowDensity;
            dynutil.GetSnowDriftInformation(snowLoad_ground_psf, snowLoad_flatRoof_psf, roofHeight_delta_ft, roofLength_lower_ft, roofLength_upper_ft, out designLen, out designHeight, out designMaxLoad, out designSnowDensity);

            double fac = designLen;
            if (flipDirectioin) fac *= -1;

            Dyn.PolyCurve edgeCrvEx = Dyn.PolyCurve.ByJoinedCurves(new Dyn.Curve[1] { edgeCurve.ExtendStart(adjustmentFactor_ft).ExtendEnd(adjustmentFactor_ft) });
            Dyn.PolyCurve driftCrv_onRoof = Dyn.PolyCurve.ByJoinedCurves(new Dyn.Curve[1] { edgeCrvEx.Offset(fac) }); 
            Dyn.PolyCurve driftCrv_onEdge = Dyn.PolyCurve.ByJoinedCurves(new Dyn.Curve[1] { edgeCrvEx.Translate(Dyn.Vector.ZAxis().Scale(designHeight)) as Dyn.Curve });

            List<Dyn.Solid> snows = new List<Dyn.Solid>();
            for(int i = 0; i < driftCrv_onRoof.NumberOfCurves; ++i)
            {
                List<Dyn.PolyCurve> cs = new List<Dyn.PolyCurve>();
                cs.Add(Dyn.PolyCurve.ByPoints(new Dyn.Point[3] { driftCrv_onRoof.CurveAtIndex(i).StartPoint, driftCrv_onEdge.CurveAtIndex(i).StartPoint, edgeCrvEx.CurveAtIndex(i).StartPoint }, true));
                cs.Add(Dyn.PolyCurve.ByPoints(new Dyn.Point[3] { driftCrv_onRoof.CurveAtIndex(i).EndPoint, driftCrv_onEdge.CurveAtIndex(i).EndPoint, edgeCrvEx.CurveAtIndex(i).EndPoint }, true));
                snows.Add(Dyn.Solid.ByLoft(cs));
            }

            Dyn.Solid snowSolid = Dyn.Solid.ByUnion(snows);

           // edgeCrvEx.Dispose();
           // driftCrv_onEdge.Dispose();
           // driftCrv_onRoof.Dispose();

            return new Dictionary<string, object>
            {
                 { "DesignLength_ft", designLen },
                 { "DesignHeight_ft", designHeight },
                 { "DesignMaxLoad_psf", designMaxLoad },
                 { "DesignDensity_pcf", designSnowDensity },
                 { "DrfitVolumeSolid", snowSolid},
            };
        }
    }

    internal class sDynamoUtilities
    {
        internal void GetSnowDriftInformation(double sl_ground_psf, double sl_flatRoof_psf, double height_delta_ft, double len_lower_ft, double len_upper_ft, out double designLen_ft, out double designHeight_ft, out double designMaxLoad_psf, out double designSnowDensity_pcf)
        {

            double snow_density_pcf = Math.Min(30, (0.13 * sl_ground_psf + 14));
            designSnowDensity_pcf = snow_density_pcf;

            double snow_height_onFlat_ft = (sl_flatRoof_psf / snow_density_pcf);

            double snow_potential_driftHeight_ft = height_delta_ft - snow_height_onFlat_ft;

            double snow_height_LeewardDrift_ft = ((0.43 * Math.Pow(len_upper_ft, 1.0 / 3.0) * Math.Pow(sl_ground_psf + 10, 1.0 / 4.0)) - 1.5);
            double snow_height_WindwardDrift_ft = 0.75 * ((0.43 * Math.Pow(len_lower_ft, 1.0 / 3.0) * Math.Pow(sl_ground_psf + 10, 1.0 / 4.0)) - 1.5);

            double snow_design_driftHeight_ft = 0.0;

            if ((snow_potential_driftHeight_ft / snow_height_onFlat_ft) > 0.2)
            {
                snow_design_driftHeight_ft = Math.Min(snow_potential_driftHeight_ft, Math.Max(snow_height_LeewardDrift_ft, snow_height_WindwardDrift_ft));
            }

            double snow_design_driftLength_ft = 0.0;
            if (snow_design_driftHeight_ft <= snow_potential_driftHeight_ft)
            {
                snow_design_driftLength_ft = Math.Min(8 * snow_potential_driftHeight_ft, 4 * snow_design_driftHeight_ft);
            }
            else if ((snow_potential_driftHeight_ft / snow_height_onFlat_ft) <= 0.2)
            {
                snow_design_driftLength_ft = Math.Min(8 * snow_potential_driftHeight_ft, 0);
            }
            else
            {
                snow_design_driftLength_ft = Math.Min(8 * snow_potential_driftHeight_ft, (4 * snow_design_driftLength_ft * snow_design_driftLength_ft) / snow_potential_driftHeight_ft);
            }

            double maxSnowload_Drift_psf = snow_design_driftHeight_ft * snow_density_pcf;

            designLen_ft = snow_design_driftLength_ft;
            designHeight_ft = snow_design_driftHeight_ft;
            designMaxLoad_psf = maxSnowload_Drift_psf;
        }


    }
}
