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
    public class To_RhinoBeamSetResults : GH_Component
    {

        public To_RhinoBeamSetResults()
            : base("sBeamSet ResultRange", "sBeamSet ResultRange", "...", "ASKSGH.Bridgify", "To Rhino")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; } // | GH_Exposure.obscure;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSet", "sBeamSet", "sBeamSet", GH_ParamAccess.item);
 
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        public eColorMode colMode = eColorMode.Stress_Combined_Absolute;
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            for (int i = 0; i < 15; ++i)
            {
                if (i != 1 && i != 4 && i != 5 && i != 6)
                {
                    System.Windows.Forms.ToolStripItem it = menu.Items.Add(((eColorMode)i).ToString());

                    it.MouseDown += new System.Windows.Forms.MouseEventHandler(it_MouseDown);
                    it.Tag = (eColorMode)i;
                }
            }

        }

        void it_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.ToolStripItem it = sender as System.Windows.Forms.ToolStripItem;
            if (it == null) return;

            SetColorMode((eColorMode)it.Tag);
        }

        void SetColorMode(eColorMode cmode)
        {
            colMode = cmode;
            this.ExpireSolution(true);
        }

        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("cType", (int)colMode);
            return base.Write(writer);
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            int mt = (int)eColorMode.Stress_Combined_Absolute;
            reader.TryGetInt32("cType", ref mt);

            //CrossSectionType = (CROSSSECTIONS)cs;
            SetColorMode((eColorMode)mt);

            return base.Read(reader);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("sBeamPlane", "sBeamPlane", "sBeamPlane", GH_ParamAccess.item);
            pManager.AddTextParameter("ResultLabel", "ResultLabel", "ResultLabel", GH_ParamAccess.item);
            pManager.AddNumberParameter("ResultMaxValue", "ResultMaxValue", "ResultMaxValue", GH_ParamAccess.item);
            pManager.AddNumberParameter("ResultMinValue", "ResultMinValue", "ResultMinValue", GH_ParamAccess.item);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            sFrameSet sb = null;

            if (!DA.GetData(0, ref sb)) return;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter("Meters", modelUnit);

            this.Message = "Beam Result\n" + colMode.ToString();

            sRange ran = sb.GetFrameSetResultRange(colMode);

            Curve rc = rhcon.EnsureUnit(rhcon.ToRhinoCurve(sb.parentCrv));
            Point3d cp = rc.PointAtNormalizedLength(0.5);
            Vector3d x = rc.PointAtNormalizedLength(0.49)-cp;
            x.Unitize();
            Vector3d y = Vector3d.CrossProduct(x, -Vector3d.ZAxis);
            Plane pl = new Plane(cp, x, y);
        

            double max = 0.0;
            double min = 0.0;
            string unit = "";
            if(colMode == eColorMode.Deflection)
            {
                max = rhcon.EnsureUnit_Deflection(ran.max);
                min = rhcon.EnsureUnit_Deflection(ran.min);
                if (modelUnit == "Meters")
                {
                    unit = "mm";
                }
                else if(modelUnit == "Feet")
                {
                    unit = "in";
                }
            }
            else if (colMode.ToString().Contains("Stress"))
            {
                max = rhcon.EnsureUnit_Stress(ran.max);
                min = rhcon.EnsureUnit_Stress(ran.min);
                if (modelUnit == "Meters")
                {
                    max *= 1.0E-6;
                    min *= 1.0E-6;
                    unit = "MPa";
                }
                else if (modelUnit == "Feet")
                {
                    unit = "ksi";
                }
            }
            else if (colMode.ToString().Contains("Force"))
            {
                max = rhcon.EnsureUnit_Force(ran.max);
                min = rhcon.EnsureUnit_Force(ran.min);
                if (modelUnit == "Meters")
                {
                    unit = "N";
                }
                else if (modelUnit == "Feet")
                {
                    unit = "lbf";
                }
            }
            else if (colMode.ToString().Contains("Moment"))
            {
                max = rhcon.EnsureUnit_Moment(ran.max);
                min = rhcon.EnsureUnit_Moment(ran.min);
                if (modelUnit == "Meters")
                {
                    unit = "N.m";
                }
                else if (modelUnit == "Feet")
                {
                    unit = "lbf.ft";
                }
            }

            DA.SetData(0, pl);
            DA.SetData(1, "("+ Math.Round(min,2) + ") - (" + Math.Round(max,2) + ") " + unit);
            DA.SetData(2, max);
            DA.SetData(3, min);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("dd431c8f-6fd0-4996-8953-3b383f4781b8"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.GetResults;
            }
        }


    }
}
