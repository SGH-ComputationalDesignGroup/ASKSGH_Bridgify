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
    public class Get_sAISCName : GH_Component
    {
        public Get_sAISCName()
            : base("sAISCName", "sAISCName", "0: Wbeam, 1: HSS Rectangular, 2: HSS Round", "ASKSGH.Bridgify", "To sSystem")
        {
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "W Shape Beam", menuChangeMode);
            Menu_AppendItem(menu, "HSS Rectangular", menuChangeMode);
            Menu_AppendItem(menu, "HSS Round", menuChangeMode);

        }
        public int type = 0;
        private void menuChangeMode(object sender, EventArgs e)
        {
            var dataIn = sender.ToString();

            if(dataIn == "W Shape Beam")
            {
                type = 0;
            }
            else if (dataIn == "HSS Rectangular")
            {
                type = 1;
            }
            else
            {
                type = 2;
            }
            
            this.ExpireSolution(true);
        }

        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("ShapeType", type);
            return base.Write(writer);
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            int ty = 0;
            reader.TryGetInt32("ShapeType", ref ty);

            type = ty;

            return base.Read(reader);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("index", "index", "...", GH_ParamAccess.item,0);
            Params.Input[0].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("shapeName", "shapeName", "shapeName", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int index = 0;
            
            if (!DA.GetData(0, ref index)) return;

            List<string> names = new List<string>();
            

            if (type == 0)
            {
                names = sCrossSection.GetWShapeNames().ToList();
            }
            else if (type == 1)
            {
                names = sCrossSection.GetHSSRecNames().ToList();
            }
            else if (type == 2)
            {
                names = sCrossSection.GetHSSRoundNames().ToList();
            }

            if (index > names.Count - 1)
            {
                index = names.Count - 1;
            }

            this.Message = names[index];

            DA.SetData(0, names[index]);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("4028e4b8-3b7e-4284-bfd1-e32291b4fdfc"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.AISCnames;
            }
        }

    }
}
