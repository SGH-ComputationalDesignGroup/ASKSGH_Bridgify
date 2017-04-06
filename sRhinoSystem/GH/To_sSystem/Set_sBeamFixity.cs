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
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;

namespace sRhinoSystem.GH.To_sSystem
{
    public class Set_sBeamFixity : GH_Component
    {
        public Set_sBeamFixity()
            : base("Set_sBeamFixity", "Set_sBeamFixity", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quarternary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSets", "sBeamSets", "...", GH_ParamAccess.list);
            pManager.AddNumberParameter("searchTolerance", "searchTolerance", "...", GH_ParamAccess.item, 0.005);
            Params.Input[1].Optional = true;
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Fully Fixed", menuChangeMode);
            Menu_AppendItem(menu, "ReleaseMoment At Start", menuChangeMode);
            Menu_AppendItem(menu, "ReleaseMoment At End", menuChangeMode);
            Menu_AppendItem(menu, "ReleaseMoment At Both", menuChangeMode);
            Menu_AppendItem(menu, "Release By Degree of Freedom", menuChangeMode);
        }

        eFixityType fixType = eFixityType.FULLY_FIXED;
        private void menuChangeMode(object sender, EventArgs e)
        {
            var dataIn = sender.ToString();

            if (Params.Input.Count == 3)
            {
                GH_ComponentParamServer.IGH_SyncObject so = Params.EmitSyncObject();
                List<IGH_Param> pars = new List<IGH_Param>();
                pars.AddRange(Params.Input);
                foreach (IGH_Param pi in pars)
                {
                    if (pi.NickName == "DOF Start" || pi.NickName == "DOF End")
                    {
                        pi.IsolateObject();
                        Params.UnregisterInputParameter(pi);
                    }
                }
            }

            eFixityType tempFixType = eFixityType.FULLY_FIXED;

            switch (dataIn)
            {
                case "Fully Fixed":
                    tempFixType = eFixityType.FULLY_FIXED;
                    break;

                case "ReleaseMoment At Start":
                    tempFixType = eFixityType.MOMENTREALESED_START;
                    break;

                case "ReleaseMoment At End":
                    tempFixType = eFixityType.MOMENTREALESED_END;
                    break;

                case "ReleaseMoment At Both":
                    tempFixType = eFixityType.MOMENTREALESED_BOTH;
                    break;

                case "Release By Degree of Freedom":
                    NewBooleanParam("DOF Start", "DOF Start", "", GH_ParamAccess.list);
                    NewBooleanParam("DOF End", "DOF End", "", GH_ParamAccess.list);
                    tempFixType = eFixityType.FIXITIES_BY_DOF;
                    break;
            }

            SetFixType(tempFixType);
        }

        public void SetFixType(eFixityType ftype)
        {
            fixType = ftype;
            this.ExpireSolution(true);
        }

        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("FixType", (int)fixType);
            return base.Write(writer);
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            int ty = (int)eFixityType.FULLY_FIXED;
            reader.TryGetInt32("FixType", ref ty);

            SetFixType((eFixityType)ty);

            return base.Read(reader);
        }

        protected IGH_Param NewBooleanParam(string _name, string _nickname, string _desc, GH_ParamAccess paramAccess)
        {
            Param_Boolean pn = new Param_Boolean();

            pn.Name = _name;
            pn.NickName = _nickname;
            pn.Description = _desc;
            pn.Access = paramAccess;
            pn.SetPersistentData(new GH_Boolean());

            Params.RegisterInputParam(pn);
            return pn;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sBeamSets", "sBeamSets", "sBeamSets", GH_ParamAccess.list);
            pManager.AddPointParameter("ReleasedAtStart", "ReleasedAtStart", "ReleasedAtStart", GH_ParamAccess.list);
            pManager.AddPointParameter("ReleasedAtEnd", "ReleasedAtEnd", "ReleasedAtEnd", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<sBeamSet> bsets = new List<sBeamSet>();
            double tol = 0.005;
            if (!DA.GetDataList(0, bsets)) return;
            if (!DA.GetData(1, ref tol)) return;

            sFixity fxS = null;
            sFixity fxE = null;
            if (fixType == eFixityType.MOMENTREALESED_START)
            {
                fxS = new sFixity();
                fxS.release = new bool[6] { true, true, true, false, false, false };
            }
            else if (fixType == eFixityType.MOMENTREALESED_END)
            {
                fxE = new sFixity();
                fxE.release = new bool[6] { true, true, true, false, false, false };
            }
            else if (fixType == eFixityType.MOMENTREALESED_BOTH)
            {
                fxS = new sFixity();
                fxS.release = new bool[6] { true, true, true, false, false, false };
                fxE = new sFixity();
                fxE.release = new bool[6] { true, true, true, true, false, false };
            }
            else if (fixType == eFixityType.FIXITIES_BY_DOF)
            {
                List<bool> sss = new List<bool>();
                List<bool> eee = new List<bool>();

                if (!DA.GetDataList(2, sss)) return;
                if (!DA.GetDataList(3, eee)) return;


                bool[] fixedSet = new bool[6] { true,true,true,true,true,true};
                if(sss.Count != 6)
                {
                    sss = fixedSet.ToList();
                }
                if (eee.Count != 6)
                {
                    eee = fixedSet.ToList();
                }

                fxS = new sFixity();
                fxS.release = sss.ToArray();
                fxE = new sFixity();
                fxE.release = eee.ToArray();
            }
            else if (fixType == eFixityType.FULLY_FIXED)
            {
                fxS = null;
                fxE = null;
            }
            
            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");
            sRhinoConverter rhcon_ToRhinoModel = new sRhinoConverter("Meters", modelUnit);

            if (modelUnit == "Feet") tol = 0.015;

            List<sBeamSet> duplicated = new List<sBeamSet>();
            List<Point3d> locsStart = new List<Point3d>();
            List<Point3d> locsEnd = new List<Point3d>();

            foreach (sBeamSet bso in bsets)
            {
                sBeamSet bs = bso.DuplicatesBeamSet();

                bs.ResetFixities();

                if(bs.parentSegments.Count > 0)
                {
                    if(fxS != null) bs.segmentFixitiesAtStart = new List<sFixity>();
                    if (fxE != null) bs.segmentFixitiesAtEnd = new List<sFixity>();

                    foreach (sCurve segc in bs.parentSegments)
                    {
                        Curve segrc = rhcon.ToRhinoCurve(segc);
                        if (fxS != null)
                        {
                            fxS.location = rhcon.TosXYZ(segrc.PointAtStart);
                            if (fxS.IsOnLocation(bs.associatedLocations, tol) == false)
                            {
                                bs.segmentFixitiesAtStart.Add(fxS.DuplicatesFixity());
                            }
                        }
                        if (fxE != null)
                        {
                            fxE.location = rhcon.TosXYZ(segrc.PointAtEnd);
                            if(fxE.IsOnLocation(bs.associatedLocations, tol)== false)
                            {
                                bs.segmentFixitiesAtEnd.Add(fxE.DuplicatesFixity());
                            }
                        }
                    }
                    bs.AwareElementFixitiesBySegementFixities(tol);
                }
                else
                {
                    Curve rc = rhcon.ToRhinoCurve(bs.parentCrv);
                    if (fxS != null && bs.parentFixityAtStart == null)
                    {
                        fxS.location = rhcon.TosXYZ(rc.PointAtStart);
                        if (fxS.IsOnLocation(bs.associatedLocations, tol) == false)
                        {
                            bs.parentFixityAtStart = fxS.DuplicatesFixity();
                        }
                    }
                    if (fxE != null && bs.parentFixityAtEnd == null)
                    {
                        fxE.location = rhcon.TosXYZ(rc.PointAtEnd);
                        if(fxE.IsOnLocation(bs.associatedLocations, tol)== false)
                        {
                            bs.parentFixityAtEnd = fxE.DuplicatesFixity();
                        }
                    }
                    bs.AwareElementsFixitiesByParentFixity(tol);
                }

                duplicated.Add(bs);
            }

            foreach(sBeamSet bs in duplicated)
            {
                if(bs.beams.Count == 0)
                {
                    Curve rc = rhcon_ToRhinoModel.EnsureUnit(rhcon.ToRhinoCurve(bs.parentCrv));
                    if (bs.parentFixityAtStart != null)
                    {
                        locsStart.Add(rc.PointAtNormalizedLength(0.025));
                    }
                    if(bs.parentFixityAtEnd != null)
                    {
                        locsEnd.Add(rc.PointAtNormalizedLength(0.975));
                    }
                }
                else
                {
                    foreach (sBeam sb in bs.beams)
                    {
                        if (sb.fixityAtStart != null)
                        {
                            locsStart.Add(rhcon_ToRhinoModel.EnsureUnit(rhcon.ToRhinoPoint3d(sb.axis.PointAt(0.1))));
                        }
                        if (sb.fixityAtEnd != null)
                        {
                            locsEnd.Add(rhcon_ToRhinoModel.EnsureUnit(rhcon.ToRhinoPoint3d(sb.axis.PointAt(0.9))));
                        }
                    }
                }
            }
            

            this.Message = fixType.ToString();

            DA.SetDataList(0, duplicated);
            DA.SetDataList(1, locsStart);
            DA.SetDataList(2, locsEnd);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("fcd14d2d-1627-49d9-8eca-e270c1a2a05d"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.setFixity;
            }
        }

    }
}
