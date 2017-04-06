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
    public class sPointLoadGroupByPattern : GH_Component
    {
        public sPointLoadGroupByPattern()
            : base("Group sPointLoad By Pattern", "Group sPointLoad By Pattern", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sPointLoads", "sPointLoads", "...", GH_ParamAccess.list);
            // Params.Input[0].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sPointLoadGroup", "sPointLoadGroup", "sPointLoadGroup", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<sPointLoad> sups = new List<sPointLoad>();

            if (!DA.GetDataList(0, sups)) return;

            DataTree<sPointLoad> supTree = new DataTree<sPointLoad>();
              var ngrouped = sups.GroupBy(n => n.loadPatternName);
              int ngroupID = 0;
              foreach (var nngroup in ngrouped)
              {
                  GH_Path npth = new GH_Path(ngroupID);
                  foreach (sPointLoad sn in nngroup)
                  {
                      supTree.Add(sn, npth);
                  }
                  ngroupID++;
              }

            DA.SetDataTree(0, supTree);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("f57d4a44-1d4f-4cdf-97cb-b00ad2d1b5e0"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.groupByName;
            }
        }

    }
}
