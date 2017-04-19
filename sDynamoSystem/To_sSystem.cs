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

namespace ASKSGH_Bridgify
{
    public static class to_sSystemObject
    {
        [MultiReturn(new[] { "shapeName" })]
        public static object Get_sAISCName(int type, int index)
        {
            List<string> names = new List<string>();

            sCrossSection cs = new sCrossSection();

            if (type == 0)
            {
                names = cs.GetWBeamNames().ToList();
            }
            else if (type == 1)
            {
                names = cs.GetHSSRecNames().ToList();
            }
            else if (type == 2)
            {
                names = cs.GetHSSRoundNames().ToList();
            }

            if (index > names.Count - 1)
            {
                index = names.Count - 1;
            }


            return new Dictionary<string, object>
            {
                { "shapeName", names[index] }
            };
        }
        [MultiReturn(new[] { "sCrossSection" })]
        public static object Get_sAISCSection(string shapeName)
        {
            sCrossSection cs = new sCrossSection(shapeName);

            sMaterial mat = new sMaterial();
            mat.materialName = "Steel";
            mat.materialType = eMaterialType.STEEL_A36;
            cs.material = mat;

            return new Dictionary<string, object>
            {
                { "sCrossSection", cs }
            };
        }
        [MultiReturn(new[] { "materialName", "sMaterial" })]
        public static object Get_sPresetMaterial(int materialType = 0)
        {
            string typicalName = "Typical Steel" + "\nSteel A992 Fy50";
            eMaterialType matType = eMaterialType.STEEL_A992_Fy50;

            if (materialType == 0)
            {
                matType = eMaterialType.STEEL_A992_Fy50;
                typicalName = "Typical Steel" + "\nSteel A992 Fy50";
            }
            else if (materialType == 1)
            {
                matType = eMaterialType.ALUMINUM_6061_T6;
                typicalName = "Typical Aluminum" + "\nAluminum 6061 T6";
            }
            else if (materialType == 2)
            {
                matType = eMaterialType.CONCRETE_FC3000_NORMALWEIGHT;
                typicalName = "Typical Concrete" + "\nConcrete FC3000 NormalWeight";
            }
            else if (materialType == 3)
            {
                matType = eMaterialType.OAK_TYP;
                typicalName = "Typical OAK";
            }
            else if (materialType == 4)
            {
                matType = eMaterialType.CARBONFRP_TYP;
                typicalName = "Typical CarbonFRP";
            }
            else if (materialType == 5)
            {
                matType = eMaterialType.STAINLESSSTEEL_TYP;
                typicalName = "Typical StainlessSteel";
            }
            else if (materialType ==6)
            {
                matType = eMaterialType.STEEL_A36;
                typicalName = matType.ToString();
            }
            else if (materialType == 7)
            {
                matType = eMaterialType.STEEL_A53GrB;
                typicalName = matType.ToString();
            }
            else if (materialType == 8)
            {
                matType = eMaterialType.STEEL_A500GrB_Fy42;
                typicalName = matType.ToString();
            }
            else if (materialType == 9)
            {
                matType = eMaterialType.STEEL_A500GrB_Fy46;
                typicalName = matType.ToString();
            }
            else if (materialType == 10)
            {
                matType = eMaterialType.STEEL_A572Gr50;
                typicalName = matType.ToString();
            }
            else if (materialType == 11)
            {
                matType = eMaterialType.STEEL_A913Gr50;
                typicalName = matType.ToString();
            }
            else if (materialType == 12)
            {
                matType = eMaterialType.STEEL_A992_Fy50;
                typicalName = matType.ToString();
            }
            else if (materialType == 13)
            {
                matType = eMaterialType.CONCRETE_FC3000_NORMALWEIGHT;
                typicalName = matType.ToString();
            }
            else if (materialType ==14)
            {
                matType = eMaterialType.CONCRETE_FC4000_NORMALWEIGHT;
                typicalName = matType.ToString();
            }
            else if (materialType ==15)
            {
                matType = eMaterialType.CONCRETE_FC5000_NORMALWEIGHT;
                typicalName = matType.ToString();
            }
            else if (materialType == 16)
            {
                matType = eMaterialType.CONCRETE_FC6000_NORMALWEIGHT;
                typicalName = matType.ToString();
            }
            else if (materialType == 17)
            {
                matType = eMaterialType.CONCRETE_FC3000_LIGHTWEIGHT;
                typicalName = matType.ToString();
            }
            else if (materialType == 18)
            {
                matType = eMaterialType.CONCRETE_FC4000_LIGHTWEIGHT;
                typicalName = matType.ToString();
            }
            else if (materialType == 19)
            {
                matType = eMaterialType.CONCRETE_FC5000_LIGHTWEIGHT;
                typicalName = matType.ToString();
            }
            else if (materialType == 20)
            {
                matType = eMaterialType.CONCRETE_FC6000_LIGHTWEIGHT;
                typicalName = matType.ToString();
            }
            else if (materialType == 21)
            {
                matType = eMaterialType.ALUMINUM_6061_T6;
                typicalName = matType.ToString();
            }
            else if (materialType == 22)
            {
                matType = eMaterialType.ALUMINUM_6063_T6;
                typicalName = matType.ToString();
            }
            else if (materialType == 23)
            {
                matType = eMaterialType.ALUMINUM_5052_H34;
                typicalName = matType.ToString();
            }
            else if (materialType == 24)
            {
                matType = eMaterialType.COLDFORMED_Grade_33;
                typicalName = matType.ToString();
            }
            else if (materialType == 25)
            {
                matType = eMaterialType.COLDFORMED_Grade_50;
                typicalName = matType.ToString();
            }

            sMaterial mat = new sMaterial(typicalName, matType);

            string mss = typicalName;
            if (matType == eMaterialType.OAK_TYP)
            {
                mss += "\nUse in Extra Cautious";
            }
        
            //add more...

            return new Dictionary<string, object>
            {
                { "materialName", mss  },
                { "sMaterial", mat }
            };
        }
        [MultiReturn(new[] { "sCrossSection" })]
        public static object Get_sRectangularSection(sMaterial material, double width_in, double depth_in, double thickness_in = 0.0)
        {
            sCrossSection cs = new sCrossSection();
            string shapeN = "";
            if (Math.Abs(width_in - depth_in) < 0.0001)
            {
                cs.sectionType = eSectionType.SQUARE;
                shapeN += "Square_" + width_in + "x" + depth_in;
            }
            else
            {
                cs.sectionType = eSectionType.RECTANGLAR;
                shapeN += "Rectangular_" + width_in + "x" + depth_in;
            }
            if (thickness_in > 0.0)
            {
                shapeN += "x" + thickness_in;
            }

                sDynamoConverter dycon = new sDynamoConverter("Feet", "Meters");
            
            width_in /= 12.0;
            depth_in /= 12.0;
            thickness_in /= 12.0;


            width_in = Math.Round(width_in, 3);
            depth_in = Math.Round(depth_in, 3);
            thickness_in = Math.Round(thickness_in, 3);

            cs.dimensions = new List<double>();
            cs.dimensions.Add(dycon.EnsureUnit_Dimension(width_in));
            cs.dimensions.Add(dycon.EnsureUnit_Dimension(depth_in));
            if (thickness_in > 0.0)
            {
                cs.dimensions.Add(dycon.EnsureUnit_Dimension(thickness_in));
            }

            cs.shapeName = shapeN;

            cs.material = material;

            return new Dictionary<string, object>
            {
                { "sCrossSection", cs }
            };
        }
        [MultiReturn(new[] { "sCrossSection" })]
        public static object Get_sRoundSection(sMaterial material, double diameter_in, double thickness_in = 0.0)
        {
            
            sCrossSection cs = new sCrossSection();

            eSectionType stype = eSectionType.ROUND;

            cs.sectionType = stype;

            string shapeN = "Round_" + diameter_in;
            if (thickness_in > 0.0)
            {
                shapeN += "x" + thickness_in;
            }

            cs.shapeName = shapeN;
            sDynamoConverter dycon = new sDynamoConverter("Feet", "Meters");
      
            diameter_in /= 12.0;
            thickness_in /= 12.0;
            

            diameter_in = Math.Round(diameter_in, 3);
            thickness_in = Math.Round(thickness_in, 3);

            cs.dimensions = new List<double>();
            cs.dimensions.Add(dycon.EnsureUnit_Dimension(diameter_in));
            if (thickness_in > 0.0)
            {
                cs.dimensions.Add(dycon.EnsureUnit_Dimension(thickness_in));
            }
            

            cs.material = material;

            return new Dictionary<string, object>
            {
                { "sCrossSection", cs }
            };
        }
        
        [MultiReturn(new[] { "sBeams" })]
        public static object sBeamElement(string beamName, List<Dyn.Curve> beamLines, List<sCrossSection> crossSections)
        {

            List<sFrame> beams = new List<sFrame>();
            
            sDynamoConverter dycon = new sDynamoConverter("Feet", "Meters");

            for (int i = 0; i < beamLines.Count; ++i)
            {
                double len = beamLines[i].Length;
                if (len > 0.005)
                {
                    sNode jn0 = new sNode();
                    jn0.location = dycon.TosXYZ((Dyn.Point)dycon.EnsureUnit(beamLines[i].StartPoint));
                    sNode jn1 = new sNode();
                    jn1.location = dycon.TosXYZ((Dyn.Point)dycon.EnsureUnit(beamLines[i].EndPoint));

                    sFrame jb = new sFrame(jn0, jn1, sXYZ.Zaxis());
                    jb.frameName = beamName;
                    jb.frameID = i;

                    if (crossSections.Count == 1)
                    {
                        jb.crossSection = crossSections[0] as sCrossSection;
                    }
                    else if (crossSections.Count == beamLines.Count)
                    {
                        jb.crossSection = crossSections[i];
                    }
                    
                    beams.Add(jb);
                }
            }

            return new Dictionary<string, object>
            {
                { "sBeams", beams }
            };
        }
        [MultiReturn(new[] { "sBeams" })]
        public static object sBeamElement_Load(string beamName, List<Dyn.Curve> beamLines, List<sCrossSection> crossSections, List<object> lineLoads)
        {

            List<sFrame> beams = new List<sFrame>();

            sDynamoConverter dycon = new sDynamoConverter("Feet", "Meters");

            for (int i = 0; i < beamLines.Count; ++i)
            {
                double len = beamLines[i].Length;
                if (len > 0.005)
                {
                    sNode jn0 = new sNode();
                    jn0.location = dycon.TosXYZ((Dyn.Point)dycon.EnsureUnit(beamLines[i].StartPoint));
                    sNode jn1 = new sNode();
                    jn1.location = dycon.TosXYZ((Dyn.Point)dycon.EnsureUnit(beamLines[i].EndPoint));

                    sFrame jb = new sFrame(jn0, jn1, sXYZ.Zaxis());
                    jb.frameName = beamName;
                    jb.frameID = i;

                    if (crossSections.Count == 1)
                    {
                        jb.crossSection = crossSections[0] as sCrossSection;
                    }
                    else if (crossSections.Count == beamLines.Count)
                    {
                        jb.crossSection = crossSections[i];
                    }


                    if (lineLoads.Count > 0)
                    {
                        foreach (object lo in lineLoads)
                        {
                            sLineLoad sl = lo as sLineLoad;
                            if (sl != null)
                            {
                                jb.UpdateLineLoad(sl);
                            }
                            sLineLoadGroup sg = lo as sLineLoadGroup;
                            if (sg != null)
                            {
                                if (sg.loads.Count == beamLines.Count)
                                {
                                    jb.UpdateLineLoad(sg.loads[i]);
                                }
                            }
                        }
                    }

                    beams.Add(jb);
                }
            }

            return new Dictionary<string, object>
            {
                { "sBeams", beams }
            };
        }
        [MultiReturn(new[] { "sLineLoad" })]
        public static object sLineLoad(string patternName, List<Dyn.Vector> forceVectors_plf)
        {
            sDynamoConverter rhcon = new sDynamoConverter("Feet", "Meters");

            object outobj = null;
            if (forceVectors_plf.Count == 1)
            {
                sLineLoad l_before = new sLineLoad(patternName, eLoadType.DistributedLoad, true, rhcon.TosXYZ(forceVectors_plf[0]));
                outobj = rhcon.EnsureUnit(l_before);
            }
            else if (forceVectors_plf.Count > 1)
            {
                sLineLoadGroup lg = new sLineLoadGroup();
                lg.loads = new List<sLineLoad>();
                foreach (Dyn.Vector lv in forceVectors_plf)
                {
                    sLineLoad sl = new sLineLoad(patternName, eLoadType.DistributedLoad, true, rhcon.TosXYZ(lv));
                    lg.loads.Add(rhcon.EnsureUnit(sl));
                }
                outobj = lg;
            }

            return new Dictionary<string, object>
            {
                { "sLineLoad", outobj }
            };
        }
        [MultiReturn(new[] { "sPointLoad" })]
        public static object sPointLoad(string patternName, List<Dyn.Point> point, List<Dyn.Vector> forceVector_lbf)
        {
            sDynamoConverter dycon = new sDynamoConverter("Feet", "Meters");

            List<sPointLoad> pls = new List<sPointLoad>();
            if(point.Count == forceVector_lbf.Count)
            {
                for(int i = 0; i < point.Count; ++i)
                {
                    sPointLoad pl = new sPointLoad();
                    pl.location = dycon.TosXYZ((Dyn.Point)dycon.EnsureUnit(point[i]));

                    pl.loadPatternName = patternName;

                    pl.forceVector = dycon.TosXYZ(forceVector_lbf[i]);
                    pls.Add(dycon.EnsureUnit(pl));
                }
            }
            else
            {
                if(forceVector_lbf.Count == 1)
                {
                    for (int i = 0; i < point.Count; ++i)
                    {
                        sPointLoad pl = new sPointLoad();
                        pl.location = dycon.TosXYZ((Dyn.Point)dycon.EnsureUnit(point[i]));

                        pl.loadPatternName = patternName;

                        pl.forceVector = dycon.TosXYZ(forceVector_lbf[0]);
                        pls.Add(dycon.EnsureUnit(pl));
                    }
                }
            }
            

            return new Dictionary<string, object>
            {
                { "sPointLoad", pls }
            };
        }

        [MultiReturn(new[] { "sLoadCombination", "ComboInfo" })]
        public static object sLoadCombination(int comboType, string comboName, List<string> patternNames, List<double> patternFactors)
        {
            sDynamoConverter dycon = new sDynamoConverter("Feet", "Meters");

            sLoadCombination combo = null;

            string mss = "Load Combination: " + comboName;
            if (patternNames.Count == patternFactors.Count && patternFactors.Count > 0)
            {
                eCombinationType type = eCombinationType.LinearAdditive;
                if (comboType == 0)
                {
                    type = eCombinationType.LinearAdditive;
                }
                else if (comboType == 1)
                {
                    type = eCombinationType.AbsoluteAdditive;
                }
                else if (comboType == 2)
                {
                    type = eCombinationType.RangeAdditive;
                }
                else if (comboType == 3)
                {
                    type = eCombinationType.Envelope;
                }
                else
                {
                    type = eCombinationType.SRSS;
                }

                combo = new sLoadCombination(comboName, type, patternNames, patternFactors);

                mss += "\n" + combo.combinationType.ToString();

                for (int i = 0; i < patternNames.Count; ++i)
                {
                    mss += "\n" + patternFactors[i] + " X " + patternNames[i];
                }
            }


            return new Dictionary<string, object>
            {
                { "sLoadCombination", combo },
                { "ComboInfo", mss}
            };
        }
        [MultiReturn(new[] { "sPointSupport" })]
        public static object sPointSupport(List<Dyn.Point> points, int supportType, string nodeName = "")
        {
            List<sPointSupport> nodes = new List<sPointSupport>();
            
            sDynamoConverter rhcon = new sDynamoConverter("Feet", "Meters");

            if (supportType == 0 || supportType == 1)
            {
                for (int i = 0; i < points.Count; ++i)
                {
                    sXYZ sp = rhcon.TosXYZ((Dyn.Point)rhcon.EnsureUnit(points[i]));

                    sPointSupport n = new sPointSupport();
                    n.location = sp;

                    if (supportType == 0)
                    {
                        n.supportType = eSupportType.FIXED;
                    }
                    else if (supportType == 1)
                    {
                        n.supportType = eSupportType.PINNED;
                    }

                    nodes.Add(n);
                }
            }

            return new Dictionary<string, object>
            {
                { "sPointSupport", nodes }
            };
        }
        [MultiReturn(new[] { "sFixity" })]
        public static object sFixity(Dyn.Point location, bool momentRelease)
        {
  
            sDynamoConverter dycon = new sDynamoConverter("Feet", "Meters");

            sFixity sf = new sFixity();
            sf.location = dycon.TosXYZ((Dyn.Point)dycon.EnsureUnit(location));

            if (momentRelease)
            {
                sf.release = new bool[6] { true, true, true, false, false, false };
            }

            return new Dictionary<string, object>
            {
                { "sFixity", sf }
            };
        }

        [MultiReturn(new[] { "sSystemSettings" })]
        public static object sSystemSettings(string systemName, string defaultLoadCase = "DEAD", int defaultCheckType = 0, double StressThreshold_ksi =25.0, double DeflectionThreshold_in = 6.0)
        {
            sSystemSetting set = new sSystemSetting();
            set.systemName = systemName;
            set.currentCase = defaultLoadCase;

            if (defaultCheckType == 1)
            {
                set.currentCheckType = eSystemCheckType.StiffnessCheck;
            }
            else
            {
                set.currentCheckType = eSystemCheckType.StrengthCheck;
            }

            set.currentStressThreshold_pascal = StressThreshold_ksi * 6894757.28;
            set.currentDeflectionThreshold_mm = DeflectionThreshold_in * 25.4;

            return new Dictionary<string, object>
            {
                { "sSystemSettings", set }
            };
        }
        [MultiReturn(new[] { "sGeometrySettings" })]
        public static object sGeometrySettings(bool splitBeamsByBeams = false, bool splitBeamsByNodes = false, double polylineTolerance_in = 0.0, double mergeTolerance_in = 0.0, double meshDensity_in = 0.0)
        {
            sGeometrySetting set = new sGeometrySetting();
            sDynamoConverter dycon = new sDynamoConverter("Feet", "Meters");

            set.systemOriUnit = "Feet";
            set.splitBeamsByBeams = false;
            set.splitBeamsByNodes = false;
            if (polylineTolerance_in > 0) set.polylineTolerance_m = dycon.EnsureUnit_Dimension(polylineTolerance_in/12.0);
            if (mergeTolerance_in > 0) set.mergeTolerance_m = dycon.EnsureUnit_Dimension(mergeTolerance_in/12.0);
            if (meshDensity_in > 0) set.meshDensity_m = dycon.EnsureUnit_Dimension(meshDensity_in/12.0);
            

            return new Dictionary<string, object>
            {
                { "sGeometrySettings", set }
            };
        }

        [MultiReturn(new[] { "sSystem" })]
        public static object Build_sSystem(List<object> sElements,object systemSettings = null, object geometrySettings = null)
        {
            sSystemSetting sysSet = null;
            sGeometrySetting geoSet = null;
            
            if (systemSettings == null)
            {
                sysSet = new sSystemSetting();
                sysSet.currentCase = "DEAD";
                sysSet.currentCheckType = 0;
                sysSet.currentStressThreshold_pascal = 20 * 6.895E6;
                sysSet.currentDeflectionThreshold_mm = 100;
            }
            else
            {
                sysSet = systemSettings as sSystemSetting;
            }

            if (geometrySettings == null)
            {
                geoSet = new sGeometrySetting();
            }
            else
            {
                geoSet = geometrySettings as sGeometrySetting;
            }

            sSystem jsys = new sSystem();
            jsys.systemSettings = sysSet;
            jsys.geometrySettings = geoSet;
            List<IsObject> sobjs = new List<IsObject>();
            sDynamoConverter dycon = new sDynamoConverter();

            jsys.loadPatterns.Add("DEAD");

            int supCount = 0;
            int nodeID = 0;

            foreach (object so in sElements)
            {
                sFrameSet sb = so as sFrameSet;
                if (sb != null)
                {
                    jsys.frameSets.Add(sb);
                    sobjs.Add(sb);

                    if (sb.lineLoads != null)
                    {
                        foreach (sLineLoad ll in sb.lineLoads)
                        {
                            jsys.AwarePatternNames(ll.loadPatternName);
                        }
                    }
                    continue;
                }

                sPointSupport psup = so as sPointSupport;
                if (psup != null)
                {
                    if (jsys.UpdateNodeFromPointElement(psup, nodeID))
                    {
                        nodeID++;
                    }
                    supCount++;
                    continue;
                }

                sPointLoad pl = so as sPointLoad;
                if (pl != null)
                {
                    if (jsys.UpdateNodeFromPointElement(pl, nodeID))
                    {
                        nodeID++;
                        jsys.AwarePatternNames(pl.loadPatternName);
                    }
                    continue;
                }

                sLoadCombination com = so as sLoadCombination;
                if (com != null)
                {
                    jsys.SetLoadCombination(com);
                }
            }

            if (supCount > 0)
            {
                jsys.geometrySettings.systemBoundingBox = dycon.TosBoundingBox(sobjs);
                jsys.SystemName = sysSet.systemName;
                return new Dictionary<string, object>
                {
                    { "sSystem", jsys }
                };
            }
            else
            {
                return new Dictionary<string, object>
                {
                     { "sSystem", null }
                };
            }
        }


        [MultiReturn(new[] { "sSystem", "AppendedMeshNames" })]
        public static object AppendMesh(sSystem sghSystem, string meshName, List<double> verticeNineNumbers,int colorA , int colorR , int colorG , int colorB )
        {
            sDynamoConverter dycon = new sDynamoConverter("Feet", "Meters");
            
            List<string> meshNames = new List<string>();
            for(int i = 0; i < sghSystem.meshes.Count; ++i)
            {
                if (sghSystem.meshes[i].meshName == meshName)
                {
                    sghSystem.meshes.RemoveAt(i);
                }
            }
            
           sMesh sm = dycon.TosMesh(dycon.EnsureUnit(verticeNineNumbers), new sColor(colorA, colorR, colorG, colorB));
           sm.meshName = meshName;

           sghSystem.meshes.Add(sm);
            
            foreach(sMesh mm in sghSystem.meshes)
            {
                meshNames.Add(mm.meshName);
            }

            return new Dictionary<string, object>
                {
                     { "sSystem", sghSystem },
                     { "AppendedMeshNames", meshNames }
                };
        }

        [MultiReturn(new[] { "status" })]
        public static object Upload_sSystem(string hostURL, bool upload, sSystem sghSystem)
        {
            string url = hostURL + "jsonDataExchange.asmx/ReceiveFromGrasshopper";
            string mmes = "";

            if (sghSystem != null)
            {

                sSystem ssys = sghSystem as sSystem;

                string jsonData = ssys.Jsonify();

                if (upload)
                {
                    try
                    {
                        var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                        request.ContentType = "application/json";
                        request.Method = "POST";
                        request.Expect = "application/json";

                        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                        {
                            streamWriter.Write("{'GHin':'" + jsonData + "'}");
                            streamWriter.Close();
                        }

                        var httpResponse = (System.Net.HttpWebResponse)request.GetResponse();
                        StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

                        string result = streamReader.ReadToEnd();
                        if (result.Contains("success"))
                        {
                            mmes = "Upload Completed";
                        }
                        else if (result.Contains("unstable"))
                        {
                            mmes = "System is Unstable";
                        }
                        else if (result.Contains("failed"))
                        {
                            mmes = "System Calculation Failed";
                        }
                        else if (result.Contains("invalid"))
                        {
                            mmes = "System is Invalid";
                        }
                        else
                        {
                            mmes = "Upload Failed";
                        }

                    }

                    catch (System.Net.WebException e)
                    {
                        mmes = "Couldn't Find The Server";
                        string pageContent = new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();
                        //Error = pageContent;

                    }
                }
                else
                {
                    mmes = "";
                }
            }
            else
            {
                mmes = "Please provide sghSystem";
            }


            return new Dictionary<string, object>
                {
                     { "status", mmes }
                };

        }
        

        //add GetLineLoad...
        //figure how to deal with load combo

}




}



