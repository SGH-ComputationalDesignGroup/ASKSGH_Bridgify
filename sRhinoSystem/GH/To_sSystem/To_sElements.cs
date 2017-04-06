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
    public class To_sElements : GH_Component
    {
        public To_sElements()
            : base("Parse sElements", "Parse sElements", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.senary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sghSystem", "sghSystem", "...", GH_ParamAccess.item);
            // Params.Input[0].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSets", "sBeamSets", "sBeamSets", GH_ParamAccess.list);
            pManager.AddGenericParameter("sPointLoads", "sPointLoads", "sPointLoads", GH_ParamAccess.list);
            pManager.AddGenericParameter("sPointSupports", "sPointSupports", "sPointSupports", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            sSystem ssys = null;
            
            if (!DA.GetData(0, ref ssys)) return;

            List<sBeamSet> beams = new List<sBeamSet>();
            List<sPointSupport> sups = new List<sPointSupport>();
            List<sPointLoad> pls = new List<sPointLoad>();

            if (ssys != null)
            {
                sSystem sys = ssys as sSystem;

                foreach (sBeamSet b in sys.beamSets)
                {
                    beams.Add(b);
                }
                foreach (sNode n in sys.nodes)
                {
                    if(n.pointLoads != null && n.pointLoads.Count > 0){
                      foreach(sPointLoad pl in n.pointLoads)
                        {
                            pls.Add(pl);
                        }
                    }
                    if (n.boundaryCondition != null && n.boundaryCondition.supportType != eSupportType.NONE)
                    {
                        sPointSupport sup =  n.boundaryCondition;
                        sups.Add(sup);
                    }
                }
            }

            

            DA.SetDataList(0, beams);

            DA.SetDataList(1, pls);
            DA.SetDataList(2, sups);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("18394a19-fc5a-4de3-b793-1d2cc8fa0a6a"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.ToElements;
            }
        }

    }
}
