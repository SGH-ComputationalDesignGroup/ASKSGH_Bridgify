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
using sRhinoSystem.Properties;

namespace sRhinoSystem.GH.To_sSystem
{
    public class Get_sRectangularSection : GH_Component
    {
        public Get_sRectangularSection()
            : base("sRectangularSection", "sRectangularSection", "sRectangularSection", "ASKSGH.Bridgify", "To sSystem")
        {
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("material", "material", "...", GH_ParamAccess.item);
            pManager.AddNumberParameter("width(m|in)", "width(m|in)", "...", GH_ParamAccess.item);
            pManager.AddNumberParameter("depth(m|in)", "depth(m|in)", "...", GH_ParamAccess.item);
            pManager.AddNumberParameter("thickness(m|in)", "thickness(m|in)", "...", GH_ParamAccess.item, 0.0);

            Params.Input[3].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("crossSection", "crossSection", "crossSection", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            sMaterial material = null;
            double width = 0.0;
            double depth = 0.0;
            double thickness = 0.0;

            if (!DA.GetData(0, ref material)) return;
            if (!DA.GetData(1, ref width)) return;
            if (!DA.GetData(2, ref depth)) return;
            if (!DA.GetData(3, ref thickness)) return;

            sCrossSection cs = new sCrossSection();

            string mss = "";
            string shapeN = "";

            if (Math.Abs(width - depth) < 0.0001)
            {
                cs.sectionType = eSectionType.SQUARE;
                mss = "Square: W" + width + ", D" + depth;
                shapeN += "Square_" + width + "x" + depth;
            }
            else
            {
                cs.sectionType = eSectionType.RECTANGLAR;
                mss = "Rectangular: W" + width + ", D" + depth;
                shapeN += "Rectangular_" + width + "x" + depth;
            }

            if (thickness > 0.0)
            {
                mss += ", Th " + thickness;
                shapeN += "x" + thickness;
            }
            cs.shapeName = shapeN;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");

            if(modelUnit == "Feet")
            {
                width /= 12.0;
                depth /= 12.0;
                thickness /= 12.0;
            }

            width = Math.Round(width, 3);
            depth = Math.Round(depth, 3);
            thickness = Math.Round(thickness, 3);

            cs.dimensions = new List<double>();
            cs.dimensions.Add(rhcon.EnsureUnit(width));
            cs.dimensions.Add(rhcon.EnsureUnit(depth));
            if (thickness > 0.0)
            {
                cs.dimensions.Add(rhcon.EnsureUnit(thickness));
            }
            
            cs.material = material;

            this.Message = mss;

            DA.SetData(0, cs);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("5229e13e-b8e9-4e27-b3f4-acdd199756a3"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.recSec;
            }
        }

    }
}
