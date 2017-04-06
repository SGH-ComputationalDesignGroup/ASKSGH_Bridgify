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
using System.IO;
using sRhinoSystem.Properties;

namespace sRhinoSystem.GH.To_sSystem
{
    public class Upload_sSystem : GH_Component
    {
        public Upload_sSystem()
            : base("Upload sSystem to ASKSGH", "Upload sSystem to ASKSGH", "...", "ASKSGH.Bridgify", "To sSystem")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.septenary; } // | GH_Exposure.obscure;
        }
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ASKSGH_URL", "ASKSGH_URL", "...", GH_ParamAccess.item);
            pManager.AddBooleanParameter("upload", "upload", "...", GH_ParamAccess.item);
            pManager.AddGenericParameter("sghSystem", "sghSystem", "...", GH_ParamAccess.item);
            // Params.Input[0].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Message", "Message", "Message", GH_ParamAccess.item);
        }

        public static int status = 0;
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string hostURL = "";
            bool send = false;
            
            sSystem sghSystem = null;

            if (!DA.GetData(0, ref hostURL)) return;
           // if (!DA.GetData(1, ref conid)) return;
            if (!DA.GetData(1, ref send)) return;
            if (!DA.GetData(2, ref sghSystem)) return;

            string url = hostURL + "jsonDataExchange.asmx/ReceiveFromGrasshopper";
            string mmes = "";
            string jsonData = "";
            string sysName = "";


            if (sghSystem != null)
            {
                sSystem ssys = sghSystem as sSystem;
                sysName = ssys.systemSettings.systemName;

                
                jsonData = ssys.Jsonify();

                if (send)
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
                            status = 1;
                        }
                        else if (result.Contains("unstable"))
                        {
                            status = 2;
                        }
                        else if (result.Contains("failed"))
                        {
                            status = 3;
                        }
                        else if (result.Contains("invalid"))
                        {
                            status = 3;
                        }
                        else
                        {
                            status = 3;
                        }
                    }

                    catch (System.Net.WebException e)
                    {
                        string pageContent = new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();
                        status = 4;
                    }
                }
                else
                {
                //    status = 0;
                }
            }

            if (status == 1)
            {
                this.Message = "System : " + sysName + "\nhas been uploaded\nData Size:" + Math.Round( jsonData.Length / 1.0E6 ,2) + "Mb";
            }
            else if (status == 2)
            {
                this.Message = "System is unstable\nData Size";
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, this.Message);
            }
            else if (status == 3)
            {
                this.Message = "Upload Failed";
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, this.Message);
            }
            else if (status == 4)
            {
                this.Message = "Server Failed";
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, this.Message);
            }
            else
            {
                this.Message = "";
            }

            DA.SetData(0, mmes);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("acb3a153-1f97-44c0-a262-bee842a1d928"); }
        }

       protected override System.Drawing.Bitmap Internal_Icon_24x24
       {
           get
           {
                return Resources.uploadSystem;
            }
       }


    }
}
