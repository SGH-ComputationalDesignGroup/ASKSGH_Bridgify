using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject.sGeometry;

namespace sDataObject.sElement
{
    public class sCrossSection
    {
        public string shapeName{ get; set; }
        public sMaterial material { get; set; }
        public eSectionType sectionType { get; set; }
        public List<double> dimensions { get; set; }
        public double weight { get; set; }
        public double depth { get; set; }
        public double I_StrongAxis { get; set; }
        
        public sCrossSection()
        {

        }

        public sCrossSection DuplicatesCrosssection()
        {
            sCrossSection newc = new sCrossSection();
            
            newc.shapeName = this.shapeName;
            if(this.material != null)
            {
                newc.material = this.material.DuplicatesMaterial();
            }
            newc.sectionType = this.sectionType;
            if(this.dimensions != null)
            {
                newc.dimensions = this.dimensions.ToList();
            }
            newc.weight = this.weight;
            newc.depth = this.depth;
            newc.I_StrongAxis = this.I_StrongAxis;
            return newc;
        }

        public sCrossSection(string AISCname)
        {
            this.shapeName = AISCname;
            this.material = new sMaterial("AISC_Steel", eMaterialType.STEEL_A992_Fy50);

            char[] hss = {'X'};
            if (this.shapeName.Contains("W"))
            {
                this.sectionType = eSectionType.AISC_I_BEAM;

                string xx = this.shapeName.Remove(0, 1);
                string[] infos = xx.Split(hss);
                this.depth = double.Parse(infos[0]);
                this.weight = double.Parse(infos[1]);

                double ix;
                this.GetWShapeProperties(AISCname, out ix);
                this.I_StrongAxis = ix;
            }
            else
            {
                string[] sps = this.shapeName.Split(hss);
                if(sps.Length == 2)
                {
                    this.sectionType = eSectionType.HSS_ROUND;
                }
                else if(sps.Length == 3)
                {
                    this.sectionType = eSectionType.HSS_REC;
                }
            }
        }

        public List<sXYZ> GetRoundFaceVertices_Simple(sPlane secPlane)
        {
            double r = this.dimensions[0] * 0.5;

            sXYZ sp = secPlane.origin;
            sXYZ xaxis = secPlane.Xaxis;
            sXYZ yaxis = secPlane.Yaxis;
            sXYZ zaxis = secPlane.Zaxis;

            //r *= 0.0254;

            List<sXYZ> faceVertices = new List<sXYZ>();
            for (int u = 0; u < 12; ++u)
            {
                double ang = 30.0 * (Math.PI / 180.0);
                ang *= u;
                sXYZ rov = sXYZ.Rotate(zaxis, xaxis, ang);
                rov *= r;
                sXYZ np = new sXYZ(sp.X, sp.Y, sp.Z) + rov;

                faceVertices.Add(np);
            }
            return faceVertices;
        }

        public List<sXYZ> GetSquareFaceVertices_Simple(sPlane secPlane)
        {

            double w = this.dimensions[0];
            double d = this.dimensions[0];

            sXYZ sp = secPlane.origin;
            sXYZ xaxis = secPlane.Xaxis;
            sXYZ yaxis = secPlane.Yaxis;
            sXYZ zaxis = secPlane.Zaxis;

            // d *= 0.0254;  Rhino unit meter only?
            // w *= 0.0254;

            sXYZ halfw = yaxis * (w * 0.5);
            sXYZ halfd = zaxis * (d * 0.5);

            sXYZ p0 = sp + halfw;
            sXYZ p1 = p0 - halfd;
            sXYZ p2 = p1 - halfw;
            sXYZ p3 = p2 - halfw;
            sXYZ p4 = p3 + halfd;
            sXYZ p5 = p4 + halfd;
            sXYZ p6 = p5 + halfw;
            sXYZ p7 = p6 + halfw;

            List<sXYZ> faceVertices = new List<sXYZ>();
            faceVertices.Add(p0);
            faceVertices.Add(p1);
            faceVertices.Add(p2);
            faceVertices.Add(p3);
            faceVertices.Add(p4);
            faceVertices.Add(p5);
            faceVertices.Add(p6);
            faceVertices.Add(p7);
            return faceVertices;
        }

        public List<sXYZ> GetRecFaceVertices_Simple(sPlane secPlane)
        {
            
            double w = this.dimensions[0];
            double d = this.dimensions[1];

            sXYZ sp = secPlane.origin;
            sXYZ xaxis = secPlane.Xaxis;
            sXYZ yaxis = secPlane.Yaxis;
            sXYZ zaxis = secPlane.Zaxis;

           // d *= 0.0254;  Rhino unit meter only?
           // w *= 0.0254;
           
            sXYZ halfw = yaxis * (w * 0.5);
            sXYZ halfd = zaxis * (d * 0.5);

            sXYZ p0 = sp + halfw;
            sXYZ p1 = p0 - halfd;
            sXYZ p2 = p1 - halfw;
            sXYZ p3 = p2 - halfw;
            sXYZ p4 = p3 + halfd;
            sXYZ p5 = p4 + halfd;
            sXYZ p6 = p5 + halfw;
            sXYZ p7 = p6 + halfw;

            List<sXYZ> faceVertices = new List<sXYZ>();
            faceVertices.Add(p0);
            faceVertices.Add(p1);
            faceVertices.Add(p2);
            faceVertices.Add(p3);
            faceVertices.Add(p4);
            faceVertices.Add(p5);
            faceVertices.Add(p6);
            faceVertices.Add(p7);
            return faceVertices;
        }

        public List<sXYZ> GetHSSRecFaceVertices_Simple(sPlane secPlane)
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

            GetHSSRecDimensions(this.shapeName, out w, out d, out t, out ar, out ix, out sx, out rx, out zx, out iy, out sy, out ry, out zy, out jt, out ct, out nw);

            sXYZ sp = secPlane.origin;
            sXYZ xaxis = secPlane.Xaxis;
            sXYZ yaxis = secPlane.Yaxis;
            sXYZ zaxis = secPlane.Zaxis;

            d *= 0.0254;
            w *= 0.0254;
            t *= 0.0254;

            sXYZ halfw = yaxis * (w * 0.5);
            sXYZ halfd = zaxis * (d * 0.5);

            sXYZ p0 = sp + halfw;
            sXYZ p1 = p0 - halfd;
            sXYZ p2 = p1 - halfw;
            sXYZ p3 = p2 - halfw;
            sXYZ p4 = p3 + halfd;
            sXYZ p5 = p4 + halfd;
            sXYZ p6 = p5 + halfw;
            sXYZ p7 = p6 + halfw;

            List<sXYZ> faceVertices = new List<sXYZ>();
            faceVertices.Add(p0);
            faceVertices.Add(p1);
            faceVertices.Add(p2);
            faceVertices.Add(p3);
            faceVertices.Add(p4);
            faceVertices.Add(p5);
            faceVertices.Add(p6);
            faceVertices.Add(p7);
            return faceVertices;
        }

        public List<sXYZ> GetHSSRoundFaceVertices_Simple(sPlane secPlane)
        {
            double od; ///list of variables
            double th;
            double a;
            double i;
            double s;
            double r;
            double z;
            double j;
            double c;
            double nw;

            this.GetHSSRoundDimensions(this.shapeName, out od, out th, out a, out i, out s, out r, out z, out j, out c, out nw);

            sXYZ sp = secPlane.origin;
            sXYZ xaxis = secPlane.Xaxis;
            sXYZ yaxis = secPlane.Yaxis;
            sXYZ zaxis = secPlane.Zaxis;

            r = od * 0.5 * 0.0254;//0.0254;

            List<sXYZ> faceVertices = new List<sXYZ>();
            for (int u = 0; u < 12; ++u)
            {
                double ang = 30.0 * (Math.PI / 180.0);
                ang *= u;
                sXYZ rov = sXYZ.Rotate(zaxis, xaxis, ang);
                rov *= r;
                sXYZ np = new sXYZ(sp.X, sp.Y, sp.Z) + rov;

                faceVertices.Add(np);
            }
            return faceVertices;
        }

        public List<sXYZ> GetWbeamFaceVertices(sPlane secPlane)
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

            GetWBeamDimensions(this.shapeName, out a, out tw, out d, out tf, out bf, out nw, out ix, out sx, out rx, out zx, out iy, out sy, out ry, out zy, out rts, out ho, out j, out c);

            sXYZ sp = secPlane.origin;
            sXYZ xaxis = secPlane.Xaxis;
            sXYZ yaxis = secPlane.Yaxis;
            sXYZ zaxis = secPlane.Zaxis;

            bf *= 0.0254;
            d *= 0.0254;
            tw *= 0.0254;
            tf *= 0.0254;

            double webOnly = (d - 2* tf);
            double innerFlange = (0.5 * (bf - tw));

            sXYZ p0 = sp + (yaxis * (0.5 * tw)) - (zaxis * (webOnly * 0.25));
            sXYZ p1 = p0 - (zaxis * (webOnly * 0.25));
            sXYZ p2 = p1 + (yaxis * innerFlange);
            sXYZ p3 = p2 - (zaxis * tf);
            sXYZ p4 = p3 - (yaxis * bf);
            sXYZ p5 = p4 + (zaxis * tf);
            sXYZ p6 = p5 + (yaxis * innerFlange);
            sXYZ p7 = p6 + (zaxis * (webOnly * 0.25));
            sXYZ p8 = p7 + (zaxis * (webOnly * 0.5));
            sXYZ p9 = p8 + (zaxis * (webOnly * 0.25));
            sXYZ p10 = p9 - (yaxis * innerFlange);
            sXYZ p11 = p10 + (zaxis * tf);
            sXYZ p12 = p11 + (yaxis * bf);
            sXYZ p13 = p12 - (zaxis * tf);
            sXYZ p14 = p13 - (yaxis * innerFlange);
            sXYZ p15 = p14 - (zaxis * (webOnly * 0.25));

            List<sXYZ> faceVertices = new List<sXYZ>();
            faceVertices.Add(p0 );
            faceVertices.Add(p1 );
            faceVertices.Add(p2 );
            faceVertices.Add(p3 );
            faceVertices.Add(p4 );
            faceVertices.Add(p5 );
            faceVertices.Add(p6 );
            faceVertices.Add(p7 );
            faceVertices.Add(p8 );
            faceVertices.Add(p9 );
            faceVertices.Add(p10);
            faceVertices.Add(p11);
            faceVertices.Add(p12);
            faceVertices.Add(p13);
            faceVertices.Add(p14);
            faceVertices.Add(p15);

            return faceVertices;
        }

        public void GetHSSRecDimensions(string sName, out double w, out double d, out double t, out double ar, out double ix, out double sx, out double rx, out double zx, out double iy, out double sy, out double ry, out double zy, out double jt, out double ct, out double nw)
        {
            w = 0.0; d = 0.0; t = 0.0; ar = 0.0; ix = 0.0; sx = 0.0; rx = 0.0; zx = 0.0; iy = 0.0; sy = 0.0; ry = 0.0; zy = 0.0; jt = 0.0; ct = 0.0; nw = 0.0;

            if (sName.Contains("HSS20X12X.625"))
            {
                d = 20; w = 12; ar = 35; t = 0.581; ix = 1880; zx = 230; sx = 188; rx = 7.33; iy = 851; zy = 162; sy = 142; ry = 4.93; jt = 1890; ct = 257; nw = 127.37;
            }
            else if (sName.Contains("HSS20X12X.500"))
            {
                d = 20; w = 12; ar = 28.3; t = 0.465; ix = 1550; zx = 188; sx = 155; rx = 7.39; iy = 705; zy = 132; sy = 117; ry = 4.99; jt = 1540; ct = 209; nw = 103.3;
            }
            else if (sName.Contains("HSS20X12X.375"))
            {
                d = 20; w = 12; ar = 21.5; t = 0.349; ix = 1200; zx = 144; sx = 120; rx = 7.45; iy = 547; zy = 102; sy = 91.1; ry = 5.04; jt = 1180; ct = 160; nw = 78.52;
            }
            else if (sName.Contains("HSS20X12X.313"))
            {
                d = 20; w = 12; ar = 18.1; t = 0.291; ix = 1010; zx = 122; sx = 101; rx = 7.48; iy = 464; zy = 85.8; sy = 77.3; ry = 5.07; jt = 997; ct = 134; nw = 65.87;
            }
            else if (sName.Contains("HSS20X8X.625"))
            {
                d = 20; w = 8; ar = 30.3; t = 0.581; ix = 1440; zx = 185; sx = 144; rx = 6.89; iy = 338; zy = 96.4; sy = 84.6; ry = 3.34; jt = 916; ct = 167; nw = 110.36;
            }
            else if (sName.Contains("HSS20X8X.500"))
            {
                d = 20; w = 8; ar = 24.6; t = 0.465; ix = 1190; zx = 152; sx = 119; rx = 6.96; iy = 283; zy = 79.5; sy = 70.8; ry = 3.39; jt = 757; ct = 137; nw = 89.68;
            }
            else if (sName.Contains("HSS20X8X.375"))
            {
                d = 20; w = 8; ar = 18.7; t = 0.349; ix = 926; zx = 117; sx = 92.6; rx = 7.03; iy = 222; zy = 61.5; sy = 55.6; ry = 3.44; jt = 586; ct = 105; nw = 68.31;
            }
            else if (sName.Contains("HSS20X8X.313"))
            {
                d = 20; w = 8; ar = 15.7; t = 0.291; ix = 786; zx = 98.6; sx = 78.6; rx = 7.07; iy = 189; zy = 52; sy = 47.4; ry = 3.47; jt = 496; ct = 88.3; nw = 57.36;
            }
            else if (sName.Contains("HSS20X4X.500"))
            {
                d = 20; w = 4; ar = 20.9; t = 0.465; ix = 838; zx = 115; sx = 83.8; rx = 6.33; iy = 58.7; zy = 34; sy = 29.3; ry = 1.68; jt = 195; ct = 63.8; nw = 76.07;
            }
            else if (sName.Contains("HSS20X4X.375"))
            {
                d = 20; w = 4; ar = 16; t = 0.349; ix = 657; zx = 89.3; sx = 65.7; rx = 6.42; iy = 47.6; zy = 26.8; sy = 23.8; ry = 1.73; jt = 156; ct = 49.9; nw = 58.1;
            }
            else if (sName.Contains("HSS20X4X.313"))
            {
                d = 20; w = 4; ar = 13.4; t = 0.291; ix = 560; zx = 75.6; sx = 56; rx = 6.46; iy = 41.2; zy = 22.9; sy = 20.6; ry = 1.75; jt = 134; ct = 42.4; nw = 48.86;
            }
            else if (sName.Contains("HSS20X4X.250"))
            {
                d = 20; w = 4; ar = 10.8; t = 0.233; ix = 458; zx = 61.5; sx = 45.8; rx = 6.5; iy = 34.3; zy = 18.7; sy = 17.1; ry = 1.78; jt = 111; ct = 34.7; nw = 39.43;
            }
            else if (sName.Contains("HSS18X6X.625"))
            {
                d = 18; w = 6; ar = 25.7; t = 0.581; ix = 923; zx = 135; sx = 103; rx = 6; iy = 158; zy = 61; sy = 52.7; ry = 2.48; jt = 462; ct = 109; nw = 93.34;
            }
            else if (sName.Contains("HSS18X6X.500"))
            {
                d = 18; w = 6; ar = 20.9; t = 0.465; ix = 770; zx = 112; sx = 85.6; rx = 6.07; iy = 134; zy = 50.7; sy = 44.6; ry = 2.53; jt = 387; ct = 89.9; nw = 76.07;
            }
            else if (sName.Contains("HSS18X6X.375"))
            {
                d = 18; w = 6; ar = 16; t = 0.349; ix = 602; zx = 86.4; sx = 66.9; rx = 6.15; iy = 106; zy = 39.5; sy = 35.5; ry = 2.58; jt = 302; ct = 69.5; nw = 58.1;
            }
            else if (sName.Contains("HSS18X6X.313"))
            {
                d = 18; w = 6; ar = 13.4; t = 0.291; ix = 513; zx = 73.1; sx = 57; rx = 6.18; iy = 91.3; zy = 33.5; sy = 30.4; ry = 2.61; jt = 257; ct = 58.7; nw = 48.86;
            }
            else if (sName.Contains("HSS18X6X.250"))
            {
                d = 18; w = 6; ar = 10.8; t = 0.233; ix = 419; zx = 59.4; sx = 46.5; rx = 6.22; iy = 75.1; zy = 27.3; sy = 25; ry = 2.63; jt = 210; ct = 47.7; nw = 39.43;
            }
            else if (sName.Contains("HSS16X16X.625"))
            {
                d = 16; w = 16; ar = 35; t = 0.581; ix = 1370; zx = 200; sx = 171; rx = 6.25; iy = 1370; zy = 200; sy = 171; ry = 6.25; jt = 2170; ct = 276; nw = 127.37;
            }
            else if (sName.Contains("HSS16X16X.500"))
            {
                d = 16; w = 16; ar = 28.3; t = 0.465; ix = 1130; zx = 164; sx = 141; rx = 6.31; iy = 1130; zy = 164; sy = 141; ry = 6.31; jt = 1770; ct = 224; nw = 103.3;
            }
            else if (sName.Contains("HSS16X16X.375"))
            {
                d = 16; w = 16; ar = 21.5; t = 0.349; ix = 873; zx = 126; sx = 109; rx = 6.37; iy = 873; zy = 126; sy = 109; ry = 6.37; jt = 1350; ct = 171; nw = 78.52;
            }
            else if (sName.Contains("HSS16X16X.313"))
            {
                d = 16; w = 16; ar = 18.1; t = 0.291; ix = 739; zx = 106; sx = 92.3; rx = 6.39; iy = 739; zy = 106; sy = 92.3; ry = 6.39; jt = 1140; ct = 144; nw = 65.87;
            }
            else if (sName.Contains("HSS16X12X.625"))
            {
                d = 16; w = 12; ar = 30.3; t = 0.581; ix = 1090; zx = 165; sx = 136; rx = 6; iy = 700; zy = 135; sy = 117; ry = 4.8; jt = 1370; ct = 204; nw = 110.36;
            }
            else if (sName.Contains("HSS16X12X.500"))
            {
                d = 16; w = 12; ar = 24.6; t = 0.465; ix = 904; zx = 135; sx = 113; rx = 6.06; iy = 581; zy = 111; sy = 96.8; ry = 4.86; jt = 1120; ct = 166; nw = 89.68;
            }
            else if (sName.Contains("HSS16X12X.375"))
            {
                d = 16; w = 12; ar = 18.7; t = 0.349; ix = 702; zx = 104; sx = 87.7; rx = 6.12; iy = 452; zy = 85.5; sy = 75.3; ry = 4.91; jt = 862; ct = 127; nw = 68.31;
            }
            else if (sName.Contains("HSS16X12X.313"))
            {
                d = 16; w = 12; ar = 15.7; t = 0.291; ix = 595; zx = 87.7; sx = 74.4; rx = 6.15; iy = 384; zy = 72.2; sy = 64; ry = 4.94; jt = 727; ct = 107; nw = 57.36;
            }
            else if (sName.Contains("HSS16X8X.625"))
            {
                d = 16; w = 8; ar = 25.7; t = 0.581; ix = 815; zx = 129; sx = 102; rx = 5.64; iy = 274; zy = 79.2; sy = 68.6; ry = 3.27; jt = 681; ct = 132; nw = 93.34;
            }
            else if (sName.Contains("HSS16X8X.500"))
            {
                d = 16; w = 8; ar = 20.9; t = 0.465; ix = 679; zx = 106; sx = 84.9; rx = 5.7; iy = 230; zy = 65.5; sy = 57.6; ry = 3.32; jt = 563; ct = 108; nw = 76.07;
            }
            else if (sName.Contains("HSS16X8X.375"))
            {
                d = 16; w = 8; ar = 16; t = 0.349; ix = 531; zx = 82.1; sx = 66.3; rx = 5.77; iy = 181; zy = 50.8; sy = 45.3; ry = 3.37; jt = 436; ct = 83.4; nw = 58.1;
            }
            else if (sName.Contains("HSS16X8X.313"))
            {
                d = 16; w = 8; ar = 13.4; t = 0.291; ix = 451; zx = 69.4; sx = 56.4; rx = 5.8; iy = 155; zy = 43; sy = 38.7; ry = 3.4; jt = 369; ct = 70.4; nw = 48.86;
            }
            else if (sName.Contains("HSS16X8X.250"))
            {
                d = 16; w = 8; ar = 10.8; t = 0.233; ix = 368; zx = 56.4; sx = 46.1; rx = 5.83; iy = 127; zy = 35; sy = 31.7; ry = 3.42; jt = 300; ct = 57; nw = 39.43;
            }
            else if (sName.Contains("HSS16X4X.625"))
            {
                d = 16; w = 4; ar = 21; t = 0.581; ix = 539; zx = 92.9; sx = 67.3; rx = 5.06; iy = 54.1; zy = 32.5; sy = 27; ry = 1.6; jt = 174; ct = 60.5; nw = 76.33;
            }
            else if (sName.Contains("HSS16X4X.500"))
            {
                d = 16; w = 4; ar = 17.2; t = 0.465; ix = 455; zx = 77.3; sx = 56.9; rx = 5.15; iy = 47; zy = 27.4; sy = 23.5; ry = 1.65; jt = 150; ct = 50.7; nw = 62.46;
            }
            else if (sName.Contains("HSS16X4X.375"))
            {
                d = 16; w = 4; ar = 13.2; t = 0.349; ix = 360; zx = 60.2; sx = 45; rx = 5.23; iy = 38.3; zy = 21.7; sy = 19.1; ry = 1.71; jt = 120; ct = 39.7; nw = 47.9;
            }
            else if (sName.Contains("HSS16X4X.313"))
            {
                d = 16; w = 4; ar = 11.1; t = 0.291; ix = 308; zx = 51.1; sx = 38.5; rx = 5.27; iy = 33.2; zy = 18.5; sy = 16.6; ry = 1.73; jt = 103; ct = 33.8; nw = 40.35;
            }
            else if (sName.Contains("HSS16X4X.250"))
            {
                d = 16; w = 4; ar = 8.96; t = 0.233; ix = 253; zx = 41.7; sx = 31.6; rx = 5.31; iy = 27.7; zy = 15.2; sy = 13.8; ry = 1.76; jt = 85.2; ct = 27.6; nw = 32.63;
            }
            else if (sName.Contains("HSS16X4X.188"))
            {
                d = 16; w = 4; ar = 6.76; t = 0.174; ix = 193; zx = 31.7; sx = 24.2; rx = 5.35; iy = 21.5; zy = 11.7; sy = 10.8; ry = 1.78; jt = 65.5; ct = 21.1; nw = 24.73;
            }
            else if (sName.Contains("HSS14X14X.625"))
            {
                d = 14; w = 14; ar = 30.3; t = 0.581; ix = 897; zx = 151; sx = 128; rx = 5.44; iy = 897; zy = 151; sy = 128; ry = 5.44; jt = 1430; ct = 208; nw = 110.36;
            }
            else if (sName.Contains("HSS14X14X.500"))
            {
                d = 14; w = 14; ar = 24.6; t = 0.465; ix = 743; zx = 124; sx = 106; rx = 5.49; iy = 743; zy = 124; sy = 106; ry = 5.49; jt = 1170; ct = 170; nw = 89.68;
            }
            else if (sName.Contains("HSS14X14X.375"))
            {
                d = 14; w = 14; ar = 18.7; t = 0.349; ix = 577; zx = 95.4; sx = 82.5; rx = 5.55; iy = 577; zy = 95.4; sy = 82.5; ry = 5.55; jt = 900; ct = 130; nw = 68.31;
            }
            else if (sName.Contains("HSS14X14X.313"))
            {
                d = 14; w = 14; ar = 15.7; t = 0.291; ix = 490; zx = 80.5; sx = 69.9; rx = 5.58; iy = 490; zy = 80.5; sy = 69.9; ry = 5.58; jt = 759; ct = 109; nw = 57.36;
            }
            else if (sName.Contains("HSS14X10X.625"))
            {
                d = 14; w = 10; ar = 25.7; t = 0.581; ix = 687; zx = 120; sx = 98.2; rx = 5.17; iy = 407; zy = 95.1; sy = 81.5; ry = 3.98; jt = 832; ct = 146; nw = 93.34;
            }
            else if (sName.Contains("HSS14X10X.500"))
            {
                d = 14; w = 10; ar = 20.9; t = 0.465; ix = 573; zx = 98.8; sx = 81.8; rx = 5.23; iy = 341; zy = 78.5; sy = 68.1; ry = 4.04; jt = 685; ct = 120; nw = 76.07;
            }
            else if (sName.Contains("HSS14X10X.375"))
            {
                d = 14; w = 10; ar = 16; t = 0.349; ix = 447; zx = 76.3; sx = 63.9; rx = 5.29; iy = 267; zy = 60.7; sy = 53.4; ry = 4.09; jt = 528; ct = 91.8; nw = 58.1;
            }
            else if (sName.Contains("HSS14X10X.313"))
            {
                d = 14; w = 10; ar = 13.4; t = 0.291; ix = 380; zx = 64.6; sx = 54.3; rx = 5.32; iy = 227; zy = 51.4; sy = 45.5; ry = 4.12; jt = 446; ct = 77.4; nw = 48.86;
            }
            else if (sName.Contains("HSS14X10X.250"))
            {
                d = 14; w = 10; ar = 10.8; t = 0.233; ix = 310; zx = 52.4; sx = 44.3; rx = 5.35; iy = 186; zy = 41.8; sy = 37.2; ry = 4.14; jt = 362; ct = 62.6; nw = 39.43;
            }
            else if (sName.Contains("HSS14X6X.625"))
            {
                d = 14; w = 6; ar = 21; t = 0.581; ix = 478; zx = 88.7; sx = 68.3; rx = 4.77; iy = 124; zy = 48.4; sy = 41.2; ry = 2.43; jt = 334; ct = 83.7; nw = 76.33;
            }
            else if (sName.Contains("HSS14X6X.500"))
            {
                d = 14; w = 6; ar = 17.2; t = 0.465; ix = 402; zx = 73.6; sx = 57.4; rx = 4.84; iy = 105; zy = 40.4; sy = 35.1; ry = 2.48; jt = 279; ct = 69.3; nw = 62.46;
            }
            else if (sName.Contains("HSS14X6X.375"))
            {
                d = 14; w = 6; ar = 13.2; t = 0.349; ix = 317; zx = 57.3; sx = 45.3; rx = 4.91; iy = 84.1; zy = 31.6; sy = 28; ry = 2.53; jt = 219; ct = 53.7; nw = 47.9;
            }
            else if (sName.Contains("HSS14X6X.313"))
            {
                d = 14; w = 6; ar = 11.1; t = 0.291; ix = 271; zx = 48.6; sx = 38.7; rx = 4.94; iy = 72.3; zy = 26.9; sy = 24.1; ry = 2.55; jt = 186; ct = 45.5; nw = 40.35;
            }
            else if (sName.Contains("HSS14X6X.250"))
            {
                d = 14; w = 6; ar = 8.96; t = 0.233; ix = 222; zx = 39.6; sx = 31.7; rx = 4.98; iy = 59.6; zy = 22; sy = 19.9; ry = 2.58; jt = 152; ct = 36.9; nw = 32.63;
            }
            else if (sName.Contains("HSS14X6X.188"))
            {
                d = 14; w = 6; ar = 6.76; t = 0.174; ix = 170; zx = 30.1; sx = 24.3; rx = 5.01; iy = 45.9; zy = 16.7; sy = 15.3; ry = 2.61; jt = 116; ct = 28; nw = 24.73;
            }
            else if (sName.Contains("HSS14X4X.625"))
            {
                d = 14; w = 4; ar = 18.7; t = 0.581; ix = 373; zx = 73.1; sx = 53.3; rx = 4.47; iy = 47.2; zy = 28.5; sy = 23.6; ry = 1.59; jt = 148; ct = 52.6; nw = 67.82;
            }
            else if (sName.Contains("HSS14X4X.500"))
            {
                d = 14; w = 4; ar = 15.3; t = 0.465; ix = 317; zx = 61; sx = 45.3; rx = 4.55; iy = 41.2; zy = 24.1; sy = 20.6; ry = 1.64; jt = 127; ct = 44.1; nw = 55.66;
            }
            else if (sName.Contains("HSS14X4X.375"))
            {
                d = 14; w = 4; ar = 11.8; t = 0.349; ix = 252; zx = 47.8; sx = 36; rx = 4.63; iy = 33.6; zy = 19.1; sy = 16.8; ry = 1.69; jt = 102; ct = 34.6; nw = 42.79;
            }
            else if (sName.Contains("HSS14X4X.313"))
            {
                d = 14; w = 4; ar = 9.92; t = 0.291; ix = 216; zx = 40.6; sx = 30.9; rx = 4.67; iy = 29.2; zy = 16.4; sy = 14.6; ry = 1.72; jt = 87.7; ct = 29.5; nw = 36.1;
            }
            else if (sName.Contains("HSS14X4X.250"))
            {
                d = 14; w = 4; ar = 8.03; t = 0.233; ix = 178; zx = 33.2; sx = 25.4; rx = 4.71; iy = 24.4; zy = 13.5; sy = 12.2; ry = 1.74; jt = 72.4; ct = 24.1; nw = 29.23;
            }
            else if (sName.Contains("HSS14X4X.188"))
            {
                d = 14; w = 4; ar = 6.06; t = 0.174; ix = 137; zx = 25.3; sx = 19.5; rx = 4.74; iy = 19; zy = 10.3; sy = 9.48; ry = 1.77; jt = 55.8; ct = 18.4; nw = 22.18;
            }
            else if (sName.Contains("HSS12X12X.625"))
            {
                d = 12; w = 12; ar = 25.7; t = 0.581; ix = 548; zx = 109; sx = 91.4; rx = 4.62; iy = 548; zy = 109; sy = 91.4; ry = 4.62; jt = 885; ct = 151; nw = 93.34;
            }
            else if (sName.Contains("HSS12X12X.500"))
            {
                d = 12; w = 12; ar = 20.9; t = 0.465; ix = 457; zx = 89.6; sx = 76.2; rx = 4.68; iy = 457; zy = 89.6; sy = 76.2; ry = 4.68; jt = 728; ct = 123; nw = 76.07;
            }
            else if (sName.Contains("HSS12X12X.375"))
            {
                d = 12; w = 12; ar = 16; t = 0.349; ix = 357; zx = 69.2; sx = 59.5; rx = 4.73; iy = 357; zy = 69.2; sy = 59.5; ry = 4.73; jt = 561; ct = 94.6; nw = 58.1;
            }
            else if (sName.Contains("HSS12X12X.313"))
            {
                d = 12; w = 12; ar = 13.4; t = 0.291; ix = 304; zx = 58.6; sx = 50.7; rx = 4.76; iy = 304; zy = 58.6; sy = 50.7; ry = 4.76; jt = 474; ct = 79.7; nw = 48.86;
            }
            else if (sName.Contains("HSS12X12X.250"))
            {
                d = 12; w = 12; ar = 10.8; t = 0.233; ix = 248; zx = 47.6; sx = 41.4; rx = 4.79; iy = 248; zy = 47.6; sy = 41.4; ry = 4.79; jt = 384; ct = 64.5; nw = 39.43;
            }
            else if (sName.Contains("HSS12X12X.188"))
            {
                d = 12; w = 12; ar = 8.15; t = 0.174; ix = 189; zx = 36; sx = 31.5; rx = 4.82; iy = 189; zy = 36; sy = 31.5; ry = 4.82; jt = 290; ct = 48.6; nw = 29.84;
            }
            else if (sName.Contains("HSS12X10X.500"))
            {
                d = 12; w = 10; ar = 19; t = 0.465; ix = 395; zx = 78.8; sx = 65.9; rx = 4.56; iy = 298; zy = 69.6; sy = 59.7; ry = 3.96; jt = 545; ct = 102; nw = 69.27;
            }
            else if (sName.Contains("HSS12X10X.375"))
            {
                d = 12; w = 10; ar = 14.6; t = 0.349; ix = 310; zx = 61.1; sx = 51.6; rx = 4.61; iy = 234; zy = 54; sy = 46.9; ry = 4.01; jt = 421; ct = 78.3; nw = 53;
            }
            else if (sName.Contains("HSS12X10X.313"))
            {
                d = 12; w = 10; ar = 12.2; t = 0.291; ix = 264; zx = 51.7; sx = 44; rx = 4.64; iy = 200; zy = 45.7; sy = 40; ry = 4.04; jt = 356; ct = 66.1; nw = 44.6;
            }
            else if (sName.Contains("HSS12X10X.250"))
            {
                d = 12; w = 10; ar = 9.9; t = 0.233; ix = 216; zx = 42.1; sx = 36; rx = 4.67; iy = 164; zy = 37.2; sy = 32.7; ry = 4.07; jt = 289; ct = 53.5; nw = 36.03;
            }
            else if (sName.Contains("HSS12X8X.625"))
            {
                d = 12; w = 8; ar = 21; t = 0.581; ix = 397; zx = 82.1; sx = 66.1; rx = 4.34; iy = 210; zy = 61.9; sy = 52.5; ry = 3.16; jt = 454; ct = 97.7; nw = 76.33;
            }
            else if (sName.Contains("HSS12X8X.500"))
            {
                d = 12; w = 8; ar = 17.2; t = 0.465; ix = 333; zx = 68.1; sx = 55.6; rx = 4.41; iy = 178; zy = 51.5; sy = 44.4; ry = 3.21; jt = 377; ct = 80.4; nw = 62.46;
            }
            else if (sName.Contains("HSS12X8X.375"))
            {
                d = 12; w = 8; ar = 13.2; t = 0.349; ix = 262; zx = 53; sx = 43.7; rx = 4.47; iy = 140; zy = 40.1; sy = 35.1; ry = 3.27; jt = 293; ct = 62.1; nw = 47.9;
            }
            else if (sName.Contains("HSS12X8X.313"))
            {
                d = 12; w = 8; ar = 11.1; t = 0.291; ix = 224; zx = 44.9; sx = 37.4; rx = 4.5; iy = 120; zy = 34.1; sy = 30.1; ry = 3.29; jt = 248; ct = 52.4; nw = 40.35;
            }
            else if (sName.Contains("HSS12X8X.250"))
            {
                d = 12; w = 8; ar = 8.96; t = 0.233; ix = 184; zx = 36.6; sx = 30.6; rx = 4.53; iy = 98.8; zy = 27.8; sy = 24.7; ry = 3.32; jt = 202; ct = 42.5; nw = 32.63;
            }
            else if (sName.Contains("HSS12X8X.188"))
            {
                d = 12; w = 8; ar = 6.76; t = 0.174; ix = 140; zx = 27.8; sx = 23.4; rx = 4.56; iy = 75.7; zy = 21.1; sy = 18.9; ry = 3.35; jt = 153; ct = 32.2; nw = 24.73;
            }
            else if (sName.Contains("HSS12X6X.625"))
            {
                d = 12; w = 6; ar = 18.7; t = 0.581; ix = 321; zx = 68.8; sx = 53.4; rx = 4.14; iy = 107; zy = 42.1; sy = 35.5; ry = 2.39; jt = 271; ct = 71.1; nw = 67.82;
            }
            else if (sName.Contains("HSS12X6X.500"))
            {
                d = 12; w = 6; ar = 15.3; t = 0.465; ix = 271; zx = 57.4; sx = 45.2; rx = 4.21; iy = 91.1; zy = 35.2; sy = 30.4; ry = 2.44; jt = 227; ct = 59; nw = 55.66;
            }
            else if (sName.Contains("HSS12X6X.375"))
            {
                d = 12; w = 6; ar = 11.8; t = 0.349; ix = 215; zx = 44.8; sx = 35.9; rx = 4.28; iy = 72.9; zy = 27.7; sy = 24.3; ry = 2.49; jt = 178; ct = 45.8; nw = 42.79;
            }
            else if (sName.Contains("HSS12X6X.313"))
            {
                d = 12; w = 6; ar = 9.92; t = 0.291; ix = 184; zx = 38.1; sx = 30.7; rx = 4.31; iy = 62.8; zy = 23.6; sy = 20.9; ry = 2.52; jt = 152; ct = 38.8; nw = 36.1;
            }
            else if (sName.Contains("HSS12X6X.250"))
            {
                d = 12; w = 6; ar = 8.03; t = 0.233; ix = 151; zx = 31.1; sx = 25.2; rx = 4.34; iy = 51.9; zy = 19.3; sy = 17.3; ry = 2.54; jt = 124; ct = 31.6; nw = 29.23;
            }
            else if (sName.Contains("HSS12X6X.188"))
            {
                d = 12; w = 6; ar = 6.06; t = 0.174; ix = 116; zx = 23.7; sx = 19.4; rx = 4.38; iy = 40; zy = 14.7; sy = 13.3; ry = 2.57; jt = 94.6; ct = 24; nw = 22.18;
            }
            else if (sName.Contains("HSS12X4X.625"))
            {
                d = 12; w = 4; ar = 16.4; t = 0.581; ix = 245; zx = 55.5; sx = 40.8; rx = 3.87; iy = 40.4; zy = 24.5; sy = 20.2; ry = 1.57; jt = 122; ct = 44.6; nw = 59.32;
            }
            else if (sName.Contains("HSS12X4X.500"))
            {
                d = 12; w = 4; ar = 13.5; t = 0.465; ix = 210; zx = 46.7; sx = 34.9; rx = 3.95; iy = 35.3; zy = 20.9; sy = 17.7; ry = 1.62; jt = 105; ct = 37.5; nw = 48.85;
            }
            else if (sName.Contains("HSS12X4X.375"))
            {
                d = 12; w = 4; ar = 10.4; t = 0.349; ix = 168; zx = 36.7; sx = 28; rx = 4.02; iy = 28.9; zy = 16.6; sy = 14.5; ry = 1.67; jt = 84.1; ct = 29.5; nw = 37.69;
            }
            else if (sName.Contains("HSS12X4X.313"))
            {
                d = 12; w = 4; ar = 8.76; t = 0.291; ix = 144; zx = 31.3; sx = 24.1; rx = 4.06; iy = 25.2; zy = 14.2; sy = 12.6; ry = 1.7; jt = 72.4; ct = 25.2; nw = 31.84;
            }
            else if (sName.Contains("HSS12X4X.250"))
            {
                d = 12; w = 4; ar = 7.1; t = 0.233; ix = 119; zx = 25.6; sx = 19.9; rx = 4.1; iy = 21; zy = 11.7; sy = 10.5; ry = 1.72; jt = 59.8; ct = 20.6; nw = 25.82;
            }
            else if (sName.Contains("HSS12X4X.188"))
            {
                d = 12; w = 4; ar = 5.37; t = 0.174; ix = 91.8; zx = 19.6; sx = 15.3; rx = 4.13; iy = 16.4; zy = 9; sy = 8.2; ry = 1.75; jt = 46.1; ct = 15.7; nw = 19.63;
            }
            else if (sName.Contains("HSS12X3-1/2X.375"))
            {
                d = 12; w = 3.5; ar = 10; t = 0.349; ix = 156; zx = 34.7; sx = 26; rx = 3.94; iy = 21.3; zy = 14; sy = 12.2; ry = 1.46; jt = 64.7; ct = 25.5; nw = 36.41;
            }
            else if (sName.Contains("HSS12X3-1/2X.313"))
            {
                d = 12; w = 3.5; ar = 8.46; t = 0.291; ix = 134; zx = 29.6; sx = 22.4; rx = 3.98; iy = 18.6; zy = 12.1; sy = 10.6; ry = 1.48; jt = 56; ct = 21.8; nw = 30.78;
            }
            else if (sName.Contains("HSS12X3X.313"))
            {
                d = 12; w = 3; ar = 8.17; t = 0.291; ix = 124; zx = 27.9; sx = 20.7; rx = 3.9; iy = 13.1; zy = 10; sy = 8.73; ry = 1.27; jt = 41.3; ct = 18.4; nw = 29.72;
            }
            else if (sName.Contains("HSS12X3X.250"))
            {
                d = 12; w = 3; ar = 6.63; t = 0.233; ix = 103; zx = 22.9; sx = 17.2; rx = 3.94; iy = 11.1; zy = 8.28; sy = 7.38; ry = 1.29; jt = 34.5; ct = 15.1; nw = 24.12;
            }
            else if (sName.Contains("HSS12X3X.188"))
            {
                d = 12; w = 3; ar = 5.02; t = 0.174; ix = 79.6; zx = 17.5; sx = 13.3; rx = 3.98; iy = 8.72; zy = 6.4; sy = 5.81; ry = 1.32; jt = 26.8; ct = 11.6; nw = 18.35;
            }
            else if (sName.Contains("HSS12X2X.313"))
            {
                d = 12; w = 2; ar = 7.59; t = 0.291; ix = 104; zx = 24.5; sx = 17.4; rx = 3.71; iy = 5.1; zy = 6.05; sy = 5.1; ry = 0.82; jt = 17.6; ct = 11.6; nw = 27.59;
            }
            else if (sName.Contains("HSS12X2X.250"))
            {
                d = 12; w = 2; ar = 6.17; t = 0.233; ix = 86.9; zx = 20.1; sx = 14.5; rx = 3.75; iy = 4.41; zy = 5.08; sy = 4.41; ry = 0.845; jt = 15.1; ct = 9.64; nw = 22.42;
            }
            else if (sName.Contains("HSS12X2X.188"))
            {
                d = 12; w = 2; ar = 4.67; t = 0.174; ix = 67.4; zx = 15.5; sx = 11.2; rx = 3.8; iy = 3.55; zy = 3.97; sy = 3.55; ry = 0.872; jt = 12; ct = 7.49; nw = 17.08;
            }
            else if (sName.Contains("HSS10X10X.625"))
            {
                d = 10; w = 10; ar = 21; t = 0.581; ix = 304; zx = 73.2; sx = 60.8; rx = 3.8; iy = 304; zy = 73.2; sy = 60.8; ry = 3.8; jt = 498; ct = 102; nw = 76.33;
            }
            else if (sName.Contains("HSS10X10X.500"))
            {
                d = 10; w = 10; ar = 17.2; t = 0.465; ix = 256; zx = 60.7; sx = 51.2; rx = 3.86; iy = 256; zy = 60.7; sy = 51.2; ry = 3.86; jt = 412; ct = 84.2; nw = 62.46;
            }
            else if (sName.Contains("HSS10X10X.375"))
            {
                d = 10; w = 10; ar = 13.2; t = 0.349; ix = 202; zx = 47.2; sx = 40.4; rx = 3.92; iy = 202; zy = 47.2; sy = 40.4; ry = 3.92; jt = 320; ct = 64.8; nw = 47.9;
            }
            else if (sName.Contains("HSS10X10X.313"))
            {
                d = 10; w = 10; ar = 11.1; t = 0.291; ix = 172; zx = 40.1; sx = 34.5; rx = 3.94; iy = 172; zy = 40.1; sy = 34.5; ry = 3.94; jt = 271; ct = 54.8; nw = 40.35;
            }
            else if (sName.Contains("HSS10X10X.250"))
            {
                d = 10; w = 10; ar = 8.96; t = 0.233; ix = 141; zx = 32.7; sx = 28.3; rx = 3.97; iy = 141; zy = 32.7; sy = 28.3; ry = 3.97; jt = 220; ct = 44.4; nw = 32.63;
            }
            else if (sName.Contains("HSS10X10X.188"))
            {
                d = 10; w = 10; ar = 6.76; t = 0.174; ix = 108; zx = 24.8; sx = 21.6; rx = 4; iy = 108; zy = 24.8; sy = 21.6; ry = 4; jt = 167; ct = 33.6; nw = 24.73;
            }
            else if (sName.Contains("HSS10X8X.625"))
            {
                d = 10; w = 8; ar = 18.7; t = 0.581; ix = 253; zx = 62.2; sx = 50.5; rx = 3.68; iy = 178; zy = 53.3; sy = 44.5; ry = 3.09; jt = 346; ct = 80.4; nw = 67.82;
            }
            else if (sName.Contains("HSS10X8X.500"))
            {
                d = 10; w = 8; ar = 15.3; t = 0.465; ix = 214; zx = 51.9; sx = 42.7; rx = 3.73; iy = 151; zy = 44.5; sy = 37.8; ry = 3.14; jt = 288; ct = 66.4; nw = 55.66;
            }
            else if (sName.Contains("HSS10X8X.375"))
            {
                d = 10; w = 8; ar = 11.8; t = 0.349; ix = 169; zx = 40.5; sx = 33.9; rx = 3.79; iy = 120; zy = 34.8; sy = 30; ry = 3.19; jt = 224; ct = 51.4; nw = 42.79;
            }
            else if (sName.Contains("HSS10X8X.313"))
            {
                d = 10; w = 8; ar = 9.92; t = 0.291; ix = 145; zx = 34.4; sx = 29; rx = 3.82; iy = 103; zy = 29.6; sy = 25.7; ry = 3.22; jt = 190; ct = 43.5; nw = 36.1;
            }
            else if (sName.Contains("HSS10X8X.250"))
            {
                d = 10; w = 8; ar = 8.03; t = 0.233; ix = 119; zx = 28.1; sx = 23.8; rx = 3.85; iy = 84.7; zy = 24.2; sy = 21.2; ry = 3.25; jt = 155; ct = 35.3; nw = 29.23;
            }
            else if (sName.Contains("HSS10X8X.188"))
            {
                d = 10; w = 8; ar = 6.06; t = 0.174; ix = 91.4; zx = 21.4; sx = 18.3; rx = 3.88; iy = 65.1; zy = 18.4; sy = 16.3; ry = 3.28; jt = 118; ct = 26.7; nw = 22.18;
            }
            else if (sName.Contains("HSS10X6X.625"))
            {
                d = 10; w = 6; ar = 16.4; t = 0.581; ix = 201; zx = 51.3; sx = 40.2; rx = 3.5; iy = 89.4; zy = 35.8; sy = 29.8; ry = 2.34; jt = 209; ct = 58.6; nw = 59.32;
            }
            else if (sName.Contains("HSS10X6X.500"))
            {
                d = 10; w = 6; ar = 13.5; t = 0.465; ix = 171; zx = 43; sx = 34.3; rx = 3.57; iy = 76.8; zy = 30.1; sy = 25.6; ry = 2.39; jt = 176; ct = 48.7; nw = 48.85;
            }
            else if (sName.Contains("HSS10X6X.375"))
            {
                d = 10; w = 6; ar = 10.4; t = 0.349; ix = 137; zx = 33.8; sx = 27.4; rx = 3.63; iy = 61.8; zy = 23.7; sy = 20.6; ry = 2.44; jt = 139; ct = 37.9; nw = 37.69;
            }
            else if (sName.Contains("HSS10X6X.313"))
            {
                d = 10; w = 6; ar = 8.76; t = 0.291; ix = 118; zx = 28.8; sx = 23.5; rx = 3.66; iy = 53.3; zy = 20.2; sy = 17.8; ry = 2.47; jt = 118; ct = 32.2; nw = 31.84;
            }
            else if (sName.Contains("HSS10X6X.250"))
            {
                d = 10; w = 6; ar = 7.1; t = 0.233; ix = 96.9; zx = 23.6; sx = 19.4; rx = 3.69; iy = 44.1; zy = 16.6; sy = 14.7; ry = 2.49; jt = 96.7; ct = 26.2; nw = 25.82;
            }
            else if (sName.Contains("HSS10X6X.188"))
            {
                d = 10; w = 6; ar = 5.37; t = 0.174; ix = 74.6; zx = 18; sx = 14.9; rx = 3.73; iy = 34.1; zy = 12.7; sy = 11.4; ry = 2.52; jt = 73.8; ct = 19.9; nw = 19.63;
            }
            else if (sName.Contains("HSS10X5X.375"))
            {
                d = 10; w = 5; ar = 9.67; t = 0.349; ix = 120; zx = 30.4; sx = 24.1; rx = 3.53; iy = 40.6; zy = 18.7; sy = 16.2; ry = 2.05; jt = 100; ct = 31.2; nw = 35.13;
            }
            else if (sName.Contains("HSS10X5X.313"))
            {
                d = 10; w = 5; ar = 8.17; t = 0.291; ix = 104; zx = 26; sx = 20.8; rx = 3.56; iy = 35.2; zy = 16; sy = 14.1; ry = 2.07; jt = 86; ct = 26.5; nw = 29.72;
            }
            else if (sName.Contains("HSS10X5X.250"))
            {
                d = 10; w = 5; ar = 6.63; t = 0.233; ix = 85.8; zx = 21.3; sx = 17.2; rx = 3.6; iy = 29.3; zy = 13.2; sy = 11.7; ry = 2.1; jt = 70.7; ct = 21.6; nw = 24.12;
            }
            else if (sName.Contains("HSS10X5X.188"))
            {
                d = 10; w = 5; ar = 5.02; t = 0.174; ix = 66.2; zx = 16.3; sx = 13.2; rx = 3.63; iy = 22.7; zy = 10.1; sy = 9.09; ry = 2.13; jt = 54.1; ct = 16.5; nw = 18.35;
            }
            else if (sName.Contains("HSS10X4X.625"))
            {
                d = 10; w = 4; ar = 14; t = 0.581; ix = 149; zx = 40.3; sx = 29.9; rx = 3.26; iy = 33.5; zy = 20.6; sy = 16.8; ry = 1.54; jt = 95.7; ct = 36.7; nw = 50.81;
            }
            else if (sName.Contains("HSS10X4X.500"))
            {
                d = 10; w = 4; ar = 11.6; t = 0.465; ix = 129; zx = 34.1; sx = 25.8; rx = 3.34; iy = 29.5; zy = 17.6; sy = 14.7; ry = 1.59; jt = 82.6; ct = 31; nw = 42.05;
            }
            else if (sName.Contains("HSS10X4X.375"))
            {
                d = 10; w = 4; ar = 8.97; t = 0.349; ix = 104; zx = 27; sx = 20.8; rx = 3.41; iy = 24.3; zy = 14; sy = 12.1; ry = 1.64; jt = 66.5; ct = 24.4; nw = 32.58;
            }
            else if (sName.Contains("HSS10X4X.313"))
            {
                d = 10; w = 4; ar = 7.59; t = 0.291; ix = 90.1; zx = 23.1; sx = 18; rx = 3.44; iy = 21.2; zy = 12.1; sy = 10.6; ry = 1.67; jt = 57.3; ct = 20.9; nw = 27.59;
            }
            else if (sName.Contains("HSS10X4X.250"))
            {
                d = 10; w = 4; ar = 6.17; t = 0.233; ix = 74.7; zx = 19; sx = 14.9; rx = 3.48; iy = 17.7; zy = 10; sy = 8.87; ry = 1.7; jt = 47.4; ct = 17.1; nw = 22.42;
            }
            else if (sName.Contains("HSS10X4X.188"))
            {
                d = 10; w = 4; ar = 4.67; t = 0.174; ix = 57.8; zx = 14.6; sx = 11.6; rx = 3.52; iy = 13.9; zy = 7.66; sy = 6.93; ry = 1.72; jt = 36.5; ct = 13.1; nw = 17.08;
            }
            else if (sName.Contains("HSS10X4X.125"))
            {
                d = 10; w = 4; ar = 3.16; t = 0.116; ix = 39.8; zx = 10; sx = 7.97; rx = 3.55; iy = 9.65; zy = 5.26; sy = 4.83; ry = 1.75; jt = 25.1; ct = 8.9; nw = 11.56;
            }
            else if (sName.Contains("HSS10X3-1/2X.500"))
            {
                d = 10; w = 3.5; ar = 11.1; t = 0.465; ix = 118; zx = 31.9; sx = 23.7; rx = 3.26; iy = 21.4; zy = 14.7; sy = 12.2; ry = 1.39; jt = 63.2; ct = 26.5; nw = 40.34;
            }
            else if (sName.Contains("HSS10X3-1/2X.375"))
            {
                d = 10; w = 3.5; ar = 8.62; t = 0.349; ix = 96.1; zx = 25.3; sx = 19.2; rx = 3.34; iy = 17.8; zy = 11.8; sy = 10.2; ry = 1.44; jt = 51.5; ct = 21.1; nw = 31.31;
            }
            else if (sName.Contains("HSS10X3-1/2X.313"))
            {
                d = 10; w = 3.5; ar = 7.3; t = 0.291; ix = 83.2; zx = 21.7; sx = 16.6; rx = 3.38; iy = 15.6; zy = 10.2; sy = 8.92; ry = 1.46; jt = 44.6; ct = 18; nw = 26.53;
            }
            else if (sName.Contains("HSS10X3-1/2X.250"))
            {
                d = 10; w = 3.5; ar = 5.93; t = 0.233; ix = 69.1; zx = 17.9; sx = 13.8; rx = 3.41; iy = 13.1; zy = 8.45; sy = 7.51; ry = 1.49; jt = 37; ct = 14.8; nw = 21.57;
            }
            else if (sName.Contains("HSS10X3-1/2X.188"))
            {
                d = 10; w = 3.5; ar = 4.5; t = 0.174; ix = 53.6; zx = 13.7; sx = 10.7; rx = 3.45; iy = 10.3; zy = 6.52; sy = 5.89; ry = 1.51; jt = 28.6; ct = 11.4; nw = 16.44;
            }
            else if (sName.Contains("HSS10X3-1/2X.125"))
            {
                d = 10; w = 3.5; ar = 3.04; t = 0.116; ix = 37; zx = 9.37; sx = 7.4; rx = 3.49; iy = 7.22; zy = 4.48; sy = 4.12; ry = 1.54; jt = 19.8; ct = 7.75; nw = 11.13;
            }
            else if (sName.Contains("HSS10X3X.375"))
            {
                d = 10; w = 3; ar = 8.27; t = 0.349; ix = 88; zx = 23.7; sx = 17.6; rx = 3.26; iy = 12.4; zy = 9.73; sy = 8.28; ry = 1.22; jt = 37.8; ct = 17.7; nw = 30.03;
            }
            else if (sName.Contains("HSS10X3X.313"))
            {
                d = 10; w = 3; ar = 7.01; t = 0.291; ix = 76.3; zx = 20.3; sx = 15.3; rx = 3.3; iy = 11; zy = 8.42; sy = 7.3; ry = 1.25; jt = 33; ct = 15.2; nw = 25.46;
            }
            else if (sName.Contains("HSS10X3X.250"))
            {
                d = 10; w = 3; ar = 5.7; t = 0.233; ix = 63.6; zx = 16.7; sx = 12.7; rx = 3.34; iy = 9.28; zy = 6.99; sy = 6.19; ry = 1.28; jt = 27.6; ct = 12.5; nw = 20.72;
            }
            else if (sName.Contains("HSS10X3X.188"))
            {
                d = 10; w = 3; ar = 4.32; t = 0.174; ix = 49.4; zx = 12.8; sx = 9.87; rx = 3.38; iy = 7.33; zy = 5.41; sy = 4.89; ry = 1.3; jt = 21.5; ct = 9.64; nw = 15.8;
            }
            else if (sName.Contains("HSS10X3X.125"))
            {
                d = 10; w = 3; ar = 2.93; t = 0.116; ix = 34.2; zx = 8.8; sx = 6.83; rx = 3.42; iy = 5.16; zy = 3.74; sy = 3.44; ry = 1.33; jt = 14.9; ct = 6.61; nw = 10.71;
            }
            else if (sName.Contains("HSS10X2X.375"))
            {
                d = 10; w = 2; ar = 7.58; t = 0.349; ix = 71.7; zx = 20.3; sx = 14.3; rx = 3.08; iy = 4.7; zy = 5.76; sy = 4.7; ry = 0.787; jt = 15.9; ct = 11; nw = 27.48;
            }
            else if (sName.Contains("HSS10X2X.313"))
            {
                d = 10; w = 2; ar = 6.43; t = 0.291; ix = 62.6; zx = 17.5; sx = 12.5; rx = 3.12; iy = 4.24; zy = 5.06; sy = 4.24; ry = 0.812; jt = 14.2; ct = 9.56; nw = 23.34;
            }
            else if (sName.Contains("HSS10X2X.250"))
            {
                d = 10; w = 2; ar = 5.24; t = 0.233; ix = 52.5; zx = 14.4; sx = 10.5; rx = 3.17; iy = 3.67; zy = 4.26; sy = 3.67; ry = 0.838; jt = 12.2; ct = 7.99; nw = 19.02;
            }
            else if (sName.Contains("HSS10X2X.188"))
            {
                d = 10; w = 2; ar = 3.98; t = 0.174; ix = 41; zx = 11.1; sx = 8.19; rx = 3.21; iy = 2.97; zy = 3.34; sy = 2.97; ry = 0.864; jt = 9.74; ct = 6.22; nw = 14.53;
            }
            else if (sName.Contains("HSS10X2X.125"))
            {
                d = 10; w = 2; ar = 2.7; t = 0.116; ix = 28.5; zx = 7.65; sx = 5.7; rx = 3.25; iy = 2.14; zy = 2.33; sy = 2.14; ry = 0.89; jt = 6.9; ct = 4.31; nw = 9.86;
            }
            else if (sName.Contains("HSS9X9X.625"))
            {
                d = 9; w = 9; ar = 18.7; t = 0.581; ix = 216; zx = 58.1; sx = 47.9; rx = 3.4; iy = 216; zy = 58.1; sy = 47.9; ry = 3.4; jt = 356; ct = 81.6; nw = 67.82;
            }
            else if (sName.Contains("HSS9X9X.500"))
            {
                d = 9; w = 9; ar = 15.3; t = 0.465; ix = 183; zx = 48.4; sx = 40.6; rx = 3.45; iy = 183; zy = 48.4; sy = 40.6; ry = 3.45; jt = 296; ct = 67.4; nw = 55.66;
            }
            else if (sName.Contains("HSS9X9X.375"))
            {
                d = 9; w = 9; ar = 11.8; t = 0.349; ix = 145; zx = 37.8; sx = 32.2; rx = 3.51; iy = 145; zy = 37.8; sy = 32.2; ry = 3.51; jt = 231; ct = 52.1; nw = 42.79;
            }
            else if (sName.Contains("HSS9X9X.313"))
            {
                d = 9; w = 9; ar = 9.92; t = 0.291; ix = 124; zx = 32.1; sx = 27.6; rx = 3.54; iy = 124; zy = 32.1; sy = 27.6; ry = 3.54; jt = 196; ct = 44; nw = 36.1;
            }
            else if (sName.Contains("HSS9X9X.250"))
            {
                d = 9; w = 9; ar = 8.03; t = 0.233; ix = 102; zx = 26.2; sx = 22.7; rx = 3.56; iy = 102; zy = 26.2; sy = 22.7; ry = 3.56; jt = 159; ct = 35.8; nw = 29.23;
            }
            else if (sName.Contains("HSS9X9X.188"))
            {
                d = 9; w = 9; ar = 6.06; t = 0.174; ix = 78.2; zx = 20; sx = 17.4; rx = 3.59; iy = 78.2; zy = 20; sy = 17.4; ry = 3.59; jt = 121; ct = 27.1; nw = 22.18;
            }
            else if (sName.Contains("HSS9X9X.125"))
            {
                d = 9; w = 9; ar = 4.09; t = 0.116; ix = 53.5; zx = 13.6; sx = 11.9; rx = 3.62; iy = 53.5; zy = 13.6; sy = 11.9; ry = 3.62; jt = 82; ct = 18.3; nw = 14.96;
            }
            else if (sName.Contains("HSS9X7X.625"))
            {
                d = 9; w = 7; ar = 16.4; t = 0.581; ix = 174; zx = 48.3; sx = 38.7; rx = 3.26; iy = 117; zy = 40.5; sy = 33.5; ry = 2.68; jt = 235; ct = 62; nw = 59.32;
            }
            else if (sName.Contains("HSS9X7X.500"))
            {
                d = 9; w = 7; ar = 13.5; t = 0.465; ix = 149; zx = 40.5; sx = 33; rx = 3.32; iy = 100; zy = 34; sy = 28.7; ry = 2.73; jt = 197; ct = 51.5; nw = 48.85;
            }
            else if (sName.Contains("HSS9X7X.375"))
            {
                d = 9; w = 7; ar = 10.4; t = 0.349; ix = 119; zx = 31.8; sx = 26.4; rx = 3.38; iy = 80.4; zy = 26.7; sy = 23; ry = 2.78; jt = 154; ct = 40; nw = 37.69;
            }
            else if (sName.Contains("HSS9X7X.313"))
            {
                d = 9; w = 7; ar = 8.76; t = 0.291; ix = 102; zx = 27.1; sx = 22.6; rx = 3.41; iy = 69.2; zy = 22.8; sy = 19.8; ry = 2.81; jt = 131; ct = 33.9; nw = 31.84;
            }
            else if (sName.Contains("HSS9X7X.250"))
            {
                d = 9; w = 7; ar = 7.1; t = 0.233; ix = 84.1; zx = 22.2; sx = 18.7; rx = 3.44; iy = 57.2; zy = 18.7; sy = 16.3; ry = 2.84; jt = 107; ct = 27.6; nw = 25.82;
            }
            else if (sName.Contains("HSS9X7X.188"))
            {
                d = 9; w = 7; ar = 5.37; t = 0.174; ix = 64.7; zx = 16.9; sx = 14.4; rx = 3.47; iy = 44.1; zy = 14.3; sy = 12.6; ry = 2.87; jt = 81.7; ct = 20.9; nw = 19.63;
            }
            else if (sName.Contains("HSS9X5X.625"))
            {
                d = 9; w = 5; ar = 14; t = 0.581; ix = 133; zx = 38.5; sx = 29.6; rx = 3.08; iy = 52; zy = 25.3; sy = 20.8; ry = 1.92; jt = 128; ct = 42.5; nw = 50.81;
            }
            else if (sName.Contains("HSS9X5X.500"))
            {
                d = 9; w = 5; ar = 11.6; t = 0.465; ix = 115; zx = 32.5; sx = 25.5; rx = 3.14; iy = 45.2; zy = 21.5; sy = 18.1; ry = 1.97; jt = 109; ct = 35.6; nw = 42.05;
            }
            else if (sName.Contains("HSS9X5X.375"))
            {
                d = 9; w = 5; ar = 8.97; t = 0.349; ix = 92.5; zx = 25.7; sx = 20.5; rx = 3.21; iy = 36.8; zy = 17.1; sy = 14.7; ry = 2.03; jt = 86.9; ct = 27.9; nw = 32.58;
            }
            else if (sName.Contains("HSS9X5X.313"))
            {
                d = 9; w = 5; ar = 7.59; t = 0.291; ix = 79.8; zx = 22; sx = 17.7; rx = 3.24; iy = 32; zy = 14.6; sy = 12.8; ry = 2.05; jt = 74.4; ct = 23.8; nw = 27.59;
            }
            else if (sName.Contains("HSS9X5X.250"))
            {
                d = 9; w = 5; ar = 6.17; t = 0.233; ix = 66.1; zx = 18.1; sx = 14.7; rx = 3.27; iy = 26.6; zy = 12; sy = 10.6; ry = 2.08; jt = 61.2; ct = 19.4; nw = 22.42;
            }
            else if (sName.Contains("HSS9X5X.188"))
            {
                d = 9; w = 5; ar = 4.67; t = 0.174; ix = 51.1; zx = 13.8; sx = 11.4; rx = 3.31; iy = 20.7; zy = 9.25; sy = 8.28; ry = 2.1; jt = 46.9; ct = 14.8; nw = 17.08;
            }
            else if (sName.Contains("HSS9X3X.500"))
            {
                d = 9; w = 3; ar = 9.74; t = 0.465; ix = 80.8; zx = 24.6; sx = 18; rx = 2.88; iy = 13.2; zy = 10.8; sy = 8.81; ry = 1.17; jt = 40; ct = 19.7; nw = 35.24;
            }
            else if (sName.Contains("HSS9X3X.375"))
            {
                d = 9; w = 3; ar = 7.58; t = 0.349; ix = 66.3; zx = 19.7; sx = 14.7; rx = 2.96; iy = 11.2; zy = 8.8; sy = 7.45; ry = 1.21; jt = 33.1; ct = 15.8; nw = 27.48;
            }
            else if (sName.Contains("HSS9X3X.313"))
            {
                d = 9; w = 3; ar = 6.43; t = 0.291; ix = 57.7; zx = 16.9; sx = 12.8; rx = 3; iy = 9.88; zy = 7.63; sy = 6.59; ry = 1.24; jt = 28.9; ct = 13.6; nw = 23.34;
            }
            else if (sName.Contains("HSS9X3X.250"))
            {
                d = 9; w = 3; ar = 5.24; t = 0.233; ix = 48.2; zx = 14; sx = 10.7; rx = 3.04; iy = 8.38; zy = 6.35; sy = 5.59; ry = 1.27; jt = 24.2; ct = 11.3; nw = 19.02;
            }
            else if (sName.Contains("HSS9X3X.188"))
            {
                d = 9; w = 3; ar = 3.98; t = 0.174; ix = 37.6; zx = 10.8; sx = 8.35; rx = 3.07; iy = 6.64; zy = 4.92; sy = 4.42; ry = 1.29; jt = 18.9; ct = 8.66; nw = 14.53;
            }
            else if (sName.Contains("HSS8X8X.625"))
            {
                d = 8; w = 8; ar = 16.4; t = 0.581; ix = 146; zx = 44.7; sx = 36.5; rx = 2.99; iy = 146; zy = 44.7; sy = 36.5; ry = 2.99; jt = 244; ct = 63.2; nw = 59.32;
            }
            else if (sName.Contains("HSS8X8X.500"))
            {
                d = 8; w = 8; ar = 13.5; t = 0.465; ix = 125; zx = 37.5; sx = 31.2; rx = 3.04; iy = 125; zy = 37.5; sy = 31.2; ry = 3.04; jt = 204; ct = 52.4; nw = 48.85;
            }
            else if (sName.Contains("HSS8X8X.375"))
            {
                d = 8; w = 8; ar = 10.4; t = 0.349; ix = 100; zx = 29.4; sx = 24.9; rx = 3.1; iy = 100; zy = 29.4; sy = 24.9; ry = 3.1; jt = 160; ct = 40.7; nw = 37.69;
            }
            else if (sName.Contains("HSS8X8X.313"))
            {
                d = 8; w = 8; ar = 8.76; t = 0.291; ix = 85.6; zx = 25.1; sx = 21.4; rx = 3.13; iy = 85.6; zy = 25.1; sy = 21.4; ry = 3.13; jt = 136; ct = 34.5; nw = 31.84;
            }
            else if (sName.Contains("HSS8X8X.250"))
            {
                d = 8; w = 8; ar = 7.1; t = 0.233; ix = 70.7; zx = 20.5; sx = 17.7; rx = 3.15; iy = 70.7; zy = 20.5; sy = 17.7; ry = 3.15; jt = 111; ct = 28.1; nw = 25.82;
            }
            else if (sName.Contains("HSS8X8X.188"))
            {
                d = 8; w = 8; ar = 5.37; t = 0.174; ix = 54.4; zx = 15.7; sx = 13.6; rx = 3.18; iy = 54.4; zy = 15.7; sy = 13.6; ry = 3.18; jt = 84.5; ct = 21.3; nw = 19.63;
            }
            else if (sName.Contains("HSS8X8X.125"))
            {
                d = 8; w = 8; ar = 3.62; t = 0.116; ix = 37.4; zx = 10.7; sx = 9.34; rx = 3.21; iy = 37.4; zy = 10.7; sy = 9.34; ry = 3.21; jt = 57.3; ct = 14.4; nw = 13.26;
            }
            else if (sName.Contains("HSS8X6X.625"))
            {
                d = 8; w = 6; ar = 14; t = 0.581; ix = 114; zx = 36.1; sx = 28.5; rx = 2.85; iy = 72.3; zy = 29.5; sy = 24.1; ry = 2.27; jt = 150; ct = 46; nw = 50.81;
            }
            else if (sName.Contains("HSS8X6X.500"))
            {
                d = 8; w = 6; ar = 11.6; t = 0.465; ix = 98.2; zx = 30.5; sx = 24.6; rx = 2.91; iy = 62.5; zy = 24.9; sy = 20.8; ry = 2.32; jt = 127; ct = 38.4; nw = 42.05;
            }
            else if (sName.Contains("HSS8X6X.375"))
            {
                d = 8; w = 6; ar = 8.97; t = 0.349; ix = 79.1; zx = 24.1; sx = 19.8; rx = 2.97; iy = 50.6; zy = 19.8; sy = 16.9; ry = 2.38; jt = 100; ct = 30; nw = 32.58;
            }
            else if (sName.Contains("HSS8X6X.313"))
            {
                d = 8; w = 6; ar = 7.59; t = 0.291; ix = 68.3; zx = 20.6; sx = 17.1; rx = 3; iy = 43.8; zy = 16.9; sy = 14.6; ry = 2.4; jt = 85.8; ct = 25.5; nw = 27.59;
            }
            else if (sName.Contains("HSS8X6X.250"))
            {
                d = 8; w = 6; ar = 6.17; t = 0.233; ix = 56.6; zx = 16.9; sx = 14.2; rx = 3.03; iy = 36.4; zy = 13.9; sy = 12.1; ry = 2.43; jt = 70.3; ct = 20.8; nw = 22.42;
            }
            else if (sName.Contains("HSS8X6X.188"))
            {
                d = 8; w = 6; ar = 4.67; t = 0.174; ix = 43.7; zx = 13; sx = 10.9; rx = 3.06; iy = 28.2; zy = 10.7; sy = 9.39; ry = 2.46; jt = 53.7; ct = 15.8; nw = 17.08;
            }
            else if (sName.Contains("HSS8X4X.625"))
            {
                d = 8; w = 4; ar = 11.7; t = 0.581; ix = 82; zx = 27.4; sx = 20.5; rx = 2.64; iy = 26.6; zy = 16.6; sy = 13.3; ry = 1.51; jt = 70.3; ct = 28.7; nw = 42.3;
            }
            else if (sName.Contains("HSS8X4X.500"))
            {
                d = 8; w = 4; ar = 9.74; t = 0.465; ix = 71.8; zx = 23.5; sx = 17.9; rx = 2.71; iy = 23.6; zy = 14.3; sy = 11.8; ry = 1.56; jt = 61.1; ct = 24.4; nw = 35.24;
            }
            else if (sName.Contains("HSS8X4X.375"))
            {
                d = 8; w = 4; ar = 7.58; t = 0.349; ix = 58.7; zx = 18.8; sx = 14.7; rx = 2.78; iy = 19.6; zy = 11.5; sy = 9.8; ry = 1.61; jt = 49.3; ct = 19.3; nw = 27.48;
            }
            else if (sName.Contains("HSS8X4X.313"))
            {
                d = 8; w = 4; ar = 6.43; t = 0.291; ix = 51; zx = 16.1; sx = 12.8; rx = 2.82; iy = 17.2; zy = 9.91; sy = 8.58; ry = 1.63; jt = 42.6; ct = 16.5; nw = 23.34;
            }
            else if (sName.Contains("HSS8X4X.250"))
            {
                d = 8; w = 4; ar = 5.24; t = 0.233; ix = 42.5; zx = 13.3; sx = 10.6; rx = 2.85; iy = 14.4; zy = 8.2; sy = 7.21; ry = 1.66; jt = 35.3; ct = 13.6; nw = 19.02;
            }
            else if (sName.Contains("HSS8X4X.188"))
            {
                d = 8; w = 4; ar = 3.98; t = 0.174; ix = 33.1; zx = 10.2; sx = 8.27; rx = 2.88; iy = 11.3; zy = 6.33; sy = 5.65; ry = 1.69; jt = 27.2; ct = 10.4; nw = 14.53;
            }
            else if (sName.Contains("HSS8X4X.125"))
            {
                d = 8; w = 4; ar = 2.7; t = 0.116; ix = 22.9; zx = 7.02; sx = 5.73; rx = 2.92; iy = 7.9; zy = 4.36; sy = 3.95; ry = 1.71; jt = 18.7; ct = 7.1; nw = 9.86;
            }
            else if (sName.Contains("HSS8X3X.500"))
            {
                d = 8; w = 3; ar = 8.81; t = 0.465; ix = 58.6; zx = 20; sx = 14.6; rx = 2.58; iy = 11.7; zy = 9.64; sy = 7.81; ry = 1.15; jt = 34.3; ct = 17.4; nw = 31.84;
            }
            else if (sName.Contains("HSS8X3X.375"))
            {
                d = 8; w = 3; ar = 6.88; t = 0.349; ix = 48.5; zx = 16.1; sx = 12.1; rx = 2.65; iy = 10; zy = 7.88; sy = 6.63; ry = 1.2; jt = 28.5; ct = 14; nw = 24.93;
            }
            else if (sName.Contains("HSS8X3X.313"))
            {
                d = 8; w = 3; ar = 5.85; t = 0.291; ix = 42.4; zx = 13.9; sx = 10.6; rx = 2.69; iy = 8.81; zy = 6.84; sy = 5.87; ry = 1.23; jt = 24.9; ct = 12.1; nw = 21.21;
            }
            else if (sName.Contains("HSS8X3X.250"))
            {
                d = 8; w = 3; ar = 4.77; t = 0.233; ix = 35.5; zx = 11.5; sx = 8.88; rx = 2.73; iy = 7.49; zy = 5.7; sy = 4.99; ry = 1.25; jt = 20.8; ct = 10; nw = 17.32;
            }
            else if (sName.Contains("HSS8X3X.188"))
            {
                d = 8; w = 3; ar = 3.63; t = 0.174; ix = 27.8; zx = 8.87; sx = 6.94; rx = 2.77; iy = 5.94; zy = 4.43; sy = 3.96; ry = 1.28; jt = 16.2; ct = 7.68; nw = 13.25;
            }
            else if (sName.Contains("HSS8X3X.125"))
            {
                d = 8; w = 3; ar = 2.46; t = 0.116; ix = 19.3; zx = 6.11; sx = 4.83; rx = 2.8; iy = 4.2; zy = 3.07; sy = 2.8; ry = 1.31; jt = 11.3; ct = 5.27; nw = 9.01;
            }
            else if (sName.Contains("HSS8X2X.375"))
            {
                d = 8; w = 2; ar = 6.18; t = 0.349; ix = 38.2; zx = 13.4; sx = 9.56; rx = 2.49; iy = 3.73; zy = 4.61; sy = 3.73; ry = 0.777; jt = 12.1; ct = 8.65; nw = 22.37;
            }
            else if (sName.Contains("HSS8X2X.313"))
            {
                d = 8; w = 2; ar = 5.26; t = 0.291; ix = 33.7; zx = 11.6; sx = 8.43; rx = 2.53; iy = 3.38; zy = 4.06; sy = 3.38; ry = 0.802; jt = 10.9; ct = 7.57; nw = 19.08;
            }
            else if (sName.Contains("HSS8X2X.250"))
            {
                d = 8; w = 2; ar = 4.3; t = 0.233; ix = 28.5; zx = 9.68; sx = 7.12; rx = 2.57; iy = 2.94; zy = 3.43; sy = 2.94; ry = 0.827; jt = 9.36; ct = 6.35; nw = 15.62;
            }
            else if (sName.Contains("HSS8X2X.188"))
            {
                d = 8; w = 2; ar = 3.28; t = 0.174; ix = 22.4; zx = 7.51; sx = 5.61; rx = 2.61; iy = 2.39; zy = 2.7; sy = 2.39; ry = 0.853; jt = 7.48; ct = 4.95; nw = 11.97;
            }
            else if (sName.Contains("HSS8X2X.125"))
            {
                d = 8; w = 2; ar = 2.23; t = 0.116; ix = 15.7; zx = 5.19; sx = 3.93; rx = 2.65; iy = 1.72; zy = 1.9; sy = 1.72; ry = 0.879; jt = 5.3; ct = 3.44; nw = 8.16;
            }
            else if (sName.Contains("HSS7X7X.625"))
            {
                d = 7; w = 7; ar = 14; t = 0.581; ix = 93.4; zx = 33.1; sx = 26.7; rx = 2.58; iy = 93.4; zy = 33.1; sy = 26.7; ry = 2.58; jt = 158; ct = 47.1; nw = 50.81;
            }
            else if (sName.Contains("HSS7X7X.500"))
            {
                d = 7; w = 7; ar = 11.6; t = 0.465; ix = 80.5; zx = 27.9; sx = 23; rx = 2.63; iy = 80.5; zy = 27.9; sy = 23; ry = 2.63; jt = 133; ct = 39.3; nw = 42.05;
            }
            else if (sName.Contains("HSS7X7X.375"))
            {
                d = 7; w = 7; ar = 8.97; t = 0.349; ix = 65; zx = 22.1; sx = 18.6; rx = 2.69; iy = 65; zy = 22.1; sy = 18.6; ry = 2.69; jt = 105; ct = 30.7; nw = 32.58;
            }
            else if (sName.Contains("HSS7X7X.313"))
            {
                d = 7; w = 7; ar = 7.59; t = 0.291; ix = 56.1; zx = 18.9; sx = 16; rx = 2.72; iy = 56.1; zy = 18.9; sy = 16; ry = 2.72; jt = 89.7; ct = 26.1; nw = 27.59;
            }
            else if (sName.Contains("HSS7X7X.250"))
            {
                d = 7; w = 7; ar = 6.17; t = 0.233; ix = 46.5; zx = 15.5; sx = 13.3; rx = 2.75; iy = 46.5; zy = 15.5; sy = 13.3; ry = 2.75; jt = 73.5; ct = 21.3; nw = 22.42;
            }
            else if (sName.Contains("HSS7X7X.188"))
            {
                d = 7; w = 7; ar = 4.67; t = 0.174; ix = 36; zx = 11.9; sx = 10.3; rx = 2.77; iy = 36; zy = 11.9; sy = 10.3; ry = 2.77; jt = 56.1; ct = 16.2; nw = 17.08;
            }
            else if (sName.Contains("HSS7X7X.125"))
            {
                d = 7; w = 7; ar = 3.16; t = 0.116; ix = 24.8; zx = 8.13; sx = 7.09; rx = 2.8; iy = 24.8; zy = 8.13; sy = 7.09; ry = 2.8; jt = 38.2; ct = 11; nw = 11.56;
            }
            else if (sName.Contains("HSS7X5X.500"))
            {
                d = 7; w = 5; ar = 9.74; t = 0.465; ix = 60.6; zx = 21.9; sx = 17.3; rx = 2.5; iy = 35.6; zy = 17.3; sy = 14.2; ry = 1.91; jt = 75.8; ct = 27.2; nw = 35.24;
            }
            else if (sName.Contains("HSS7X5X.375"))
            {
                d = 7; w = 5; ar = 7.58; t = 0.349; ix = 49.5; zx = 17.5; sx = 14.1; rx = 2.56; iy = 29.3; zy = 13.8; sy = 11.7; ry = 1.97; jt = 60.6; ct = 21.4; nw = 27.48;
            }
            else if (sName.Contains("HSS7X5X.313"))
            {
                d = 7; w = 5; ar = 6.43; t = 0.291; ix = 43; zx = 15; sx = 12.3; rx = 2.59; iy = 25.5; zy = 11.9; sy = 10.2; ry = 1.99; jt = 52.1; ct = 18.3; nw = 23.34;
            }
            else if (sName.Contains("HSS7X5X.250"))
            {
                d = 7; w = 5; ar = 5.24; t = 0.233; ix = 35.9; zx = 12.4; sx = 10.2; rx = 2.62; iy = 21.3; zy = 9.83; sy = 8.53; ry = 2.02; jt = 42.9; ct = 15; nw = 19.02;
            }
            else if (sName.Contains("HSS7X5X.188"))
            {
                d = 7; w = 5; ar = 3.98; t = 0.174; ix = 27.9; zx = 9.52; sx = 7.96; rx = 2.65; iy = 16.6; zy = 7.57; sy = 6.65; ry = 2.05; jt = 32.9; ct = 11.4; nw = 14.53;
            }
            else if (sName.Contains("HSS7X5X.125"))
            {
                d = 7; w = 5; ar = 2.7; t = 0.116; ix = 19.3; zx = 6.53; sx = 5.52; rx = 2.68; iy = 11.6; zy = 5.2; sy = 4.63; ry = 2.07; jt = 22.5; ct = 7.79; nw = 9.86;
            }
            else if (sName.Contains("HSS7X4X.500"))
            {
                d = 7; w = 4; ar = 8.81; t = 0.465; ix = 50.7; zx = 18.8; sx = 14.5; rx = 2.4; iy = 20.7; zy = 12.6; sy = 10.4; ry = 1.53; jt = 50.5; ct = 21.1; nw = 31.84;
            }
            else if (sName.Contains("HSS7X4X.375"))
            {
                d = 7; w = 4; ar = 6.88; t = 0.349; ix = 41.8; zx = 15.1; sx = 11.9; rx = 2.46; iy = 17.3; zy = 10.2; sy = 8.63; ry = 1.58; jt = 41; ct = 16.8; nw = 24.93;
            }
            else if (sName.Contains("HSS7X4X.313"))
            {
                d = 7; w = 4; ar = 5.85; t = 0.291; ix = 36.5; zx = 13.1; sx = 10.4; rx = 2.5; iy = 15.2; zy = 8.83; sy = 7.58; ry = 1.61; jt = 35.4; ct = 14.4; nw = 21.21;
            }
            else if (sName.Contains("HSS7X4X.250"))
            {
                d = 7; w = 4; ar = 4.77; t = 0.233; ix = 30.5; zx = 10.8; sx = 8.72; rx = 2.53; iy = 12.8; zy = 7.33; sy = 6.38; ry = 1.64; jt = 29.3; ct = 11.8; nw = 17.32;
            }
            else if (sName.Contains("HSS7X4X.188"))
            {
                d = 7; w = 4; ar = 3.63; t = 0.174; ix = 23.8; zx = 8.33; sx = 6.81; rx = 2.56; iy = 10; zy = 5.67; sy = 5.02; ry = 1.66; jt = 22.7; ct = 9.07; nw = 13.25;
            }
            else if (sName.Contains("HSS7X4X.125"))
            {
                d = 7; w = 4; ar = 2.46; t = 0.116; ix = 16.6; zx = 5.73; sx = 4.73; rx = 2.59; iy = 7.03; zy = 3.91; sy = 3.51; ry = 1.69; jt = 15.6; ct = 6.2; nw = 9.01;
            }
            else if (sName.Contains("HSS7X3X.500"))
            {
                d = 7; w = 3; ar = 7.88; t = 0.465; ix = 40.7; zx = 15.8; sx = 11.6; rx = 2.27; iy = 10.2; zy = 8.46; sy = 6.8; ry = 1.14; jt = 28.6; ct = 15; nw = 28.43;
            }
            else if (sName.Contains("HSS7X3X.375"))
            {
                d = 7; w = 3; ar = 6.18; t = 0.349; ix = 34.1; zx = 12.8; sx = 9.73; rx = 2.35; iy = 8.71; zy = 6.95; sy = 5.81; ry = 1.19; jt = 23.9; ct = 12.1; nw = 22.37;
            }
            else if (sName.Contains("HSS7X3X.313"))
            {
                d = 7; w = 3; ar = 5.26; t = 0.291; ix = 29.9; zx = 11.1; sx = 8.54; rx = 2.38; iy = 7.74; zy = 6.05; sy = 5.16; ry = 1.21; jt = 20.9; ct = 10.5; nw = 19.08;
            }
            else if (sName.Contains("HSS7X3X.250"))
            {
                d = 7; w = 3; ar = 4.3; t = 0.233; ix = 25.2; zx = 9.22; sx = 7.19; rx = 2.42; iy = 6.6; zy = 5.06; sy = 4.4; ry = 1.24; jt = 17.5; ct = 8.68; nw = 15.62;
            }
            else if (sName.Contains("HSS7X3X.188"))
            {
                d = 7; w = 3; ar = 3.28; t = 0.174; ix = 19.8; zx = 7.14; sx = 5.65; rx = 2.45; iy = 5.24; zy = 3.94; sy = 3.5; ry = 1.26; jt = 13.7; ct = 6.69; nw = 11.97;
            }
            else if (sName.Contains("HSS7X3X.125"))
            {
                d = 7; w = 3; ar = 2.23; t = 0.116; ix = 13.8; zx = 4.93; sx = 3.95; rx = 2.49; iy = 3.71; zy = 2.73; sy = 2.48; ry = 1.29; jt = 9.48; ct = 4.6; nw = 8.16;
            }
            else if (sName.Contains("HSS7X2X.250"))
            {
                d = 7; w = 2; ar = 3.84; t = 0.233; ix = 19.8; zx = 7.64; sx = 5.67; rx = 2.27; iy = 2.58; zy = 3.02; sy = 2.58; ry = 0.819; jt = 7.95; ct = 5.52; nw = 13.91;
            }
            else if (sName.Contains("HSS7X2X.188"))
            {
                d = 7; w = 2; ar = 2.93; t = 0.174; ix = 15.7; zx = 5.95; sx = 4.49; rx = 2.31; iy = 2.1; zy = 2.39; sy = 2.1; ry = 0.845; jt = 6.35; ct = 4.32; nw = 10.7;
            }
            else if (sName.Contains("HSS7X2X.125"))
            {
                d = 7; w = 2; ar = 2; t = 0.116; ix = 11.1; zx = 4.13; sx = 3.16; rx = 2.35; iy = 1.52; zy = 1.68; sy = 1.52; ry = 0.871; jt = 4.51; ct = 3; nw = 7.31;
            }
            else if (sName.Contains("HSS6X6X.625"))
            {
                d = 6; w = 6; ar = 11.7; t = 0.581; ix = 55.2; zx = 23.2; sx = 18.4; rx = 2.17; iy = 55.2; zy = 23.2; sy = 18.4; ry = 2.17; jt = 94.9; ct = 33.4; nw = 42.3;
            }
            else if (sName.Contains("HSS6X6X.500"))
            {
                d = 6; w = 6; ar = 9.74; t = 0.465; ix = 48.3; zx = 19.8; sx = 16.1; rx = 2.23; iy = 48.3; zy = 19.8; sy = 16.1; ry = 2.23; jt = 81.1; ct = 28.1; nw = 35.24;
            }
            else if (sName.Contains("HSS6X6X.375"))
            {
                d = 6; w = 6; ar = 7.58; t = 0.349; ix = 39.5; zx = 15.8; sx = 13.2; rx = 2.28; iy = 39.5; zy = 15.8; sy = 13.2; ry = 2.28; jt = 64.6; ct = 22.1; nw = 27.48;
            }
            else if (sName.Contains("HSS6X6X.313"))
            {
                d = 6; w = 6; ar = 6.43; t = 0.291; ix = 34.3; zx = 13.6; sx = 11.4; rx = 2.31; iy = 34.3; zy = 13.6; sy = 11.4; ry = 2.31; jt = 55.4; ct = 18.9; nw = 23.34;
            }
            else if (sName.Contains("HSS6X6X.250"))
            {
                d = 6; w = 6; ar = 5.24; t = 0.233; ix = 28.6; zx = 11.2; sx = 9.54; rx = 2.34; iy = 28.6; zy = 11.2; sy = 9.54; ry = 2.34; jt = 45.6; ct = 15.4; nw = 19.02;
            }
            else if (sName.Contains("HSS6X6X.188"))
            {
                d = 6; w = 6; ar = 3.98; t = 0.174; ix = 22.3; zx = 8.63; sx = 7.42; rx = 2.37; iy = 22.3; zy = 8.63; sy = 7.42; ry = 2.37; jt = 35; ct = 11.8; nw = 14.53;
            }
            else if (sName.Contains("HSS6X6X.125"))
            {
                d = 6; w = 6; ar = 2.7; t = 0.116; ix = 15.5; zx = 5.92; sx = 5.15; rx = 2.39; iy = 15.5; zy = 5.92; sy = 5.15; ry = 2.39; jt = 23.9; ct = 8.03; nw = 9.86;
            }
            else if (sName.Contains("HSS6X5X.500"))
            {
                d = 6; w = 5; ar = 8.81; t = 0.465; ix = 41.1; zx = 17.2; sx = 13.7; rx = 2.16; iy = 30.8; zy = 15.2; sy = 12.3; ry = 1.87; jt = 59.8; ct = 23; nw = 31.84;
            }
            else if (sName.Contains("HSS6X5X.375"))
            {
                d = 6; w = 5; ar = 6.88; t = 0.349; ix = 33.9; zx = 13.8; sx = 11.3; rx = 2.22; iy = 25.5; zy = 12.2; sy = 10.2; ry = 1.92; jt = 48.1; ct = 18.2; nw = 24.93;
            }
            else if (sName.Contains("HSS6X5X.313"))
            {
                d = 6; w = 5; ar = 5.85; t = 0.291; ix = 29.6; zx = 11.9; sx = 9.85; rx = 2.25; iy = 22.3; zy = 10.5; sy = 8.91; ry = 1.95; jt = 41.4; ct = 15.6; nw = 21.21;
            }
            else if (sName.Contains("HSS6X5X.250"))
            {
                d = 6; w = 5; ar = 4.77; t = 0.233; ix = 24.7; zx = 9.87; sx = 8.25; rx = 2.28; iy = 18.7; zy = 8.72; sy = 7.47; ry = 1.98; jt = 34.2; ct = 12.8; nw = 17.32;
            }
            else if (sName.Contains("HSS6X5X.188"))
            {
                d = 6; w = 5; ar = 3.63; t = 0.174; ix = 19.3; zx = 7.62; sx = 6.44; rx = 2.31; iy = 14.6; zy = 6.73; sy = 5.84; ry = 2.01; jt = 26.3; ct = 9.76; nw = 13.25;
            }
            else if (sName.Contains("HSS6X5X.125"))
            {
                d = 6; w = 5; ar = 2.46; t = 0.116; ix = 13.4; zx = 5.24; sx = 4.48; rx = 2.34; iy = 10.2; zy = 4.63; sy = 4.07; ry = 2.03; jt = 18; ct = 6.66; nw = 9.01;
            }
            else if (sName.Contains("HSS6X4X.500"))
            {
                d = 6; w = 4; ar = 7.88; t = 0.465; ix = 34; zx = 14.6; sx = 11.3; rx = 2.08; iy = 17.8; zy = 11; sy = 8.89; ry = 1.5; jt = 40.3; ct = 17.8; nw = 28.43;
            }
            else if (sName.Contains("HSS6X4X.375"))
            {
                d = 6; w = 4; ar = 6.18; t = 0.349; ix = 28.3; zx = 11.9; sx = 9.43; rx = 2.14; iy = 14.9; zy = 8.94; sy = 7.47; ry = 1.55; jt = 32.8; ct = 14.2; nw = 22.37;
            }
            else if (sName.Contains("HSS6X4X.313"))
            {
                d = 6; w = 4; ar = 5.26; t = 0.291; ix = 24.8; zx = 10.3; sx = 8.27; rx = 2.17; iy = 13.2; zy = 7.75; sy = 6.58; ry = 1.58; jt = 28.4; ct = 12.2; nw = 19.08;
            }
            else if (sName.Contains("HSS6X4X.250"))
            {
                d = 6; w = 4; ar = 4.3; t = 0.233; ix = 20.9; zx = 8.53; sx = 6.96; rx = 2.2; iy = 11.1; zy = 6.45; sy = 5.56; ry = 1.61; jt = 23.6; ct = 10.1; nw = 15.62;
            }
            else if (sName.Contains("HSS6X4X.188"))
            {
                d = 6; w = 4; ar = 3.28; t = 0.174; ix = 16.4; zx = 6.6; sx = 5.46; rx = 2.23; iy = 8.76; zy = 5; sy = 4.38; ry = 1.63; jt = 18.2; ct = 7.74; nw = 11.97;
            }
            else if (sName.Contains("HSS6X4X.125"))
            {
                d = 6; w = 4; ar = 2.23; t = 0.116; ix = 11.4; zx = 4.56; sx = 3.81; rx = 2.26; iy = 6.15; zy = 3.46; sy = 3.08; ry = 1.66; jt = 12.6; ct = 5.3; nw = 8.16;
            }
            else if (sName.Contains("HSS6X3X.500"))
            {
                d = 6; w = 3; ar = 6.95; t = 0.465; ix = 26.8; zx = 12.1; sx = 8.95; rx = 1.97; iy = 8.69; zy = 7.28; sy = 5.79; ry = 1.12; jt = 23.1; ct = 12.7; nw = 25.03;
            }
            else if (sName.Contains("HSS6X3X.375"))
            {
                d = 6; w = 3; ar = 5.48; t = 0.349; ix = 22.7; zx = 9.9; sx = 7.57; rx = 2.04; iy = 7.48; zy = 6.03; sy = 4.99; ry = 1.17; jt = 19.3; ct = 10.3; nw = 19.82;
            }
            else if (sName.Contains("HSS6X3X.313"))
            {
                d = 6; w = 3; ar = 4.68; t = 0.291; ix = 20.1; zx = 8.61; sx = 6.69; rx = 2.07; iy = 6.67; zy = 5.27; sy = 4.45; ry = 1.19; jt = 16.9; ct = 8.91; nw = 16.96;
            }
            else if (sName.Contains("HSS6X3X.250"))
            {
                d = 6; w = 3; ar = 3.84; t = 0.233; ix = 17; zx = 7.19; sx = 5.66; rx = 2.1; iy = 5.7; zy = 4.41; sy = 3.8; ry = 1.22; jt = 14.2; ct = 7.39; nw = 13.91;
            }
            else if (sName.Contains("HSS6X3X.188"))
            {
                d = 6; w = 3; ar = 2.93; t = 0.174; ix = 13.4; zx = 5.59; sx = 4.47; rx = 2.14; iy = 4.55; zy = 3.45; sy = 3.03; ry = 1.25; jt = 11.1; ct = 5.71; nw = 10.7;
            }
            else if (sName.Contains("HSS6X3X.125"))
            {
                d = 6; w = 3; ar = 2; t = 0.116; ix = 9.43; zx = 3.87; sx = 3.14; rx = 2.17; iy = 3.23; zy = 2.4; sy = 2.15; ry = 1.27; jt = 7.73; ct = 3.93; nw = 7.31;
            }
            else if (sName.Contains("HSS6X2X.375"))
            {
                d = 6; w = 2; ar = 4.78; t = 0.349; ix = 17.1; zx = 7.93; sx = 5.71; rx = 1.89; iy = 2.77; zy = 3.46; sy = 2.77; ry = 0.76; jt = 8.42; ct = 6.35; nw = 17.27;
            }
            else if (sName.Contains("HSS6X2X.313"))
            {
                d = 6; w = 2; ar = 4.1; t = 0.291; ix = 15.3; zx = 6.95; sx = 5.11; rx = 1.93; iy = 2.52; zy = 3.07; sy = 2.52; ry = 0.785; jt = 7.6; ct = 5.58; nw = 14.83;
            }
            else if (sName.Contains("HSS6X2X.250"))
            {
                d = 6; w = 2; ar = 3.37; t = 0.233; ix = 13.1; zx = 5.84; sx = 4.37; rx = 1.97; iy = 2.21; zy = 2.61; sy = 2.21; ry = 0.81; jt = 6.55; ct = 4.7; nw = 12.21;
            }
            else if (sName.Contains("HSS6X2X.188"))
            {
                d = 6; w = 2; ar = 2.58; t = 0.174; ix = 10.5; zx = 4.58; sx = 3.49; rx = 2.01; iy = 1.8; zy = 2.07; sy = 1.8; ry = 0.836; jt = 5.24; ct = 3.68; nw = 9.42;
            }
            else if (sName.Contains("HSS6X2X.125"))
            {
                d = 6; w = 2; ar = 1.77; t = 0.116; ix = 7.42; zx = 3.19; sx = 2.47; rx = 2.05; iy = 1.31; zy = 1.46; sy = 1.31; ry = 0.861; jt = 3.72; ct = 2.57; nw = 6.46;
            }
            else if (sName.Contains("HSS5-1/2X5-1/2X.375"))
            {
                d = 5.5; w = 5.5; ar = 6.88; t = 0.349; ix = 29.7; zx = 13.1; sx = 10.8; rx = 2.08; iy = 29.7; zy = 13.1; sy = 10.8; ry = 2.08; jt = 49; ct = 18.4; nw = 24.93;
            }
            else if (sName.Contains("HSS5-1/2X5-1/2X.313"))
            {
                d = 5.5; w = 5.5; ar = 5.85; t = 0.291; ix = 25.9; zx = 11.3; sx = 9.43; rx = 2.11; iy = 25.9; zy = 11.3; sy = 9.43; ry = 2.11; jt = 42.2; ct = 15.7; nw = 21.21;
            }
            else if (sName.Contains("HSS5-1/2X5-1/2X.250"))
            {
                d = 5.5; w = 5.5; ar = 4.77; t = 0.233; ix = 21.7; zx = 9.32; sx = 7.9; rx = 2.13; iy = 21.7; zy = 9.32; sy = 7.9; ry = 2.13; jt = 34.8; ct = 12.9; nw = 17.32;
            }
            else if (sName.Contains("HSS5-1/2X5-1/2X.188"))
            {
                d = 5.5; w = 5.5; ar = 3.63; t = 0.174; ix = 17; zx = 7.19; sx = 6.17; rx = 2.16; iy = 17; zy = 7.19; sy = 6.17; ry = 2.16; jt = 26.7; ct = 9.85; nw = 13.25;
            }
            else if (sName.Contains("HSS5-1/2X5-1/2X.125"))
            {
                d = 5.5; w = 5.5; ar = 2.46; t = 0.116; ix = 11.8; zx = 4.95; sx = 4.3; rx = 2.19; iy = 11.8; zy = 4.95; sy = 4.3; ry = 2.19; jt = 18.3; ct = 6.72; nw = 9.01;
            }
            else if (sName.Contains("HSS5X5X.500"))
            {
                d = 5; w = 5; ar = 7.88; t = 0.465; ix = 26; zx = 13.1; sx = 10.4; rx = 1.82; iy = 26; zy = 13.1; sy = 10.4; ry = 1.82; jt = 44.6; ct = 18.7; nw = 28.43;
            }
            else if (sName.Contains("HSS5X5X.375"))
            {
                d = 5; w = 5; ar = 6.18; t = 0.349; ix = 21.7; zx = 10.6; sx = 8.68; rx = 1.87; iy = 21.7; zy = 10.6; sy = 8.68; ry = 1.87; jt = 36.1; ct = 14.9; nw = 22.37;
            }
            else if (sName.Contains("HSS5X5X.313"))
            {
                d = 5; w = 5; ar = 5.26; t = 0.291; ix = 19; zx = 9.16; sx = 7.62; rx = 1.9; iy = 19; zy = 9.16; sy = 7.62; ry = 1.9; jt = 31.2; ct = 12.8; nw = 19.08;
            }
            else if (sName.Contains("HSS5X5X.250"))
            {
                d = 5; w = 5; ar = 4.3; t = 0.233; ix = 16; zx = 7.61; sx = 6.41; rx = 1.93; iy = 16; zy = 7.61; sy = 6.41; ry = 1.93; jt = 25.8; ct = 10.5; nw = 15.62;
            }
            else if (sName.Contains("HSS5X5X.188"))
            {
                d = 5; w = 5; ar = 3.28; t = 0.174; ix = 12.6; zx = 5.89; sx = 5.03; rx = 1.96; iy = 12.6; zy = 5.89; sy = 5.03; ry = 1.96; jt = 19.9; ct = 8.08; nw = 11.97;
            }
            else if (sName.Contains("HSS5X5X.125"))
            {
                d = 5; w = 5; ar = 2.23; t = 0.116; ix = 8.8; zx = 4.07; sx = 3.52; rx = 1.99; iy = 8.8; zy = 4.07; sy = 3.52; ry = 1.99; jt = 13.7; ct = 5.53; nw = 8.16;
            }
            else if (sName.Contains("HSS5X4X.500"))
            {
                d = 5; w = 4; ar = 6.95; t = 0.465; ix = 21.2; zx = 10.9; sx = 8.49; rx = 1.75; iy = 14.9; zy = 9.35; sy = 7.43; ry = 1.46; jt = 30.3; ct = 14.5; nw = 25.03;
            }
            else if (sName.Contains("HSS5X4X.375"))
            {
                d = 5; w = 4; ar = 5.48; t = 0.349; ix = 17.9; zx = 8.96; sx = 7.17; rx = 1.81; iy = 12.6; zy = 7.67; sy = 6.3; ry = 1.52; jt = 24.9; ct = 11.7; nw = 19.82;
            }
            else if (sName.Contains("HSS5X4X.313"))
            {
                d = 5; w = 4; ar = 4.68; t = 0.291; ix = 15.8; zx = 7.79; sx = 6.32; rx = 1.84; iy = 11.1; zy = 6.67; sy = 5.57; ry = 1.54; jt = 21.7; ct = 10.1; nw = 16.96;
            }
            else if (sName.Contains("HSS5X4X.250"))
            {
                d = 5; w = 4; ar = 3.84; t = 0.233; ix = 13.4; zx = 6.49; sx = 5.35; rx = 1.87; iy = 9.46; zy = 5.57; sy = 4.73; ry = 1.57; jt = 18; ct = 8.32; nw = 13.91;
            }
            else if (sName.Contains("HSS5X4X.188"))
            {
                d = 5; w = 4; ar = 2.93; t = 0.174; ix = 10.6; zx = 5.05; sx = 4.22; rx = 1.9; iy = 7.48; zy = 4.34; sy = 3.74; ry = 1.6; jt = 14; ct = 6.41; nw = 10.7;
            }
            else if (sName.Contains("HSS5X4X.125"))
            {
                d = 5; w = 4; ar = 2; t = 0.116; ix = 7.42; zx = 3.5; sx = 2.97; rx = 1.93; iy = 5.27; zy = 3.01; sy = 2.64; ry = 1.62; jt = 9.66; ct = 4.39; nw = 7.31;
            }
            else if (sName.Contains("HSS5X3X.500"))
            {
                d = 5; w = 3; ar = 6.02; t = 0.465; ix = 16.4; zx = 8.83; sx = 6.57; rx = 1.65; iy = 7.18; zy = 6.1; sy = 4.78; ry = 1.09; jt = 17.6; ct = 10.3; nw = 21.63;
            }
            else if (sName.Contains("HSS5X3X.375"))
            {
                d = 5; w = 3; ar = 4.78; t = 0.349; ix = 14.1; zx = 7.34; sx = 5.65; rx = 1.72; iy = 6.25; zy = 5.1; sy = 4.16; ry = 1.14; jt = 14.9; ct = 8.44; nw = 17.27;
            }
            else if (sName.Contains("HSS5X3X.313"))
            {
                d = 5; w = 3; ar = 4.1; t = 0.291; ix = 12.6; zx = 6.42; sx = 5.03; rx = 1.75; iy = 5.6; zy = 4.48; sy = 3.73; ry = 1.17; jt = 13.1; ct = 7.33; nw = 14.83;
            }
            else if (sName.Contains("HSS5X3X.250"))
            {
                d = 5; w = 3; ar = 3.37; t = 0.233; ix = 10.7; zx = 5.38; sx = 4.29; rx = 1.78; iy = 4.81; zy = 3.77; sy = 3.21; ry = 1.19; jt = 11; ct = 6.1; nw = 12.21;
            }
            else if (sName.Contains("HSS5X3X.188"))
            {
                d = 5; w = 3; ar = 2.58; t = 0.174; ix = 8.53; zx = 4.21; sx = 3.41; rx = 1.82; iy = 3.85; zy = 2.96; sy = 2.57; ry = 1.22; jt = 8.64; ct = 4.73; nw = 9.42;
            }
            else if (sName.Contains("HSS5X3X.125"))
            {
                d = 5; w = 3; ar = 1.77; t = 0.116; ix = 6.03; zx = 2.93; sx = 2.41; rx = 1.85; iy = 2.75; zy = 2.07; sy = 1.83; ry = 1.25; jt = 6.02; ct = 3.26; nw = 6.46;
            }
            else if (sName.Contains("HSS5X2-1/2X.250"))
            {
                d = 5; w = 2.5; ar = 3.14; t = 0.233; ix = 9.4; zx = 4.83; sx = 3.76; rx = 1.73; iy = 3.13; zy = 2.95; sy = 2.5; ry = 0.999; jt = 7.93; ct = 4.99; nw = 11.36;
            }
            else if (sName.Contains("HSS5X2-1/2X.188"))
            {
                d = 5; w = 2.5; ar = 2.41; t = 0.174; ix = 7.51; zx = 3.79; sx = 3.01; rx = 1.77; iy = 2.53; zy = 2.33; sy = 2.03; ry = 1.02; jt = 6.26; ct = 3.89; nw = 8.78;
            }
            else if (sName.Contains("HSS5X2-1/2X.125"))
            {
                d = 5; w = 2.5; ar = 1.65; t = 0.116; ix = 5.34; zx = 2.65; sx = 2.14; rx = 1.8; iy = 1.82; zy = 1.64; sy = 1.46; ry = 1.05; jt = 4.4; ct = 2.7; nw = 6.03;
            }
            else if (sName.Contains("HSS5X2X.375"))
            {
                d = 5; w = 2; ar = 4.09; t = 0.349; ix = 10.4; zx = 5.71; sx = 4.14; rx = 1.59; iy = 2.28; zy = 2.88; sy = 2.28; ry = 0.748; jt = 6.61; ct = 5.2; nw = 14.72;
            }
            else if (sName.Contains("HSS5X2X.313"))
            {
                d = 5; w = 2; ar = 3.52; t = 0.291; ix = 9.35; zx = 5.05; sx = 3.74; rx = 1.63; iy = 2.1; zy = 2.57; sy = 2.1; ry = 0.772; jt = 5.99; ct = 4.59; nw = 12.7;
            }
            else if (sName.Contains("HSS5X2X.250"))
            {
                d = 5; w = 2; ar = 2.91; t = 0.233; ix = 8.08; zx = 4.27; sx = 3.23; rx = 1.67; iy = 1.84; zy = 2.2; sy = 1.84; ry = 0.797; jt = 5.17; ct = 3.88; nw = 10.51;
            }
            else if (sName.Contains("HSS5X2X.188"))
            {
                d = 5; w = 2; ar = 2.24; t = 0.174; ix = 6.5; zx = 3.37; sx = 2.6; rx = 1.7; iy = 1.51; zy = 1.75; sy = 1.51; ry = 0.823; jt = 4.15; ct = 3.05; nw = 8.15;
            }
            else if (sName.Contains("HSS5X2X.125"))
            {
                d = 5; w = 2; ar = 1.54; t = 0.116; ix = 4.65; zx = 2.37; sx = 1.86; rx = 1.74; iy = 1.1; zy = 1.24; sy = 1.1; ry = 0.848; jt = 2.95; ct = 2.13; nw = 5.61;
            }
            else if (sName.Contains("HSS4-1/2X4-1/2X.500"))
            {
                d = 4.5; w = 4.5; ar = 6.95; t = 0.465; ix = 18.1; zx = 10.2; sx = 8.03; rx = 1.61; iy = 18.1; zy = 10.2; sy = 8.03; ry = 1.61; jt = 31.3; ct = 14.8; nw = 25.03;
            }
            else if (sName.Contains("HSS4-1/2X4-1/2X.375"))
            {
                d = 4.5; w = 4.5; ar = 5.48; t = 0.349; ix = 15.3; zx = 8.36; sx = 6.79; rx = 1.67; iy = 15.3; zy = 8.36; sy = 6.79; ry = 1.67; jt = 25.7; ct = 11.9; nw = 19.82;
            }
            else if (sName.Contains("HSS4-1/2X4-1/2X.313"))
            {
                d = 4.5; w = 4.5; ar = 4.68; t = 0.291; ix = 13.5; zx = 7.27; sx = 6; rx = 1.7; iy = 13.5; zy = 7.27; sy = 6; ry = 1.7; jt = 22.3; ct = 10.2; nw = 16.96;
            }
            else if (sName.Contains("HSS4-1/2X4-1/2X.250"))
            {
                d = 4.5; w = 4.5; ar = 3.84; t = 0.233; ix = 11.4; zx = 6.06; sx = 5.08; rx = 1.73; iy = 11.4; zy = 6.06; sy = 5.08; ry = 1.73; jt = 18.5; ct = 8.44; nw = 13.91;
            }
            else if (sName.Contains("HSS4-1/2X4-1/2X.188"))
            {
                d = 4.5; w = 4.5; ar = 2.93; t = 0.174; ix = 9.02; zx = 4.71; sx = 4.01; rx = 1.75; iy = 9.02; zy = 4.71; sy = 4.01; ry = 1.75; jt = 14.4; ct = 6.49; nw = 10.7;
            }
            else if (sName.Contains("HSS4-1/2X4-1/2X.125"))
            {
                d = 4.5; w = 4.5; ar = 2; t = 0.116; ix = 6.35; zx = 3.27; sx = 2.82; rx = 1.78; iy = 6.35; zy = 3.27; sy = 2.82; ry = 1.78; jt = 9.92; ct = 4.45; nw = 7.31;
            }
            else if (sName.Contains("HSS4X4X.500"))
            {
                d = 4; w = 4; ar = 6.02; t = 0.465; ix = 11.9; zx = 7.7; sx = 5.97; rx = 1.41; iy = 11.9; zy = 7.7; sy = 5.97; ry = 1.41; jt = 21; ct = 11.2; nw = 21.63;
            }
            else if (sName.Contains("HSS4X4X.375"))
            {
                d = 4; w = 4; ar = 4.78; t = 0.349; ix = 10.3; zx = 6.39; sx = 5.13; rx = 1.47; iy = 10.3; zy = 6.39; sy = 5.13; ry = 1.47; jt = 17.5; ct = 9.14; nw = 17.27;
            }
            else if (sName.Contains("HSS4X4X.313"))
            {
                d = 4; w = 4; ar = 4.1; t = 0.291; ix = 9.14; zx = 5.59; sx = 4.57; rx = 1.49; iy = 9.14; zy = 5.59; sy = 4.57; ry = 1.49; jt = 15.3; ct = 7.91; nw = 14.83;
            }
            else if (sName.Contains("HSS4X4X.250"))
            {
                d = 4; w = 4; ar = 3.37; t = 0.233; ix = 7.8; zx = 4.69; sx = 3.9; rx = 1.52; iy = 7.8; zy = 4.69; sy = 3.9; ry = 1.52; jt = 12.8; ct = 6.56; nw = 12.21;
            }
            else if (sName.Contains("HSS4X4X.188"))
            {
                d = 4; w = 4; ar = 2.58; t = 0.174; ix = 6.21; zx = 3.67; sx = 3.1; rx = 1.55; iy = 6.21; zy = 3.67; sy = 3.1; ry = 1.55; jt = 10; ct = 5.07; nw = 9.42;
            }
            else if (sName.Contains("HSS4X4X.125"))
            {
                d = 4; w = 4; ar = 1.77; t = 0.116; ix = 4.4; zx = 2.56; sx = 2.2; rx = 1.58; iy = 4.4; zy = 2.56; sy = 2.2; ry = 1.58; jt = 6.91; ct = 3.49; nw = 6.46;
            }
            else if (sName.Contains("HSS4X3X.375"))
            {
                d = 4; w = 3; ar = 4.09; t = 0.349; ix = 7.93; zx = 5.12; sx = 3.97; rx = 1.39; iy = 5.01; zy = 4.18; sy = 3.34; ry = 1.11; jt = 10.6; ct = 6.59; nw = 14.72;
            }
            else if (sName.Contains("HSS4X3X.313"))
            {
                d = 4; w = 3; ar = 3.52; t = 0.291; ix = 7.14; zx = 4.51; sx = 3.57; rx = 1.42; iy = 4.52; zy = 3.69; sy = 3.02; ry = 1.13; jt = 9.41; ct = 5.75; nw = 12.7;
            }
            else if (sName.Contains("HSS4X3X.250"))
            {
                d = 4; w = 3; ar = 2.91; t = 0.233; ix = 6.15; zx = 3.81; sx = 3.07; rx = 1.45; iy = 3.91; zy = 3.12; sy = 2.61; ry = 1.16; jt = 7.96; ct = 4.81; nw = 10.51;
            }
            else if (sName.Contains("HSS4X3X.188"))
            {
                d = 4; w = 3; ar = 2.24; t = 0.174; ix = 4.93; zx = 3; sx = 2.47; rx = 1.49; iy = 3.16; zy = 2.46; sy = 2.1; ry = 1.19; jt = 6.26; ct = 3.74; nw = 8.15;
            }
            else if (sName.Contains("HSS4X3X.125"))
            {
                d = 4; w = 3; ar = 1.54; t = 0.116; ix = 3.52; zx = 2.11; sx = 1.76; rx = 1.52; iy = 2.27; zy = 1.73; sy = 1.51; ry = 1.21; jt = 4.38; ct = 2.59; nw = 5.61;
            }
            else if (sName.Contains("HSS4X2-1/2X.375"))
            {
                d = 4; w = 2.5; ar = 3.74; t = 0.349; ix = 6.77; zx = 4.48; sx = 3.38; rx = 1.35; iy = 3.17; zy = 3.2; sy = 2.54; ry = 0.922; jt = 7.57; ct = 5.32; nw = 13.44;
            }
            else if (sName.Contains("HSS4X2-1/2X.313"))
            {
                d = 4; w = 2.5; ar = 3.23; t = 0.291; ix = 6.13; zx = 3.97; sx = 3.07; rx = 1.38; iy = 2.89; zy = 2.85; sy = 2.32; ry = 0.947; jt = 6.77; ct = 4.67; nw = 11.64;
            }
            else if (sName.Contains("HSS4X2-1/2X.250"))
            {
                d = 4; w = 2.5; ar = 2.67; t = 0.233; ix = 5.32; zx = 3.38; sx = 2.66; rx = 1.41; iy = 2.53; zy = 2.43; sy = 2.02; ry = 0.973; jt = 5.78; ct = 3.93; nw = 9.66;
            }
            else if (sName.Contains("HSS4X2-1/2X.188"))
            {
                d = 4; w = 2.5; ar = 2.06; t = 0.174; ix = 4.3; zx = 2.67; sx = 2.15; rx = 1.44; iy = 2.06; zy = 1.93; sy = 1.65; ry = 0.999; jt = 4.59; ct = 3.08; nw = 7.51;
            }
            else if (sName.Contains("HSS4X2-1/2X.125"))
            {
                d = 4; w = 2.5; ar = 1.42; t = 0.116; ix = 3.09; zx = 1.88; sx = 1.54; rx = 1.47; iy = 1.49; zy = 1.36; sy = 1.19; ry = 1.03; jt = 3.23; ct = 2.14; nw = 5.18;
            }
            else if (sName.Contains("HSS4X2X.375"))
            {
                d = 4; w = 2; ar = 3.39; t = 0.349; ix = 5.6; zx = 3.84; sx = 2.8; rx = 1.29; iy = 1.8; zy = 2.31; sy = 1.8; ry = 0.729; jt = 4.83; ct = 4.04; nw = 12.17;
            }
            else if (sName.Contains("HSS4X2X.313"))
            {
                d = 4; w = 2; ar = 2.94; t = 0.291; ix = 5.13; zx = 3.43; sx = 2.56; rx = 1.32; iy = 1.67; zy = 2.08; sy = 1.67; ry = 0.754; jt = 4.4; ct = 3.59; nw = 10.58;
            }
            else if (sName.Contains("HSS4X2X.250"))
            {
                d = 4; w = 2; ar = 2.44; t = 0.233; ix = 4.49; zx = 2.94; sx = 2.25; rx = 1.36; iy = 1.48; zy = 1.79; sy = 1.48; ry = 0.779; jt = 3.82; ct = 3.05; nw = 8.81;
            }
            else if (sName.Contains("HSS4X2X.188"))
            {
                d = 4; w = 2; ar = 1.89; t = 0.174; ix = 3.66; zx = 2.34; sx = 1.83; rx = 1.39; iy = 1.22; zy = 1.43; sy = 1.22; ry = 0.804; jt = 3.08; ct = 2.41; nw = 6.87;
            }
            else if (sName.Contains("HSS4X2X.125"))
            {
                d = 4; w = 2; ar = 1.3; t = 0.116; ix = 2.65; zx = 1.66; sx = 1.32; rx = 1.43; iy = 0.898; zy = 1.02; sy = 0.898; ry = 0.83; jt = 2.2; ct = 1.69; nw = 4.75;
            }
            else if (sName.Contains("HSS3-1/2X3-1/2X.375"))
            {
                d = 3.5; w = 3.5; ar = 4.09; t = 0.349; ix = 6.49; zx = 4.69; sx = 3.71; rx = 1.26; iy = 6.49; zy = 4.69; sy = 3.71; ry = 1.26; jt = 11.2; ct = 6.77; nw = 14.72;
            }
            else if (sName.Contains("HSS3-1/2X3-1/2X.313"))
            {
                d = 3.5; w = 3.5; ar = 3.52; t = 0.291; ix = 5.84; zx = 4.14; sx = 3.34; rx = 1.29; iy = 5.84; zy = 4.14; sy = 3.34; ry = 1.29; jt = 9.89; ct = 5.9; nw = 12.7;
            }
            else if (sName.Contains("HSS3-1/2X3-1/2X.250"))
            {
                d = 3.5; w = 3.5; ar = 2.91; t = 0.233; ix = 5.04; zx = 3.5; sx = 2.88; rx = 1.32; iy = 5.04; zy = 3.5; sy = 2.88; ry = 1.32; jt = 8.35; ct = 4.92; nw = 10.51;
            }
            else if (sName.Contains("HSS3-1/2X3-1/2X.188"))
            {
                d = 3.5; w = 3.5; ar = 2.24; t = 0.174; ix = 4.05; zx = 2.76; sx = 2.31; rx = 1.35; iy = 4.05; zy = 2.76; sy = 2.31; ry = 1.35; jt = 6.56; ct = 3.83; nw = 8.15;
            }
            else if (sName.Contains("HSS3-1/2X3-1/2X.125"))
            {
                d = 3.5; w = 3.5; ar = 1.54; t = 0.116; ix = 2.9; zx = 1.93; sx = 1.66; rx = 1.37; iy = 2.9; zy = 1.93; sy = 1.66; ry = 1.37; jt = 4.58; ct = 2.65; nw = 5.61;
            }
            else if (sName.Contains("HSS3-1/2X2-1/2X.375"))
            {
                d = 3.5; w = 2.5; ar = 3.39; t = 0.349; ix = 4.75351325768565; zx = 3.59093418951379; sx = 2.71629329010609; rx = 1.18465347727178; iy = 2.76565677927178; zy = 2.82256749931395; sy = 2.21252542341743; ry = 0.903614352012787; jt = 6.15705793408725; ct = 4.56670166963923; nw = 12.17;
            }
            else if (sName.Contains("HSS3-1/2X2-1/2X.313"))
            {
                d = 3.5; w = 2.5; ar = 2.94; t = 0.291; ix = 4.34141914695052; zx = 3.20373450707522; sx = 2.48081094111458; rx = 1.21617687504311; iy = 2.53708615949927; zy = 2.52445169582727; sy = 2.02966892759942; ry = 0.929712115948344; jt = 5.53430619728493; ct = 4.03042374921947; nw = 10.58;
            }
            else if (sName.Contains("HSS3-1/2X2-1/2X.250"))
            {
                d = 3.5; w = 2.5; ar = 2.44; t = 0.233; ix = 3.79433836098526; zx = 2.73688673626114; sx = 2.16819334913444; rx = 1.24726508554105; iy = 2.22921312540217; zy = 2.16207885090504; sy = 1.78337050032174; ry = 0.956019020401681; jt = 4.7464583495471; ct = 3.40246839686392; nw = 8.81;
            }
            else if (sName.Contains("HSS3-1/2X2-1/2X.188"))
            {
                d = 3.5; w = 2.5; ar = 1.89; t = 0.174; ix = 3.08735885423477; zx = 2.17726960971144; sx = 1.76420505956272; rx = 1.27845600504381; iy = 1.82489340490839; zy = 1.72452932094131; sy = 1.45991472392671; ry = 0.982903728019064; jt = 3.77934544469524; ct = 2.67187455073801; nw = 6.87;
            }
            else if (sName.Contains("HSS3-1/2X2-1/2X.125"))
            {
                d = 3.5; w = 2.5; ar = 1.3; t = 0.116; ix = 2.23256081647586; zx = 1.54090358964716; sx = 1.27574903798621; rx = 1.30870621405736; iy = 1.32810544388873; zy = 1.2236856835271; sy = 1.06248435511098; ry = 1.00938487749612; jt = 2.67010016492518; ct = 1.86562031132978; nw = 4.75;
            }
            else if (sName.Contains("HSS3-1/2X2X.250"))
            {
                d = 3.5; w = 2; ar = 2.21; t = 0.233; ix = 3.17; zx = 2.36; sx = 1.81; rx = 1.2; iy = 1.3; zy = 1.58; sy = 1.3; ry = 0.766; jt = 3.16; ct = 2.64; nw = 7.96;
            }
            else if (sName.Contains("HSS3-1/2X2X.188"))
            {
                d = 3.5; w = 2; ar = 1.71; t = 0.174; ix = 2.61; zx = 1.89; sx = 1.49; rx = 1.23; iy = 1.08; zy = 1.27; sy = 1.08; ry = 0.792; jt = 2.55; ct = 2.09; nw = 6.23;
            }
            else if (sName.Contains("HSS3-1/2X2X.125"))
            {
                d = 3.5; w = 2; ar = 1.19; t = 0.116; ix = 1.9; zx = 1.34; sx = 1.09; rx = 1.27; iy = 0.795; zy = 0.912; sy = 0.795; ry = 0.818; jt = 1.83; ct = 1.47; nw = 4.33;
            }
            else if (sName.Contains("HSS3-1/2X1-1/2X.250"))
            {
                d = 3.5; w = 1.5; ar = 1.97; t = 0.233; ix = 2.55; zx = 1.98; sx = 1.46; rx = 1.14; iy = 0.638; zy = 1.06; sy = 0.851; ry = 0.569; jt = 1.79; ct = 1.88; nw = 7.11;
            }
            else if (sName.Contains("HSS3-1/2X1-1/2X.188"))
            {
                d = 3.5; w = 1.5; ar = 1.54; t = 0.174; ix = 2.12; zx = 1.6; sx = 1.21; rx = 1.17; iy = 0.544; zy = 0.867; sy = 0.725; ry = 0.594; jt = 1.49; ct = 1.51; nw = 5.59;
            }
            else if (sName.Contains("HSS3-1/2X1-1/2X.125"))
            {
                d = 3.5; w = 1.5; ar = 1.07; t = 0.116; ix = 1.57; zx = 1.15; sx = 0.896; rx = 1.21; iy = 0.411; zy = 0.63; sy = 0.548; ry = 0.619; jt = 1.09; ct = 1.08; nw = 3.9;
            }
            else if (sName.Contains("HSS3X3X.375"))
            {
                d = 3; w = 3; ar = 3.39; t = 0.349; ix = 3.78; zx = 3.25; sx = 2.52; rx = 1.06; iy = 3.78; zy = 3.25; sy = 2.52; ry = 1.06; jt = 6.64; ct = 4.74; nw = 12.17;
            }
            else if (sName.Contains("HSS3X3X.313"))
            {
                d = 3; w = 3; ar = 2.94; t = 0.291; ix = 3.45; zx = 2.9; sx = 2.3; rx = 1.08; iy = 3.45; zy = 2.9; sy = 2.3; ry = 1.08; jt = 5.94; ct = 4.18; nw = 10.58;
            }
            else if (sName.Contains("HSS3X3X.250"))
            {
                d = 3; w = 3; ar = 2.44; t = 0.233; ix = 3.02; zx = 2.48; sx = 2.01; rx = 1.11; iy = 3.02; zy = 2.48; sy = 2.01; ry = 1.11; jt = 5.08; ct = 3.52; nw = 8.81;
            }
            else if (sName.Contains("HSS3X3X.188"))
            {
                d = 3; w = 3; ar = 1.89; t = 0.174; ix = 2.46; zx = 1.97; sx = 1.64; rx = 1.14; iy = 2.46; zy = 1.97; sy = 1.64; ry = 1.14; jt = 4.03; ct = 2.76; nw = 6.87;
            }
            else if (sName.Contains("HSS3X3X.125"))
            {
                d = 3; w = 3; ar = 1.3; t = 0.116; ix = 1.78; zx = 1.4; sx = 1.19; rx = 1.17; iy = 1.78; zy = 1.4; sy = 1.19; ry = 1.17; jt = 2.84; ct = 1.92; nw = 4.75;
            }
            else if (sName.Contains("HSS3X2-1/2X.313"))
            {
                d = 3; w = 2.5; ar = 2.64; t = 0.291; ix = 2.92; zx = 2.51; sx = 1.94; rx = 1.05; iy = 2.18; zy = 2.2; sy = 1.74; ry = 0.908; jt = 4.34; ct = 3.39; nw = 9.51;
            }
            else if (sName.Contains("HSS3X2-1/2X.250"))
            {
                d = 3; w = 2.5; ar = 2.21; t = 0.233; ix = 2.57; zx = 2.16; sx = 1.72; rx = 1.08; iy = 1.93; zy = 1.9; sy = 1.54; ry = 0.935; jt = 3.74; ct = 2.87; nw = 7.96;
            }
            else if (sName.Contains("HSS3X2-1/2X.188"))
            {
                d = 3; w = 2.5; ar = 1.71; t = 0.174; ix = 2.11; zx = 1.73; sx = 1.41; rx = 1.11; iy = 1.59; zy = 1.52; sy = 1.27; ry = 0.963; jt = 3; ct = 2.27; nw = 6.23;
            }
            else if (sName.Contains("HSS3X2-1/2X.125"))
            {
                d = 3; w = 2.5; ar = 1.19; t = 0.116; ix = 1.54; zx = 1.23; sx = 1.03; rx = 1.14; iy = 1.16; zy = 1.09; sy = 0.931; ry = 0.99; jt = 2.13; ct = 1.59; nw = 4.33;
            }
            else if (sName.Contains("HSS3X2X.313"))
            {
                d = 3; w = 2; ar = 2.35; t = 0.291; ix = 2.38; zx = 2.11; sx = 1.59; rx = 1.01; iy = 1.24; zy = 1.58; sy = 1.24; ry = 0.725; jt = 2.87; ct = 2.6; nw = 8.45;
            }
            else if (sName.Contains("HSS3X2X.250"))
            {
                d = 3; w = 2; ar = 1.97; t = 0.233; ix = 2.13; zx = 1.83; sx = 1.42; rx = 1.04; iy = 1.11; zy = 1.38; sy = 1.11; ry = 0.751; jt = 2.52; ct = 2.23; nw = 7.11;
            }
            else if (sName.Contains("HSS3X2X.188"))
            {
                d = 3; w = 2; ar = 1.54; t = 0.174; ix = 1.77; zx = 1.48; sx = 1.18; rx = 1.07; iy = 0.932; zy = 1.12; sy = 0.932; ry = 0.778; jt = 2.05; ct = 1.78; nw = 5.59;
            }
            else if (sName.Contains("HSS3X2X.125"))
            {
                d = 3; w = 2; ar = 1.07; t = 0.116; ix = 1.3; zx = 1.06; sx = 0.867; rx = 1.1; iy = 0.692; zy = 0.803; sy = 0.692; ry = 0.804; jt = 1.47; ct = 1.25; nw = 3.9;
            }
            else if (sName.Contains("HSS3X1-1/2X.250"))
            {
                d = 3; w = 1.5; ar = 1.74; t = 0.233; ix = 1.68; zx = 1.51; sx = 1.12; rx = 0.982; iy = 0.543; zy = 0.911; sy = 0.725; ry = 0.559; jt = 1.44; ct = 1.58; nw = 6.26;
            }
            else if (sName.Contains("HSS3X1-1/2X.188"))
            {
                d = 3; w = 1.5; ar = 1.37; t = 0.174; ix = 1.42; zx = 1.24; sx = 0.945; rx = 1.02; iy = 0.467; zy = 0.752; sy = 0.622; ry = 0.584; jt = 1.21; ct = 1.28; nw = 4.96;
            }
            else if (sName.Contains("HSS3X1-1/2X.125"))
            {
                d = 3; w = 1.5; ar = 0.956; t = 0.116; ix = 1.06; zx = 0.895; sx = 0.706; rx = 1.05; iy = 0.355; zy = 0.55; sy = 0.474; ry = 0.61; jt = 0.886; ct = 0.92; nw = 3.48;
            }
            else if (sName.Contains("HSS3X1X.188"))
            {
                d = 3; w = 1; ar = 1.19; t = 0.174; ix = 1.07; zx = 0.989; sx = 0.713; rx = 0.947; iy = 0.173; zy = 0.432; sy = 0.345; ry = 0.38; jt = 0.526; ct = 0.792; nw = 4.32;
            }
            else if (sName.Contains("HSS3X1X.125"))
            {
                d = 3; w = 1; ar = 0.84; t = 0.116; ix = 0.817; zx = 0.728; sx = 0.545; rx = 0.987; iy = 0.138; zy = 0.325; sy = 0.276; ry = 0.405; jt = 0.408; ct = 0.585; nw = 3.05;
            }
            else if (sName.Contains("HSS2-1/2X2-1/2X.313"))
            {
                d = 2.5; w = 2.5; ar = 2.35; t = 0.291; ix = 1.82; zx = 1.88; sx = 1.46; rx = 0.88; iy = 1.82; zy = 1.88; sy = 1.46; ry = 0.88; jt = 3.2; ct = 2.74; nw = 8.45;
            }
            else if (sName.Contains("HSS2-1/2X2-1/2X.250"))
            {
                d = 2.5; w = 2.5; ar = 1.97; t = 0.233; ix = 1.63; zx = 1.63; sx = 1.3; rx = 0.908; iy = 1.63; zy = 1.63; sy = 1.3; ry = 0.908; jt = 2.79; ct = 2.35; nw = 7.11;
            }
            else if (sName.Contains("HSS2-1/2X2-1/2X.188"))
            {
                d = 2.5; w = 2.5; ar = 1.54; t = 0.174; ix = 1.35; zx = 1.32; sx = 1.08; rx = 0.937; iy = 1.35; zy = 1.32; sy = 1.08; ry = 0.937; jt = 2.25; ct = 1.86; nw = 5.59;
            }
            else if (sName.Contains("HSS2-1/2X2-1/2X.125"))
            {
                d = 2.5; w = 2.5; ar = 1.07; t = 0.116; ix = 0.998; zx = 0.947; sx = 0.799; rx = 0.965; iy = 0.998; zy = 0.947; sy = 0.799; ry = 0.965; jt = 1.61; ct = 1.31; nw = 3.9;
            }
            else if (sName.Contains("HSS2-1/2X2X.250"))
            {
                d = 2.5; w = 2; ar = 1.74; t = 0.233; ix = 1.33; zx = 1.37; sx = 1.06; rx = 0.874; iy = 0.93; zy = 1.17; sy = 0.93; ry = 0.731; jt = 1.9; ct = 1.82; nw = 6.26;
            }
            else if (sName.Contains("HSS2-1/2X2X.188"))
            {
                d = 2.5; w = 2; ar = 1.37; t = 0.174; ix = 1.12; zx = 1.12; sx = 0.894; rx = 0.904; iy = 0.786; zy = 0.956; sy = 0.786; ry = 0.758; jt = 1.55; ct = 1.46; nw = 4.96;
            }
            else if (sName.Contains("HSS2-1/2X2X.125"))
            {
                d = 2.5; w = 2; ar = 0.956; t = 0.116; ix = 0.833; zx = 0.809; sx = 0.667; rx = 0.934; iy = 0.589; zy = 0.694; sy = 0.589; ry = 0.785; jt = 1.12; ct = 1.04; nw = 3.48;
            }
            else if (sName.Contains("HSS2-1/2X1-1/2X.250"))
            {
                d = 2.5; w = 1.5; ar = 1.51; t = 0.233; ix = 1.03; zx = 1.11; sx = 0.822; rx = 0.826; iy = 0.449; zy = 0.764; sy = 0.599; ry = 0.546; jt = 1.1; ct = 1.29; nw = 5.41;
            }
            else if (sName.Contains("HSS2-1/2X1-1/2X.188"))
            {
                d = 2.5; w = 1.5; ar = 1.19; t = 0.174; ix = 0.882; zx = 0.915; sx = 0.705; rx = 0.86; iy = 0.39; zy = 0.636; sy = 0.52; ry = 0.572; jt = 0.929; ct = 1.05; nw = 4.32;
            }
            else if (sName.Contains("HSS2-1/2X1-1/2X.125"))
            {
                d = 2.5; w = 1.5; ar = 0.84; t = 0.116; ix = 0.668; zx = 0.671; sx = 0.535; rx = 0.892; iy = 0.3; zy = 0.469; sy = 0.399; ry = 0.597; jt = 0.687; ct = 0.759; nw = 3.05;
            }
            else if (sName.Contains("HSS2-1/2X1X.188"))
            {
                d = 2.5; w = 1; ar = 1.02; t = 0.174; ix = 0.646; zx = 0.713; sx = 0.517; rx = 0.796; iy = 0.143; zy = 0.36; sy = 0.285; ry = 0.374; jt = 0.412; ct = 0.648; nw = 3.68;
            }
            else if (sName.Contains("HSS2-1/2X1X.125"))
            {
                d = 2.5; w = 1; ar = 0.724; t = 0.116; ix = 0.503; zx = 0.532; sx = 0.403; rx = 0.834; iy = 0.115; zy = 0.274; sy = 0.23; ry = 0.399; jt = 0.322; ct = 0.483; nw = 2.63;
            }
            else if (sName.Contains("HSS2-1/4X2-1/4X.250"))
            {
                d = 2.25; w = 2.25; ar = 1.74; t = 0.233; ix = 1.13; zx = 1.28; sx = 1.01; rx = 0.806; iy = 1.13; zy = 1.28; sy = 1.01; ry = 0.806; jt = 1.96; ct = 1.85; nw = 6.26;
            }
            else if (sName.Contains("HSS2-1/4X2-1/4X.188"))
            {
                d = 2.25; w = 2.25; ar = 1.37; t = 0.174; ix = 0.953; zx = 1.04; sx = 0.847; rx = 0.835; iy = 0.953; zy = 1.04; sy = 0.847; ry = 0.835; jt = 1.6; ct = 1.48; nw = 4.96;
            }
            else if (sName.Contains("HSS2-1/4X2-1/4X.125"))
            {
                d = 2.25; w = 2.25; ar = 0.956; t = 0.116; ix = 0.712; zx = 0.755; sx = 0.633; rx = 0.863; iy = 0.712; zy = 0.755; sy = 0.633; ry = 0.863; jt = 1.15; ct = 1.05; nw = 3.48;
            }
            else if (sName.Contains("HSS2-1/4X2X.188"))
            {
                d = 2.25; w = 2; ar = 1.28; t = 0.174; ix = 0.859; zx = 0.952; sx = 0.764; rx = 0.819; iy = 0.713; zy = 0.877; sy = 0.713; ry = 0.747; jt = 1.32; ct = 1.3; nw = 4.64;
            }
            else if (sName.Contains("HSS2-1/4X2X.125"))
            {
                d = 2.25; w = 2; ar = 0.898; t = 0.116; ix = 0.646; zx = 0.693; sx = 0.574; rx = 0.848; iy = 0.538; zy = 0.639; sy = 0.538; ry = 0.774; jt = 0.957; ct = 0.927; nw = 3.27;
            }
            else if (sName.Contains("HSS2X2X.250"))
            {
                d = 2; w = 2; ar = 1.51; t = 0.233; ix = 0.747; zx = 0.964; sx = 0.747; rx = 0.704; iy = 0.747; zy = 0.964; sy = 0.747; ry = 0.704; jt = 1.31; ct = 1.41; nw = 5.41;
            }
            else if (sName.Contains("HSS2X2X.188"))
            {
                d = 2; w = 2; ar = 1.19; t = 0.174; ix = 0.641; zx = 0.797; sx = 0.641; rx = 0.733; iy = 0.641; zy = 0.797; sy = 0.641; ry = 0.733; jt = 1.09; ct = 1.14; nw = 4.32;
            }
            else if (sName.Contains("HSS2X2X.125"))
            {
                d = 2; w = 2; ar = 0.84; t = 0.116; ix = 0.486; zx = 0.584; sx = 0.486; rx = 0.761; iy = 0.486; zy = 0.584; sy = 0.486; ry = 0.761; jt = 0.796; ct = 0.817; nw = 3.05;
            }
            else if (sName.Contains("HSS2X1-1/2X.188"))
            {
                d = 2; w = 1.5; ar = 1.02; t = 0.174; ix = 0.495; zx = 0.639; sx = 0.495; rx = 0.697; iy = 0.313; zy = 0.521; sy = 0.417; ry = 0.554; jt = 0.664; ct = 0.822; nw = 3.68;
            }
            else if (sName.Contains("HSS2X1-1/2X.125"))
            {
                d = 2; w = 1.5; ar = 0.724; t = 0.116; ix = 0.383; zx = 0.475; sx = 0.383; rx = 0.728; iy = 0.244; zy = 0.389; sy = 0.325; ry = 0.581; jt = 0.496; ct = 0.599; nw = 2.63;
            }
            else if (sName.Contains("HSS2X1X.188"))
            {
                d = 2; w = 1; ar = 0.845; t = 0.174; ix = 0.35; zx = 0.48; sx = 0.35; rx = 0.643; iy = 0.112; zy = 0.288; sy = 0.225; ry = 0.365; jt = 0.301; ct = 0.505; nw = 3.04;
            }
            else if (sName.Contains("HSS2X1X.125"))
            {
                d = 2; w = 1; ar = 0.608; t = 0.116; ix = 0.28; zx = 0.366; sx = 0.28; rx = 0.679; iy = 0.0922; zy = 0.223; sy = 0.184; ry = 0.39; jt = 0.238; ct = 0.38; nw = 2.2;
            }
        }

        public static List<string> GetHSSRecNames()
        {
            List<string> names = new List<string>();

            names.Add("HSS20X12X.625");
            names.Add("HSS20X12X.500");
            names.Add("HSS20X12X.375");
            names.Add("HSS20X12X.313");
            names.Add("HSS20X8X.625");
            names.Add("HSS20X8X.500");
            names.Add("HSS20X8X.375");
            names.Add("HSS20X8X.313");
            names.Add("HSS20X4X.500");
            names.Add("HSS20X4X.375");
            names.Add("HSS20X4X.313");
            names.Add("HSS20X4X.250");
            names.Add("HSS18X6X.625");
            names.Add("HSS18X6X.500");
            names.Add("HSS18X6X.375");
            names.Add("HSS18X6X.313");
            names.Add("HSS18X6X.250");
            names.Add("HSS16X16X.625");
            names.Add("HSS16X16X.500");
            names.Add("HSS16X16X.375");
            names.Add("HSS16X16X.313");
            names.Add("HSS16X12X.625");
            names.Add("HSS16X12X.500");
            names.Add("HSS16X12X.375");
            names.Add("HSS16X12X.313");
            names.Add("HSS16X8X.625");
            names.Add("HSS16X8X.500");
            names.Add("HSS16X8X.375");
            names.Add("HSS16X8X.313");
            names.Add("HSS16X8X.250");
            names.Add("HSS16X4X.625");
            names.Add("HSS16X4X.500");
            names.Add("HSS16X4X.375");
            names.Add("HSS16X4X.313");
            names.Add("HSS16X4X.250");
            names.Add("HSS16X4X.188");
            names.Add("HSS14X14X.625");
            names.Add("HSS14X14X.500");
            names.Add("HSS14X14X.375");
            names.Add("HSS14X14X.313");
            names.Add("HSS14X10X.625");
            names.Add("HSS14X10X.500");
            names.Add("HSS14X10X.375");
            names.Add("HSS14X10X.313");
            names.Add("HSS14X10X.250");
            names.Add("HSS14X6X.625");
            names.Add("HSS14X6X.500");
            names.Add("HSS14X6X.375");
            names.Add("HSS14X6X.313");
            names.Add("HSS14X6X.250");
            names.Add("HSS14X6X.188");
            names.Add("HSS14X4X.625");
            names.Add("HSS14X4X.500");
            names.Add("HSS14X4X.375");
            names.Add("HSS14X4X.313");
            names.Add("HSS14X4X.250");
            names.Add("HSS14X4X.188");
            names.Add("HSS12X12X.625");
            names.Add("HSS12X12X.500");
            names.Add("HSS12X12X.375");
            names.Add("HSS12X12X.313");
            names.Add("HSS12X12X.250");
            names.Add("HSS12X12X.188");
            names.Add("HSS12X10X.500");
            names.Add("HSS12X10X.375");
            names.Add("HSS12X10X.313");
            names.Add("HSS12X10X.250");
            names.Add("HSS12X8X.625");
            names.Add("HSS12X8X.500");
            names.Add("HSS12X8X.375");
            names.Add("HSS12X8X.313");
            names.Add("HSS12X8X.250");
            names.Add("HSS12X8X.188");
            names.Add("HSS12X6X.625");
            names.Add("HSS12X6X.500");
            names.Add("HSS12X6X.375");
            names.Add("HSS12X6X.313");
            names.Add("HSS12X6X.250");
            names.Add("HSS12X6X.188");
            names.Add("HSS12X4X.625");
            names.Add("HSS12X4X.500");
            names.Add("HSS12X4X.375");
            names.Add("HSS12X4X.313");
            names.Add("HSS12X4X.250");
            names.Add("HSS12X4X.188");
            names.Add("HSS12X3-1/2X.375");
            names.Add("HSS12X3-1/2X.313");
            names.Add("HSS12X3X.313");
            names.Add("HSS12X3X.250");
            names.Add("HSS12X3X.188");
            names.Add("HSS12X2X.313");
            names.Add("HSS12X2X.250");
            names.Add("HSS12X2X.188");
            names.Add("HSS10X10X.625");
            names.Add("HSS10X10X.500");
            names.Add("HSS10X10X.375");
            names.Add("HSS10X10X.313");
            names.Add("HSS10X10X.250");
            names.Add("HSS10X10X.188");
            names.Add("HSS10X8X.625");
            names.Add("HSS10X8X.500");
            names.Add("HSS10X8X.375");
            names.Add("HSS10X8X.313");
            names.Add("HSS10X8X.250");
            names.Add("HSS10X8X.188");
            names.Add("HSS10X6X.625");
            names.Add("HSS10X6X.500");
            names.Add("HSS10X6X.375");
            names.Add("HSS10X6X.313");
            names.Add("HSS10X6X.250");
            names.Add("HSS10X6X.188");
            names.Add("HSS10X5X.375");
            names.Add("HSS10X5X.313");
            names.Add("HSS10X5X.250");
            names.Add("HSS10X5X.188");
            names.Add("HSS10X4X.625");
            names.Add("HSS10X4X.500");
            names.Add("HSS10X4X.375");
            names.Add("HSS10X4X.313");
            names.Add("HSS10X4X.250");
            names.Add("HSS10X4X.188");
            names.Add("HSS10X4X.125");
            names.Add("HSS10X3-1/2X.500");
            names.Add("HSS10X3-1/2X.375");
            names.Add("HSS10X3-1/2X.313");
            names.Add("HSS10X3-1/2X.250");
            names.Add("HSS10X3-1/2X.188");
            names.Add("HSS10X3-1/2X.125");
            names.Add("HSS10X3X.375");
            names.Add("HSS10X3X.313");
            names.Add("HSS10X3X.250");
            names.Add("HSS10X3X.188");
            names.Add("HSS10X3X.125");
            names.Add("HSS10X2X.375");
            names.Add("HSS10X2X.313");
            names.Add("HSS10X2X.250");
            names.Add("HSS10X2X.188");
            names.Add("HSS10X2X.125");
            names.Add("HSS9X9X.625");
            names.Add("HSS9X9X.500");
            names.Add("HSS9X9X.375");
            names.Add("HSS9X9X.313");
            names.Add("HSS9X9X.250");
            names.Add("HSS9X9X.188");
            names.Add("HSS9X9X.125");
            names.Add("HSS9X7X.625");
            names.Add("HSS9X7X.500");
            names.Add("HSS9X7X.375");
            names.Add("HSS9X7X.313");
            names.Add("HSS9X7X.250");
            names.Add("HSS9X7X.188");
            names.Add("HSS9X5X.625");
            names.Add("HSS9X5X.500");
            names.Add("HSS9X5X.375");
            names.Add("HSS9X5X.313");
            names.Add("HSS9X5X.250");
            names.Add("HSS9X5X.188");
            names.Add("HSS9X3X.500");
            names.Add("HSS9X3X.375");
            names.Add("HSS9X3X.313");
            names.Add("HSS9X3X.250");
            names.Add("HSS9X3X.188");
            names.Add("HSS8X8X.625");
            names.Add("HSS8X8X.500");
            names.Add("HSS8X8X.375");
            names.Add("HSS8X8X.313");
            names.Add("HSS8X8X.250");
            names.Add("HSS8X8X.188");
            names.Add("HSS8X8X.125");
            names.Add("HSS8X6X.625");
            names.Add("HSS8X6X.500");
            names.Add("HSS8X6X.375");
            names.Add("HSS8X6X.313");
            names.Add("HSS8X6X.250");
            names.Add("HSS8X6X.188");
            names.Add("HSS8X4X.625");
            names.Add("HSS8X4X.500");
            names.Add("HSS8X4X.375");
            names.Add("HSS8X4X.313");
            names.Add("HSS8X4X.250");
            names.Add("HSS8X4X.188");
            names.Add("HSS8X4X.125");
            names.Add("HSS8X3X.500");
            names.Add("HSS8X3X.375");
            names.Add("HSS8X3X.313");
            names.Add("HSS8X3X.250");
            names.Add("HSS8X3X.188");
            names.Add("HSS8X3X.125");
            names.Add("HSS8X2X.375");
            names.Add("HSS8X2X.313");
            names.Add("HSS8X2X.250");
            names.Add("HSS8X2X.188");
            names.Add("HSS8X2X.125");
            names.Add("HSS7X7X.625");
            names.Add("HSS7X7X.500");
            names.Add("HSS7X7X.375");
            names.Add("HSS7X7X.313");
            names.Add("HSS7X7X.250");
            names.Add("HSS7X7X.188");
            names.Add("HSS7X7X.125");
            names.Add("HSS7X5X.500");
            names.Add("HSS7X5X.375");
            names.Add("HSS7X5X.313");
            names.Add("HSS7X5X.250");
            names.Add("HSS7X5X.188");
            names.Add("HSS7X5X.125");
            names.Add("HSS7X4X.500");
            names.Add("HSS7X4X.375");
            names.Add("HSS7X4X.313");
            names.Add("HSS7X4X.250");
            names.Add("HSS7X4X.188");
            names.Add("HSS7X4X.125");
            names.Add("HSS7X3X.500");
            names.Add("HSS7X3X.375");
            names.Add("HSS7X3X.313");
            names.Add("HSS7X3X.250");
            names.Add("HSS7X3X.188");
            names.Add("HSS7X3X.125");
            names.Add("HSS7X2X.250");
            names.Add("HSS7X2X.188");
            names.Add("HSS7X2X.125");
            names.Add("HSS6X6X.625");
            names.Add("HSS6X6X.500");
            names.Add("HSS6X6X.375");
            names.Add("HSS6X6X.313");
            names.Add("HSS6X6X.250");
            names.Add("HSS6X6X.188");
            names.Add("HSS6X6X.125");
            names.Add("HSS6X5X.500");
            names.Add("HSS6X5X.375");
            names.Add("HSS6X5X.313");
            names.Add("HSS6X5X.250");
            names.Add("HSS6X5X.188");
            names.Add("HSS6X5X.125");
            names.Add("HSS6X4X.500");
            names.Add("HSS6X4X.375");
            names.Add("HSS6X4X.313");
            names.Add("HSS6X4X.250");
            names.Add("HSS6X4X.188");
            names.Add("HSS6X4X.125");
            names.Add("HSS6X3X.500");
            names.Add("HSS6X3X.375");
            names.Add("HSS6X3X.313");
            names.Add("HSS6X3X.250");
            names.Add("HSS6X3X.188");
            names.Add("HSS6X3X.125");
            names.Add("HSS6X2X.375");
            names.Add("HSS6X2X.313");
            names.Add("HSS6X2X.250");
            names.Add("HSS6X2X.188");
            names.Add("HSS6X2X.125");
            names.Add("HSS5-1/2X5-1/2X.375");
            names.Add("HSS5-1/2X5-1/2X.313");
            names.Add("HSS5-1/2X5-1/2X.250");
            names.Add("HSS5-1/2X5-1/2X.188");
            names.Add("HSS5-1/2X5-1/2X.125");
            names.Add("HSS5X5X.500");
            names.Add("HSS5X5X.375");
            names.Add("HSS5X5X.313");
            names.Add("HSS5X5X.250");
            names.Add("HSS5X5X.188");
            names.Add("HSS5X5X.125");
            names.Add("HSS5X4X.500");
            names.Add("HSS5X4X.375");
            names.Add("HSS5X4X.313");
            names.Add("HSS5X4X.250");
            names.Add("HSS5X4X.188");
            names.Add("HSS5X4X.125");
            names.Add("HSS5X3X.500");
            names.Add("HSS5X3X.375");
            names.Add("HSS5X3X.313");
            names.Add("HSS5X3X.250");
            names.Add("HSS5X3X.188");
            names.Add("HSS5X3X.125");
            names.Add("HSS5X2-1/2X.250");
            names.Add("HSS5X2-1/2X.188");
            names.Add("HSS5X2-1/2X.125");
            names.Add("HSS5X2X.375");
            names.Add("HSS5X2X.313");
            names.Add("HSS5X2X.250");
            names.Add("HSS5X2X.188");
            names.Add("HSS5X2X.125");
            names.Add("HSS4-1/2X4-1/2X.500");
            names.Add("HSS4-1/2X4-1/2X.375");
            names.Add("HSS4-1/2X4-1/2X.313");
            names.Add("HSS4-1/2X4-1/2X.250");
            names.Add("HSS4-1/2X4-1/2X.188");
            names.Add("HSS4-1/2X4-1/2X.125");
            names.Add("HSS4X4X.500");
            names.Add("HSS4X4X.375");
            names.Add("HSS4X4X.313");
            names.Add("HSS4X4X.250");
            names.Add("HSS4X4X.188");
            names.Add("HSS4X4X.125");
            names.Add("HSS4X3X.375");
            names.Add("HSS4X3X.313");
            names.Add("HSS4X3X.250");
            names.Add("HSS4X3X.188");
            names.Add("HSS4X3X.125");
            names.Add("HSS4X2-1/2X.375");
            names.Add("HSS4X2-1/2X.313");
            names.Add("HSS4X2-1/2X.250");
            names.Add("HSS4X2-1/2X.188");
            names.Add("HSS4X2-1/2X.125");
            names.Add("HSS4X2X.375");
            names.Add("HSS4X2X.313");
            names.Add("HSS4X2X.250");
            names.Add("HSS4X2X.188");
            names.Add("HSS4X2X.125");
            names.Add("HSS3-1/2X3-1/2X.375");
            names.Add("HSS3-1/2X3-1/2X.313");
            names.Add("HSS3-1/2X3-1/2X.250");
            names.Add("HSS3-1/2X3-1/2X.188");
            names.Add("HSS3-1/2X3-1/2X.125");
            names.Add("HSS3-1/2X2-1/2X.375");
            names.Add("HSS3-1/2X2-1/2X.313");
            names.Add("HSS3-1/2X2-1/2X.250");
            names.Add("HSS3-1/2X2-1/2X.188");
            names.Add("HSS3-1/2X2-1/2X.125");
            names.Add("HSS3-1/2X2X.250");
            names.Add("HSS3-1/2X2X.188");
            names.Add("HSS3-1/2X2X.125");
            names.Add("HSS3-1/2X1-1/2X.250");
            names.Add("HSS3-1/2X1-1/2X.188");
            names.Add("HSS3-1/2X1-1/2X.125");
            names.Add("HSS3X3X.375");
            names.Add("HSS3X3X.313");
            names.Add("HSS3X3X.250");
            names.Add("HSS3X3X.188");
            names.Add("HSS3X3X.125");
            names.Add("HSS3X2-1/2X.313");
            names.Add("HSS3X2-1/2X.250");
            names.Add("HSS3X2-1/2X.188");
            names.Add("HSS3X2-1/2X.125");
            names.Add("HSS3X2X.313");
            names.Add("HSS3X2X.250");
            names.Add("HSS3X2X.188");
            names.Add("HSS3X2X.125");
            names.Add("HSS3X1-1/2X.250");
            names.Add("HSS3X1-1/2X.188");
            names.Add("HSS3X1-1/2X.125");
            names.Add("HSS3X1X.188");
            names.Add("HSS3X1X.125");
            names.Add("HSS2-1/2X2-1/2X.313");
            names.Add("HSS2-1/2X2-1/2X.250");
            names.Add("HSS2-1/2X2-1/2X.188");
            names.Add("HSS2-1/2X2-1/2X.125");
            names.Add("HSS2-1/2X2X.250");
            names.Add("HSS2-1/2X2X.188");
            names.Add("HSS2-1/2X2X.125");
            names.Add("HSS2-1/2X1-1/2X.250");
            names.Add("HSS2-1/2X1-1/2X.188");
            names.Add("HSS2-1/2X1-1/2X.125");
            names.Add("HSS2-1/2X1X.188");
            names.Add("HSS2-1/2X1X.125");
            names.Add("HSS2-1/4X2-1/4X.250");
            names.Add("HSS2-1/4X2-1/4X.188");
            names.Add("HSS2-1/4X2-1/4X.125");
            names.Add("HSS2-1/4X2X.188");
            names.Add("HSS2-1/4X2X.125");
            names.Add("HSS2X2X.250");
            names.Add("HSS2X2X.188");
            names.Add("HSS2X2X.125");
            names.Add("HSS2X1-1/2X.188");
            names.Add("HSS2X1-1/2X.125");
            names.Add("HSS2X1X.188");
            names.Add("HSS2X1X.125");



            return names;
        }

        public void GetHSSRoundDimensions(string sName, out double od, out double th, out double a, out double i, out double s, out double r, out double z, out double j, out double c, out double nw)
        {
            od = 0.0; th = 0.0; a = 0.0; i = 0.0; s = 0.0; r = 0.0; z = 0.0; j = 0.0; c = 0.0; nw = 0.0;

            if (sName == "HSS20X.500")
            {
                od = 20; th = 0.465; a = 28.5; i = 1360; s = 136; r = 6.91; z = 177; j = 2720; c = 272; nw = 104;
            }
            else if (sName == "HSS20X.375")
            {
                od = 20; th = 0.349; a = 21.5; i = 1040; s = 104; r = 6.95; z = 135; j = 2080; c = 208; nw = 78.67;
            }
            else if (sName == "HSS18X.500")
            {
                od = 18; th = 0.465; a = 25.6; i = 985; s = 109; r = 6.2; z = 143; j = 1970; c = 219; nw = 93.54;
            }
            else if (sName == "HSS18X.375")
            {
                od = 18; th = 0.349; a = 19.4; i = 754; s = 83.8; r = 6.24; z = 109; j = 1510; c = 168; nw = 70.66;
            }
            else if (sName == "HSS16X.625")
            {
                od = 16; th = 0.581; a = 28.1; i = 838; s = 105; r = 5.46; z = 138; j = 1680; c = 209; nw = 103;
            }
            else if (sName == "HSS16X.500")
            {
                od = 16; th = 0.465; a = 22.7; i = 685; s = 85.7; r = 5.49; z = 112; j = 1370; c = 171; nw = 82.85;
            }
            else if (sName == "HSS16X.438")
            {
                od = 16; th = 0.407; a = 19.9; i = 606; s = 75.8; r = 5.51; z = 99; j = 1210; c = 152; nw = 72.87;
            }
            else if (sName == "HSS16X.375")
            {
                od = 16; th = 0.349; a = 17.2; i = 526; s = 65.7; r = 5.53; z = 85.5; j = 1050; c = 131; nw = 62.64;
            }
            else if (sName == "HSS16X.312")
            {
                od = 16; th = 0.291; a = 14.4; i = 443; s = 55.4; r = 5.55; z = 71.8; j = 886; c = 111; nw = 52.32;
            }
            else if (sName == "HSS16X.250")
            {
                od = 16; th = 0.233; a = 11.5; i = 359; s = 44.8; r = 5.58; z = 57.9; j = 717; c = 89.7; nw = 42.09;
            }
            else if (sName == "HSS14X.625")
            {
                od = 14; th = 0.581; a = 24.5; i = 552; s = 78.9; r = 4.75; z = 105; j = 1100; c = 158; nw = 89.36;
            }
            else if (sName == "HSS14X.500")
            {
                od = 14; th = 0.465; a = 19.8; i = 453; s = 64.8; r = 4.79; z = 85.2; j = 907; c = 130; nw = 72.16;
            }
            else if (sName == "HSS14X.375")
            {
                od = 14; th = 0.349; a = 15; i = 349; s = 49.8; r = 4.83; z = 65.1; j = 698; c = 100; nw = 54.62;
            }
            else if (sName == "HSS14X.312")
            {
                od = 14; th = 0.291; a = 12.5; i = 295; s = 42.1; r = 4.85; z = 54.7; j = 589; c = 84.2; nw = 45.65;
            }
            else if (sName == "HSS14X.250")
            {
                od = 14; th = 0.233; a = 10.1; i = 239; s = 34.1; r = 4.87; z = 44.2; j = 478; c = 68.2; nw = 36.75;
            }
            else if (sName == "HSS12.75X.500")
            {
                od = 12.75; th = 0.465; a = 17.9; i = 339; s = 53.2; r = 4.35; z = 70.2; j = 678; c = 106; nw = 65.48;
            }
            else if (sName == "HSS12.75X.375")
            {
                od = 12.75; th = 0.349; a = 13.6; i = 262; s = 41; r = 4.39; z = 53.7; j = 523; c = 82.1; nw = 49.61;
            }
            else if (sName == "HSS12.75X.250")
            {
                od = 12.75; th = 0.233; a = 9.16; i = 180; s = 28.2; r = 4.43; z = 36.5; j = 359; c = 56.3; nw = 33.41;
            }
            else if (sName == "HSS10.75X.500")
            {
                od = 10.75; th = 0.465; a = 15; i = 199; s = 37; r = 3.64; z = 49.2; j = 398; c = 74.1; nw = 54.79;
            }
            else if (sName == "HSS10.75X.375")
            {
                od = 10.75; th = 0.349; a = 11.4; i = 154; s = 28.7; r = 3.68; z = 37.8; j = 309; c = 57.4; nw = 41.59;
            }
            else if (sName == "HSS10.75X.250")
            {
                od = 10.75; th = 0.233; a = 7.7; i = 106; s = 19.8; r = 3.72; z = 25.8; j = 213; c = 39.6; nw = 28.06;
            }
            else if (sName == "HSS10X.625")
            {
                od = 10; th = 0.581; a = 17.2; i = 191; s = 38.3; r = 3.34; z = 51.6; j = 383; c = 76.6; nw = 62.64;
            }
            else if (sName == "HSS10X.500")
            {
                od = 10; th = 0.465; a = 13.9; i = 159; s = 31.7; r = 3.38; z = 42.3; j = 317; c = 63.5; nw = 50.78;
            }
            else if (sName == "HSS10X.375")
            {
                od = 10; th = 0.349; a = 10.6; i = 123; s = 24.7; r = 3.41; z = 32.5; j = 247; c = 49.3; nw = 38.58;
            }
            else if (sName == "HSS10X.312")
            {
                od = 10; th = 0.291; a = 8.88; i = 105; s = 20.9; r = 3.43; z = 27.4; j = 209; c = 41.9; nw = 32.31;
            }
            else if (sName == "HSS10X.250")
            {
                od = 10; th = 0.233; a = 7.15; i = 85.3; s = 17.1; r = 3.45; z = 22.2; j = 171; c = 34.1; nw = 26.06;
            }
            else if (sName == "HSS10X.188")
            {
                od = 10; th = 0.174; a = 5.37; i = 64.8; s = 13; r = 3.47; z = 16.8; j = 130; c = 25.9; nw = 19.72;
            }
            else if (sName == "HSS9.625X.500")
            {
                od = 9.625; th = 0.465; a = 13.4; i = 141; s = 29.2; r = 3.24; z = 39; j = 281; c = 58.5; nw = 48.77;
            }
            else if (sName == "HSS9.625X.375")
            {
                od = 9.625; th = 0.349; a = 10.2; i = 110; s = 22.8; r = 3.28; z = 30; j = 219; c = 45.5; nw = 37.08;
            }
            else if (sName == "HSS9.625X.312")
            {
                od = 9.625; th = 0.291; a = 8.53; i = 93; s = 19.3; r = 3.3; z = 25.4; j = 186; c = 38.7; nw = 31.06;
            }
            else if (sName == "HSS9.625X.250")
            {
                od = 9.625; th = 0.233; a = 6.87; i = 75.9; s = 15.8; r = 3.32; z = 20.6; j = 152; c = 31.5; nw = 25.06;
            }
            else if (sName == "HSS9.625X.188")
            {
                od = 9.625; th = 0.174; a = 5.17; i = 57.7; s = 12; r = 3.34; z = 15.5; j = 115; c = 24; nw = 18.97;
            }
            else if (sName == "HSS8.625X.625")
            {
                od = 8.625; th = 0.581; a = 14.7; i = 119; s = 27.7; r = 2.85; z = 37.7; j = 239; c = 55.4; nw = 53.45;
            }
            else if (sName == "HSS8.625X.500")
            {
                od = 8.625; th = 0.465; a = 11.9; i = 100; s = 23.1; r = 2.89; z = 31; j = 199; c = 46.2; nw = 43.43;
            }
            else if (sName == "HSS8.625X.375")
            {
                od = 8.625; th = 0.349; a = 9.07; i = 77.8; s = 18; r = 2.93; z = 23.9; j = 156; c = 36.1; nw = 33.07;
            }
            else if (sName == "HSS8.625X.322")
            {
                od = 8.625; th = 0.3; a = 7.85; i = 68.1; s = 15.8; r = 2.95; z = 20.8; j = 136; c = 31.6; nw = 28.58;
            }
            else if (sName == "HSS8.625X.250")
            {
                od = 8.625; th = 0.233; a = 6.14; i = 54.1; s = 12.5; r = 2.97; z = 16.4; j = 108; c = 25.1; nw = 22.38;
            }
            else if (sName == "HSS8.625X.188")
            {
                od = 8.625; th = 0.174; a = 4.62; i = 41.3; s = 9.57; r = 2.99; z = 12.4; j = 82.5; c = 19.1; nw = 16.96;
            }
            else if (sName == "HSS7.625X.375")
            {
                od = 7.625; th = 0.349; a = 7.98; i = 52.9; s = 13.9; r = 2.58; z = 18.5; j = 106; c = 27.8; nw = 29.06;
            }
            else if (sName == "HSS7.625X.328")
            {
                od = 7.625; th = 0.305; a = 7.01; i = 47.1; s = 12.3; r = 2.59; z = 16.4; j = 94.1; c = 24.7; nw = 25.59;
            }
            else if (sName == "HSS7.5X.500")
            {
                od = 7.5; th = 0.465; a = 10.3; i = 63.9; s = 17; r = 2.49; z = 23; j = 128; c = 34.1; nw = 37.42;
            }
            else if (sName == "HSS7.5X.375")
            {
                od = 7.5; th = 0.349; a = 7.84; i = 50.2; s = 13.4; r = 2.53; z = 17.9; j = 100; c = 26.8; nw = 28.56;
            }
            else if (sName == "HSS7.5X.312")
            {
                od = 7.5; th = 0.291; a = 6.59; i = 42.9; s = 11.4; r = 2.55; z = 15.1; j = 85.8; c = 22.9; nw = 23.97;
            }
            else if (sName == "HSS7.5X.250")
            {
                od = 7.5; th = 0.233; a = 5.32; i = 35.2; s = 9.37; r = 2.57; z = 12.3; j = 70.3; c = 18.7; nw = 19.38;
            }
            else if (sName == "HSS7.5X.188")
            {
                od = 7.5; th = 0.174; a = 4; i = 26.9; s = 7.17; r = 2.59; z = 9.34; j = 53.8; c = 14.3; nw = 14.7;
            }
            else if (sName == "HSS7X.500")
            {
                od = 7; th = 0.465; a = 9.55; i = 51.2; s = 14.6; r = 2.32; z = 19.9; j = 102; c = 29.3; nw = 34.74;
            }
            else if (sName == "HSS7X.375")
            {
                od = 7; th = 0.349; a = 7.29; i = 40.4; s = 11.6; r = 2.35; z = 15.5; j = 80.9; c = 23.1; nw = 26.56;
            }
            else if (sName == "HSS7X.312")
            {
                od = 7; th = 0.291; a = 6.13; i = 34.6; s = 9.88; r = 2.37; z = 13.1; j = 69.1; c = 19.8; nw = 22.31;
            }
            else if (sName == "HSS7X.250")
            {
                od = 7; th = 0.233; a = 4.95; i = 28.4; s = 8.11; r = 2.39; z = 10.7; j = 56.8; c = 16.2; nw = 18.04;
            }
            else if (sName == "HSS7X.188")
            {
                od = 7; th = 0.174; a = 3.73; i = 21.7; s = 6.21; r = 2.41; z = 8.11; j = 43.5; c = 12.4; nw = 13.69;
            }
            else if (sName == "HSS7X.125")
            {
                od = 7; th = 0.116; a = 2.51; i = 14.9; s = 4.25; r = 2.43; z = 5.5; j = 29.7; c = 8.49; nw = 9.19;
            }
            else if (sName == "HSS6.875X.500")
            {
                od = 6.875; th = 0.465; a = 9.36; i = 48.3; s = 14.1; r = 2.27; z = 19.1; j = 96.7; c = 28.1; nw = 34.07;
            }
            else if (sName == "HSS6.875X.375")
            {
                od = 6.875; th = 0.349; a = 7.16; i = 38.2; s = 11.1; r = 2.31; z = 14.9; j = 76.4; c = 22.2; nw = 26.06;
            }
            else if (sName == "HSS6.875X.312")
            {
                od = 6.875; th = 0.291; a = 6.02; i = 32.7; s = 9.51; r = 2.33; z = 12.6; j = 65.4; c = 19; nw = 21.89;
            }
            else if (sName == "HSS6.875X.250")
            {
                od = 6.875; th = 0.233; a = 4.86; i = 26.8; s = 7.81; r = 2.35; z = 10.3; j = 53.7; c = 15.6; nw = 17.71;
            }
            else if (sName == "HSS6.875X.188")
            {
                od = 6.875; th = 0.174; a = 3.66; i = 20.6; s = 5.99; r = 2.37; z = 7.81; j = 41.1; c = 12; nw = 13.44;
            }
            else if (sName == "HSS6.625X.500")
            {
                od = 6.625; th = 0.465; a = 9; i = 42.9; s = 13; r = 2.18; z = 17.7; j = 85.9; c = 25.9; nw = 32.74;
            }
            else if (sName == "HSS6.625X.432")
            {
                od = 6.625; th = 0.402; a = 7.86; i = 38.2; s = 11.5; r = 2.2; z = 15.6; j = 76.4; c = 23.1; nw = 28.6;
            }
            else if (sName == "HSS6.625X.375")
            {
                od = 6.625; th = 0.349; a = 6.88; i = 34; s = 10.3; r = 2.22; z = 13.8; j = 68; c = 20.5; nw = 25.06;
            }
            else if (sName == "HSS6.625X.312")
            {
                od = 6.625; th = 0.291; a = 5.79; i = 29.1; s = 8.79; r = 2.24; z = 11.7; j = 58.2; c = 17.6; nw = 21.06;
            }
            else if (sName == "HSS6.625X.280")
            {
                od = 6.625; th = 0.26; a = 5.2; i = 26.4; s = 7.96; r = 2.25; z = 10.5; j = 52.7; c = 15.9; nw = 18.99;
            }
            else if (sName == "HSS6.625X.250")
            {
                od = 6.625; th = 0.233; a = 4.68; i = 23.9; s = 7.22; r = 2.26; z = 9.52; j = 47.9; c = 14.4; nw = 17.04;
            }
            else if (sName == "HSS6.625X.188")
            {
                od = 6.625; th = 0.174; a = 3.53; i = 18.4; s = 5.54; r = 2.28; z = 7.24; j = 36.7; c = 11.1; nw = 12.94;
            }
            else if (sName == "HSS6.625X.125")
            {
                od = 6.625; th = 0.116; a = 2.37; i = 12.6; s = 3.79; r = 2.3; z = 4.92; j = 25.1; c = 7.59; nw = 8.69;
            }
            else if (sName == "HSS6X.500")
            {
                od = 6; th = 0.465; a = 8.09; i = 31.2; s = 10.4; r = 1.96; z = 14.3; j = 62.4; c = 20.8; nw = 29.4;
            }
            else if (sName == "HSS6X.375")
            {
                od = 6; th = 0.349; a = 6.2; i = 24.8; s = 8.28; r = 2; z = 11.2; j = 49.7; c = 16.6; nw = 22.55;
            }
            else if (sName == "HSS6X.312")
            {
                od = 6; th = 0.291; a = 5.22; i = 21.3; s = 7.11; r = 2.02; z = 9.49; j = 42.6; c = 14.2; nw = 18.97;
            }
            else if (sName == "HSS6X.280")
            {
                od = 6; th = 0.26; a = 4.69; i = 19.3; s = 6.45; r = 2.03; z = 8.57; j = 38.7; c = 12.9; nw = 17.12;
            }
            else if (sName == "HSS6X.250")
            {
                od = 6; th = 0.233; a = 4.22; i = 17.6; s = 5.86; r = 2.04; z = 7.75; j = 35.2; c = 11.7; nw = 15.37;
            }
            else if (sName == "HSS6X.188")
            {
                od = 6; th = 0.174; a = 3.18; i = 13.5; s = 4.51; r = 2.06; z = 5.91; j = 27; c = 9.02; nw = 11.68;
            }
            else if (sName == "HSS6X.125")
            {
                od = 6; th = 0.116; a = 2.14; i = 9.28; s = 3.09; r = 2.08; z = 4.02; j = 18.6; c = 6.19; nw = 7.85;
            }
            else if (sName == "HSS5.563X.500")
            {
                od = 5.563; th = 0.465; a = 7.45; i = 24.4; s = 8.77; r = 1.81; z = 12.1; j = 48.8; c = 17.5; nw = 27.06;
            }
            else if (sName == "HSS5.563X.375")
            {
                od = 5.563; th = 0.349; a = 5.72; i = 19.5; s = 7.02; r = 1.85; z = 9.5; j = 39; c = 14; nw = 20.8;
            }
            else if (sName == "HSS5.563X.258")
            {
                od = 5.563; th = 0.24; a = 4.01; i = 14.2; s = 5.12; r = 1.88; z = 6.8; j = 28.5; c = 10.2; nw = 14.63;
            }
            else if (sName == "HSS5.563X.188")
            {
                od = 5.563; th = 0.174; a = 2.95; i = 10.7; s = 3.85; r = 1.91; z = 5.05; j = 21.4; c = 7.7; nw = 10.8;
            }
            else if (sName == "HSS5.563X.134")
            {
                od = 5.563; th = 0.124; a = 2.12; i = 7.84; s = 2.82; r = 1.92; z = 3.67; j = 15.7; c = 5.64; nw = 7.78;
            }
            else if (sName == "HSS5.5X.500")
            {
                od = 5.5; th = 0.465; a = 7.36; i = 23.5; s = 8.55; r = 1.79; z = 11.8; j = 47; c = 17.1; nw = 26.73;
            }
            else if (sName == "HSS5.5X.375")
            {
                od = 5.5; th = 0.349; a = 5.65; i = 18.8; s = 6.84; r = 1.83; z = 9.27; j = 37.6; c = 13.7; nw = 20.55;
            }
            else if (sName == "HSS5.5X.258")
            {
                od = 5.5; th = 0.24; a = 3.97; i = 13.7; s = 5; r = 1.86; z = 6.64; j = 27.5; c = 10; nw = 14.46;
            }
            else if (sName == "HSS5X.500")
            {
                od = 5; th = 0.465; a = 6.62; i = 17.2; s = 6.88; r = 1.61; z = 9.6; j = 34.4; c = 13.8; nw = 24.05;
            }
            else if (sName == "HSS5X.375")
            {
                od = 5; th = 0.349; a = 5.1; i = 13.9; s = 5.55; r = 1.65; z = 7.56; j = 27.7; c = 11.1; nw = 18.54;
            }
            else if (sName == "HSS5X.312")
            {
                od = 5; th = 0.291; a = 4.3; i = 12; s = 4.79; r = 1.67; z = 6.46; j = 24; c = 9.58; nw = 15.64;
            }
            else if (sName == "HSS5X.258")
            {
                od = 5; th = 0.24; a = 3.59; i = 10.2; s = 4.08; r = 1.69; z = 5.44; j = 20.4; c = 8.15; nw = 13.08;
            }
            else if (sName == "HSS5X.250")
            {
                od = 5; th = 0.233; a = 3.49; i = 9.94; s = 3.97; r = 1.69; z = 5.3; j = 19.9; c = 7.95; nw = 12.69;
            }
            else if (sName == "HSS5X.188")
            {
                od = 5; th = 0.174; a = 2.64; i = 7.69; s = 3.08; r = 1.71; z = 4.05; j = 15.4; c = 6.15; nw = 9.67;
            }
            else if (sName == "HSS5X.125")
            {
                od = 5; th = 0.116; a = 1.78; i = 5.31; s = 2.12; r = 1.73; z = 2.77; j = 10.6; c = 4.25; nw = 6.51;
            }
            else if (sName == "HSS4.5X.375")
            {
                od = 4.5; th = 0.349; a = 4.55; i = 9.87; s = 4.39; r = 1.47; z = 6.03; j = 19.7; c = 8.78; nw = 16.54;
            }
            else if (sName == "HSS4.5X.337")
            {
                od = 4.5; th = 0.313; a = 4.12; i = 9.07; s = 4.03; r = 1.48; z = 5.5; j = 18.1; c = 8.06; nw = 15;
            }
            else if (sName == "HSS4.5X.237")
            {
                od = 4.5; th = 0.22; a = 2.96; i = 6.79; s = 3.02; r = 1.52; z = 4.03; j = 13.6; c = 6.04; nw = 10.8;
            }
            else if (sName == "HSS4.5X.188")
            {
                od = 4.5; th = 0.174; a = 2.36; i = 5.54; s = 2.46; r = 1.53; z = 3.26; j = 11.1; c = 4.93; nw = 8.67;
            }
            else if (sName == "HSS4.5X.125")
            {
                od = 4.5; th = 0.116; a = 1.6; i = 3.84; s = 1.71; r = 1.55; z = 2.23; j = 7.68; c = 3.41; nw = 5.85;
            }
            else if (sName == "HSS4X.313")
            {
                od = 4; th = 0.291; a = 3.39; i = 5.87; s = 2.93; r = 1.32; z = 4.01; j = 11.7; c = 5.87; nw = 12.34;
            }
            else if (sName == "HSS4X.250")
            {
                od = 4; th = 0.233; a = 2.76; i = 4.91; s = 2.45; r = 1.33; z = 3.31; j = 9.82; c = 4.91; nw = 10;
            }
            else if (sName == "HSS4X.237")
            {
                od = 4; th = 0.22; a = 2.61; i = 4.68; s = 2.34; r = 1.34; z = 3.15; j = 9.36; c = 4.68; nw = 9.53;
            }
            else if (sName == "HSS4X.226")
            {
                od = 4; th = 0.21; a = 2.5; i = 4.5; s = 2.25; r = 1.34; z = 3.02; j = 9.01; c = 4.5; nw = 9.12;
            }
            else if (sName == "HSS4X.220")
            {
                od = 4; th = 0.205; a = 2.44; i = 4.41; s = 2.21; r = 1.34; z = 2.96; j = 8.83; c = 4.41; nw = 8.89;
            }
            else if (sName == "HSS4X.188")
            {
                od = 4; th = 0.174; a = 2.09; i = 3.83; s = 1.92; r = 1.35; z = 2.55; j = 7.67; c = 3.83; nw = 7.66;
            }
            else if (sName == "HSS4X.125")
            {
                od = 4; th = 0.116; a = 1.42; i = 2.67; s = 1.34; r = 1.37; z = 1.75; j = 5.34; c = 2.67; nw = 5.18;
            }
            else if (sName == "HSS3.5X.313")
            {
                od = 3.5; th = 0.291; a = 2.93; i = 3.81; s = 2.18; r = 1.14; z = 3; j = 7.61; c = 4.35; nw = 10.66;
            }
            else if (sName == "HSS3.5X.300")
            {
                od = 3.5; th = 0.279; a = 2.82; i = 3.69; s = 2.11; r = 1.14; z = 2.9; j = 7.38; c = 4.22; nw = 10.26;
            }
            else if (sName == "HSS3.5X.250")
            {
                od = 3.5; th = 0.233; a = 2.39; i = 3.21; s = 1.83; r = 1.16; z = 2.49; j = 6.41; c = 3.66; nw = 8.69;
            }
            else if (sName == "HSS3.5X.216")
            {
                od = 3.5; th = 0.201; a = 2.08; i = 2.84; s = 1.63; r = 1.17; z = 2.19; j = 5.69; c = 3.25; nw = 7.58;
            }
            else if (sName == "HSS3.5X.203")
            {
                od = 3.5; th = 0.189; a = 1.97; i = 2.7; s = 1.54; r = 1.17; z = 2.07; j = 5.41; c = 3.09; nw = 7.15;
            }
            else if (sName == "HSS3.5X.188")
            {
                od = 3.5; th = 0.174; a = 1.82; i = 2.52; s = 1.44; r = 1.18; z = 1.93; j = 5.04; c = 2.88; nw = 6.66;
            }
            else if (sName == "HSS3.5X.125")
            {
                od = 3.5; th = 0.116; a = 1.23; i = 1.77; s = 1.01; r = 1.2; z = 1.33; j = 3.53; c = 2.02; nw = 4.51;
            }
            else if (sName == "HSS3X.250")
            {
                od = 3; th = 0.233; a = 2.03; i = 1.95; s = 1.3; r = 0.982; z = 1.79; j = 3.9; c = 2.6; nw = 7.35;
            }
            else if (sName == "HSS3X.216")
            {
                od = 3; th = 0.201; a = 1.77; i = 1.74; s = 1.16; r = 0.992; z = 1.58; j = 3.48; c = 2.32; nw = 6.43;
            }
            else if (sName == "HSS3X.203")
            {
                od = 3; th = 0.189; a = 1.67; i = 1.66; s = 1.1; r = 0.996; z = 1.5; j = 3.31; c = 2.21; nw = 6.07;
            }
            else if (sName == "HSS3X.188")
            {
                od = 3; th = 0.174; a = 1.54; i = 1.55; s = 1.03; r = 1; z = 1.39; j = 3.1; c = 2.06; nw = 5.65;
            }
            else if (sName == "HSS3X.152")
            {
                od = 3; th = 0.141; a = 1.27; i = 1.3; s = 0.865; r = 1.01; z = 1.15; j = 2.59; c = 1.73; nw = 4.63;
            }
            else if (sName == "HSS3X.134")
            {
                od = 3; th = 0.124; a = 1.12; i = 1.16; s = 0.774; r = 1.02; z = 1.03; j = 2.32; c = 1.55; nw = 4.11;
            }
            else if (sName == "HSS3X.125")
            {
                od = 3; th = 0.116; a = 1.05; i = 1.09; s = 0.73; r = 1.02; z = 0.965; j = 2.19; c = 1.46; nw = 3.84;
            }
            else if (sName == "HSS2.875X.250")
            {
                od = 2.875; th = 0.233; a = 1.93; i = 1.7; s = 1.18; r = 0.938; z = 1.63; j = 3.4; c = 2.37; nw = 7.02;
            }
            else if (sName == "HSS2.875X.203")
            {
                od = 2.875; th = 0.189; a = 1.59; i = 1.45; s = 1.01; r = 0.952; z = 1.37; j = 2.89; c = 2.01; nw = 5.8;
            }
            else if (sName == "HSS2.875X.188")
            {
                od = 2.875; th = 0.174; a = 1.48; i = 1.35; s = 0.941; r = 0.957; z = 1.27; j = 2.7; c = 1.88; nw = 5.4;
            }
            else if (sName == "HSS2.875X.125")
            {
                od = 2.875; th = 0.116; a = 1.01; i = 0.958; s = 0.667; r = 0.976; z = 0.884; j = 1.92; c = 1.33; nw = 3.67;
            }
            else if (sName == "HSS2.5X.250")
            {
                od = 2.5; th = 0.233; a = 1.66; i = 1.08; s = 0.862; r = 0.806; z = 1.2; j = 2.15; c = 1.72; nw = 6.01;
            }
            else if (sName == "HSS2.5X.188")
            {
                od = 2.5; th = 0.174; a = 1.27; i = 0.865; s = 0.692; r = 0.825; z = 0.943; j = 1.73; c = 1.38; nw = 4.65;
            }
            else if (sName == "HSS2.5X.125")
            {
                od = 2.5; th = 0.116; a = 0.869; i = 0.619; s = 0.495; r = 0.844; z = 0.66; j = 1.24; c = 0.99; nw = 3.17;
            }
            else if (sName == "HSS2.375X.250")
            {
                od = 2.375; th = 0.233; a = 1.57; i = 0.91; s = 0.766; r = 0.762; z = 1.07; j = 1.82; c = 1.53; nw = 5.68;
            }
            else if (sName == "HSS2.375X.218")
            {
                od = 2.375; th = 0.203; a = 1.39; i = 0.824; s = 0.694; r = 0.771; z = 0.96; j = 1.65; c = 1.39; nw = 5.03;
            }
            else if (sName == "HSS2.375X.188")
            {
                od = 2.375; th = 0.174; a = 1.2; i = 0.733; s = 0.617; r = 0.781; z = 0.845; j = 1.47; c = 1.23; nw = 4.4;
            }
            else if (sName == "HSS2.375X.154")
            {
                od = 2.375; th = 0.143; a = 1; i = 0.627; s = 0.528; r = 0.791; z = 0.713; j = 1.25; c = 1.06; nw = 3.66;
            }
            else if (sName == "HSS2.375X.125")
            {
                od = 2.375; th = 0.116; a = 0.823; i = 0.527; s = 0.443; r = 0.8; z = 0.592; j = 1.05; c = 0.887; nw = 3.01;
            }
            else if (sName == "HSS1.9X.188")
            {
                od = 1.9; th = 0.174; a = 0.943; i = 0.355; s = 0.374; r = 0.613; z = 0.52; j = 0.71; c = 0.747; nw = 3.44;
            }
            else if (sName == "HSS1.9X.145")
            {
                od = 1.9; th = 0.135; a = 0.749; i = 0.293; s = 0.309; r = 0.626; z = 0.421; j = 0.586; c = 0.617; nw = 2.72;
            }
            else if (sName == "HSS1.9X.120")
            {
                od = 1.9; th = 0.111; a = 0.624; i = 0.251; s = 0.264; r = 0.634; z = 0.356; j = 0.501; c = 0.527; nw = 2.28;
            }
            else if (sName == "HSS1.66X.140")
            {
                od = 1.66; th = 0.13; a = 0.625; i = 0.184; s = 0.222; r = 0.543; z = 0.305; j = 0.368; c = 0.444; nw = 2.27;
            }
        }

        public static List<string> GetHSSRoundNames()
        {
            List<string> names = new List<string>();

            names.Add("HSS20X.500");
            names.Add("HSS20X.375");
            names.Add("HSS18X.500");
            names.Add("HSS18X.375");
            names.Add("HSS16X.625");
            names.Add("HSS16X.500");
            names.Add("HSS16X.438");
            names.Add("HSS16X.375");
            names.Add("HSS16X.312");
            names.Add("HSS16X.250");
            names.Add("HSS14X.625");
            names.Add("HSS14X.500");
            names.Add("HSS14X.375");
            names.Add("HSS14X.312");
            names.Add("HSS14X.250");
            names.Add("HSS12.75X.500");
            names.Add("HSS12.75X.375");
            names.Add("HSS12.75X.250");
            names.Add("HSS10.75X.500");
            names.Add("HSS10.75X.375");
            names.Add("HSS10.75X.250");
            names.Add("HSS10X.625");
            names.Add("HSS10X.500");
            names.Add("HSS10X.375");
            names.Add("HSS10X.312");
            names.Add("HSS10X.250");
            names.Add("HSS10X.188");
            names.Add("HSS9.625X.500");
            names.Add("HSS9.625X.375");
            names.Add("HSS9.625X.312");
            names.Add("HSS9.625X.250");
            names.Add("HSS9.625X.188");
            names.Add("HSS8.625X.625");
            names.Add("HSS8.625X.500");
            names.Add("HSS8.625X.375");
            names.Add("HSS8.625X.322");
            names.Add("HSS8.625X.250");
            names.Add("HSS8.625X.188");
            names.Add("HSS7.625X.375");
            names.Add("HSS7.625X.328");
            names.Add("HSS7.5X.500");
            names.Add("HSS7.5X.375");
            names.Add("HSS7.5X.312");
            names.Add("HSS7.5X.250");
            names.Add("HSS7.5X.188");
            names.Add("HSS7X.500");
            names.Add("HSS7X.375");
            names.Add("HSS7X.312");
            names.Add("HSS7X.250");
            names.Add("HSS7X.188");
            names.Add("HSS7X.125");
            names.Add("HSS6.875X.500");
            names.Add("HSS6.875X.375");
            names.Add("HSS6.875X.312");
            names.Add("HSS6.875X.250");
            names.Add("HSS6.875X.188");
            names.Add("HSS6.625X.500");
            names.Add("HSS6.625X.432");
            names.Add("HSS6.625X.375");
            names.Add("HSS6.625X.312");
            names.Add("HSS6.625X.280");
            names.Add("HSS6.625X.250");
            names.Add("HSS6.625X.188");
            names.Add("HSS6.625X.125");
            names.Add("HSS6X.500");
            names.Add("HSS6X.375");
            names.Add("HSS6X.312");
            names.Add("HSS6X.280");
            names.Add("HSS6X.250");
            names.Add("HSS6X.188");
            names.Add("HSS6X.125");
            names.Add("HSS5.563X.500");
            names.Add("HSS5.563X.375");
            names.Add("HSS5.563X.258");
            names.Add("HSS5.563X.188");
            names.Add("HSS5.563X.134");
            names.Add("HSS5.5X.500");
            names.Add("HSS5.5X.375");
            names.Add("HSS5.5X.258");
            names.Add("HSS5X.500");
            names.Add("HSS5X.375");
            names.Add("HSS5X.312");
            names.Add("HSS5X.258");
            names.Add("HSS5X.250");
            names.Add("HSS5X.188");
            names.Add("HSS5X.125");
            names.Add("HSS4.5X.375");
            names.Add("HSS4.5X.337");
            names.Add("HSS4.5X.237");
            names.Add("HSS4.5X.188");
            names.Add("HSS4.5X.125");
            names.Add("HSS4X.313");
            names.Add("HSS4X.250");
            names.Add("HSS4X.237");
            names.Add("HSS4X.226");
            names.Add("HSS4X.220");
            names.Add("HSS4X.188");
            names.Add("HSS4X.125");
            names.Add("HSS3.5X.313");
            names.Add("HSS3.5X.300");
            names.Add("HSS3.5X.250");
            names.Add("HSS3.5X.216");
            names.Add("HSS3.5X.203");
            names.Add("HSS3.5X.188");
            names.Add("HSS3.5X.125");
            names.Add("HSS3X.250");
            names.Add("HSS3X.216");
            names.Add("HSS3X.203");
            names.Add("HSS3X.188");
            names.Add("HSS3X.152");
            names.Add("HSS3X.134");
            names.Add("HSS3X.125");
            names.Add("HSS2.875X.250");
            names.Add("HSS2.875X.203");
            names.Add("HSS2.875X.188");
            names.Add("HSS2.875X.125");
            names.Add("HSS2.5X.250");
            names.Add("HSS2.5X.188");
            names.Add("HSS2.5X.125");
            names.Add("HSS2.375X.250");
            names.Add("HSS2.375X.218");
            names.Add("HSS2.375X.188");
            names.Add("HSS2.375X.154");
            names.Add("HSS2.375X.125");
            names.Add("HSS1.9X.188");
            names.Add("HSS1.9X.145");
            names.Add("HSS1.9X.120");
            names.Add("HSS1.66X.140");

            return names;
        }

        public void GetWBeamDimensions(string sName, out double a, out double tw, out double d, out double tf, out double bf, out double nw, out double ix, out double sx, out double rx, out double zx, out double iy, out double sy, out double ry, out double zy, out double rts, out double ho, out double j, out double c)
        {
            a = 0.00; tw = 0.00; d = 0.00; tf = 0.00; bf = 0.00; nw = 0.00; ix = 0.00; sx = 0.00; rx = 0.00; zx = 0.00; iy = 0.00; sy = 0.00; ry = 0.00; zy = 0.00; rts = 0.00; ho = 0.00; j = 0.00; c = 0.00;
            if (sName == "W44X335")
            {
                nw = 335; a = 98.5; d = 44; bf = 15.9; tw = 1.03; tf = 1.77; ix = 31100; zx = 1620; sx = 1410; rx = 17.8; iy = 1200; zy = 236; sy = 150; ry = 3.49; j = 74.7;
            }
            else if (sName == "W44X290")
            {
                nw = 290; a = 85.4; d = 43.6; bf = 15.8; tw = 0.865; tf = 1.58; ix = 27000; zx = 1410; sx = 1240; rx = 17.8; iy = 1040; zy = 205; sy = 132; ry = 3.49; j = 50.9;
            }
            else if (sName == "W44X262")
            {
                nw = 262; a = 77.2; d = 43.3; bf = 15.8; tw = 0.785; tf = 1.42; ix = 24100; zx = 1270; sx = 1110; rx = 17.7; iy = 923; zy = 182; sy = 117; ry = 3.47; j = 37.3;
            }
            else if (sName == "W44X230")
            {
                nw = 230; a = 67.8; d = 42.9; bf = 15.8; tw = 0.71; tf = 1.22; ix = 20800; zx = 1100; sx = 971; rx = 17.5; iy = 796; zy = 157; sy = 101; ry = 3.43; j = 24.9;
            }
            else if (sName == "W40X593")
            {
                nw = 593; a = 174; d = 43; bf = 16.7; tw = 1.79; tf = 3.23; ix = 50400; zx = 2760; sx = 2340; rx = 17; iy = 2520; zy = 481; sy = 302; ry = 3.8; j = 445;
            }
            else if (sName == "W40X503")
            {
                nw = 503; a = 148; d = 42.1; bf = 16.4; tw = 1.54; tf = 2.76; ix = 41600; zx = 2320; sx = 1980; rx = 16.8; iy = 2040; zy = 394; sy = 249; ry = 3.72; j = 277;
            }
            else if (sName == "W40X431")
            {
                nw = 431; a = 127; d = 41.3; bf = 16.2; tw = 1.34; tf = 2.36; ix = 34800; zx = 1960; sx = 1690; rx = 16.6; iy = 1690; zy = 328; sy = 208; ry = 3.65;j = 177;
            }
            else if (sName == "W40X397")
            {
                nw = 397; a = 117; d = 41; bf = 16.1; tw = 1.22; tf = 2.2; ix = 32000; zx = 1800; sx = 1560; rx = 16.6; iy = 1540; zy = 300; sy = 191; ry = 3.64;j = 142;
            }
            else if (sName == "W40X372")
            {
                nw = 372; a = 110; d = 40.6; bf = 16.1; tw = 1.16; tf = 2.05; ix = 29600; zx = 1680; sx = 1460; rx = 16.5; iy = 1420; zy = 277; sy = 177; ry = 3.6;j = 116;
            }
            else if (sName == "W40X362")
            {
                nw = 362; a = 106; d = 40.6; bf = 16; tw = 1.12; tf = 2.01; ix = 28900; zx = 1640; sx = 1420; rx = 16.5; iy = 1380; zy = 270; sy = 173; ry = 3.6;j = 109;
            }
            else if (sName == "W40X324")
            {
                nw = 324; a = 95.3; d = 40.2; bf = 15.9; tw = 1; tf = 1.81; ix = 25600; zx = 1460; sx = 1280; rx = 16.4; iy = 1220; zy = 239; sy = 153; ry = 3.58;j = 79.4;
            }
            else if (sName == "W40X297")
            {
                nw = 297; a = 87.3; d = 39.8; bf = 15.8; tw = 0.93; tf = 1.65; ix = 23200; zx = 1330; sx = 1170; rx = 16.3; iy = 1090; zy = 215; sy = 138; ry = 3.54;j = 61.2;
            }
            else if (sName == "W40X277")
            {
                nw = 277; a = 81.5; d = 39.7; bf = 15.8; tw = 0.83; tf = 1.58; ix = 21900; zx = 1250; sx = 1100; rx = 16.4; iy = 1040; zy = 204; sy = 132; ry = 3.58;j = 51.5;
            }
            else if (sName == "W40X249")
            {
                nw = 249; a = 73.5; d = 39.4; bf = 15.8; tw = 0.75; tf = 1.42; ix = 19600; zx = 1120; sx = 993; rx = 16.3; iy = 926; zy = 182; sy = 118; ry = 3.55;j = 38.1;
            }
            else if (sName == "W40X215")
            {
                nw = 215; a = 63.5; d = 39; bf = 15.8; tw = 0.65; tf = 1.22; ix = 16700; zx = 964; sx = 859; rx = 16.2; iy = 803; zy = 156; sy = 101; ry = 3.54;j = 24.8;
            }
            else if (sName == "W40X199")
            {
                nw = 199; a = 58.8; d = 38.7; bf = 15.8; tw = 0.65; tf = 1.07; ix = 14900; zx = 869; sx = 770; rx = 16; iy = 695; zy = 137; sy = 88.2; ry = 3.45;j = 18.3;
            }
            else if (sName == "W40X392")
            {
                nw = 392; a = 116; d = 41.6; bf = 12.4; tw = 1.42; tf = 2.52; ix = 29900; zx = 1710; sx = 1440; rx = 16.1; iy = 803; zy = 212; sy = 130; ry = 2.64;j = 172;
            }
            else if (sName == "W40X331")
            {
                nw = 331; a = 97.7; d = 40.8; bf = 12.2; tw = 1.22; tf = 2.13; ix = 24700; zx = 1430; sx = 1210; rx = 15.9; iy = 644; zy = 172; sy = 106; ry = 2.57;j = 105;
            }
            else if (sName == "W40X327")
            {
                nw = 327; a = 95.9; d = 40.8; bf = 12.1; tw = 1.18; tf = 2.13; ix = 24500; zx = 1410; sx = 1200; rx = 16; iy = 640; zy = 170; sy = 105; ry = 2.58;j = 103;
            }
            else if (sName == "W40X294")
            {
                nw = 294; a = 86.2; d = 40.4; bf = 12; tw = 1.06; tf = 1.93; ix = 21900; zx = 1270; sx = 1080; rx = 15.9; iy = 562; zy = 150; sy = 93.5; ry = 2.55;j = 76.6;
            }
            else if (sName == "W40X278")
            {
                nw = 278; a = 82.3; d = 40.2; bf = 12; tw = 1.03; tf = 1.81; ix = 20500; zx = 1190; sx = 1020; rx = 15.8; iy = 521; zy = 140; sy = 87.1; ry = 2.52;j = 65.0;
            }
            else if (sName == "W40X264")
            {
                nw = 264; a = 77.4; d = 40; bf = 11.9; tw = 0.96; tf = 1.73; ix = 19400; zx = 1130; sx = 971; rx = 15.8; iy = 493; zy = 132; sy = 82.6; ry = 2.52;j = 56.1;
            }
            else if (sName == "W40X235")
            {
                nw = 235; a = 69.1; d = 39.7; bf = 11.9; tw = 0.83; tf = 1.58; ix = 17400; zx = 1010; sx = 875; rx = 15.9; iy = 444; zy = 118; sy = 74.6; ry = 2.54;j = 41.3;
            }
            else if (sName == "W40X211")
            {
                nw = 211; a = 62.1; d = 39.4; bf = 11.8; tw = 0.75; tf = 1.42; ix = 15500; zx = 906; sx = 786; rx = 15.8; iy = 390; zy = 105; sy = 66.1; ry = 2.51;j = 30.4;
            }
            else if (sName == "W40X183")
            {
                nw = 183; a = 53.3; d = 39; bf = 11.8; tw = 0.65; tf = 1.2; ix = 13200; zx = 774; sx = 675; rx = 15.7; iy = 331; zy = 88.3; sy = 56; ry = 2.49;j = 19.3;
            }
            else if (sName == "W40X167")
            {
                nw = 167; a = 49.3; d = 38.6; bf = 11.8; tw = 0.65; tf = 1.03; ix = 11600; zx = 693; sx = 600; rx = 15.3; iy = 283; zy = 76; sy = 47.9; ry = 2.4;j = 14.0;
            }
            else if (sName == "W40X149")
            {
                nw = 149; a = 43.8; d = 38.2; bf = 11.8; tw = 0.63; tf = 0.83; ix = 9800; zx = 598; sx = 513; rx = 15; iy = 229; zy = 62.2; sy = 38.8; ry = 2.29;j = 9.36; 
            }
            else if (sName == "W36X652")
            {
                nw = 652; a = 192; d = 41.1; bf = 17.6; tw = 1.97; tf = 3.54; ix = 50600; zx = 2910; sx = 2460; rx = 16.2; iy = 3230; zy = 581; sy = 367; ry = 4.1;j = 593;
            }
            else if (sName == "W36X529")
            {
                nw = 529; a = 156; d = 39.8; bf = 17.2; tw = 1.61; tf = 2.91; ix = 39600; zx = 2330; sx = 1990; rx = 16; iy = 2490; zy = 454; sy = 289; ry = 4;j = 327;
            }
            else if (sName == "W36X487")
            {
                nw = 487; a = 143; d = 39.3; bf = 17.1; tw = 1.5; tf = 2.68; ix = 36000; zx = 2130; sx = 1830; rx = 15.8; iy = 2250; zy = 412; sy = 263; ry = 3.96;j = 258;
            }
            else if (sName == "W36X441")
            {
                nw = 441; a = 130; d = 38.9; bf = 17; tw = 1.36; tf = 2.44; ix = 32100; zx = 1910; sx = 1650; rx = 15.7; iy = 1990; zy = 368; sy = 235; ry = 3.92;j = 194;
            }
            else if (sName == "W36X395")
            {
                nw = 395; a = 116; d = 38.4; bf = 16.8; tw = 1.22; tf = 2.2; ix = 28500; zx = 1710; sx = 1490; rx = 15.7; iy = 1750; zy = 325; sy = 208; ry = 3.88;j = 142;
            }
            else if (sName == "W36X361")
            {
                nw = 361; a = 106; d = 38; bf = 16.7; tw = 1.12; tf = 2.01; ix = 25700; zx = 1550; sx = 1350; rx = 15.6; iy = 1570; zy = 293; sy = 188; ry = 3.85;j = 109;
            }
            else if (sName == "W36X330")
            {
                nw = 330; a = 96.9; d = 37.7; bf = 16.6; tw = 1.02; tf = 1.85; ix = 23300; zx = 1410; sx = 1240; rx = 15.5; iy = 1420; zy = 265; sy = 171; ry = 3.83;j = 84.3;
            }
            else if (sName == "W36X302")
            {
                nw = 302; a = 89; d = 37.3; bf = 16.7; tw = 0.945; tf = 1.68; ix = 21100; zx = 1280; sx = 1130; rx = 15.4; iy = 1300; zy = 241; sy = 156; ry = 3.82;j = 64.3;
            }
            else if (sName == "W36X282")
            {
                nw = 282; a = 82.9; d = 37.1; bf = 16.6; tw = 0.885; tf = 1.57; ix = 19600; zx = 1190; sx = 1050; rx = 15.4; iy = 1200; zy = 223; sy = 144; ry = 3.8;j = 52.7;
            }
            else if (sName == "W36X262")
            {
                nw = 262; a = 77.2; d = 36.9; bf = 16.6; tw = 0.84; tf = 1.44; ix = 17900; zx = 1100; sx = 972; rx = 15.3; iy = 1090; zy = 204; sy = 132; ry = 3.76;j = 41.6;
            }
            else if (sName == "W36X247")
            {
                nw = 247; a = 72.5; d = 36.7; bf = 16.5; tw = 0.8; tf = 1.35; ix = 16700; zx = 1030; sx = 913; rx = 15.2; iy = 1010; zy = 190; sy = 123; ry = 3.74;j = 34.7;
            }
            else if (sName == "W36X231")
            {
                nw = 231; a = 68.2; d = 36.5; bf = 16.5; tw = 0.76; tf = 1.26; ix = 15600; zx = 963; sx = 854; rx = 15.1; iy = 940; zy = 176; sy = 114; ry = 3.71;j = 28.7;
            }
            else if (sName == "W36X256")
            {
                nw = 256; a = 75.3; d = 37.4; bf = 12.2; tw = 0.96; tf = 1.73; ix = 16800; zx = 1040; sx = 895; rx = 14.9; iy = 528; zy = 137; sy = 86.5; ry = 2.65;j = 52.9;
            }
            else if (sName == "W36X232")
            {
                nw = 232; a = 68; d = 37.1; bf = 12.1; tw = 0.87; tf = 1.57; ix = 15000; zx = 936; sx = 809; rx = 14.8; iy = 468; zy = 122; sy = 77.2; ry = 2.62;j = 39.6;
            }
            else if (sName == "W36X210")
            {
                nw = 210; a = 61.9; d = 36.7; bf = 12.2; tw = 0.83; tf = 1.36; ix = 13200; zx = 833; sx = 719; rx = 14.6; iy = 411; zy = 107; sy = 67.5; ry = 2.58;j = 28.0;
            }
            else if (sName == "W36X194")
            {
                nw = 194; a = 57; d = 36.5; bf = 12.1; tw = 0.765; tf = 1.26; ix = 12100; zx = 767; sx = 664; rx = 14.6; iy = 375; zy = 97.7; sy = 61.9; ry = 2.56;j = 22.2;
            }
            else if (sName == "W36X182")
            {
                nw = 182; a = 53.6; d = 36.3; bf = 12.1; tw = 0.725; tf = 1.18; ix = 11300; zx = 718; sx = 623; rx = 14.5; iy = 347; zy = 90.7; sy = 57.6; ry = 2.55;j = 18.5;
            }
            else if (sName == "W36X170")
            {
                nw = 170; a = 50; d = 36.2; bf = 12; tw = 0.68; tf = 1.1; ix = 10500; zx = 668; sx = 581; rx = 14.5; iy = 320; zy = 83.8; sy = 53.2; ry = 2.53;j = 15.1;
            }
            else if (sName == "W36X160")
            {
                nw = 160; a = 47; d = 36; bf = 12; tw = 0.65; tf = 1.02; ix = 9760; zx = 624; sx = 542; rx = 14.4; iy = 295; zy = 77.3; sy = 49.1; ry = 2.5;j = 12.4;
            }
            else if (sName == "W36X150")
            {
                nw = 150; a = 44.3; d = 35.9; bf = 12; tw = 0.625; tf = 0.94; ix = 9040; zx = 581; sx = 504; rx = 14.3; iy = 270; zy = 70.9; sy = 45.1; ry = 2.47;j = 10.1;
            }
            else if (sName == "W36X135")
            {
                nw = 135; a = 39.9; d = 35.6; bf = 12; tw = 0.6; tf = 0.79; ix = 7800; zx = 509; sx = 439; rx = 14; iy = 225; zy = 59.7; sy = 37.7; ry = 2.38;j = 7.00;
            }
            else if (sName == "W33X387")
            {
                nw = 387; a = 114; d = 36; bf = 16.2; tw = 1.26; tf = 2.28; ix = 24300; zx = 1560; sx = 1350; rx = 14.6; iy = 1620; zy = 312; sy = 200; ry = 3.77;j = 148;
            }
            else if (sName == "W33X354")
            {
                nw = 354; a = 104; d = 35.6; bf = 16.1; tw = 1.16; tf = 2.09; ix = 22000; zx = 1420; sx = 1240; rx = 14.5; iy = 1460; zy = 282; sy = 181; ry = 3.74;j = 115;
            }
            else if (sName == "W33X318")
            {
                nw = 318; a = 93.7; d = 35.2; bf = 16; tw = 1.04; tf = 1.89; ix = 19500; zx = 1270; sx = 1110; rx = 14.5; iy = 1290; zy = 250; sy = 161; ry = 3.71;j = 84.4;
            }
            else if (sName == "W33X291")
            {
                nw = 291; a = 85.6; d = 34.8; bf = 15.9; tw = 0.96; tf = 1.73; ix = 17700; zx = 1160; sx = 1020; rx = 14.4; iy = 1160; zy = 226; sy = 146; ry = 3.68;j = 65.1;
            }
            else if (sName == "W33X263")
            {
                nw = 263; a = 77.4; d = 34.5; bf = 15.8; tw = 0.87; tf = 1.57; ix = 15900; zx = 1040; sx = 919; rx = 14.3; iy = 1040; zy = 202; sy = 131; ry = 3.66;j = 48.7;
            }
            else if (sName == "W33X241")
            {
                nw = 241; a = 71.1; d = 34.2; bf = 15.9; tw = 0.83; tf = 1.4; ix = 14200; zx = 940; sx = 831; rx = 14.1; iy = 933; zy = 182; sy = 118; ry = 3.62;j = 36.2;
            }
            else if (sName == "W33X221")
            {
                nw = 221; a = 65.3; d = 33.9; bf = 15.8; tw = 0.775; tf = 1.28; ix = 12900; zx = 857; sx = 759; rx = 14.1; iy = 840; zy = 164; sy = 106; ry = 3.59;j = 27.8;
            }
            else if (sName == "W33X201")
            {
                nw = 201; a = 59.1; d = 33.7; bf = 15.7; tw = 0.715; tf = 1.15; ix = 11600; zx = 773; sx = 686; rx = 14; iy = 749; zy = 147; sy = 95.2; ry = 3.56;j = 20.8;
            }
            else if (sName == "W33X169")
            {
                nw = 169; a = 49.5; d = 33.8; bf = 11.5; tw = 0.67; tf = 1.22; ix = 9290; zx = 629; sx = 549; rx = 13.7; iy = 310; zy = 84.4; sy = 53.9; ry = 2.5;j = 17.7;
            }
            else if (sName == "W33X152")
            {
                nw = 152; a = 44.9; d = 33.5; bf = 11.6; tw = 0.635; tf = 1.06; ix = 8160; zx = 559; sx = 487; rx = 13.5; iy = 273; zy = 73.9; sy = 47.2; ry = 2.47;j = 12.4;
            }
            else if (sName == "W33X141")
            {
                nw = 141; a = 41.5; d = 33.3; bf = 11.5; tw = 0.605; tf = 0.96; ix = 7450; zx = 514; sx = 448; rx = 13.4; iy = 246; zy = 66.9; sy = 42.7; ry = 2.43;j = 9.7;
            }
            else if (sName == "W33X130")
            {
                nw = 130; a = 38.3; d = 33.1; bf = 11.5; tw = 0.58; tf = 0.855; ix = 6710; zx = 467; sx = 406; rx = 13.2; iy = 218; zy = 59.5; sy = 37.9; ry = 2.39;j = 7.37;
            }
            else if (sName == "W33X118")
            {
                nw = 118; a = 34.7; d = 32.9; bf = 11.5; tw = 0.55; tf = 0.74; ix = 5900; zx = 415; sx = 359; rx = 13; iy = 187; zy = 51.3; sy = 32.6; ry = 2.32;j = 5.3;
            }
            else if (sName == "W30X391")
            {
                nw = 391; a = 115; d = 33.2; bf = 15.6; tw = 1.36; tf = 2.44; ix = 20700; zx = 1450; sx = 1250; rx = 13.4; iy = 1550; zy = 310; sy = 198; ry = 3.67;j = 173;
            }
            else if (sName == "W30X357")
            {
                nw = 357; a = 105; d = 32.8; bf = 15.5; tw = 1.24; tf = 2.24; ix = 18700; zx = 1320; sx = 1140; rx = 13.3; iy = 1390; zy = 279; sy = 179; ry = 3.64;j = 134;
            }
            else if (sName == "W30X326")
            {
                nw = 326; a = 95.9; d = 32.4; bf = 15.4; tw = 1.14; tf = 2.05; ix = 16800; zx = 1190; sx = 1040; rx = 13.2; iy = 1240; zy = 252; sy = 162; ry = 3.6;j = 103;
            }
            else if (sName == "W30X292")
            {
                nw = 292; a = 86; d = 32; bf = 15.3; tw = 1.02; tf = 1.85; ix = 14900; zx = 1060; sx = 930; rx = 13.2; iy = 1100; zy = 223; sy = 144; ry = 3.58; j = 75.2;
            }
            else if (sName == "W30X261")
            {
                nw = 261; a = 77; d = 31.6; bf = 15.2; tw = 0.93; tf = 1.65; ix = 13100; zx = 943; sx = 829; rx = 13.1; iy = 959; zy = 196; sy = 127; ry = 3.53;j = 54.1;
            }
            else if (sName == "W30X235")
            {
                nw = 235; a = 69.3; d = 31.3; bf = 15.1; tw = 0.83; tf = 1.5; ix = 11700; zx = 847; sx = 748; rx = 13; iy = 855; zy = 175; sy = 114; ry = 3.51;j = 40.3;
            }
            else if (sName == "W30X211")
            {
                nw = 211; a = 62.3; d = 30.9; bf = 15.1; tw = 0.775; tf = 1.32; ix = 10300; zx = 751; sx = 665; rx = 12.9; iy = 757; zy = 155; sy = 100; ry = 3.49;j = 28.4;
            }
            else if (sName == "W30X191")
            {
                nw = 191; a = 56.1; d = 30.7; bf = 15; tw = 0.71; tf = 1.19; ix = 9200; zx = 675; sx = 600; rx = 12.8; iy = 673; zy = 138; sy = 89.5; ry = 3.46;j = 21.0;
            }
            else if (sName == "W30X173")
            {
                nw = 173; a = 50.9; d = 30.4; bf = 15; tw = 0.655; tf = 1.07; ix = 8230; zx = 607; sx = 541; rx = 12.7; iy = 598; zy = 123; sy = 79.8; ry = 3.42;j = 15.6;
            }
            else if (sName == "W30X148")
            {
                nw = 148; a = 43.6; d = 30.7; bf = 10.5; tw = 0.65; tf = 1.18; ix = 6680; zx = 500; sx = 436; rx = 12.4; iy = 227; zy = 68; sy = 43.3; ry = 2.28;j = 14.5;
            }
            else if (sName == "W30X132")
            {
                nw = 132; a = 38.8; d = 30.3; bf = 10.5; tw = 0.615; tf = 1; ix = 5770; zx = 437; sx = 380; rx = 12.2; iy = 196; zy = 58.4; sy = 37.2; ry = 2.25;j = 9.72;
            }
            else if (sName == "W30X124")
            {
                nw = 124; a = 36.5; d = 30.2; bf = 10.5; tw = 0.585; tf = 0.93; ix = 5360; zx = 408; sx = 355; rx = 12.1; iy = 181; zy = 54; sy = 34.4; ry = 2.23;j = 7.99;
            }
            else if (sName == "W30X116")
            {
                nw = 116; a = 34.2; d = 30; bf = 10.5; tw = 0.565; tf = 0.85; ix = 4930; zx = 378; sx = 329; rx = 12; iy = 164; zy = 49.2; sy = 31.3; ry = 2.19;j = 6.43;
            }
            else if (sName == "W30X108")
            {
                nw = 108; a = 31.7; d = 29.8; bf = 10.5; tw = 0.545; tf = 0.76; ix = 4470; zx = 346; sx = 299; rx = 11.9; iy = 146; zy = 43.9; sy = 27.9; ry = 2.15;j = 4.99;
            }
            else if (sName == "W30X99")
            {
                nw = 99; a = 29; d = 29.7; bf = 10.5; tw = 0.52; tf = 0.67; ix = 3990; zx = 312; sx = 269; rx = 11.7; iy = 128; zy = 38.6; sy = 24.5; ry = 2.1;j = 3.77;
            }
            else if (sName == "W30X90")
            {
                nw = 90; a = 26.3; d = 29.5; bf = 10.4; tw = 0.47; tf = 0.61; ix = 3610; zx = 283; sx = 245; rx = 11.7; iy = 115; zy = 34.7; sy = 22.1; ry = 2.09;j = 2.84;
            }
            else if (sName == "W27X539")
            {
                nw = 539; a = 159; d = 32.5; bf = 15.3; tw = 1.97; tf = 3.54; ix = 25600; zx = 1890; sx = 1570; rx = 12.7; iy = 2110; zy = 437; sy = 277; ry = 3.65;j = 496;
            }
            else if (sName == "W27X368")
            {
                nw = 368; a = 109; d = 30.4; bf = 14.7; tw = 1.38; tf = 2.48; ix = 16200; zx = 1240; sx = 1060; rx = 12.2; iy = 1310; zy = 279; sy = 179; ry = 3.48;j = 170;
            }
            else if (sName == "W27X336")
            {
                nw = 336; a = 99.2; d = 30; bf = 14.6; tw = 1.26; tf = 2.28; ix = 14600; zx = 1130; sx = 972; rx = 12.1; iy = 1180; zy = 252; sy = 162; ry = 3.45;j = 131;
            }
            else if (sName == "W27X307")
            {
                nw = 307; a = 90.2; d = 29.6; bf = 14.4; tw = 1.16; tf = 2.09; ix = 13100; zx = 1030; sx = 887; rx = 12; iy = 1050; zy = 227; sy = 146; ry = 3.41;j = 101;
            }
            else if (sName == "W27X281")
            {
                nw = 281; a = 83.1; d = 29.3; bf = 14.4; tw = 1.06; tf = 1.93; ix = 11900; zx = 936; sx = 814; rx = 12; iy = 953; zy = 206; sy = 133; ry = 3.39;j = 79.5;
            }
            else if (sName == "W27X258")
            {
                nw = 258; a = 76.1; d = 29; bf = 14.3; tw = 0.98; tf = 1.77; ix = 10800; zx = 852; sx = 745; rx = 11.9; iy = 859; zy = 187; sy = 120; ry = 3.36;j = 61.6;
            }
            else if (sName == "W27X235")
            {
                nw = 235; a = 69.4; d = 28.7; bf = 14.2; tw = 0.91; tf = 1.61; ix = 9700; zx = 772; sx = 677; rx = 11.8; iy = 769; zy = 168; sy = 108; ry = 3.33;j = 47;
            }
            else if (sName == "W27X217")
            {
                nw = 217; a = 63.9; d = 28.4; bf = 14.1; tw = 0.83; tf = 1.5; ix = 8910; zx = 711; sx = 627; rx = 11.8; iy = 704; zy = 154; sy = 100; ry = 3.32;j = 37.6;
            }
            else if (sName == "W27X194")
            {
                nw = 194; a = 57.1; d = 28.1; bf = 14; tw = 0.75; tf = 1.34; ix = 7860; zx = 631; sx = 559; rx = 11.7; iy = 619; zy = 136; sy = 88.1; ry = 3.29;j = 27.1;
            }
            else if (sName == "W27X178")
            {
                nw = 178; a = 52.5; d = 27.8; bf = 14.1; tw = 0.725; tf = 1.19; ix = 7020; zx = 570; sx = 505; rx = 11.6; iy = 555; zy = 122; sy = 78.8; ry = 3.25;j = 20.1;
            }
            else if (sName == "W27X161")
            {
                nw = 161; a = 47.6; d = 27.6; bf = 14; tw = 0.66; tf = 1.08; ix = 6310; zx = 515; sx = 458; rx = 11.5; iy = 497; zy = 109; sy = 70.9; ry = 3.23;j = 15.1;
            }
            else if (sName == "W27X146")
            {
                nw = 146; a = 43.2; d = 27.4; bf = 14; tw = 0.605; tf = 0.975; ix = 5660; zx = 464; sx = 414; rx = 11.5; iy = 443; zy = 97.7; sy = 63.5; ry = 3.2;j = 11.3;
            }
            else if (sName == "W27X129")
            {
                nw = 129; a = 37.8; d = 27.6; bf = 10; tw = 0.61; tf = 1.1; ix = 4760; zx = 395; sx = 345; rx = 11.2; iy = 184; zy = 57.6; sy = 36.8; ry = 2.21;j = 11.1;
            }
            else if (sName == "W27X114")
            {
                nw = 114; a = 33.6; d = 27.3; bf = 10.1; tw = 0.57; tf = 0.93; ix = 4080; zx = 343; sx = 299; rx = 11; iy = 159; zy = 49.3; sy = 31.5; ry = 2.18;j = 7.33;
            }
            else if (sName == "W27X102")
            {
                nw = 102; a = 30; d = 27.1; bf = 10; tw = 0.515; tf = 0.83; ix = 3620; zx = 305; sx = 267; rx = 11; iy = 139; zy = 43.4; sy = 27.8; ry = 2.15;j = 5.28;
            }
            else if (sName == "W27X94")
            {
                nw = 94; a = 27.6; d = 26.9; bf = 10; tw = 0.49; tf = 0.745; ix = 3270; zx = 278; sx = 243; rx = 10.9; iy = 124; zy = 38.8; sy = 24.8; ry = 2.12;j = 4.03;
            }
            else if (sName == "W27X84")
            {
                nw = 84; a = 24.7; d = 26.7; bf = 10; tw = 0.46; tf = 0.64; ix = 2850; zx = 244; sx = 213; rx = 10.7; iy = 106; zy = 33.2; sy = 21.2; ry = 2.07;j = 2.81;
            }
            else if (sName == "W24X370")
            {
                nw = 370; a = 109; d = 28; bf = 13.7; tw = 1.52; tf = 2.72; ix = 13400; zx = 1130; sx = 957; rx = 11.1; iy = 1160; zy = 267; sy = 170; ry = 3.27;j = 201;
            }
            else if (sName == "W24X335")
            {
                nw = 335; a = 98.3; d = 27.5; bf = 13.5; tw = 1.38; tf = 2.48; ix = 11900; zx = 1020; sx = 864; rx = 11; iy = 1030; zy = 238; sy = 152; ry = 3.23; j = 152;
            }
            else if (sName == "W24X306")
            {
                nw = 306; a = 89.7; d = 27.1; bf = 13.4; tw = 1.26; tf = 2.28; ix = 10700; zx = 922; sx = 789; rx = 10.9; iy = 919; zy = 214; sy = 137; ry = 3.2; j = 117;
            }
            else if (sName == "W24X279")
            {
                nw = 279; a = 81.9; d = 26.7; bf = 13.3; tw = 1.16; tf = 2.09; ix = 9600; zx = 835; sx = 718; rx = 10.8; iy = 823; zy = 193; sy = 124; ry = 3.17; j = 90.5;
            }
            else if (sName == "W24X250")
            {
                nw = 250; a = 73.5; d = 26.3; bf = 13.2; tw = 1.04; tf = 1.89; ix = 8490; zx = 744; sx = 644; rx = 10.7; iy = 724; zy = 171; sy = 110; ry = 3.14; j = 66.6;
            }
            else if (sName == "W24X229")
            {
                nw = 229; a = 67.2; d = 26; bf = 13.1; tw = 0.96; tf = 1.73; ix = 7650; zx = 675; sx = 588; rx = 10.7; iy = 651; zy = 154; sy = 99.4; ry = 3.11; j = 51.3;
            }
            else if (sName == "W24X207")
            {
                nw = 207; a = 60.7; d = 25.7; bf = 13; tw = 0.87; tf = 1.57; ix = 6820; zx = 606; sx = 531; rx = 10.6; iy = 578; zy = 137; sy = 88.8; ry = 3.08; j = 38.3;
            }
            else if (sName == "W24X192")
            {
                nw = 192; a = 56.5; d = 25.5; bf = 13; tw = 0.81; tf = 1.46; ix = 6260; zx = 559; sx = 491; rx = 10.5; iy = 530; zy = 126; sy = 81.8; ry = 3.07; j = 30.8;
            }
            else if (sName == "W24X176")
            {
                nw = 176; a = 51.7; d = 25.2; bf = 12.9; tw = 0.75; tf = 1.34; ix = 5680; zx = 511; sx = 450; rx = 10.5; iy = 479; zy = 115; sy = 74.3; ry = 3.04; j = 23.9;
            }
            else if (sName == "W24X162")
            {
                nw = 162; a = 47.8; d = 25; bf = 13; tw = 0.705; tf = 1.22; ix = 5170; zx = 468; sx = 414; rx = 10.4; iy = 443; zy = 105; sy = 68.4; ry = 3.05; j = 18.5;
            }
            else if (sName == "W24X146")
            {
                nw = 146; a = 43; d = 24.7; bf = 12.9; tw = 0.65; tf = 1.09; ix = 4580; zx = 418; sx = 371; rx = 10.3; iy = 391; zy = 93.2; sy = 60.5; ry = 3.01; j = 13.4;
            }
            else if (sName == "W24X131")
            {
                nw = 131; a = 38.6; d = 24.5; bf = 12.9; tw = 0.605; tf = 0.96; ix = 4020; zx = 370; sx = 329; rx = 10.2; iy = 340; zy = 81.5; sy = 53; ry = 2.97; j = 9.5;
            }
            else if (sName == "W24X117")
            {
                nw = 117; a = 34.4; d = 24.3; bf = 12.8; tw = 0.55; tf = 0.85; ix = 3540; zx = 327; sx = 291; rx = 10.1; iy = 297; zy = 71.4; sy = 46.5; ry = 2.94; j = 6.72;
            }
            else if (sName == "W24X104")
            {
                nw = 104; a = 30.7; d = 24.1; bf = 12.8; tw = 0.5; tf = 0.75; ix = 3100; zx = 289; sx = 258; rx = 10.1; iy = 259; zy = 62.4; sy = 40.7; ry = 2.91; j = 4.72;
            }
            else if (sName == "W24X103")
            {
                nw = 103; a = 30.3; d = 24.5; bf = 9; tw = 0.55; tf = 0.98; ix = 3000; zx = 280; sx = 245; rx = 10; iy = 119; zy = 41.5; sy = 26.5; ry = 1.99; j = 7.07;
            }
            else if (sName == "W24X94")
            {
                nw = 94; a = 27.7; d = 24.3; bf = 9.07; tw = 0.515; tf = 0.875; ix = 2700; zx = 254; sx = 222; rx = 9.87; iy = 109; zy = 37.5; sy = 24; ry = 1.98; j = 5.26;
            }
            else if (sName == "W24X84")
            {
                nw = 84; a = 24.7; d = 24.1; bf = 9.02; tw = 0.47; tf = 0.77; ix = 2370; zx = 224; sx = 196; rx = 9.79; iy = 94.4; zy = 32.6; sy = 20.9; ry = 1.95; j = 3.70;
            }
            else if (sName == "W24X76")
            {
                nw = 76; a = 22.4; d = 23.9; bf = 8.99; tw = 0.44; tf = 0.68; ix = 2100; zx = 200; sx = 176; rx = 9.69; iy = 82.5; zy = 28.6; sy = 18.4; ry = 1.92; j = 2.68;
            }
            else if (sName == "W24X68")
            {
                nw = 68; a = 20.1; d = 23.7; bf = 8.97; tw = 0.415; tf = 0.585; ix = 1830; zx = 177; sx = 154; rx = 9.55; iy = 70.4; zy = 24.5; sy = 15.7; ry = 1.87; j = 1.87;
            }
            else if (sName == "W24X62")
            {
                nw = 62; a = 18.2; d = 23.7; bf = 7.04; tw = 0.43; tf = 0.59; ix = 1550; zx = 153; sx = 131; rx = 9.23; iy = 34.5; zy = 15.7; sy = 9.8; ry = 1.38; j = 1.71;
            }
            else if (sName == "W24X55")
            {
                nw = 55; a = 16.2; d = 23.6; bf = 7.01; tw = 0.395; tf = 0.505; ix = 1350; zx = 134; sx = 114; rx = 9.11; iy = 29.1; zy = 13.3; sy = 8.3; ry = 1.34; j = 1.18;
            }
            else if (sName == "W21X201")
            {
                nw = 201; a = 59.3; d = 23; bf = 12.6; tw = 0.91; tf = 1.63; ix = 5310; zx = 530; sx = 461; rx = 9.47; iy = 542; zy = 133; sy = 86.1; ry = 3.02; j = 40.9;
            }
            else if (sName == "W21X182")
            {
                nw = 182; a = 53.6; d = 22.7; bf = 12.5; tw = 0.83; tf = 1.48; ix = 4730; zx = 476; sx = 417; rx = 9.4; iy = 483; zy = 119; sy = 77.2; ry = 3; j = 30.7;
            }
            else if (sName == "W21X166")
            {
                nw = 166; a = 48.8; d = 22.5; bf = 12.4; tw = 0.75; tf = 1.36; ix = 4280; zx = 432; sx = 380; rx = 9.36; iy = 435; zy = 108; sy = 70; ry = 2.99; j = 23.6;
            }
            else if (sName == "W21X147")
            {
                nw = 147; a = 43.2; d = 22.1; bf = 12.5; tw = 0.72; tf = 1.15; ix = 3630; zx = 373; sx = 329; rx = 9.17; iy = 376; zy = 92.6; sy = 60.1; ry = 2.95; j = 15.4;
            }
            else if (sName == "W21X132")
            {
                nw = 132; a = 38.8; d = 21.8; bf = 12.4; tw = 0.65; tf = 1.04; ix = 3220; zx = 333; sx = 295; rx = 9.12; iy = 333; zy = 82.3; sy = 53.5; ry = 2.93; j = 11.3;
            }
            else if (sName == "W21X122")
            {
                nw = 122; a = 35.9; d = 21.7; bf = 12.4; tw = 0.6; tf = 0.96; ix = 2960; zx = 307; sx = 273; rx = 9.09; iy = 305; zy = 75.6; sy = 49.2; ry = 2.92; j = 8.98;
            }
            else if (sName == "W21X111")
            {
                nw = 111; a = 32.6; d = 21.5; bf = 12.3; tw = 0.55; tf = 0.875; ix = 2670; zx = 279; sx = 249; rx = 9.05; iy = 274; zy = 68.2; sy = 44.5; ry = 2.9; j = 6.83;
            }
            else if (sName == "W21X101")
            {
                nw = 101; a = 29.8; d = 21.4; bf = 12.3; tw = 0.5; tf = 0.8; ix = 2420; zx = 253; sx = 227; rx = 9.02; iy = 248; zy = 61.7; sy = 40.3; ry = 2.89; j = 5.21;
            }
            else if (sName == "W21X93")
            {
                nw = 93; a = 27.3; d = 21.6; bf = 8.42; tw = 0.58; tf = 0.93; ix = 2070; zx = 221; sx = 192; rx = 8.7; iy = 92.9; zy = 34.7; sy = 22.1; ry = 1.84; j = 6.03;
            }
            else if (sName == "W21X83")
            {
                nw = 83; a = 24.4; d = 21.4; bf = 8.36; tw = 0.515; tf = 0.835; ix = 1830; zx = 196; sx = 171; rx = 8.67; iy = 81.4; zy = 30.5; sy = 19.5; ry = 1.83; j = 4.34;
            }
            else if (sName == "W21X73")
            {
                nw = 73; a = 21.5; d = 21.2; bf = 8.3; tw = 0.455; tf = 0.74; ix = 1600; zx = 172; sx = 151; rx = 8.64; iy = 70.6; zy = 26.6; sy = 17; ry = 1.81; j = 3.02;
            }
            else if (sName == "W21X68")
            {
                nw = 68; a = 20; d = 21.1; bf = 8.27; tw = 0.43; tf = 0.685; ix = 1480; zx = 160; sx = 140; rx = 8.6; iy = 64.7; zy = 24.4; sy = 15.7; ry = 1.8; j = 2.45;
            }
            else if (sName == "W21X62")
            {
                nw = 62; a = 18.3; d = 21; bf = 8.24; tw = 0.4; tf = 0.615; ix = 1330; zx = 144; sx = 127; rx = 8.54; iy = 57.5; zy = 21.7; sy = 14; ry = 1.77; j = 1.83;
            }
            else if (sName == "W21X55")
            {
                nw = 55; a = 16.2; d = 20.8; bf = 8.22; tw = 0.375; tf = 0.522; ix = 1140; zx = 126; sx = 110; rx = 8.4; iy = 48.4; zy = 18.4; sy = 11.8; ry = 1.73; j = 1.24;
            }
            else if (sName == "W21X48")
            {
                nw = 48; a = 14.1; d = 20.6; bf = 8.14; tw = 0.35; tf = 0.43; ix = 959; zx = 107; sx = 93; rx = 8.24; iy = 38.7; zy = 14.9; sy = 9.52; ry = 1.66; j = 0.803;
            }
            else if (sName == "W21X57")
            {
                nw = 57; a = 16.7; d = 21.1; bf = 6.56; tw = 0.405; tf = 0.65; ix = 1170; zx = 129; sx = 111; rx = 8.36; iy = 30.6; zy = 14.8; sy = 9.35; ry = 1.35; j = 1.77;
            }
            else if (sName == "W21X50")
            {
                nw = 50; a = 14.7; d = 20.8; bf = 6.53; tw = 0.38; tf = 0.535; ix = 984; zx = 110; sx = 94.5; rx = 8.18; iy = 24.9; zy = 12.2; sy = 7.64; ry = 1.3; j = 1.14;
            }
            else if (sName == "W21X44")
            {
                nw = 44; a = 13; d = 20.7; bf = 6.5; tw = 0.35; tf = 0.45; ix = 843; zx = 95.4; sx = 81.6; rx = 8.06; iy = 20.7; zy = 10.2; sy = 6.37; ry = 1.26; j = 0.77;
            }
            else if (sName == "W18X311")
            {
                nw = 311; a = 91.6; d = 22.3; bf = 12; tw = 1.52; tf = 2.74; ix = 6970; zx = 754; sx = 624; rx = 8.72; iy = 795; zy = 207; sy = 132; ry = 2.95; j = 176;
            }
            else if (sName == "W18X283")
            {
                nw = 283; a = 83.3; d = 21.9; bf = 11.9; tw = 1.4; tf = 2.5; ix = 6170; zx = 676; sx = 565; rx = 8.61; iy = 704; zy = 185; sy = 118; ry = 2.91; j = 134;
            }
            else if (sName == "W18X258")
            {
                nw = 258; a = 76; d = 21.5; bf = 11.8; tw = 1.28; tf = 2.3; ix = 5510; zx = 611; sx = 514; rx = 8.53; iy = 628; zy = 166; sy = 107; ry = 2.88; j = 103;
            }
            else if (sName == "W18X234")
            {
                nw = 234; a = 68.6; d = 21.1; bf = 11.7; tw = 1.16; tf = 2.11; ix = 4900; zx = 549; sx = 466; rx = 8.44; iy = 558; zy = 149; sy = 95.8; ry = 2.85; j = 78.7;
            }
            else if (sName == "W18X211")
            {
                nw = 211; a = 62.3; d = 20.7; bf = 11.6; tw = 1.06; tf = 1.91; ix = 4330; zx = 490; sx = 419; rx = 8.35; iy = 493; zy = 132; sy = 85.3; ry = 2.82; j = 58.6;
            }
            else if (sName == "W18X192")
            {
                nw = 192; a = 56.2; d = 20.4; bf = 11.5; tw = 0.96; tf = 1.75; ix = 3870; zx = 442; sx = 380; rx = 8.28; iy = 440; zy = 119; sy = 76.8; ry = 2.79; j = 44.7;
            }
            else if (sName == "W18X175")
            {
                nw = 175; a = 51.4; d = 20; bf = 11.4; tw = 0.89; tf = 1.59; ix = 3450; zx = 398; sx = 344; rx = 8.2; iy = 391; zy = 106; sy = 68.8; ry = 2.76; j = 33.8;
            }
            else if (sName == "W18X158")
            {
                nw = 158; a = 46.3; d = 19.7; bf = 11.3; tw = 0.81; tf = 1.44; ix = 3060; zx = 356; sx = 310; rx = 8.12; iy = 347; zy = 94.8; sy = 61.4; ry = 2.74; j = 25.2;
            }
            else if (sName == "W18X143")
            {
                nw = 143; a = 42; d = 19.5; bf = 11.2; tw = 0.73; tf = 1.32; ix = 2750; zx = 322; sx = 282; rx = 8.09; iy = 311; zy = 85.4; sy = 55.5; ry = 2.72; j = 19.2;
            }
            else if (sName == "W18X130")
            {
                nw = 130; a = 38.3; d = 19.3; bf = 11.2; tw = 0.67; tf = 1.2; ix = 2460; zx = 290; sx = 256; rx = 8.03; iy = 278; zy = 76.7; sy = 49.9; ry = 2.7; j = 14.5;
            }
            else if (sName == "W18X119")
            {
                nw = 119; a = 35.1; d = 19; bf = 11.3; tw = 0.655; tf = 1.06; ix = 2190; zx = 262; sx = 231; rx = 7.9; iy = 253; zy = 69.1; sy = 44.9; ry = 2.69; j = 10.6;
            }
            else if (sName == "W18X106")
            {
                nw = 106; a = 31.1; d = 18.7; bf = 11.2; tw = 0.59; tf = 0.94; ix = 1910; zx = 230; sx = 204; rx = 7.84; iy = 220; zy = 60.5; sy = 39.4; ry = 2.66; j = 7.48;
            }
            else if (sName == "W18X97")
            {
                nw = 97; a = 28.5; d = 18.6; bf = 11.1; tw = 0.535; tf = 0.87; ix = 1750; zx = 211; sx = 188; rx = 7.82; iy = 201; zy = 55.3; sy = 36.1; ry = 2.65; j = 5.86;
            }
            else if (sName == "W18X86")
            {
                nw = 86; a = 25.3; d = 18.4; bf = 11.1; tw = 0.48; tf = 0.77; ix = 1530; zx = 186; sx = 166; rx = 7.77; iy = 175; zy = 48.4; sy = 31.6; ry = 2.63; j = 4.10;
            }
            else if (sName == "W18X76")
            {
                nw = 76; a = 22.3; d = 18.2; bf = 11; tw = 0.425; tf = 0.68; ix = 1330; zx = 163; sx = 146; rx = 7.73; iy = 152; zy = 42.2; sy = 27.6; ry = 2.61; j = 2.83;
            }
            else if (sName == "W18X71")
            {
                nw = 71; a = 20.9; d = 18.5; bf = 7.64; tw = 0.495; tf = 0.81; ix = 1170; zx = 146; sx = 127; rx = 7.5; iy = 60.3; zy = 24.7; sy = 15.8; ry = 1.7; j = 3.49;
            }
            else if (sName == "W18X65")
            {
                nw = 65; a = 19.1; d = 18.4; bf = 7.59; tw = 0.45; tf = 0.75; ix = 1070; zx = 133; sx = 117; rx = 7.49; iy = 54.8; zy = 22.5; sy = 14.4; ry = 1.69; j = 2.73;
            }
            else if (sName == "W18X60")
            {
                nw = 60; a = 17.6; d = 18.2; bf = 7.56; tw = 0.415; tf = 0.695; ix = 984; zx = 123; sx = 108; rx = 7.47; iy = 50.1; zy = 20.6; sy = 13.3; ry = 1.68; j = 2.17;
            }
            else if (sName == "W18X55")
            {
                nw = 55; a = 16.2; d = 18.1; bf = 7.53; tw = 0.39; tf = 0.63; ix = 890; zx = 112; sx = 98.3; rx = 7.41; iy = 44.9; zy = 18.5; sy = 11.9; ry = 1.67; j = 1.66;
            }
            else if (sName == "W18X50")
            {
                nw = 50; a = 14.7; d = 18; bf = 7.5; tw = 0.355; tf = 0.57; ix = 800; zx = 101; sx = 88.9; rx = 7.38; iy = 40.1; zy = 16.6; sy = 10.7; ry = 1.65; j = 1.24;
            }
            else if (sName == "W18X46")
            {
                nw = 46; a = 13.5; d = 18.1; bf = 6.06; tw = 0.36; tf = 0.605; ix = 712; zx = 90.7; sx = 78.8; rx = 7.25; iy = 22.5; zy = 11.7; sy = 7.43; ry = 1.29; j = 1.22;
            }
            else if (sName == "W18X40")
            {
                nw = 40; a = 11.8; d = 17.9; bf = 6.02; tw = 0.315; tf = 0.525; ix = 612; zx = 78.4; sx = 68.4; rx = 7.21; iy = 19.1; zy = 10; sy = 6.35; ry = 1.27; j = 0.81;
            }
            else if (sName == "W18X35")
            {
                nw = 35; a = 10.3; d = 17.7; bf = 6; tw = 0.3; tf = 0.425; ix = 510; zx = 66.5; sx = 57.6; rx = 7.04; iy = 15.3; zy = 8.06; sy = 5.12; ry = 1.22; j = 0.506;
            }
            else if (sName == "W16X100")
            {
                nw = 100; a = 29.4; d = 17; bf = 10.4; tw = 0.585; tf = 0.985; ix = 1490; zx = 198; sx = 175; rx = 7.1; iy = 186; zy = 54.9; sy = 35.7; ry = 2.51; j = 7.73;
            }
            else if (sName == "W16X89")
            {
                nw = 89; a = 26.2; d = 16.8; bf = 10.4; tw = 0.525; tf = 0.875; ix = 1300; zx = 175; sx = 155; rx = 7.05; iy = 163; zy = 48.1; sy = 31.4; ry = 2.49; j = 5.45;
            }
            else if (sName == "W16X77")
            {
                nw = 77; a = 22.6; d = 16.5; bf = 10.3; tw = 0.455; tf = 0.76; ix = 1110; zx = 150; sx = 134; rx = 7; iy = 138; zy = 41.1; sy = 26.9; ry = 2.47; j = 3.57;
            }
            else if (sName == "W16X67")
            {
                nw = 67; a = 19.6; d = 16.3; bf = 10.2; tw = 0.395; tf = 0.665; ix = 954; zx = 130; sx = 117; rx = 6.96; iy = 119; zy = 35.5; sy = 23.2; ry = 2.46; j = 2.39;
            }
            else if (sName == "W16X57")
            {
                nw = 57; a = 16.8; d = 16.4; bf = 7.12; tw = 0.43; tf = 0.715; ix = 758; zx = 105; sx = 92.2; rx = 6.72; iy = 43.1; zy = 18.9; sy = 12.1; ry = 1.6; j = 2.22;
            }
            else if (sName == "W16X50")
            {
                nw = 50; a = 14.7; d = 16.3; bf = 7.07; tw = 0.38; tf = 0.63; ix = 659; zx = 92; sx = 81; rx = 6.68; iy = 37.2; zy = 16.3; sy = 10.5; ry = 1.59; j = 1.52;
            }
            else if (sName == "W16X45")
            {
                nw = 45; a = 13.3; d = 16.1; bf = 7.04; tw = 0.345; tf = 0.565; ix = 586; zx = 82.3; sx = 72.7; rx = 6.65; iy = 32.8; zy = 14.5; sy = 9.34; ry = 1.57; j = 1.11;
            }
            else if (sName == "W16X40")
            {
                nw = 40; a = 11.8; d = 16; bf = 7; tw = 0.305; tf = 0.505; ix = 518; zx = 73; sx = 64.7; rx = 6.63; iy = 28.9; zy = 12.7; sy = 8.25; ry = 1.57; j = 0.794;
            }
            else if (sName == "W16X36")
            {
                nw = 36; a = 10.6; d = 15.9; bf = 6.99; tw = 0.295; tf = 0.43; ix = 448; zx = 64; sx = 56.5; rx = 6.51; iy = 24.5; zy = 10.8; sy = 7; ry = 1.52; j = 0.545;
            }
            else if (sName == "W16X31")
            {
                nw = 31; a = 9.13; d = 15.9; bf = 5.53; tw = 0.275; tf = 0.44; ix = 375; zx = 54; sx = 47.2; rx = 6.41; iy = 12.4; zy = 7.03; sy = 4.49; ry = 1.17; j = 0.461;
            }
            else if (sName == "W16X26")
            {
                nw = 26; a = 7.68; d = 15.7; bf = 5.5; tw = 0.25; tf = 0.345; ix = 301; zx = 44.2; sx = 38.4; rx = 6.26; iy = 9.59; zy = 5.48; sy = 3.49; ry = 1.12; j = 0.262;
            }
            else if (sName == "W14X730")
            {
                nw = 730; a = 215; d = 22.4; bf = 17.9; tw = 3.07; tf = 4.91; ix = 14300; zx = 1660; sx = 1280; rx = 8.17; iy = 4720; zy = 816; sy = 527; ry = 4.69; j = 1450;
            }
            else if (sName == "W14X665")
            {
                nw = 665; a = 196; d = 21.6; bf = 17.7; tw = 2.83; tf = 4.52; ix = 12400; zx = 1480; sx = 1150; rx = 7.98; iy = 4170; zy = 730; sy = 472; ry = 4.62; j = 1120;
            }
            else if (sName == "W14X605")
            {
                nw = 605; a = 178; d = 20.9; bf = 17.4; tw = 2.6; tf = 4.16; ix = 10800; zx = 1320; sx = 1040; rx = 7.8; iy = 3680; zy = 652; sy = 423; ry = 4.55; j = 869;
            }
            else if (sName == "W14X550")
            {
                nw = 550; a = 162; d = 20.2; bf = 17.2; tw = 2.38; tf = 3.82; ix = 9430; zx = 1180; sx = 931; rx = 7.63; iy = 3250; zy = 583; sy = 378; ry = 4.49; j = 669;
            }
            else if (sName == "W14X500")
            {
                nw = 500; a = 147; d = 19.6; bf = 17; tw = 2.19; tf = 3.5; ix = 8210; zx = 1050; sx = 838; rx = 7.48; iy = 2880; zy = 522; sy = 339; ry = 4.43; j = 514;
            }
            else if (sName == "W14X455")
            {
                nw = 455; a = 134; d = 19; bf = 16.8; tw = 2.02; tf = 3.21; ix = 7190; zx = 936; sx = 756; rx = 7.33; iy = 2560; zy = 468; sy = 304; ry = 4.38; j = 395;
            }
            else if (sName == "W14X426")
            {
                nw = 426; a = 125; d = 18.7; bf = 16.7; tw = 1.88; tf = 3.04; ix = 6600; zx = 869; sx = 706; rx = 7.26; iy = 2360; zy = 434; sy = 283; ry = 4.34; j = 331;
            }
            else if (sName == "W14X398")
            {
                nw = 398; a = 117; d = 18.3; bf = 16.6; tw = 1.77; tf = 2.85; ix = 6000; zx = 801; sx = 656; rx = 7.16; iy = 2170; zy = 402; sy = 262; ry = 4.31; j = 273;
            }
            else if (sName == "W14X370")
            {
                nw = 370; a = 109; d = 17.9; bf = 16.5; tw = 1.66; tf = 2.66; ix = 5440; zx = 736; sx = 607; rx = 7.07; iy = 1990; zy = 370; sy = 241; ry = 4.27; j = 222;
            }
            else if (sName == "W14X342")
            {
                nw = 342; a = 101; d = 17.5; bf = 16.4; tw = 1.54; tf = 2.47; ix = 4900; zx = 672; sx = 558; rx = 6.98; iy = 1810; zy = 338; sy = 221; ry = 4.24; j = 178;
            }
            else if (sName == "W14X311")
            {
                nw = 311; a = 91.4; d = 17.1; bf = 16.2; tw = 1.41; tf = 2.26; ix = 4330; zx = 603; sx = 506; rx = 6.88; iy = 1610; zy = 304; sy = 199; ry = 4.2; j = 136;
            }
            else if (sName == "W14X283")
            {
                nw = 283; a = 83.3; d = 16.7; bf = 16.1; tw = 1.29; tf = 2.07; ix = 3840; zx = 542; sx = 459; rx = 6.79; iy = 1440; zy = 274; sy = 179; ry = 4.17; j = 104;
            }
            else if (sName == "W14X257")
            {
                nw = 257; a = 75.6; d = 16.4; bf = 16; tw = 1.18; tf = 1.89; ix = 3400; zx = 487; sx = 415; rx = 6.71; iy = 1290; zy = 246; sy = 161; ry = 4.13; j = 79.1;
            }
            else if (sName == "W14X233")
            {
                nw = 233; a = 68.5; d = 16; bf = 15.9; tw = 1.07; tf = 1.72; ix = 3010; zx = 436; sx = 375; rx = 6.63; iy = 1150; zy = 221; sy = 145; ry = 4.1; j = 59.5;
            }
            else if (sName == "W14X211")
            {
                nw = 211; a = 62; d = 15.7; bf = 15.8; tw = 0.98; tf = 1.56; ix = 2660; zx = 390; sx = 338; rx = 6.55; iy = 1030; zy = 198; sy = 130; ry = 4.07; j = 44.6;
            }
            else if (sName == "W14X193")
            {
                nw = 193; a = 56.8; d = 15.5; bf = 15.7; tw = 0.89; tf = 1.44; ix = 2400; zx = 355; sx = 310; rx = 6.5; iy = 931; zy = 180; sy = 119; ry = 4.05; j = 34.8;
            }
            else if (sName == "W14X176")
            {
                nw = 176; a = 51.8; d = 15.2; bf = 15.7; tw = 0.83; tf = 1.31; ix = 2140; zx = 320; sx = 281; rx = 6.43; iy = 838; zy = 163; sy = 107; ry = 4.02; j = 26.5;
            }
            else if (sName == "W14X159")
            {
                nw = 159; a = 46.7; d = 15; bf = 15.6; tw = 0.745; tf = 1.19; ix = 1900; zx = 287; sx = 254; rx = 6.38; iy = 748; zy = 146; sy = 96.2; ry = 4; j = 19.7;
            }
            else if (sName == "W14X145")
            {
                nw = 145; a = 42.7; d = 14.8; bf = 15.5; tw = 0.68; tf = 1.09; ix = 1710; zx = 260; sx = 232; rx = 6.33; iy = 677; zy = 133; sy = 87.3; ry = 3.98; j = 15.2;
            }
            else if (sName == "W14X132")
            {
                nw = 132; a = 38.8; d = 14.7; bf = 14.7; tw = 0.645; tf = 1.03; ix = 1530; zx = 234; sx = 209; rx = 6.28; iy = 548; zy = 113; sy = 74.5; ry = 3.76; j = 12.3;
            }
            else if (sName == "W14X120")
            {
                nw = 120; a = 35.3; d = 14.5; bf = 14.7; tw = 0.59; tf = 0.94; ix = 1380; zx = 212; sx = 190; rx = 6.24; iy = 495; zy = 102; sy = 67.5; ry = 3.74; j = 9.37;
            }
            else if (sName == "W14X109")
            {
                nw = 109; a = 32; d = 14.3; bf = 14.6; tw = 0.525; tf = 0.86; ix = 1240; zx = 192; sx = 173; rx = 6.22; iy = 447; zy = 92.7; sy = 61.2; ry = 3.73; j = 7.12;
            }
            else if (sName == "W14X99")
            {
                nw = 99; a = 29.1; d = 14.2; bf = 14.6; tw = 0.485; tf = 0.78; ix = 1110; zx = 173; sx = 157; rx = 6.17; iy = 402; zy = 83.6; sy = 55.2; ry = 3.71; j = 5.37;
            }
            else if (sName == "W14X90")
            {
                nw = 90; a = 26.5; d = 14; bf = 14.5; tw = 0.44; tf = 0.71; ix = 999; zx = 157; sx = 143; rx = 6.14; iy = 362; zy = 75.6; sy = 49.9; ry = 3.7; j = 4.06;
            }
            else if (sName == "W14X82")
            {
                nw = 82; a = 24; d = 14.3; bf = 10.1; tw = 0.51; tf = 0.855; ix = 881; zx = 139; sx = 123; rx = 6.05; iy = 148; zy = 44.8; sy = 29.3; ry = 2.48; j = 5.07;
            }
            else if (sName == "W14X74")
            {
                nw = 74; a = 21.8; d = 14.2; bf = 10.1; tw = 0.45; tf = 0.785; ix = 795; zx = 126; sx = 112; rx = 6.04; iy = 134; zy = 40.5; sy = 26.6; ry = 2.48; j = 3.87;
            }
            else if (sName == "W14X68")
            {
                nw = 68; a = 20; d = 14; bf = 10; tw = 0.415; tf = 0.72; ix = 722; zx = 115; sx = 103; rx = 6.01; iy = 121; zy = 36.9; sy = 24.2; ry = 2.46; j = 3.01;
            }
            else if (sName == "W14X61")
            {
                nw = 61; a = 17.9; d = 13.9; bf = 10; tw = 0.375; tf = 0.645; ix = 640; zx = 102; sx = 92.1; rx = 5.98; iy = 107; zy = 32.8; sy = 21.5; ry = 2.45; j = 2.19;
            }
            else if (sName == "W14X53")
            {
                nw = 53; a = 15.6; d = 13.9; bf = 8.06; tw = 0.37; tf = 0.66; ix = 541; zx = 87.1; sx = 77.8; rx = 5.89; iy = 57.7; zy = 22; sy = 14.3; ry = 1.92; j = 1.94;
            }
            else if (sName == "W14X48")
            {
                nw = 48; a = 14.1; d = 13.8; bf = 8.03; tw = 0.34; tf = 0.595; ix = 484; zx = 78.4; sx = 70.2; rx = 5.85; iy = 51.4; zy = 19.6; sy = 12.8; ry = 1.91; j = 1.45;
            }
            else if (sName == "W14X43")
            {
                nw = 43; a = 12.6; d = 13.7; bf = 8; tw = 0.305; tf = 0.53; ix = 428; zx = 69.6; sx = 62.6; rx = 5.82; iy = 45.2; zy = 17.3; sy = 11.3; ry = 1.89; j = 1.05;
            }
            else if (sName == "W14X38")
            {
                nw = 38; a = 11.2; d = 14.1; bf = 6.77; tw = 0.31; tf = 0.515; ix = 385; zx = 61.5; sx = 54.6; rx = 5.87; iy = 26.7; zy = 12.1; sy = 7.88; ry = 1.55; j = 0.798;
            }
            else if (sName == "W14X34")
            {
                nw = 34; a = 10; d = 14; bf = 6.75; tw = 0.285; tf = 0.455; ix = 340; zx = 54.6; sx = 48.6; rx = 5.83; iy = 23.3; zy = 10.6; sy = 6.91; ry = 1.53; j = 0.569;
            }
            else if (sName == "W14X30")
            {
                nw = 30; a = 8.85; d = 13.8; bf = 6.73; tw = 0.27; tf = 0.385; ix = 291; zx = 47.3; sx = 42; rx = 5.73; iy = 19.6; zy = 8.99; sy = 5.82; ry = 1.49; j = 0.380;
            }
            else if (sName == "W14X26")
            {
                nw = 26; a = 7.69; d = 13.9; bf = 5.03; tw = 0.255; tf = 0.42; ix = 245; zx = 40.2; sx = 35.3; rx = 5.65; iy = 8.91; zy = 5.54; sy = 3.55; ry = 1.08; j = 0.358;
            }
            else if (sName == "W14X22")
            {
                nw = 22; a = 6.49; d = 13.7; bf = 5; tw = 0.23; tf = 0.335; ix = 199; zx = 33.2; sx = 29; rx = 5.54; iy = 7; zy = 4.39; sy = 2.8; ry = 1.04; j = 0.208;
            }
            else if (sName == "W12X336")
            {
                nw = 336; a = 98.9; d = 16.8; bf = 13.4; tw = 1.78; tf = 2.96; ix = 4060; zx = 603; sx = 483; rx = 6.41; iy = 1190; zy = 274; sy = 177; ry = 3.47; j = 243;
            }
            else if (sName == "W12X305")
            {
                nw = 305; a = 89.5; d = 16.3; bf = 13.2; tw = 1.63; tf = 2.71; ix = 3550; zx = 537; sx = 435; rx = 6.29; iy = 1050; zy = 244; sy = 159; ry = 3.42; j = 185;
            }
            else if (sName == "W12X279")
            {
                nw = 279; a = 81.9; d = 15.9; bf = 13.1; tw = 1.53; tf = 2.47; ix = 3110; zx = 481; sx = 393; rx = 6.16; iy = 937; zy = 220; sy = 143; ry = 3.38; j = 143;
            }
            else if (sName == "W12X252")
            {
                nw = 252; a = 74.1; d = 15.4; bf = 13; tw = 1.4; tf = 2.25; ix = 2720; zx = 428; sx = 353; rx = 6.06; iy = 828; zy = 196; sy = 127; ry = 3.34; j = 108;
            }
            else if (sName == "W12X230")
            {
                nw = 230; a = 67.7; d = 15.1; bf = 12.9; tw = 1.29; tf = 2.07; ix = 2420; zx = 386; sx = 321; rx = 5.97; iy = 742; zy = 177; sy = 115; ry = 3.31; j = 83.8;
            }
            else if (sName == "W12X210")
            {
                nw = 210; a = 61.8; d = 14.7; bf = 12.8; tw = 1.18; tf = 1.9; ix = 2140; zx = 348; sx = 292; rx = 5.89; iy = 664; zy = 159; sy = 104; ry = 3.28; j = 64.7;
            }
            else if (sName == "W12X190")
            {
                nw = 190; a = 56; d = 14.4; bf = 12.7; tw = 1.06; tf = 1.74; ix = 1890; zx = 311; sx = 263; rx = 5.82; iy = 589; zy = 143; sy = 93; ry = 3.25; j = 48.8;
            }
            else if (sName == "W12X170")
            {
                nw = 170; a = 50; d = 14; bf = 12.6; tw = 0.96; tf = 1.56; ix = 1650; zx = 275; sx = 235; rx = 5.74; iy = 517; zy = 126; sy = 82.3; ry = 3.22; j = 35.6;
            }
            else if (sName == "W12X152")
            {
                nw = 152; a = 44.7; d = 13.7; bf = 12.5; tw = 0.87; tf = 1.4; ix = 1430; zx = 243; sx = 209; rx = 5.66; iy = 454; zy = 111; sy = 72.8; ry = 3.19; j = 25.8;
            }
            else if (sName == "W12X136")
            {
                nw = 136; a = 39.9; d = 13.4; bf = 12.4; tw = 0.79; tf = 1.25; ix = 1240; zx = 214; sx = 186; rx = 5.58; iy = 398; zy = 98; sy = 64.2; ry = 3.16; j = 18.5;
            }
            else if (sName == "W12X120")
            {
                nw = 120; a = 35.2; d = 13.1; bf = 12.3; tw = 0.71; tf = 1.11; ix = 1070; zx = 186; sx = 163; rx = 5.51; iy = 345; zy = 85.4; sy = 56; ry = 3.13; j = 12.9;
            }
            else if (sName == "W12X106")
            {
                nw = 106; a = 31.2; d = 12.9; bf = 12.2; tw = 0.61; tf = 0.99; ix = 933; zx = 164; sx = 145; rx = 5.47; iy = 301; zy = 75.1; sy = 49.3; ry = 3.11; j = 9.13;
            }
            else if (sName == "W12X96")
            {
                nw = 96; a = 28.2; d = 12.7; bf = 12.2; tw = 0.55; tf = 0.9; ix = 833; zx = 147; sx = 131; rx = 5.44; iy = 270; zy = 67.5; sy = 44.4; ry = 3.09; j = 6.85;
            }
            else if (sName == "W12X87")
            {
                nw = 87; a = 25.6; d = 12.5; bf = 12.1; tw = 0.515; tf = 0.81; ix = 740; zx = 132; sx = 118; rx = 5.38; iy = 241; zy = 60.4; sy = 39.7; ry = 3.07; j = 5.10;
            }
            else if (sName == "W12X79")
            {
                nw = 79; a = 23.2; d = 12.4; bf = 12.1; tw = 0.47; tf = 0.735; ix = 662; zx = 119; sx = 107; rx = 5.34; iy = 216; zy = 54.3; sy = 35.8; ry = 3.05; j = 3.84;
            }
            else if (sName == "W12X72")
            {
                nw = 72; a = 21.1; d = 12.3; bf = 12; tw = 0.43; tf = 0.67; ix = 597; zx = 108; sx = 97.4; rx = 5.31; iy = 195; zy = 49.2; sy = 32.4; ry = 3.04; j = 2.93;
            }
            else if (sName == "W12X65")
            {
                nw = 65; a = 19.1; d = 12.1; bf = 12; tw = 0.39; tf = 0.605; ix = 533; zx = 96.8; sx = 87.9; rx = 5.28; iy = 174; zy = 44.1; sy = 29.1; ry = 3.02; j = 2.18;
            }
            else if (sName == "W12X58")
            {
                nw = 58; a = 17; d = 12.2; bf = 10; tw = 0.36; tf = 0.64; ix = 475; zx = 86.4; sx = 78; rx = 5.28; iy = 107; zy = 32.5; sy = 21.4; ry = 2.51; j = 2.10;
            }
            else if (sName == "W12X53")
            {
                nw = 53; a = 15.6; d = 12.1; bf = 10; tw = 0.345; tf = 0.575; ix = 425; zx = 77.9; sx = 70.6; rx = 5.23; iy = 95.8; zy = 29.1; sy = 19.2; ry = 2.48; j = 1.58;
            }
            else if (sName == "W12X50")
            {
                nw = 50; a = 14.6; d = 12.2; bf = 8.08; tw = 0.37; tf = 0.64; ix = 391; zx = 71.9; sx = 64.2; rx = 5.18; iy = 56.3; zy = 21.3; sy = 13.9; ry = 1.96; j = 1.71;
            }
            else if (sName == "W12X45")
            {
                nw = 45; a = 13.1; d = 12.1; bf = 8.05; tw = 0.335; tf = 0.575; ix = 348; zx = 64.2; sx = 57.7; rx = 5.15; iy = 50; zy = 19; sy = 12.4; ry = 1.95; j = 1.26;
            }
            else if (sName == "W12X40")
            {
                nw = 40; a = 11.7; d = 11.9; bf = 8.01; tw = 0.295; tf = 0.515; ix = 307; zx = 57; sx = 51.5; rx = 5.13; iy = 44.1; zy = 16.8; sy = 11; ry = 1.94; j = 0.906;
            }
            else if (sName == "W12X35")
            {
                nw = 35; a = 10.3; d = 12.5; bf = 6.56; tw = 0.3; tf = 0.52; ix = 285; zx = 51.2; sx = 45.6; rx = 5.25; iy = 24.5; zy = 11.5; sy = 7.47; ry = 1.54; j = 0.741;
            }
            else if (sName == "W12X30")
            {
                nw = 30; a = 8.79; d = 12.3; bf = 6.52; tw = 0.26; tf = 0.44; ix = 238; zx = 43.1; sx = 38.6; rx = 5.21; iy = 20.3; zy = 9.56; sy = 6.24; ry = 1.52; j = 0.457;
            }
            else if (sName == "W12X26")
            {
                nw = 26; a = 7.65; d = 12.2; bf = 6.49; tw = 0.23; tf = 0.38; ix = 204; zx = 37.2; sx = 33.4; rx = 5.17; iy = 17.3; zy = 8.17; sy = 5.34; ry = 1.51; j = 0.3;
            }
            else if (sName == "W12X22")
            {
                nw = 22; a = 6.48; d = 12.3; bf = 4.03; tw = 0.26; tf = 0.425; ix = 156; zx = 29.3; sx = 25.4; rx = 4.91; iy = 4.66; zy = 3.66; sy = 2.31; ry = 0.848; j = 0.293;
            }
            else if (sName == "W12X19")
            {
                nw = 19; a = 5.57; d = 12.2; bf = 4.01; tw = 0.235; tf = 0.35; ix = 130; zx = 24.7; sx = 21.3; rx = 4.82; iy = 3.76; zy = 2.98; sy = 1.88; ry = 0.822; j = 0.18;
            }
            else if (sName == "W12X16")
            {
                nw = 16; a = 4.71; d = 12; bf = 3.99; tw = 0.22; tf = 0.265; ix = 103; zx = 20.1; sx = 17.1; rx = 4.67; iy = 2.82; zy = 2.26; sy = 1.41; ry = 0.773; j = 0.103;
            }
            else if (sName == "W12X14")
            {
                nw = 14; a = 4.16; d = 11.9; bf = 3.97; tw = 0.2; tf = 0.225; ix = 88.6; zx = 17.4; sx = 14.9; rx = 4.62; iy = 2.36; zy = 1.9; sy = 1.19; ry = 0.753; j = 0.0704;
            }
            else if (sName == "W10X112")
            {
                nw = 112; a = 32.9; d = 11.4; bf = 10.4; tw = 0.755; tf = 1.25; ix = 716; zx = 147; sx = 126; rx = 4.66; iy = 236; zy = 69.2; sy = 45.3; ry = 2.68; j = 15.1;
            }
            else if (sName == "W10X100")
            {
                nw = 100; a = 29.3; d = 11.1; bf = 10.3; tw = 0.68; tf = 1.12; ix = 623; zx = 130; sx = 112; rx = 4.6; iy = 207; zy = 61; sy = 40; ry = 2.65; j = 10.9;
            }
            else if (sName == "W10X88")
            {
                nw = 88; a = 26; d = 10.8; bf = 10.3; tw = 0.605; tf = 0.99; ix = 534; zx = 113; sx = 98.5; rx = 4.54; iy = 179; zy = 53.1; sy = 34.8; ry = 2.63; j = 7.53;
            }
            else if (sName == "W10X77")
            {
                nw = 77; a = 22.7; d = 10.6; bf = 10.2; tw = 0.53; tf = 0.87; ix = 455; zx = 97.6; sx = 85.9; rx = 4.49; iy = 154; zy = 45.9; sy = 30.1; ry = 2.6; j = 5.11;
            }
            else if (sName == "W10X68")
            {
                nw = 68; a = 19.9; d = 10.4; bf = 10.1; tw = 0.47; tf = 0.77; ix = 394; zx = 85.3; sx = 75.7; rx = 4.44; iy = 134; zy = 40.1; sy = 26.4; ry = 2.59; j = 3.56;
            }
            else if (sName == "W10X60")
            {
                nw = 60; a = 17.7; d = 10.2; bf = 10.1; tw = 0.42; tf = 0.68; ix = 341; zx = 74.6; sx = 66.7; rx = 4.39; iy = 116; zy = 35; sy = 23; ry = 2.57; j = 2.48;
            }
            else if (sName == "W10X54")
            {
                nw = 54; a = 15.8; d = 10.1; bf = 10; tw = 0.37; tf = 0.615; ix = 303; zx = 66.6; sx = 60; rx = 4.37; iy = 103; zy = 31.3; sy = 20.6; ry = 2.56; j = 1.82;
            }
            else if (sName == "W10X49")
            {
                nw = 49; a = 14.4; d = 10; bf = 10; tw = 0.34; tf = 0.56; ix = 272; zx = 60.4; sx = 54.6; rx = 4.35; iy = 93.4; zy = 28.3; sy = 18.7; ry = 2.54; j = 1.39;
            }
            else if (sName == "W10X45")
            {
                nw = 45; a = 13.3; d = 10.1; bf = 8.02; tw = 0.35; tf = 0.62; ix = 248; zx = 54.9; sx = 49.1; rx = 4.32; iy = 53.4; zy = 20.3; sy = 13.3; ry = 2.01; j = 1.51;
            }
            else if (sName == "W10X39")
            {
                nw = 39; a = 11.5; d = 9.92; bf = 7.99; tw = 0.315; tf = 0.53; ix = 209; zx = 46.8; sx = 42.1; rx = 4.27; iy = 45; zy = 17.2; sy = 11.3; ry = 1.98; j = 0.976;
            }
            else if (sName == "W10X33")
            {
                nw = 33; a = 9.71; d = 9.73; bf = 7.96; tw = 0.29; tf = 0.435; ix = 171; zx = 38.8; sx = 35; rx = 4.19; iy = 36.6; zy = 14; sy = 9.2; ry = 1.94; j = 0.583;
            }
            else if (sName == "W10X30")
            {
                nw = 30; a = 8.84; d = 10.5; bf = 5.81; tw = 0.3; tf = 0.51; ix = 170; zx = 36.6; sx = 32.4; rx = 4.38; iy = 16.7; zy = 8.84; sy = 5.75; ry = 1.37; j = 0.622;
            }
            else if (sName == "W10X26")
            {
                nw = 26; a = 7.61; d = 10.3; bf = 5.77; tw = 0.26; tf = 0.44; ix = 144; zx = 31.3; sx = 27.9; rx = 4.35; iy = 14.1; zy = 7.5; sy = 4.89; ry = 1.36; j = 0.402;
            }
            else if (sName == "W10X22")
            {
                nw = 22; a = 6.49; d = 10.2; bf = 5.75; tw = 0.24; tf = 0.36; ix = 118; zx = 26; sx = 23.2; rx = 4.27; iy = 11.4; zy = 6.1; sy = 3.97; ry = 1.33; j = 0.239;
            }
            else if (sName == "W10X19")
            {
                nw = 19; a = 5.62; d = 10.2; bf = 4.02; tw = 0.25; tf = 0.395; ix = 96.3; zx = 21.6; sx = 18.8; rx = 4.14; iy = 4.29; zy = 3.35; sy = 2.14; ry = 0.874; j = 0.233;
            }
            else if (sName == "W10X17")
            {
                nw = 17; a = 4.99; d = 10.1; bf = 4.01; tw = 0.24; tf = 0.33; ix = 81.9; zx = 18.7; sx = 16.2; rx = 4.05; iy = 3.56; zy = 2.8; sy = 1.78; ry = 0.845; j = 0.156;
            }
            else if (sName == "W10X15")
            {
                nw = 15; a = 4.41; d = 9.99; bf = 4; tw = 0.23; tf = 0.27; ix = 68.9; zx = 16; sx = 13.8; rx = 3.95; iy = 2.89; zy = 2.3; sy = 1.45; ry = 0.81; j = 0.104;
            }
            else if (sName == "W10X12")
            {
                nw = 12; a = 3.54; d = 9.87; bf = 3.96; tw = 0.19; tf = 0.21; ix = 53.8; zx = 12.6; sx = 10.9; rx = 3.9; iy = 2.18; zy = 1.74; sy = 1.1; ry = 0.785; j = 0.0547;
            }
            else if (sName == "W8X67")
            {
                nw = 67; a = 19.7; d = 9; bf = 8.28; tw = 0.57; tf = 0.935; ix = 272; zx = 70.1; sx = 60.4; rx = 3.72; iy = 88.6; zy = 32.7; sy = 21.4; ry = 2.12; j = 5.05;
            }
            else if (sName == "W8X58")
            {
                nw = 58; a = 17.1; d = 8.75; bf = 8.22; tw = 0.51; tf = 0.81; ix = 228; zx = 59.8; sx = 52; rx = 3.65; iy = 75.1; zy = 27.9; sy = 18.3; ry = 2.1; j = 3.33;
            }
            else if (sName == "W8X48")
            {
                nw = 48; a = 14.1; d = 8.5; bf = 8.11; tw = 0.4; tf = 0.685; ix = 184; zx = 49; sx = 43.2; rx = 3.61; iy = 60.9; zy = 22.9; sy = 15; ry = 2.08; j = 1.96;
            }
            else if (sName == "W8X40")
            {
                nw = 40; a = 11.7; d = 8.25; bf = 8.07; tw = 0.36; tf = 0.56; ix = 146; zx = 39.8; sx = 35.5; rx = 3.53; iy = 49.1; zy = 18.5; sy = 12.2; ry = 2.04; j = 1.12;
            }
            else if (sName == "W8X35")
            {
                nw = 35; a = 10.3; d = 8.12; bf = 8.02; tw = 0.31; tf = 0.495; ix = 127; zx = 34.7; sx = 31.2; rx = 3.51; iy = 42.6; zy = 16.1; sy = 10.6; ry = 2.03; j = 0.769;
            }
            else if (sName == "W8X31")
            {
                nw = 31; a = 9.13; d = 8; bf = 8; tw = 0.285; tf = 0.435; ix = 110; zx = 30.4; sx = 27.5; rx = 3.47; iy = 37.1; zy = 14.1; sy = 9.27; ry = 2.02; j = 0.536;
            }
            else if (sName == "W8X28")
            {
                nw = 28; a = 8.25; d = 8.06; bf = 6.54; tw = 0.285; tf = 0.465; ix = 98; zx = 27.2; sx = 24.3; rx = 3.45; iy = 21.7; zy = 10.1; sy = 6.63; ry = 1.62; j = 0.537;
            }
            else if (sName == "W8X24")
            {
                nw = 24; a = 7.08; d = 7.93; bf = 6.5; tw = 0.245; tf = 0.4; ix = 82.7; zx = 23.1; sx = 20.9; rx = 3.42; iy = 18.3; zy = 8.57; sy = 5.63; ry = 1.61; j = 0.346;
            }
            else if (sName == "W8X21")
            {
                nw = 21; a = 6.16; d = 8.28; bf = 5.27; tw = 0.25; tf = 0.4; ix = 75.3; zx = 20.4; sx = 18.2; rx = 3.49; iy = 9.77; zy = 5.69; sy = 3.71; ry = 1.26; j = 0.282;
            }
            else if (sName == "W8X18")
            {
                nw = 18; a = 5.26; d = 8.14; bf = 5.25; tw = 0.23; tf = 0.33; ix = 61.9; zx = 17; sx = 15.2; rx = 3.43; iy = 7.97; zy = 4.66; sy = 3.04; ry = 1.23; j = 0.172;
            }
            else if (sName == "W8X15")
            {
                nw = 15; a = 4.44; d = 8.11; bf = 4.015; tw = 0.245; tf = 0.315; ix = 48; zx = 13.6; sx = 11.8; rx = 3.29; iy = 3.41; zy = 2.67; sy = 1.7; ry = 0.876; j = 0.137;
            }
            else if (sName == "W8X13")
            {
                nw = 13; a = 3.84; d = 7.99; bf = 4; tw = 0.23; tf = 0.255; ix = 39.6; zx = 11.4; sx = 9.91; rx = 3.21; iy = 2.73; zy = 2.15; sy = 1.37; ry = 0.843; j = 0.0871;
            }
            else if (sName == "W8X10")
            {
                nw = 10; a = 2.96; d = 7.89; bf = 3.94; tw = 0.17; tf = 0.205; ix = 30.8; zx = 8.87; sx = 7.81; rx = 3.22; iy = 2.09; zy = 1.66; sy = 1.06; ry = 0.841; j = 0.0426;
            }
            else if (sName == "W6X25")
            {
                nw = 25; a = 7.34; d = 6.38; bf = 6.08; tw = 0.32; tf = 0.455; ix = 53.4; zx = 18.9; sx = 16.7; rx = 2.7; iy = 17.1; zy = 8.56; sy = 5.61; ry = 1.52; j = 0.461;
            }
            else if (sName == "W6X20")
            {
                nw = 20; a = 5.87; d = 6.2; bf = 6.02; tw = 0.26; tf = 0.365; ix = 41.4; zx = 15; sx = 13.4; rx = 2.66; iy = 13.3; zy = 6.72; sy = 4.41; ry = 1.5; j = 0.240;
            }
            else if (sName == "W6X15")
            {
                nw = 15; a = 4.43; d = 5.99; bf = 5.99; tw = 0.23; tf = 0.26; ix = 29.1; zx = 10.8; sx = 9.72; rx = 2.56; iy = 9.32; zy = 4.75; sy = 3.11; ry = 1.45; j = 0.101;
            }
            else if (sName == "W6X16")
            {
                nw = 16; a = 4.74; d = 6.28; bf = 4.03; tw = 0.26; tf = 0.405; ix = 32.1; zx = 11.7; sx = 10.2; rx = 2.6; iy = 4.43; zy = 3.39; sy = 2.2; ry = 0.967; j = 0.223;
            }
            else if (sName == "W6X12")
            {
                nw = 12; a = 3.55; d = 6.03; bf = 4; tw = 0.23; tf = 0.28; ix = 22.1; zx = 8.3; sx = 7.31; rx = 2.49; iy = 2.99; zy = 2.32; sy = 1.5; ry = 0.918; j = 0.0903;
            }
            else if (sName == "W6X9")
            {
                nw = 9; a = 2.68; d = 5.9; bf = 3.94; tw = 0.17; tf = 0.215; ix = 16.4; zx = 6.23; sx = 5.56; rx = 2.47; iy = 2.2; zy = 1.72; sy = 1.11; ry = 0.905; j = 0.0405;
            }
            else if (sName == "W6X8.5")
            {
                nw = 8.5; a = 2.52; d = 5.83; bf = 3.94; tw = 0.17; tf = 0.195; ix = 14.9; zx = 5.73; sx = 5.1; rx = 2.43; iy = 1.99; zy = 1.56; sy = 1.01; ry = 0.89; j = 0.0333;
            }
            else if (sName == "W5X19")
            {
                nw = 19; a = 5.56; d = 5.15; bf = 5.03; tw = 0.27; tf = 0.43; ix = 26.3; zx = 11.6; sx = 10.2; rx = 2.17; iy = 9.13; zy = 5.53; sy = 3.63; ry = 1.28; j = 0.316;
            }
            else if (sName == "W5X16")
            {
                nw = 16; a = 4.71; d = 5.01; bf = 5; tw = 0.24; tf = 0.36; ix = 21.4; zx = 9.63; sx = 8.55; rx = 2.13; iy = 7.51; zy = 4.58; sy = 3; ry = 1.26; j = 0.192;
            }
            else if (sName == "W4X13")
            {
                nw = 13; a = 3.83; d = 4.16; bf = 4.06; tw = 0.28; tf = 0.345; ix = 11.3; zx = 6.28; sx = 5.46; rx = 1.72; iy = 3.86; zy = 2.92; sy = 1.9; ry = 1; j = 0.151;
            }
        }

        public static List<string> GetWShapeNames()
        {
            List<string> names = new List<string>();

            names.Add("W44X335");
            names.Add("W44X290");
            names.Add("W44X262");
            names.Add("W44X230");
            names.Add("W40X593");
            names.Add("W40X503");
            names.Add("W40X431");
            names.Add("W40X397");
            names.Add("W40X372");
            names.Add("W40X362");
            names.Add("W40X324");
            names.Add("W40X297");
            names.Add("W40X277");
            names.Add("W40X249");
            names.Add("W40X215");
            names.Add("W40X199");
            names.Add("W40X392");
            names.Add("W40X331");
            names.Add("W40X327");
            names.Add("W40X294");
            names.Add("W40X278");
            names.Add("W40X264");
            names.Add("W40X235");
            names.Add("W40X211");
            names.Add("W40X183");
            names.Add("W40X167");
            names.Add("W40X149");
            //names.Add("W36X800"); //KodeStruct doesn't have this.
            names.Add("W36X652");
            names.Add("W36X529");
            names.Add("W36X487");
            names.Add("W36X441");
            names.Add("W36X395");
            names.Add("W36X361");
            names.Add("W36X330");
            names.Add("W36X302");
            names.Add("W36X282");
            names.Add("W36X262");
            names.Add("W36X247");
            names.Add("W36X231");
            names.Add("W36X256");
            names.Add("W36X232");
            names.Add("W36X210");
            names.Add("W36X194");
            names.Add("W36X182");
            names.Add("W36X170");
            names.Add("W36X160");
            names.Add("W36X150");
            names.Add("W36X135");
            names.Add("W33X387");
            names.Add("W33X354");
            names.Add("W33X318");
            names.Add("W33X291");
            names.Add("W33X263");
            names.Add("W33X241");
            names.Add("W33X221");
            names.Add("W33X201");
            names.Add("W33X169");
            names.Add("W33X152");
            names.Add("W33X141");
            names.Add("W33X130");
            names.Add("W33X118");
            names.Add("W30X391");
            names.Add("W30X357");
            names.Add("W30X326");
            names.Add("W30X292");
            names.Add("W30X261");
            names.Add("W30X235");
            names.Add("W30X211");
            names.Add("W30X191");
            names.Add("W30X173");
            names.Add("W30X148");
            names.Add("W30X132");
            names.Add("W30X124");
            names.Add("W30X116");
            names.Add("W30X108");
            names.Add("W30X99");
            names.Add("W30X90");
            names.Add("W27X539");
            names.Add("W27X368");
            names.Add("W27X336");
            names.Add("W27X307");
            names.Add("W27X281");
            names.Add("W27X258");
            names.Add("W27X235");
            names.Add("W27X217");
            names.Add("W27X194");
            names.Add("W27X178");
            names.Add("W27X161");
            names.Add("W27X146");
            names.Add("W27X129");
            names.Add("W27X114");
            names.Add("W27X102");
            names.Add("W27X94");
            names.Add("W27X84");
            names.Add("W24X370");
            names.Add("W24X335");
            names.Add("W24X306");
            names.Add("W24X279");
            names.Add("W24X250");
            names.Add("W24X229");
            names.Add("W24X207");
            names.Add("W24X192");
            names.Add("W24X176");
            names.Add("W24X162");
            names.Add("W24X146");
            names.Add("W24X131");
            names.Add("W24X117");
            names.Add("W24X104");
            names.Add("W24X103");
            names.Add("W24X94");
            names.Add("W24X84");
            names.Add("W24X76");
            names.Add("W24X68");
            names.Add("W24X62");
            names.Add("W24X55");
            names.Add("W21X201");
            names.Add("W21X182");
            names.Add("W21X166");
            names.Add("W21X147");
            names.Add("W21X132");
            names.Add("W21X122");
            names.Add("W21X111");
            names.Add("W21X101");
            names.Add("W21X93");
            names.Add("W21X83");
            names.Add("W21X73");
            names.Add("W21X68");
            names.Add("W21X62");
            names.Add("W21X55");
            names.Add("W21X48");
            names.Add("W21X57");
            names.Add("W21X50");
            names.Add("W21X44");
            names.Add("W18X311");
            names.Add("W18X283");
            names.Add("W18X258");
            names.Add("W18X234");
            names.Add("W18X211");
            names.Add("W18X192");
            names.Add("W18X175");
            names.Add("W18X158");
            names.Add("W18X143");
            names.Add("W18X130");
            names.Add("W18X119");
            names.Add("W18X106");
            names.Add("W18X97");
            names.Add("W18X86");
            names.Add("W18X76");
            names.Add("W18X71");
            names.Add("W18X65");
            names.Add("W18X60");
            names.Add("W18X55");
            names.Add("W18X50");
            names.Add("W18X46");
            names.Add("W18X40");
            names.Add("W18X35");
            names.Add("W16X100");
            names.Add("W16X89");
            names.Add("W16X77");
            names.Add("W16X67");
            names.Add("W16X57");
            names.Add("W16X50");
            names.Add("W16X45");
            names.Add("W16X40");
            names.Add("W16X36");
            names.Add("W16X31");
            names.Add("W16X26");
            names.Add("W14X730");
            names.Add("W14X665");
            names.Add("W14X605");
            names.Add("W14X550");
            names.Add("W14X500");
            names.Add("W14X455");
            names.Add("W14X426");
            names.Add("W14X398");
            names.Add("W14X370");
            names.Add("W14X342");
            names.Add("W14X311");
            names.Add("W14X283");
            names.Add("W14X257");
            names.Add("W14X233");
            names.Add("W14X211");
            names.Add("W14X193");
            names.Add("W14X176");
            names.Add("W14X159");
            names.Add("W14X145");
            names.Add("W14X132");
            names.Add("W14X120");
            names.Add("W14X109");
            names.Add("W14X99");
            names.Add("W14X90");
            names.Add("W14X82");
            names.Add("W14X74");
            names.Add("W14X68");
            names.Add("W14X61");
            names.Add("W14X53");
            names.Add("W14X48");
            names.Add("W14X43");
            names.Add("W14X38");
            names.Add("W14X34");
            names.Add("W14X30");
            names.Add("W14X26");
            names.Add("W14X22");
            names.Add("W12X336");
            names.Add("W12X305");
            names.Add("W12X279");
            names.Add("W12X252");
            names.Add("W12X230");
            names.Add("W12X210");
            names.Add("W12X190");
            names.Add("W12X170");
            names.Add("W12X152");
            names.Add("W12X136");
            names.Add("W12X120");
            names.Add("W12X106");
            names.Add("W12X96");
            names.Add("W12X87");
            names.Add("W12X79");
            names.Add("W12X72");
            names.Add("W12X65");
            names.Add("W12X58");
            names.Add("W12X53");
            names.Add("W12X50");
            names.Add("W12X45");
            names.Add("W12X40");
            names.Add("W12X35");
            names.Add("W12X30");
            names.Add("W12X26");
            names.Add("W12X22");
            names.Add("W12X19");
            names.Add("W12X16");
            names.Add("W12X14");
            names.Add("W10X112");
            names.Add("W10X100");
            names.Add("W10X88");
            names.Add("W10X77");
            names.Add("W10X68");
            names.Add("W10X60");
            names.Add("W10X54");
            names.Add("W10X49");
            names.Add("W10X45");
            names.Add("W10X39");
            names.Add("W10X33");
            names.Add("W10X30");
            names.Add("W10X26");
            names.Add("W10X22");
            names.Add("W10X19");
            names.Add("W10X17");
            names.Add("W10X15");
            names.Add("W10X12");
            names.Add("W8X67");
            names.Add("W8X58");
            names.Add("W8X48");
            names.Add("W8X40");
            names.Add("W8X35");
            names.Add("W8X31");
            names.Add("W8X28");
            names.Add("W8X24");
            names.Add("W8X21");
            names.Add("W8X18");
            names.Add("W8X15");
            names.Add("W8X13");
            names.Add("W8X10");
            names.Add("W6X25");
            names.Add("W6X20");
            names.Add("W6X15");
            names.Add("W6X16");
            names.Add("W6X12");
            names.Add("W6X9");
            names.Add("W6X8.5");
            names.Add("W5X19");
            names.Add("W5X16");
            names.Add("W4X13");


            return names;
        }

        public void GetWShapeProperties(string name, out double Ixx)
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

            this.GetWBeamDimensions(name, out a, out tw, out d, out tf, out bf, out nw, out ix, out sx, out rx, out zx, out iy, out sy, out ry, out zy, out rts, out ho, out j, out c);

            Ixx = ix;
        }

        public static List<sCrossSection> GetAllWShapes()
        {
            List<sCrossSection> wshapes = new List<sCrossSection>();
            foreach(string wn in sCrossSection.GetWShapeNames())
            {
                sCrossSection cs = new sCrossSection(wn);

                double ix;
                cs.GetWShapeProperties(wn, out ix);

                cs.I_StrongAxis = ix;

                wshapes.Add(cs);
            }
            return wshapes;
        }

    }

    public enum eSectionType
    {
        AISC_I_BEAM = 0,
        HSS_REC = 1,
        HSS_ROUND = 2,
        RECTANGLAR = 3,
        SQUARE = 4,
        ROUND = 5
    }
}
