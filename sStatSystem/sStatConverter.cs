using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SawapanStatica;
using sDataObject.sElement;
using sDataObject.sGeometry;

namespace sNStatSystem
{
    public class sStatConverter
    {
        public sStatConverter()
        {

        }

        public StatCrossSection ToStatCrossSection(sFrame jb)
        {
            StatCrossSection cs = null;
            StatMaterial mat = GetStatMaterial(jb);

            if (jb.crossSection.sectionType == eSectionType.AISC_I_BEAM)
            {
                cs = GetWbeamStatCrossSection(jb, mat);
            }
            else if (jb.crossSection.sectionType == eSectionType.HSS_REC)
            {
                cs = GetHSSRectangleStatCrossSection(jb, mat);
            }
            else if (jb.crossSection.sectionType == eSectionType.HSS_ROUND)
            {
                cs = GetHSSRoundStatCrossSection(jb, mat);
            }
            else if (jb.crossSection.sectionType == eSectionType.SQUARE)
            {
                cs = GetSquareStatCrossSection(jb, mat);
            }
            else if (jb.crossSection.sectionType == eSectionType.RECTANGLAR)
            {
                cs = GetRectangleStatCrossSection(jb, mat);
            }
            else if (jb.crossSection.sectionType == eSectionType.ROUND)
            {
                cs = GetRoundCrossSection(jb, mat);
            }

            return cs;
        }
 
        public C_vector ToCVector(sNode jn)
        {
            return new C_vector(jn.location.X, jn.location.Y, jn.location.Z);
        }

        public C_vector ToCVector(sXYZ jxyz)
        {
            return new C_vector(jxyz.X, jxyz.Y, jxyz.Z);
        }

        public sXYZ TosXYZ(C_vector cv)
        {
            return new sXYZ(cv.x, cv.y, cv.z);
        }
        
        private StatMaterial GetStatMaterial(sFrame jb)
        {
            StatMaterial mat = null;

            if (jb.crossSection.material.materialType.ToString().Contains("STEEL"))
            {
                //mat = new StatMaterial(MATERIALTYPES.STEEL, "StatSteel");
                mat = new StatMaterial(MATERIALTYPES.GENERIC, "StatSteel");
                mat.Em = 1.999E11;
                mat.Gm = 7.69E10;
                mat.Density = 7849.0474;
                mat.Poisson = 0.3;

            }
            else if (jb.crossSection.material.materialType.ToString().Contains("ALUMINUM"))
            {
                mat = new StatMaterial(MATERIALTYPES.ALUMINUM, "StatAluminum");
            }
            else if (jb.crossSection.material.materialType.ToString().Contains("CONCRETE"))
            {
                mat = new StatMaterial(MATERIALTYPES.CONCRETE, "StatConcrete");
            }
            else if (jb.crossSection.material.materialType.ToString().Contains("CARBONFRP"))
            {
                mat = new StatMaterial(MATERIALTYPES.CARBONFRP, "StatConcrete");
            }
            else if (jb.crossSection.material.materialType.ToString().Contains("OAK"))
            {
                mat = new StatMaterial(MATERIALTYPES.OAK, "StatConcrete");
            }
            else if (jb.crossSection.material.materialType.ToString().Contains("STAINLESSSTEEL"))
            {
                mat = new StatMaterial(MATERIALTYPES.STAINLESSSTEEL, "StatConcrete");
            }
            else if (jb.crossSection.material.materialType.ToString().Contains("Isotropic"))
            {
                mat = new StatMaterial(MATERIALTYPES.GENERIC, "StatIsotropic");
                //how to assure unit??
                //
                mat.Em = jb.crossSection.material.E;
                mat.Density = jb.crossSection.material.massPerVol;
                
                //what else??
            }

            return mat;
        }

        private StatCrossSection GetRoundCrossSection(sFrame jb, StatMaterial mat)
        {
            StatCrossSection cs = new StatCrossSection(jb.crossSection.shapeName, mat);
            
            if(jb.crossSection.dimensions.Count > 0)
            {
                if(jb.crossSection.dimensions.Count == 1)
                {
                    double or = jb.crossSection.dimensions[0] * 0.5;
                    cs.CircSolid(or);
                }
                else if(jb.crossSection.dimensions.Count == 2)
                {
                    double or = jb.crossSection.dimensions[0] * 0.5;
                    double ir = (jb.crossSection.dimensions[0] * 0.5) - jb.crossSection.dimensions[1];

                    cs.CircHollow(ir, or);
                }
            }
            return cs;
        }

        private StatCrossSection GetHSSRoundStatCrossSection(sFrame jb, StatMaterial mat)
        {
            double od;
            double th;
            double a;
            double i;
            double s;
            double r;
            double z;
            double j;
            double c;
            double nw;

            jb.crossSection.GetHSSRoundDimensions(jb.crossSection.shapeName, out od, out th, out a, out i, out s, out r, out z, out j, out c, out nw);
            StatCrossSection cs = new StatCrossSection(jb.crossSection.shapeName, mat);

            //cs.RectangleHollow(w * 0.0254, d * 0.0254, t * 0.0254);
            double ijFac = 4.162314256E-07;
            double areaFac = 0.00064516;
            cs.Generic(a * areaFac, i * ijFac, i * ijFac, j * ijFac);

            return cs;
        }

        private StatCrossSection GetSquareStatCrossSection(sFrame jb, StatMaterial mat)
        {
            StatCrossSection cs = new StatCrossSection(jb.crossSection.shapeName, mat);
            //currently cannot trust Millipede
            if (jb.crossSection.dimensions.Count > 0)
            {
                if (jb.crossSection.dimensions.Count == 2)
                {
                    double wid = jb.crossSection.dimensions[0];
                    double dep = jb.crossSection.dimensions[1];

                    double area;
                    double iyy;
                    double izz;
                    double jxx;
                    GetRectangularSolidProperties(wid, dep, out area, out iyy, out izz, out jxx);

                    //no factor as it is calculated in metric
                    cs.Generic(area, iyy, izz, jxx);
                    //cs.SqrSolid(jb.crossSection.dimensions[0]);
                }
                else if (jb.crossSection.dimensions.Count == 3)
                {
                    double wid = jb.crossSection.dimensions[0];
                    double dep = jb.crossSection.dimensions[1];
                    double th = jb.crossSection.dimensions[2];

                    double area;
                    double iyy;
                    double izz;
                    double jxx;
                    GetRectangularHollowProperties(wid, dep, th, out area, out iyy, out izz, out jxx);

                    //no factor as it is calculated in metric
                    cs.Generic(area, iyy, izz, jxx);
                    //cs.SqrHollow(jb.crossSection.dimensions[0], jb.crossSection.dimensions[1]);
                }
            }
            return cs;
        }

        private StatCrossSection GetRectangleStatCrossSection(sFrame jb, StatMaterial mat)
        {
            StatCrossSection cs = new StatCrossSection(jb.crossSection.shapeName, mat);
            //currently cannot trust Millipede
            if (jb.crossSection.dimensions.Count > 0)
            {
                if (jb.crossSection.dimensions.Count == 2)
                {
                    double wid = jb.crossSection.dimensions[0];
                    double dep = jb.crossSection.dimensions[1];

                    double area;
                    double iyy;
                    double izz;
                    double jxx;
                    GetRectangularSolidProperties(wid, dep, out area, out iyy, out izz, out jxx);

                    //no factor as it is calculated in metric
                    cs.Generic(area, iyy, izz, jxx);

                    //cs.RectangleSolid(jb.crossSection.dimensions[0], jb.crossSection.dimensions[1]);
                }
                else if (jb.crossSection.dimensions.Count == 3)
                {
                    double wid = jb.crossSection.dimensions[0];
                    double dep = jb.crossSection.dimensions[1];
                    double th = jb.crossSection.dimensions[2];

                    double area;
                    double iyy;
                    double izz;
                    double jxx;
                    GetRectangularHollowProperties(wid, dep, th, out area, out iyy, out izz, out jxx);

                    //no factor as it is calculated in metric
                    cs.Generic(area, iyy, izz, jxx );

                    //cs.RectangleHollow(jb.crossSection.dimensions[0], jb.crossSection.dimensions[1], jb.crossSection.dimensions[2]);
                }
            }
            return cs;
        }

        private void GetRectangularSolidProperties(double wid, double dep, out double area, out double iyy, out double izz, out double jxx)
        {
            //all metric
            double B = wid;
            double H = dep;

            area = (B * H);
            iyy = (B * H * H * H) / 12.0;
            izz = (H * B * B * B) / 12.0;
            //jxx = ((B*H)*(B*B + H*H)) / 12.0;
            jxx = (H * B * B * B) / 3.0;
        }

        private void GetRectangularHollowProperties(double wid, double dep, double th, out double area, out double iyy, out double izz, out double jxx)
        {
            //all metric
            double B = wid;
            double H = dep;
            double b = wid - th - th;
            double h = dep - th - th;

            area = (B*H) - (b*h);
            iyy = ((B * H * H * H) / 12.0) - ((b * h * h * h) / 12.0);
            izz = ((H * B * B * B) / 12.0) - ((h * b * b * b) / 12.0);
            jxx = ((2 * (th * B * B * B)) + (2 * (h * th * th * th)) / 3.0);
            //double jxx = ((((B * H) * ((B * B) + (H * H))) - ((b * h) * ((b * b) + (h * h)))) / 12.0);
        }

        private StatCrossSection GetHSSRectangleStatCrossSection(sFrame jb, StatMaterial mat)
        {
            double w; ///list of variables
            double d;
            double t;
            double ar;
            double ix;
            double sx;
            double rx;
            double zx;
            double iy;
            double sy;
            double ry;
            double zy;
            double jt;
            double ct;
            double nw;

            jb.crossSection.GetHSSRecDimensions(jb.crossSection.shapeName, out w, out d, out t, out ar, out ix, out sx, out rx, out zx, out iy, out sy, out ry, out zy, out jt, out ct, out nw);
            
            StatCrossSection cs = new StatCrossSection(jb.crossSection.shapeName, mat);

            //cs.RectangleHollow(w * 0.0254, d * 0.0254, t * 0.0254);
            double ijFac = 4.162314256E-07;
            double areaFac = 0.00064516;
            cs.Generic(ar * areaFac, ix * ijFac, iy * ijFac, jt * ijFac);

            return cs;
        }

        private StatCrossSection GetWbeamStatCrossSection(sFrame jb, StatMaterial mat)
        {
            double a;
            double tw;
            double d;
            double tf;
            double bf;
            double nw;
            double ix;
            double sx;
            double rx;
            double zx;
            double iy;
            double sy;
            double ry;
            double zy;
            double rts;
            double ho;
            double j;
            double c;

            jb.crossSection.GetWBeamDimensions(jb.crossSection.shapeName, out a, out tw, out d, out tf, out bf, out nw, out ix, out sx, out rx, out zx, out iy, out sy, out ry, out zy, out rts, out ho, out j, out c);
            
            StatCrossSection cs = new StatCrossSection(jb.crossSection.shapeName, mat);

            //cs.Wbeam(d * 0.0254, bf * 0.0254, tf * 0.0254, tw * 0.0254);
            double ijFac = 4.162314256E-07;
            double areaFac = 0.00064516;
            cs.Generic(a * areaFac, ix * ijFac, iy * ijFac, j * ijFac);

            return cs;
        }
    }
}
