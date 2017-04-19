using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace sDataObject.sGeometry
{
    public class sColorGradient
    {
        public List<sColor> colors { get; set; }
        public List<double> parameters { get; set; }

        public sColorGradient()
        {
            this.colors = new List<sColor>();
            this.parameters = new List<double>();
        }
        
        public void AddColorsItems(List<sColor> cols, List<double> paras)
        {
            this.colors.Clear();
            this.parameters.Clear();

            this.colors = cols.ToList();
            this.parameters = paras.ToList();
        }

        public static sColorGradient GetCyanRedGradient()
        {
            sColorGradient cg = new sColorGradient();

            cg.colors.Add(new sColor(180.0, 0.4, 1.0));
            cg.colors.Add(new sColor(0.0, 0.0, 0.45));
            cg.colors.Add(new sColor(0.0, 0.6, 1.0));

            cg.parameters.Add(0.0);
            cg.parameters.Add(0.5);
            cg.parameters.Add(1.0);

            return cg;
        }

        public static sColorGradient GetCyanRedGradient(sRange dataRange, sRange threshold = null)
        {
            sColorGradient cg = new sColorGradient();

            
            if (dataRange.length < 0.0)
            {
                cg.colors.Add(new sColor(0.0, 0.0, 0.5));
                cg.colors.Add(new sColor(0.0, 0.0, 0.5));
                cg.colors.Add(new sColor(0.0, 0.0, 0.5));
            }
            else
            {
                cg.colors.Add(new sColor(180.0, 0.4, 1.0));
                cg.colors.Add(new sColor(0.0, 0.0, 0.5));
                cg.colors.Add(new sColor(0.0, 0.6, 1.0));
            }

            if (threshold == null)
            {
                cg.parameters.Add(0.0);
                cg.parameters.Add(0.5);
                cg.parameters.Add(1.0);
            }
            else
            {
                double midHi_param = dataRange.GetNormalizedAt(threshold.max);
                if (midHi_param < 0.505)
                {
                    midHi_param = 0.505;
                }
                double mid3 = 0.5;
                double midLow_param = (1.0 - midHi_param);

                cg.parameters.Add(midLow_param);
                cg.parameters.Add(mid3);
                cg.parameters.Add(midHi_param);
            }
            return cg;
        }

        public static sColorGradient GetRainbowLikeGradient()
        {
            sColorGradient cg = new sColorGradient();

            cg.colors.Add(new sColor(220.0, 0.55, 1.0));
            cg.colors.Add(new sColor(185.0, 0.55, 1.0));
            cg.colors.Add(new sColor(150.0, 0.55, 1.0));
            cg.colors.Add(new sColor(0.0, 0.75, 1.0));

            cg.parameters.Add(0.0);
            cg.parameters.Add(0.25);
            cg.parameters.Add(0.75);
            cg.parameters.Add(1.0);

            return cg;
        }

        public static sColorGradient GetRainbowLikeGradient(sRange dataRange, sRange threshold = null)
        {
            sColorGradient cg = new sColorGradient();

            if(dataRange.length < 0.0)
            {
                cg.colors.Add(new sColor(220.0, 0.55, 1.0));
                cg.colors.Add(new sColor(220.0, 0.55, 1.0));
                cg.colors.Add(new sColor(220.0, 0.55, 1.0));
                cg.colors.Add(new sColor(220.0, 0.55, 1.0));
            }
            else
            {
                cg.colors.Add(new sColor(220.0, 0.55, 1.0));
                cg.colors.Add(new sColor(185.0, 0.55, 1.0));
                cg.colors.Add(new sColor(150.0, 0.55, 1.0));
                cg.colors.Add(new sColor(0.0, 0.75, 1.0));
            }


            if (threshold == null)
            {
                cg.parameters.Add(0.0);
                cg.parameters.Add(0.25);
                cg.parameters.Add(0.75);
                cg.parameters.Add(1.0);
            }
            else
            {
                double midHi_param = dataRange.GetNormalizedAt(threshold.max);
                double midLow_param = (midHi_param / 3.0);
                double midMid_param = ((2*midHi_param) / 3.0);

                cg.parameters.Add(0.0);
                cg.parameters.Add(midLow_param);
                cg.parameters.Add(midMid_param);
                cg.parameters.Add(midHi_param);
            }
            return cg;
        }

        public sColor ColorAt(double parameter)
        {
            List<sColorRangePair> pairs = new List<sColorRangePair>();

            for(int i = 0; i < this.parameters.Count - 1; ++i)
            {
                sColorRangePair pa = new sColorRangePair();
                pa.range = new sRange(this.parameters[i], this.parameters[i + 1]);

                pa.col1 = this.colors[i];
                pa.col2 = this.colors[i + 1];

                pairs.Add(pa);
            }

            sColor col = new sColor();
            for(int i = 0; i < pairs.Count; ++i)
            {
                if (pairs[i].range.Includes(parameter))
                {
                    col = sColor.BlendColors(pairs[i].col1, pairs[i].col2, pairs[i].range.GetNormalizedAt(parameter));
                    break;
                }
                else
                {
                    if(parameter >= this.parameters.Max())
                    {
                        col = this.colors[this.colors.Count - 1];
                    }
                    if(parameter <= this.parameters.Min())
                    {
                        col = this.colors[0];
                    }
                }
                
            }

            return col;
        }

    }

    public class sColorRangePair
    {
        public sRange range = new sRange();
        public sColor col1 = new sColor();
        public sColor col2 = new sColor();

        public sColorRangePair()
        {

        }
    }


    public class sColor : sGeometryBase
    {
        public int a { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public sColor()
        {
            this.a = 255;
            this.R = 255;
            this.G = 255;
            this.B = 255;
        }

        public sColor(int r, int g, int b)
        {
            this.a = 255;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public sColor(int a, int r, int g, int b)
        {
            this.a = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public sColor(double h, double s, double v)
        {
            this.a = 255;

            sColor temp = ColorFromHSVA(h, s, v, 255);
            this.R = temp.R;
            this.G = temp.G;
            this.B = temp.B;
        }

        public sColor DuplicatesColor()
        {
            sColor nc = new sColor(this.a, this.R, this.G, this.B);
            return nc;
        }

        public Color ToWinColor()
        {
            return Color.FromArgb(this.a, this.R, this.G, this.B);
        }

        public static sColor FromWinColor(Color col)
        {
            sColor sc = new sColor();
            sc.a = col.A;
            sc.R = col.R;
            sc.G = col.G;
            sc.B = col.B;
            return sc;
        }

        public static sColor BlendColors(sColor col1, sColor col2, double param)
        {
            byte r = (byte)((col2.R * param) + col1.R * (1 - param));
            byte g = (byte)((col2.G * param) + col1.G * (1 - param));
            byte b = (byte)((col2.B * param) + col1.B * (1 - param));

            sColor sc = new sColor();
            sc.a = 255;
            sc.R = (int)r;
            sc.G = (int)g;
            sc.B = (int)b;
            return sc;
        }

        public static sColor ColorFromHSVA(double hue, double saturation, double value, int alpha)
        {
            return sColor.FromWinColor(WinColorFromHSVA(hue, saturation, value, alpha));
        }

        public static Color WinColorFromHSVA(double hue, double saturation, double value, int alpha)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(alpha, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(alpha, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(alpha, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(alpha, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(alpha, t, p, v);
            else
                return Color.FromArgb(alpha, v, p, q);
        }
    }


}
