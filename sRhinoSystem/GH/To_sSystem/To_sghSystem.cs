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
using Grasshopper.Kernel.Types;
using sRhinoSystem.Properties;

namespace sRhinoSystem.GH.To_sSystem
{
    public class To_sghSystem : GH_Component
    {
        public To_sghSystem()
            : base("Build sSystem", "Build sSystem", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.quinary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("systemSettings", "systemSettings", "settings", GH_ParamAccess.item);
            pManager.AddGenericParameter("sElements", "sElements", "...", GH_ParamAccess.list);

            Params.Input[0].Optional = true;
            pManager[1].DataMapping = GH_DataMapping.Flatten;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sghSystem", "sghSystem", "sghSystem", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            sSystemSetting sysSetting = null;
            List<object> sElement = new List<object>();

            DA.GetData(0, ref sysSetting);
            if (!DA.GetDataList(1, sElement)) return;


            sRhinoConverter rhcon = new sRhinoConverter();
            string currentUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();

            if (sysSetting == null)
            {
                sysSetting = new sSystemSetting();
                sysSetting.systemOriUnit = currentUnit;

                sysSetting.systemName = "DefaultSetting";
                sysSetting.currentCase = "DEAD";
                sysSetting.currentCheckType = eSystemCheckType.StrengthCheck;

                sysSetting.currentStressThreshold_pascal = 25 * 6894757.28;
                sysSetting.currentDeflectionThreshold_mm = 100;

                sysSetting.mergeTolerance_m = 0.005;
                sysSetting.meshDensity_m = 0.5;
            }

            if (currentUnit == "Meters" || currentUnit == "Feet")
            {
                sSystem jsys = new sSystem();
                jsys.systemSettings = sysSetting;
                List<IsObject> sObjs = new List<IsObject>();

                jsys.loadPatterns.Add("DEAD");

                int supCount = 0;
                int nodeID = 0;

                try
                {
                    foreach (object so in sElement)
                    {
                        GH_ObjectWrapper wap = new GH_ObjectWrapper(so);

                        sFrameSet bs = wap.Value as sFrameSet;
                        if(bs != null)
                        {
                            //jsys.beamSets.Add(bs);
                            jsys.AddsBeamSet(bs);
                            //??
                            sObjs.Add(bs);
                            if(bs.lineLoads != null)
                            {
                                foreach (sLineLoad ll in bs.lineLoads)
                                {
                                    jsys.AwarePatternNames(ll.loadPatternName);
                                }
                            }
                        }

                        sPointLoad pl = wap.Value as sPointLoad;
                        if (pl != null)
                        {
                            if (jsys.UpdateNodeFromPointElement(pl, nodeID))
                            {
                                nodeID++;
                                jsys.AwarePatternNames(pl.loadPatternName);
                            }
                        }

                        sLoadCombination com = wap.Value as sLoadCombination;
                        if (com != null)
                        {
                            jsys.SetLoadCombination(com);
                        }
                    }

                    foreach (object so in sElement)
                    {
                        GH_ObjectWrapper wap = new GH_ObjectWrapper(so);
                        sPointSupport psup = wap.Value as sPointSupport;
                        if (psup != null)
                        {
                            if (jsys.UpdateNodeFromPointElement(psup, nodeID))
                            {
                                nodeID++;
                            }
                            supCount++;
                        }
                    }

                    foreach(sMesh am in jsys.meshes)
                    {
                      //  sObjs.Add(am);
                    }

                    if (supCount > 0)
                    {
                        this.Message = "System : " + sysSetting.systemName + "\nis instantiated";
                        jsys.systemSettings.systemBoundingBox = rhcon.TosBoundingBox(sObjs);

                        //jsys.SystemName = sysSetting.systemName;
                        DA.SetData(0, jsys);
                    }
                    else
                    {
                        this.Message = "System : " + sysSetting.systemName + "\nneeds supports";
                        //jsys.SystemName = sysSetting.systemName;
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, this.Message);
                        DA.SetData(0, null);
                    }
                }
                catch (Exception e)
                {
                    this.Message = "System : " + sysSetting.systemName + "\ninstantiation failed";
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
                    DA.SetData(0, null);
                }

            }
            else
            {
                this.Message = "ASKSGH.Bridgify only works in\n Meters or Feet";
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, this.Message);
                DA.SetData(0, null);
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("215cd9e0-389b-4a33-8783-0cf3fe1247e1"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.setSystem;
            }
        }

    }
}
