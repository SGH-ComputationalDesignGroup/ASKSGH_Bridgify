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
using Grasshopper.Kernel.Data;
using sRhinoSystem.Properties;

namespace sRhinoSystem.GH.To_sSystem
{
    public class sPointSupportGroupBySupportType : GH_Component
    {
        public sPointSupportGroupBySupportType()
            : base("Group sPointSupports By Support", "Group sPointSupports By Support", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sPointSupports", "sPointSupports", "...", GH_ParamAccess.list);
            // Params.Input[0].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sPointSupportGroup", "sPointSupportGroup", "sPointSupportGroup", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<sPointSupport> sups = new List<sPointSupport>();

            if (!DA.GetDataList(0, sups)) return;

            DataTree<sPointSupport> supTree = new DataTree<sPointSupport>();
              var ngrouped = sups.GroupBy(n => n.supportType);
              int ngroupID = 0;
              foreach (var nngroup in ngrouped)
              {
                  GH_Path npth = new GH_Path(ngroupID);
                  foreach (sPointSupport sn in nngroup)
                  {
                      supTree.Add(sn, npth);
                  }
                  ngroupID++;
              }

            DA.SetDataTree(0, supTree);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("86019200-57f0-4a37-a74e-d0ed5b06ff66"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.groupBySupp;
            }
        }

    }
}
