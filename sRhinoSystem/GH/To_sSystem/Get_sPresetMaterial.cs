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

namespace sRhinoSystem.GH.To_sSystem
{
    public class Get_sPresetMaterial : GH_Component
    {
        public Get_sPresetMaterial()
            : base("sPresetMaterial", "sPresetMaterial", "Right click to change material", "ASKSGH.Bridgify", "To sSystem")
        {
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Steel(Typical)", menuChangeMode);
            Menu_AppendItem(menu, "Aluminum(Typical)", menuChangeMode);
            Menu_AppendItem(menu, "Concrete(Typical)", menuChangeMode);
            Menu_AppendItem(menu, "OAK(Typical)", menuChangeMode);
            Menu_AppendItem(menu, "CarbonFRP(Typical)", menuChangeMode);
            Menu_AppendItem(menu, "StainlessSteel(Typical)", menuChangeMode);

            Menu_AppendItem(menu,"STEEL_A36",menuChangeMode);
            Menu_AppendItem(menu,"STEEL_A53GrB",menuChangeMode);
            Menu_AppendItem(menu,"STEEL_A500GrB_Fy42",menuChangeMode);
            Menu_AppendItem(menu,"STEEL_A500GrB_Fy46",menuChangeMode);
            Menu_AppendItem(menu,"STEEL_A572Gr50",menuChangeMode);
            Menu_AppendItem(menu,"STEEL_A913Gr50",menuChangeMode);
            Menu_AppendItem(menu,"STEEL_A992_Fy50",menuChangeMode);
            Menu_AppendItem(menu,"CONCRETE_FC3000_NORMALWEIGHT",menuChangeMode);
            Menu_AppendItem(menu,"CONCRETE_FC4000_NORMALWEIGHT",menuChangeMode);
            Menu_AppendItem(menu,"CONCRETE_FC5000_NORMALWEIGHT",menuChangeMode);
            Menu_AppendItem(menu,"CONCRETE_FC6000_NORMALWEIGHT",menuChangeMode);
            Menu_AppendItem(menu,"CONCRETE_FC3000_LIGHTWEIGHT",menuChangeMode);
            Menu_AppendItem(menu,"CONCRETE_FC4000_LIGHTWEIGHT",menuChangeMode);
            Menu_AppendItem(menu,"CONCRETE_FC5000_LIGHTWEIGHT",menuChangeMode);
            Menu_AppendItem(menu,"CONCRETE_FC6000_LIGHTWEIGHT",menuChangeMode);
            Menu_AppendItem(menu,"ALUMINUM_6061_T6",menuChangeMode);
            Menu_AppendItem(menu,"ALUMINUM_6063_T6",menuChangeMode);
            Menu_AppendItem(menu,"ALUMINUM_5052_H34",menuChangeMode);
            Menu_AppendItem(menu,"COLDFORMED_Grade_33",menuChangeMode);
            Menu_AppendItem(menu,"COLDFORMED_Grade_50",menuChangeMode);
        }
        string typicalName = "Typical Steel" + "\nSteel A992 Fy50";
        eMaterialType matType = eMaterialType.STEEL_A992_Fy50;
        private void menuChangeMode(object sender, EventArgs e)
        {
            var dataIn = sender.ToString();
            if(dataIn == "Steel(Typical)")
            {
                matType = eMaterialType.STEEL_A992_Fy50;
                typicalName = "Typical Steel" + "\nSteel A992 Fy50";
            }
            else if(dataIn == "Aluminum(Typical)")
            {
                matType = eMaterialType.ALUMINUM_6061_T6;
                typicalName = "Typical Aluminum" + "\nAluminum 6061 T6";
            }
            else if (dataIn == "Concrete(Typical)")
            {
                matType = eMaterialType.CONCRETE_FC3000_NORMALWEIGHT;
                typicalName = "Typical Concrete" + "\nConcrete FC3000 NormalWeight";
            }
            else if (dataIn == "OAK(Typical)")
            {
                matType = eMaterialType.OAK_TYP;
                typicalName = "Typical OAK";
            }
            else if (dataIn == "CarbonFRP(Typical)")
            {
                matType = eMaterialType.CARBONFRP_TYP;
                typicalName = "Typical CarbonFRP";
            }
            else if (dataIn == "StainlessSteel(Typical)")
            {
                matType = eMaterialType.STAINLESSSTEEL_TYP;
                typicalName = "Typical StainlessSteel";
            }
            else if (dataIn == "STEEL_A36")
            {
                matType = eMaterialType.STEEL_A36;
                typicalName = matType.ToString();
            }
            else if (dataIn == "STEEL_A53GrB")
            {
                matType = eMaterialType.STEEL_A53GrB;
                typicalName = matType.ToString();
            }
            else if (dataIn == "STEEL_A500GrB_Fy42")
            {
                matType = eMaterialType.STEEL_A500GrB_Fy42;
                typicalName = matType.ToString();
            }
            else if (dataIn == "STEEL_A500GrB_Fy46")
            {
                matType = eMaterialType.STEEL_A500GrB_Fy46;
                typicalName = matType.ToString();
            }
            else if (dataIn == "STEEL_A572Gr50")
            {
                matType = eMaterialType.STEEL_A572Gr50;
                typicalName = matType.ToString();
            }
            else if (dataIn == "STEEL_A913Gr50")
            {
                matType = eMaterialType.STEEL_A913Gr50;
                typicalName = matType.ToString();
            }
            else if (dataIn == "STEEL_A992_Fy50")
            {
                matType = eMaterialType.STEEL_A992_Fy50;
                typicalName = matType.ToString();
            }
            else if (dataIn == "CONCRETE_FC3000_NORMALWEIGHT")
            {
                matType = eMaterialType.CONCRETE_FC3000_NORMALWEIGHT;
                typicalName = matType.ToString();
            }
            else if (dataIn == "CONCRETE_FC4000_NORMALWEIGHT")
            {
                matType = eMaterialType.CONCRETE_FC4000_NORMALWEIGHT;
                typicalName = matType.ToString();
            }
            else if (dataIn == "CONCRETE_FC5000_NORMALWEIGHT")
            {
                matType = eMaterialType.CONCRETE_FC5000_NORMALWEIGHT;
                typicalName = matType.ToString();
            }
            else if (dataIn == "CONCRETE_FC6000_NORMALWEIGHT")
            {
                matType = eMaterialType.CONCRETE_FC6000_NORMALWEIGHT;
                typicalName = matType.ToString();
            }
            else if (dataIn == "CONCRETE_FC3000_LIGHTWEIGHT")
            {
                matType = eMaterialType.CONCRETE_FC3000_LIGHTWEIGHT;
                typicalName = matType.ToString();
            }
            else if (dataIn == "CONCRETE_FC4000_LIGHTWEIGHT")
            {
                matType = eMaterialType.CONCRETE_FC4000_LIGHTWEIGHT;
                typicalName = matType.ToString();
            }
            else if (dataIn == "CONCRETE_FC5000_LIGHTWEIGHT")
            {
                matType = eMaterialType.CONCRETE_FC5000_LIGHTWEIGHT;
                typicalName = matType.ToString();
            }
            else if (dataIn == "CONCRETE_FC6000_LIGHTWEIGHT")
            {
                matType = eMaterialType.CONCRETE_FC6000_LIGHTWEIGHT;
                typicalName = matType.ToString();
            }
            else if (dataIn == "ALUMINUM_6061_T6")
            {
                matType = eMaterialType.ALUMINUM_6061_T6;
                typicalName = matType.ToString();
            }
            else if (dataIn == "ALUMINUM_6063_T6")
            {
                matType = eMaterialType.ALUMINUM_6063_T6;
                typicalName = matType.ToString();
            }
            else if (dataIn == "ALUMINUM_5052_H34")
            {
                matType = eMaterialType.ALUMINUM_5052_H34;
                typicalName = matType.ToString();
            }
            else if (dataIn == "COLDFORMED_Grade_33")
            {
                matType = eMaterialType.COLDFORMED_Grade_33;
                typicalName = matType.ToString();
            }
            else if (dataIn == "COLDFORMED_Grade_50")
            {
                matType = eMaterialType.COLDFORMED_Grade_50;
                typicalName = matType.ToString();
            }

            RecordUndoEvent("Constant changed");
            this.ExpireSolution(true);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sMaterial", "sMaterial", "sMaterial", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            sMaterial mat = new sMaterial(typicalName, matType);

            string mss = typicalName;
            if(matType == eMaterialType.OAK_TYP)
            {
                mss += "\nUse in Extra Cautious";
            }

            this.Message = mss;

            DA.SetData(0, mat);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("013b3a1c-8d9a-4f2f-9fa3-142a4ef2620d"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.materials;
            }
        }

    }
}
