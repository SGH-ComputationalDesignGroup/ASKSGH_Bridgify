using System;
using System.Collections.Generic;
using Rhino.Geometry;
using Grasshopper.Kernel;
using sDataObject.sElement;
using sDataObject.sGeometry;
using Grasshopper.Kernel.Types;
using sRhinoSystem.Properties;
using sDataObject.IElement;
using Grasshopper.Kernel.Parameters;
using sDataObject.sSteelElement;

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

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "NonSpecified", menuChangeMode);
            Menu_AppendItem(menu, "SteelMember", menuChangeMode);
            Menu_AppendItem(menu, "CompositeSteelMember", menuChangeMode);
        }
        int memberType = 0;
        private void menuChangeMode(object sender, EventArgs e)
        {
            var dataIn = sender.ToString();
            if (Params.Input.Count > 1)
            {
                GH_ComponentParamServer.IGH_SyncObject so = Params.EmitSyncObject();
                List<IGH_Param> pars = new List<IGH_Param>();
                pars.AddRange(Params.Input);
                foreach (IGH_Param pi in pars)
                {
                    if (pi.NickName == "MemberType")
                    {
                        pi.IsolateObject();
                        Params.UnregisterInputParameter(pi);
                    }
                }
            }

            int type = 0;
            switch (dataIn)
            {
                case "NonSpecified":
                    type = 0;
                    break;

                case "SteelMember":
                    if (Params.Input.Count == 4)
                    {
                        NewIntParam("MemberType", "MemberType", "0=Beam, 1=Girder, 2=Column", GH_ParamAccess.item, 0);
                    }
                    type = 1;
                    break;

                case "CompositeSteelMember":
                    if (Params.Input.Count == 4)
                    {
                        NewIntParam("MemberType", "MemberType", "0=Beam, 1=Girder, 2=Column", GH_ParamAccess.item, 0);
                    }
                    type = 2;
                    break;
            }
            
            SetSupportType(type);
        }

        public void SetSupportType(int stype)
        {
            memberType = stype;
            this.ExpireSolution(true);
        }

        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("MemType", memberType);
            return base.Write(writer);
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            int ty = 0;
            reader.TryGetInt32("MemType", ref ty);

            SetSupportType(ty);

            return base.Read(reader);
        }

        protected IGH_Param NewIntParam(string _name, string _nickname, string _desc, GH_ParamAccess paramAccess, int _default)
        {
            Param_Integer pn = new Param_Integer();

            pn.Name = _name;
            pn.NickName = _nickname;
            pn.Description = _desc;
            pn.Access = paramAccess;
            pn.SetPersistentData(new GH_Integer(_default));

            Params.RegisterInputParam(pn);
            
            return pn;
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



            List<IFrameSet> sets = new List<IFrameSet>();

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");


            int minuteCount = 0;
            for (int i = 0; i < beamSetCurves.Count; ++i)
            {
                if (beamSetCurves[i].GetLength() > 0.005)
                {
                    Curve rc = rhcon.EnsureUnit(beamSetCurves[i]);
                    sCurve setCrv = rhcon.TosCurve(rc);
                    IFrameSet bset = null;
                    if (memberType == 0)
                    {
                        bset = new sFrameSet(setCrv);
                    }
                    else if (memberType == 1)
                    {
                        bset = new sSteelFrameSet(setCrv);
                    }
                    else if(memberType == 2)
                    {
                        bset = new sSteelFrameSet(setCrv, true);
                    }
                    
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
