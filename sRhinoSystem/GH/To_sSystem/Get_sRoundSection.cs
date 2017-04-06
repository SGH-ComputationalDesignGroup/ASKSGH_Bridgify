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
    public class Get_sRoundSection : GH_Component
    {
        public Get_sRoundSection()
            : base("sRoundSection", "sRoundSection", "sRoundSection", "ASKSGH.Bridgify", "To sSystem")
        {
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("material", "material", "...", GH_ParamAccess.item);
            pManager.AddNumberParameter("diameter(m|in)", "diameter(m|in)", "...", GH_ParamAccess.item);
            pManager.AddNumberParameter("thickness(m|in)", "thickness(m|in)", "...", GH_ParamAccess.item, 0.0);

            Params.Input[2].Optional = true;
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
            double diameter = 0.0;
            double thickness = 0.0;

            if (!DA.GetData(0, ref material)) return;
            if (!DA.GetData(1, ref diameter)) return;
            if (!DA.GetData(2, ref thickness)) return;

            sCrossSection cs = new sCrossSection();

            eSectionType stype = eSectionType.ROUND;
            
            string mss = "Round: Dia " + diameter;
            string shapeN = "Round_" + diameter;
            if (thickness > 0.0)
            {
                mss += ", Th " + thickness;
                shapeN += "x" + thickness;
            }
            cs.shapeName = shapeN;
            this.Message = mss;

            cs.sectionType = stype;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");

            if (modelUnit == "Feet")
            {
                diameter /= 12.0;
                thickness /= 12.0;
            }

            diameter = Math.Round(diameter, 3);
            thickness = Math.Round(thickness, 3);

            cs.dimensions = new List<double>();
            cs.dimensions.Add(rhcon.EnsureUnit(diameter));
            if (thickness > 0.0)
            {
                cs.dimensions.Add(rhcon.EnsureUnit(thickness));
            }
            
            cs.material = material;


            DA.SetData(0, cs);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("b6f5a956-4dc4-4614-b793-7eaf51243a5a"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.roundSec;
            }
        }

    }
}
