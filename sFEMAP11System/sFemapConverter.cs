using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

namespace sFEMAP11System
{
    public class sFemapConverter
    {
        public femap.model feMo { get; set; }
        //igs > wrl

        public sFemapConverter()
        {
            
        }

        public bool InitiateFEmapModel(bool visible = false)
        {
            try
            {
                this.feMo = new femap.model();
                this.feMo.feAppVisible(visible);

                
                return true;
            }
            catch(Exception e)
            {
                return false;
                throw new Exception(e.Message);
            }
        }

        public void DisposeFEmapModel()
        {
            this.feMo.feFileClose(false);
            this.feMo = null;
            //??
            System.GC.Collect();
        }

        public void ExportIGESGeometryFromRhino(Rhino.RhinoDoc rhdoc, List<Guid> guids)
        {
            List<Rhino.DocObjects.RhinoObject> robjs = new List<Rhino.DocObjects.RhinoObject>();
            foreach (Guid gid in guids)
            {
                Rhino.DocObjects.RhinoObject rhobj = rhdoc.Objects.Find(gid);
                rhobj.Select(true);
                robjs.Add(rhobj);
            }
            string path = @"C:\\temp.igs";
            Rhino.RhinoApp.RunScript("_-Export " + path + " _Enter", true);

            foreach (Rhino.DocObjects.RhinoObject rhobj in robjs)
            {
                rhobj.Select(false);
            }
        }

        //????
        public void ImportIGESGeometryToFEMAP11()
        {
            this.feMo.feFileReadIgesAdv(false,"C:\\temp.igs", true, true, true, true, 0, 2);
            
            //this.feMo.feAppSetActiveView(0);

            //this.feMo.feViewAutoscaleAll(0, false);
        }

        public void FindIntersections()
        {
            femap.Set feset = this.feMo.feSet;
            feset.AddAll(femap.zDataType.FT_SOLID);
            
            this.feMo.feSolidIntersect(feset.ID, true);

            feset.clear();
            //?? why Sequential functions doesn't work?..............FUCK
            feset.AddAll(femap.zDataType.FT_CURVE);
            this.feMo.feMeshSizeCurve(feset.ID, 0, 2.0, 1, 12, 2, 0, 0, 1, 2, false);

            feset.clear();
            feset = null;
        }

        public void SetMeshSize(double size)
        {
            femap.Set curveset = this.feMo.feSet;
            curveset.AddAll(femap.zDataType.FT_CURVE);
            this.feMo.feMeshSizeCurve(curveset.ID, 0, size, 1, 12, 2, 0, 0, 1, 2, false);

            //curveset.clear();
            curveset = null;
        }

        public void Meshing()
        {
            femap.Set srfset = this.feMo.feSet;
            srfset.AddAll(femap.zDataType.FT_SURFACE);
            femap.Prop prop = this.feMo.feProp;

            this.feMo.feMeshSurface2(srfset.ID, prop.ID, femap.zTopologyType.FTO_QUAD4, true, false);

            //srfset.clear();
            srfset = null;
            
        }
    }
}
