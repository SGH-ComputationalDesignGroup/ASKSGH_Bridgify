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
    public class To_sLoadCombination : GH_Component
    {
        public To_sLoadCombination()
            : base("sLoadCombination", "sLoadCombination", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary | GH_Exposure.obscure; }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("comboName", "comboName", "...", GH_ParamAccess.item);
            pManager.AddTextParameter("patternNames", "patternNames", "...", GH_ParamAccess.list);
            pManager.AddNumberParameter("patternFactors", "patternFactors", "...", GH_ParamAccess.list);
        }

        eCombinationType comType = eCombinationType.LinearAdditive;
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            for (int i = 0; i < 5; ++i)
            {
                System.Windows.Forms.ToolStripItem it = menu.Items.Add(((eCombinationType)i).ToString());

                it.MouseDown += new System.Windows.Forms.MouseEventHandler(it_MouseDown);
                it.Tag = (eCombinationType)i;
            }
        }

        void it_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.ToolStripItem it = sender as System.Windows.Forms.ToolStripItem;
            if (it == null) return;

            SetMaterial((eCombinationType)it.Tag);
        }

        public void SetMaterial(eCombinationType ctype)
        {
            comType = ctype;
            this.ExpireSolution(true);
        }

        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("ComType", (int)comType);
            return base.Write(writer);
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            int mt = (int)eCombinationType.LinearAdditive;
            reader.TryGetInt32("ComType", ref mt);
            SetMaterial((eCombinationType)mt);

            return base.Read(reader);
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sLoadCombination", "sLoadCombination", "sLoadCombination", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string comboName = "";
            List<string> patternNames = new List<string>();
            List<double> patternFactors = new List<double>();
            
            if (!DA.GetData(0, ref comboName)) return;
            if (!DA.GetDataList(1, patternNames)) return;
            if (!DA.GetDataList(2, patternFactors)) return;

            sLoadCombination combo = null;

            if(patternNames.Count == patternFactors.Count && patternFactors.Count > 0)
            {
                combo = new sLoadCombination(comboName, comType, patternNames, patternFactors);
                string mss = "Load Combination: " + comboName;
                mss += "\n" + combo.combinationType.ToString();

                for(int i = 0; i < patternNames.Count; ++i)
                {
                    mss += "\n" + patternFactors[i] + " X " + patternNames[i];
                }
                this.Message = mss;
            }
            else
            {
                this.Message = "Please provide same number of data\nfor names and factors";
            }
            
            DA.SetData(0, combo);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("e99cb16e-fd49-4140-aac4-6a5d713fec96"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.combo;
            }
        }

    }
}
