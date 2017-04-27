using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace sDataObject.sGeometry
{
    public class sMesh : sGeometryBase
    {
        public List<sVertex> vertices { get; set; }
        public List<sFace> faces { get; set; }
        public string meshName { get; set; }
        public double opacity { get; set; }
        public object data { get; set; }
        
        public sMesh()
        {
            this.vertices = new List<sVertex>();
            this.faces = new List<sFace>();
        }
        
        public sMesh DuplicatesMesh()
        {
            sMesh nm = new sMesh();
            nm.vertices = new List<sVertex>();
            foreach(sVertex v in this.vertices)
            {
                nm.vertices.Add(v.DuplicatesVertex());
            }
            nm.faces = new List<sFace>();
            foreach(sFace f in this.faces)
            {
                nm.faces.Add(f.DuplicatesFace());
            }
            nm.meshName = this.meshName;
            nm.data = this.data;
            nm.opacity = this.opacity;
            return nm;
        }

        public void SetVertex(int vertexID, sXYZ loc)
        {
            sVertex v = new sVertex(vertexID, loc);
            this.vertices.Add(v);
        }

        public void SetVertex(int vertexID, sXYZ loc, sColor col)
        {
            sVertex v = new sVertex(vertexID, loc);
            v.color = col;
            this.vertices.Add(v);
        }

        public void SetVertex(int vertexID, sXYZ loc, object dataIn)
        {
            sVertex v = new sVertex(vertexID, loc);
            v.data = dataIn;
            this.vertices.Add(v);
        }

        public void SetFace(int faceID, int v0ID, int v1ID, int v2ID)
        {
            sFace f = new sFace();
            f.ID = faceID;
            f.A = v0ID;
            f.B = v1ID;
            f.C = v2ID;

            this.vertices[v0ID].faceIndices.Add(faceID);
            this.vertices[v1ID].faceIndices.Add(faceID);
            this.vertices[v2ID].faceIndices.Add(faceID);

            this.faces.Add(f);
        }

        public void SetFace(int faceID1, int faceID2, int v0ID, int v1ID, int v2ID, int v3ID)
        {
            sFace f1 = new sFace();
            f1.ID = faceID1;
            f1.A = v0ID;
            f1.B = v1ID;
            f1.C = v2ID;

            this.vertices[v0ID].faceIndices.Add(faceID1);
            this.vertices[v1ID].faceIndices.Add(faceID1);
            this.vertices[v2ID].faceIndices.Add(faceID1);

            sFace f2 = new sFace();
            f2.ID = faceID2;
            f2.A = v0ID;
            f2.B = v2ID;
            f2.C = v3ID;

            this.vertices[v0ID].faceIndices.Add(faceID2);
            this.vertices[v2ID].faceIndices.Add(faceID2);
            this.vertices[v3ID].faceIndices.Add(faceID2);


            this.faces.Add(f1);
            this.faces.Add(f2);
        }

        public void ComputeNormals()
        {
            foreach(sFace f in this.faces)
            {
                f.ComputeFaceNormal(this);
            }

            foreach(sVertex v in this.vertices)
            {
                v.ComputeNormal(this);
            }
        }

        public static sMesh Objectify(string jsonFile)
        {
            return JsonConvert.DeserializeObject<sMesh>(jsonFile);
        }

        public string Jsonify()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
