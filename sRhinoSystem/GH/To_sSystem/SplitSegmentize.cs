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
using Grasshopper.Kernel.Types;

namespace sRhinoSystem.GH.To_sSystem
{
    public class SplitSegmentize : GH_Component
    {
        public SplitSegmentize()
            : base("Split_Segmentize sBeamSets", "Split_Segmentize sBeamSets", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSets", "sBeamSets", "...", GH_ParamAccess.list);
            pManager.AddGenericParameter("sPointElements", "sPointElements", "...", GH_ParamAccess.list);
            pManager.AddNumberParameter("intersectTolerance", "intersectTolerance", "...", GH_ParamAccess.item, 0.005);
            pManager.AddNumberParameter("segmentLength", "segmentLength", "...", GH_ParamAccess.item, 0.5);
            Params.Input[1].Optional = true;
            Params.Input[2].Optional = true;
            Params.Input[3].Optional = true;
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
            List<object> pele = new List<object>();
            double intTol = 0.005;
            double segTol = 0.5;
            if (!DA.GetDataList(0, beams)) return;
            DA.GetDataList(1, pele);
            if (!DA.GetData(2, ref intTol)) return;
            if (!DA.GetData(3, ref segTol)) return;

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");

            if(modelUnit == "Feet")
            {
                intTol = 0.015;
                segTol = 1.5;
            }

            List<sBeamSet> sets = new List<sBeamSet>();
            foreach (sBeamSet bsori in beams)
            {
                sets.Add(bsori.DuplicatesBeamSet());
            }

            List<object> pelements = new List<object>();
            if(pele.Count == 0)
            {
                pelements = null;
            }
            else
            {
                foreach(object o in pele)
                {
                    GH_ObjectWrapper wap = new GH_ObjectWrapper(o);
                    pelements.Add(wap.Value);
                }
            }

            rhcon.SplitSegmentizesBeamSet(ref sets, intTol, segTol, pelements);

            string groupInfo = "";
            DataTree<sBeamSet> beamTree = new DataTree<sBeamSet>();
            var grouped = sets.GroupBy(b => b.beamSetName);
            int groupID = 0;
            foreach (var bgroup in grouped)
            {
                GH_Path bpth = new GH_Path(groupID);
                groupInfo += "BeamSet" + groupID + ": " + bgroup.ElementAt(0).beamSetName + "\n";
                foreach (sBeamSet sb in bgroup)
                {
                    beamTree.Add(sb, bpth);
                }
                groupID++;
            }

            this.Message = groupInfo;

            DA.SetDataTree(0, beamTree);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("a5527d71-adb5-4a0b-bd60-5c587e2a2908"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.Split_Seg;
            }
        }

    }
}
