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
using sDataObject.IElement;

namespace sRhinoSystem.GH.ToRhinoSystem
{
    public class To_RhinoMesh_Appended : GH_Component
    {

        public To_RhinoMesh_Appended()
            : base("sSystem Appendix Mesh", "sSystem Appendix Mesh", "...", "ASKSGH.Bridgify", "To Rhino")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; } // | GH_Exposure.obscure;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sghSystem", "sghSystem", "...", GH_ParamAccess.item);
            
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Meshes", "Meshes", "Meshes", GH_ParamAccess.list);
        }
        
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ISystem sghSystem = null;
            
            if (!DA.GetData(0, ref sghSystem)) return;

            List<Mesh> ms = new List<Mesh>();
            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter("Meters", modelUnit);

            if (sghSystem != null)
            {
                foreach (sMesh sm in sghSystem.meshes)
                {
                    Mesh rm = rhcon.ToRhinoMesh(sm);
                    ms.Add(rhcon.EnsureUnit(rm) as Mesh);
                }
            }
            
            DA.SetDataList(0, ms);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("7c4f3aa5-c187-49e4-9147-9f13ebeb1afb"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.AppendixMeshBox;
            }
        }


    }
}
