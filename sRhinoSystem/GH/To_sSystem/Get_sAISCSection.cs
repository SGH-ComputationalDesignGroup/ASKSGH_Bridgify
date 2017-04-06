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
    public class Get_sAISCSection : GH_Component
    {
        public Get_sAISCSection()
            : base("sAISCSection", "sAISCSection", "sAISCSection", "ASKSGH.Bridgify", "To sSystem")
        {
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("shapeName", "shapeName", "...", GH_ParamAccess.item);
            // Params.Input[0].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sCrossSection", "sCrossSection", "sCrossSection", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string shapeName = "";

            if (!DA.GetData(0, ref shapeName)) return;

            sCrossSection cs = new sCrossSection(shapeName);

            sMaterial mat = new sMaterial();
            mat.materialName = "Steel";
            mat.materialType = eMaterialType.STEEL_A36;
            cs.material = mat;

            this.Message = cs.shapeName;

            DA.SetData(0, cs);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("22ee5ee6-cb21-4eac-9599-d9a8660978b0"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.AISCSection;
            }
        }

    }
}
