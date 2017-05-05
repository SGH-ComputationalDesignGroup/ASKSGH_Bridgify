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
using System.Drawing;
using sRhinoSystem.Properties;
using sDataObject.IElement;

namespace sRhinoSystem.GH.ToRhinoSystem
{
    public class To_RhinoColorMesh_System : GH_Component
    {

        public To_RhinoColorMesh_System()
            : base("sSystem Color Mesh", "sSystem Color Mesh", "...", "ASKSGH.Bridgify", "To Rhino")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; } // | GH_Exposure.obscure;
        }

        public eColorMode colMode = eColorMode.Stress_Combined_Absolute;
        string paramName = "colorThreshold(MPa|Ksi)";
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            for (int i = 0; i < 15; ++i)
            {
                if(i != 1 && i != 4 && i != 5 && i != 6)
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
            if (colMode == eColorMode.Stress_Combined_Absolute)
            {
                paramName = "colorThreshold(MPa|Ksi)";
            }
            else if (colMode == eColorMode.Deflection)
            {
                paramName = "colorThreshold(mm|in)";
            }
            else if (colMode == eColorMode.Force_X)
            {
                paramName = "colorThreshold(N|lbs)";
            }
            else if (colMode == eColorMode.Force_Y)
            {
                paramName = "colorThreshold(N|lbs)";
            }
            else if (colMode == eColorMode.Force_Z)
            {
                paramName = "colorThreshold(N|lbs)";
            }
            else if (colMode == eColorMode.Moment_X)
            {
                paramName = "colorThreshold(N.m|lb.ft)";
            }
            else if (colMode == eColorMode.Moment_Y)
            {
                paramName = "colorThreshold(N.m|lb.ft)";
            }
            else if (colMode == eColorMode.Moment_Z)
            {
                paramName = "colorThreshold(N.m|lb.ft)";
            }
            //else if (colMode == eColorMode.Stress_Axial_X)
            //{
            //    paramName = "colorThreshold(N.m|lb.ft)";
            //}
            // else if (colMode == "Stress_Axial_Y")
            // {
            //     colMode = eColorMode.Stress_Axial_Y;
            // }
            // else if (colMode == "Stress_Axial_Z")
            // {
            //     colMode = eColorMode.Stress_Axial_Z;
            // }
            // else if (colMode == "Stress_Moment_X")
            // {
            //     colMode = eColorMode.Stress_Moment_X;
            // }
            else if (colMode == eColorMode.Stress_Moment_Y)
            {
                paramName = "colorThreshold(MPa|Ksi)";
            }
            else if (colMode == eColorMode.Stress_Moment_Z)
            {
                paramName = "colorThreshold(MPa|Ksi)";
            }

            Params.Input[2].NickName = paramName;
            Params.Input[2].Name = paramName;
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
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sghSystem", "sghSystem", "...", GH_ParamAccess.item);
            pManager.AddNumberParameter("deformFactor", "deformFactor", "...", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("colorThreshold(MPa|Ksi)", "colorThreshold(MPa|Ksi)", "...", GH_ParamAccess.item, -1.0);

            Params.Input[1].Optional = true;
            Params.Input[2].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("ColorMesh", "ColorMesh", "ColorMesh", GH_ParamAccess.item);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ISystem sghSystem = null;
            double du = 0.0;
            double the = -1.0;

            if (!DA.GetData(0, ref sghSystem)) return;
            if (!DA.GetData(1, ref du)) return;
            if (!DA.GetData(2, ref the)) return;
  
            Mesh m = new Mesh();
            string mss = "";

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter("Meters", modelUnit);

            if (sghSystem != null)
            {
                ISystem sys = sghSystem as ISystem;

                sRange th = null;
                if (the > 0.0)
                {
                    if (colMode.ToString().Contains("Stress")) the *= 1.0E6;
                    th = new sRange(0.0, rhcon.EnsureUnit( the, colMode));
                }

                List<sMesh> mms = new List<sMesh>();
                sRange resultRange;
                sys.ConstructBeamResultMesh(colMode, ref mms, out resultRange, th, du);
                mss += "Color Mode\n" + colMode.ToString();
                

                Interval reRangeRh = rhcon.EnsureUnit_DataRange(rhcon.ToRhinoInterval(resultRange), colMode, true); //true means Pa > MPa
                mss += "\nData Range: " + Math.Round(reRangeRh.Min, 2) + " to " + Math.Round(reRangeRh.Max, 2);

                foreach (sMesh sm in mms)
                {
                    Mesh rm = rhcon.ToRhinoMesh(sm);
                    m.Append(rhcon.EnsureUnit(rm) as Mesh);
                }
                
            }
            this.Message = mss;

            DA.SetData(0, m);

        }

        public override Guid ComponentGuid
        {
            get { return new Guid("9ffa3821-5b40-4270-9440-a1ede8dc6e72"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.ToRhinoColorMesh;
            }
        }


    }
}
