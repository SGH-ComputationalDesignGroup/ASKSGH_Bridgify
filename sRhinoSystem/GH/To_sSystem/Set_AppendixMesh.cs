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
    public class Set_AppendixMesh : GH_Component
    {
        public Set_AppendixMesh()
            : base("Append Mesh to sSystem", "Append Mesh to sSystem", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }// | GH_Exposure.obscure; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sghSystem", "sghSystem", "...", GH_ParamAccess.item);
            pManager.AddTextParameter("meshName", "meshName", "...", GH_ParamAccess.item);
            pManager.AddMeshParameter("meshes", "meshes", "...", GH_ParamAccess.list);
            pManager.AddColourParameter("meshColor", "meshColor", "...", GH_ParamAccess.item, System.Drawing.Color.White);


            Params.Input[3].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sghSystem", "sghSystem", "...", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            sSystem ssys = null;
            string meshName = "";
            List<Mesh> ms = new List<Mesh>();
            System.Drawing.Color col = System.Drawing.Color.Empty;


            if (!DA.GetData(0, ref ssys)) return;
            if (!DA.GetData(1, ref meshName)) return;
            if (!DA.GetDataList(2, ms)) return;
            if (!DA.GetData(3, ref col)) return;


            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");

            Mesh bm = new Mesh();
            foreach (Mesh m in ms)
            {
                Mesh ensuredM = rhcon.EnsureUnit(m) as Mesh;
                bm.Append(ensuredM);
            }
            bm.Vertices.CombineIdentical(false, false);

            for (int i = 0; i < bm.Vertices.Count; ++i)
            {
                bm.VertexColors.SetColor(i, col);
            }

            int count = 0;
            List<string> meshNames = new List<string>();
            foreach (sMesh sm in ssys.meshes)
            {
                if (sm.meshName == meshName)
                {
                    count++;
                }
                meshNames.Add(sm.meshName);
            }

            string mms = "Appended Meshes";
            if (count == 0)
            {
                sMesh sm = rhcon.TosMesh(bm);
                sm.opacity = (double)(col.A) / (255.0);
                sm.meshName = meshName;

                ssys.meshes.Add(sm);
            }

            foreach (string mn in meshNames)
            {
                mms += "\n" + mn;
            }
            this.Message = mms;


            DA.SetData(0, ssys);

        }

        public override Guid ComponentGuid
        {
            get { return new Guid("02af2f31-9778-4845-96be-1e011dff69eb"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.appendMesh;
            }
        }

    }
}
