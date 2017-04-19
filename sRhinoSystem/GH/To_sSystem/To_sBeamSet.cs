using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using sDataObject.sElement;
using sDataObject.sGeometry;
using Grasshopper.Kernel.Types;
using sRhinoSystem.Properties;

namespace sRhinoSystem.GH.To_sSystem
{
    public class To_sBeamSet : GH_Component
    {
        public To_sBeamSet()
            : base("sBeamSet", "sBeamSet", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("beamSetName", "beamSetName", "...", GH_ParamAccess.item);
            pManager.AddCurveParameter("beamCurves", "beamCurves", "...", GH_ParamAccess.list);
            pManager.AddGenericParameter("crossSections", "crossSections", "...", GH_ParamAccess.list);
            pManager.AddGenericParameter("lineLoads", "lineLoads", "...", GH_ParamAccess.list);

            Params.Input[3].Optional = true;
            pManager[3].DataMapping = GH_DataMapping.Flatten;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSets", "sBeamSets", "sBeamSets", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string beamSetName = "";
            List<Curve> beamSetCurves = new List<Curve>();
            List<sCrossSection> crossSections = new List<sCrossSection>();
            List<object> lineLoadObjs = new List<object>();

            if (!DA.GetData(0, ref beamSetName)) return;
            if (!DA.GetDataList(1, beamSetCurves)) return;
            if (!DA.GetDataList(2, crossSections)) return;
            DA.GetDataList(3, lineLoadObjs);


            List<sFrameSet> sets = new List<sFrameSet>();

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");


            int minuteCount = 0;
            for (int i = 0; i < beamSetCurves.Count; ++i)
            {
                if (beamSetCurves[i].GetLength() > 0.005)
                {
                    Curve rc = rhcon.EnsureUnit(beamSetCurves[i]);
                    sCurve setCrv = rhcon.TosCurve(rc);
                    sFrameSet bset = new sFrameSet(setCrv);
                    bset.frameSetName = beamSetName;
                    bset.setId = i;

                    if (crossSections.Count == 1)
                    {
                        bset.crossSection = crossSections[0] as sCrossSection;
                    }
                    else if (crossSections.Count == beamSetCurves.Count)
                    {
                        bset.crossSection = crossSections[i] as sCrossSection;
                    }

                    if (lineLoadObjs.Count > 0)
                    {
                        foreach (object lo in lineLoadObjs)
                        {
                            GH_ObjectWrapper wap = new GH_ObjectWrapper(lo);
                            sLineLoad sl = wap.Value as sLineLoad;
                            if (sl != null)
                            {
                                bset.UpdateLineLoad(sl);
                            }
                            sLineLoadGroup sg = wap.Value as sLineLoadGroup;
                            if (sg != null)
                            {
                                if (sg.loads.Count == beamSetCurves.Count)
                                {
                                    bset.UpdateLineLoad(sg.loads[i]);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                    sets.Add(bset);
                }
                else
                {
                    minuteCount++;
                }
            }

            if (minuteCount > 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, minuteCount + "Beams are too short");
            }
            DA.SetDataList(0, sets);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("b6b7d5a0-613a-481b-90ef-d5ace3547b72"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.TosBeam;
            }
        }
    }
}
