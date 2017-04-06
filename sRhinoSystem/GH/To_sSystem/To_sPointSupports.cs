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
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

namespace sRhinoSystem.GH.To_sSystem
{
    public class To_sPointSupports : GH_Component
    {
        public To_sPointSupports()
            : base("sPointSupport", "sPointSupport", "Right Click To Change Settings", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("points", "points", "...", GH_ParamAccess.list);
          //  pManager.AddIntegerParameter("supportType", "supportType", "...", GH_ParamAccess.item);
            
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "AllFixed", menuChangeMode);
            Menu_AppendItem(menu, "AllPinned", menuChangeMode);
            Menu_AppendItem(menu, "ByConstraints", menuChangeMode);
        }
        eSupportType supType = eSupportType.FIXED;
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
                    if (pi.NickName == "points") continue;
                    pi.IsolateObject();
                    Params.UnregisterInputParameter(pi);
                }
            }

            eSupportType tempSupType = eSupportType.FIXED;

            switch (dataIn)
            {
                case "AllFixed":
                    tempSupType = eSupportType.FIXED;
                    break;

                case "AllPinned":
                    tempSupType = eSupportType.PINNED;
                    break;

                case "ByConstraints":
                    NewBooleanParam("x", "x", "", GH_ParamAccess.item, true);
                    NewBooleanParam("y", "y", "", GH_ParamAccess.item, true);
                    NewBooleanParam("z", "z", "", GH_ParamAccess.item, true);
                    NewBooleanParam("rx", "rx", "", GH_ParamAccess.item, true);
                    NewBooleanParam("ry", "ry", "", GH_ParamAccess.item, true);
                    NewBooleanParam("rz", "rz", "", GH_ParamAccess.item, true);
                    tempSupType = eSupportType.CUSTOM;
                    break;
            }

            SetSupportType(tempSupType);
        }

        public void SetSupportType(eSupportType stype)
        {
            supType = stype;
            this.ExpireSolution(true);
        }

        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("SupType", (int)supType);
            return base.Write(writer);
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            int ty = (int)eSupportType.FIXED;
            reader.TryGetInt32("SupType", ref ty);

            SetSupportType((eSupportType)ty);

            return base.Read(reader);
        }

        protected IGH_Param NewBooleanParam(string _name, string _nickname, string _desc, GH_ParamAccess paramAccess, bool _default)
        {
            Param_Boolean pn = new Param_Boolean();

            pn.Name = _name;
            pn.NickName = _nickname;
            pn.Description = _desc;
            pn.Access = paramAccess;
            pn.SetPersistentData(new GH_Boolean(_default));

            Params.RegisterInputParam(pn);
            return pn;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sPointSupports", "sPointSupports", "sPointSupports", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();

            if (!DA.GetDataList(0, points)) return;

            List<sPointSupport> nodes = new List<sPointSupport>();

            string modelUnit = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            sRhinoConverter rhcon = new sRhinoConverter(modelUnit, "Meters");

            if (supType == eSupportType.FIXED || supType == eSupportType.PINNED)
            {
                for (int i = 0; i < points.Count; ++i)
                {
                    sXYZ sp = rhcon.TosXYZ((Point3d)rhcon.EnsureUnit(points[i]));

                    sPointSupport n = new sPointSupport();
                    n.location = sp;

                    n.supportType = supType;

                    nodes.Add(n);
                }
            }
            else
            {
                bool xx = true;
                bool yy = true;
                bool zz = true;
                bool rxx = true;
                bool ryy = true;
                bool rzz = true;
                if (!DA.GetData(1,ref xx)) return;
                if (!DA.GetData(2, ref yy)) return;
                if (!DA.GetData(3, ref zz)) return;
                if (!DA.GetData(4, ref rxx)) return;
                if (!DA.GetData(5, ref ryy)) return;
                if (!DA.GetData(6, ref rzz)) return;

                foreach (Point3d p in points)
                {
                    sXYZ sp = rhcon.TosXYZ((Point3d)rhcon.EnsureUnit(p));

                    sPointSupport n = new sPointSupport();
                    n.location = sp;
                    n.supportType = supType;

                    n.constraints = new bool[6] {xx,yy,zz,rxx,ryy,rzz };

                    nodes.Add(n);
                }
            }

            if(supType == eSupportType.FIXED)
            {
                this.Message = "ALL FIXED";
            }
            else if(supType == eSupportType.PINNED)
            {
                this.Message = "ALL PINNED";
            }
            else
            {
                this.Message = "By Constraints";
            }

            DA.SetDataList(0, nodes);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("57cde489-fd23-4cfe-ba2b-fa9aa81e7ed5"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.pointSupport;
            }
        }

    }
}
