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
    public class sBeamGroupByName : GH_Component
    {
        public sBeamGroupByName()
            : base("Group sBeamSets By Name", "Group sBeamSets By Name", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSets", "sBeamSets", "...", GH_ParamAccess.list);
            // Params.Input[0].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSetGroups", "sBeamSetGroups", "sBeamSetGroups", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<sBeamSet> beams = new List<sBeamSet>();

            if (!DA.GetDataList(0, beams)) return;

                DataTree<sBeamSet> beamTree = new DataTree<sBeamSet>();
                var grouped = beams.GroupBy(b => b.beamSetName);
                int groupID = 0;
                foreach (var bgroup in grouped)
                {
                    GH_Path bpth = new GH_Path(groupID);
                    foreach (sBeamSet sb in bgroup)
                    {
                        beamTree.Add(sb, bpth);
                    }
                    groupID++;
                }

            DA.SetDataTree(0, beamTree);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("8a9edf91-a86c-4526-8d39-7069e090ef42"); }
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
