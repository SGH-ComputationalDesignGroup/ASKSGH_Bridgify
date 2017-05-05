using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject;
using sDataObject.sGeometry;
using sDataObject.sElement;
using Dyn = Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;

using System.Drawing;
using sDataObject.IElement;
using sDataObject.ElementBase;

namespace ASKSGH_Bridgify
{
    public static class to_DynamoObject
    {
        [MultiReturn(new[] { "sghSystem" })]
        public static object Load_sSystem(string hostURL, bool load)
        {
            string url = hostURL + "jsonDataExchange.asmx/ReceiveFromServer";

            if (load)
            {
                try
                {
                    var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    request.Expect = "application/json";

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write("{'GHin':'" + "ask" + "'}");
                        streamWriter.Close();
                    }
                    var httpResponse = (System.Net.HttpWebResponse)request.GetResponse();

                    StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

                    string resp = streamReader.ReadToEnd();

                    sJsonReceiver jj = Newtonsoft.Json.JsonConvert.DeserializeObject<sJsonReceiver>(resp);

                    string jsonSys = jj.d;
                    

                    return new Dictionary<string, object>
                     {
                          { "sghSystem", SystemBase.Objectify(jsonSys) }
                     };
                }
                catch (System.Net.WebException e)
                {
                    string pageContent = new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();

                    return new Dictionary<string, object>
                     {
                          { "sghSystem", pageContent }
                     };
                }
            }
            else
            {
                return new Dictionary<string, object>
                     {
                          { "sghSystem", "" }
                     };
            }
        }
        [MultiReturn(new[] { "areaElements", "beamElements", "pointLoads", "pointSupports", "loadCombos" })]
        public static object To_sElements(sSystem sghSystem)
        {
            List<object> obj0 = new List<object>();
            List<object> obj1 = new List<object>();
            List<object> obj2 = new List<object>();
            List<object> obj3 = new List<object>();
            List<object> obj4 = new List<object>();
            if (sghSystem != null)
            {
                //??????????
                //??????????
                //??????????
                //??????????
                foreach (sFrameSet b in sghSystem.frameSets)
                {
                    obj1.Add(b);
                }

                foreach (sNode n in sghSystem.nodes)
                {
                    if(n.boundaryCondition != null)
                    {
                        obj3.Add(n.boundaryCondition);
                    }
                    if (n.pointLoads != null && n.pointLoads.Count > 0)
                    {
                        foreach(sPointLoad spl in n.pointLoads)
                        {
                            obj2.Add(spl);
                        }
                    }
                }

                if (sghSystem.loadCombinations != null && sghSystem.loadCombinations.Count > 0)
                {
                    foreach (sLoadCombination com in sghSystem.loadCombinations)
                    {
                        obj4.Add(com);
                    }
                }
            }

            return new Dictionary<string, object>
            {
                { "areaElements", obj0 },
                { "beamElements", obj1 },
                { "pointLoads", obj2 },
                { "pointSupports", obj3 },
                { "loadCombos", obj4 }
            };
        }

   //how to ensure unit for mesh?...
 //      [MultiReturn(new[] { "dynamoMesh" })]
 //      public static object To_DynamoMesh(sSystem sghSystem, double deformation = 0.0)
 //      {
 //          object returnobj = null;
 //          List<List<Color>> verticeColor = null;
 //          if (sghSystem != null)
 //          {
 //              sDynamoConverter dyncon = new sDynamoConverter("Meters", "Feet");
 //   
 //              sRange th = null;
 //   
 //              List<Dyn.Mesh> emesh = new List<Dyn.Mesh>();
 //              foreach(Dyn.Mesh dm in dyncon.ToDynamoMesh(sghSystem, eColorMode.Stress_Combined_Absolute, out verticeColor, deformation, th){
 //                  emesh.Add(dyncon.EnsureUnit(dm));
 //              }
 //          }
 //   
 //          return new Dictionary<string, object>
 //          {
 //              { "dynamoMesh", returnobj }
 //          };
 //      }
       [MultiReturn(new[] { "vertice", "indice", "R", "G", "B" })]
       public static object To_DynamoMeshData(sSystem sghSystem, double colorThreshold = 0.0, double deformation = 0.0)
       {
           List<List<Dyn.Point>> vertice = null;
           List<List<int>> faceIndice = null;
           List<List<int>> Rs = null;
           List<List<int>> Gs = null;
           List<List<int>> Bs = null;
           if (sghSystem != null)
           {
               sDynamoConverter dyncon = new sDynamoConverter();
    
               sRange th = null;
               if (colorThreshold > 0.0)
               {
                   th = new sRange(0.0, colorThreshold);
               }
               dyncon.ToDynamoToolKitMeshData(sghSystem, eColorMode.Stress_Combined_Absolute, out vertice, out faceIndice, out Rs, out Gs, out Bs, deformation, th);
           }
    
           return new Dictionary<string, object>
           {
               { "vertice", vertice },
               { "indice", faceIndice },
               { "R", Rs},
               { "G", Gs},
               { "B", Bs}
           };
       }
        [MultiReturn(new[] { "dynamoMesh" })]
        public static object To_DynamoMesh_Appendix(sSystem sghSystem)
        {
            List<Dyn.Mesh> emesh = new List<Dyn.Mesh>();
            List<List<Color>> verticeColor = null;
            if (sghSystem != null)
            {
                sDynamoConverter dyncon = new sDynamoConverter("Meters", "Feet");
                
                foreach(sMesh sm in sghSystem.meshes)
                {
                    List<Color> vcols;
                    emesh.Add(dyncon.ToDynamoMesh(sm, out vcols));
                    verticeColor.Add(vcols);
                }
            }

            return new Dictionary<string, object>
              {
                  { "dynamoMesh", emesh }
              };
        }
        [MultiReturn(new[] { "beamPreview"})]
        public static object To_DynamoObj_sBeamPreview(sFrame beam)
        {
            sDynamoConverter dyncon = new sDynamoConverter("Meters","Feet");
            
            return new Dictionary<string, object>
            {
                { "beamPreview", dyncon.EnsureUnit( dyncon.ToDynamoBeamPreview(beam)) }
            };
        }
        [MultiReturn(new[] { "beamName", "beamID", "beamLine",  "crossSection", "lineLoads" })]
        public static object To_DynamoObj_sBeam(sFrame beam)
        {
             sDynamoConverter dyncon = new sDynamoConverter("Meters","Feet");

            return new Dictionary<string, object>
            {
                { "beamName", beam.frameName },
                { "beamID", beam.frameID },
                { "beamLine", dyncon.EnsureUnit(dyncon.ToDynamoLine(beam.axis)) },
                { "crossSection", beam.crossSection },
                { "lineLoads", beam.lineLoads }
            };
        }
        [MultiReturn(new[] { "loadPattern", "loadType", "loadForce", "loadMoment" })]
        public static object To_DynamoObj_sLineLoad(sLineLoad lineLoad)
        {
            sDynamoConverter dyncon = new sDynamoConverter("Meters", "Feet");
            
            return new Dictionary<string, object>
            {
                { "loadPattern", lineLoad.loadPatternName },
                { "loadType", lineLoad.loadType.ToString() },
                { "loadForce", dyncon.EnsureUnit_Force(dyncon.ToDynamoVector(lineLoad.load_Force)) },
                { "loadMoment", dyncon.EnsureUnit_Force(dyncon.ToDynamoVector(lineLoad.load_Moment)) }
            };
        }

        [MultiReturn(new[] { "beamGroup", })]
        public static object To_GroupBeam_ByName(List<sFrame> beams)
        {
            sDynamoConverter dyncon = new sDynamoConverter("Meters", "Feet");

            List<List<sFrame>> plGroup = new List<List<sFrame>>();
            var grouped = beams.GroupBy(l => l.frameName);
            foreach (var lgroup in grouped)
            {
                List<sFrame> plList = new List<sFrame>();
                foreach (sFrame pl in lgroup)
                {
                    plList.Add(pl);
                }
                plGroup.Add(plList);
            }

            return new Dictionary<string, object>
            {
                { "beamGroup", plGroup }
            };
        }
        [MultiReturn(new[] { "pointLoadGroup", })]
        public static object To_GroupPointLoad_ByPattern(List<sPointLoad> pointLoads)
        {
            sDynamoConverter dyncon = new sDynamoConverter("Meters", "Feet");

            List<List<sPointLoad>> plGroup = new List<List<sPointLoad>>();
            var grouped = pointLoads.GroupBy(l => l.loadPatternName);
            foreach (var lgroup in grouped)
            {
                List<sPointLoad> plList = new List<sPointLoad>();
                foreach (sPointLoad pl in lgroup)
                {
                    plList.Add(pl);
                }
                plGroup.Add(plList);
            }

            return new Dictionary<string, object>
            {
                { "pointLoadGroup", plGroup }
            };
        }
        
        [MultiReturn(new[] { "location", "loadPattern", "forceVec", "momentVec"})]
        public static object To_DynamoObj_sPointLoad(sPointLoad pointLoad)
        {
            sDynamoConverter dyncon = new sDynamoConverter("Meters", "Feet");

            Dyn.Point lp = (Dyn.Point) dyncon.EnsureUnit((dyncon.ToDynamoPoint(pointLoad.location)));
            //Dyn.PolyCurve pc = (Dyn.PolyCurve) dyncon.EnsureUnit((dyncon.ToDynamoPolyCurve(pointLoad.tributaryArea.areaBoundary)));

            Dyn.Vector force = null;
            if(pointLoad.forceVector != null) force = dyncon.ToDynamoVector(dyncon.EnsureUnit(pointLoad).forceVector);
            Dyn.Vector moment = null;
            if (pointLoad.momentVector != null) moment = dyncon.ToDynamoVector(dyncon.EnsureUnit(pointLoad).momentVector);

            return new Dictionary<string, object>
            {
                { "location", lp },
                { "loadPattern", pointLoad.loadPatternName },
                { "forceVec", force },
                { "momentVec", moment}
            };
        }
        [MultiReturn(new[] { "location", "supportType", "constraints", "reactionForce","reactionMoment"})]
        public static object To_DynamoObj_sPointSupport(sPointSupport pointSupport)
        {
            sDynamoConverter dyncon = new sDynamoConverter("Meters", "Feet");

            Dyn.Point lp = (Dyn.Point)dyncon.EnsureUnit((dyncon.ToDynamoPoint(pointSupport.location)));

            Dyn.Vector force = null;
            if (pointSupport.reaction_force != null) force = dyncon.EnsureUnit_Force(dyncon.ToDynamoVector(pointSupport.reaction_force));
            Dyn.Vector moment = null;
            if (pointSupport.reaction_moment != null) moment = dyncon.EnsureUnit_Force(dyncon.ToDynamoVector(pointSupport.reaction_moment));

            return new Dictionary<string, object>
            {
                { "location", lp },
                { "supportType", pointSupport.supportType.ToString() },
                { "constraints", pointSupport.constraints },
                { "reactionForce", force },
                { "reactionMoment", moment },
            };
        }
    }
}
