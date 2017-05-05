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
using sDataObject.IElement;
using sDataObject.ElementBase;

namespace sRhinoSystem.GH.ToRhinoSystem
{
    public class Load_sSystem : GH_Component
    {

        public Load_sSystem()
            : base("Download sSystem from ASKSGH", "Download sSystem from ASKSGH", "...", "ASKSGH.Bridgify", "To Rhino")
        {
        }
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; } // | GH_Exposure.obscure;
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ASKSGH_URL", "ASKSGH_URL", "...", GH_ParamAccess.item);
            pManager.AddBooleanParameter("download", "download", "...", GH_ParamAccess.item);
            pManager.AddBooleanParameter("reset", "reset", "...", GH_ParamAccess.item);
            Params.Input[2].Optional = true;
        }

        public override void CreateAttributes()
        {
            base.CreateAttributes();
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("sghSystem", "sghSystem", "sghSystem", GH_ParamAccess.item);
        }

        public static string result = "";
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string hostURL = "";
            bool load = false;
            bool reset = false;

            if (!DA.GetData(0, ref hostURL)) return;
            if (!DA.GetData(1, ref load)) return;
            DA.GetData(2, ref reset);

            string url = hostURL + "sWebSystemServer.asmx/ReceiveFromServer";
            
            if (load)
            {
                result = "";
                try
                {
                    var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    request.Expect = "application/json";

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write("{'ClientMess':'" + "ask" + "'}");
                        streamWriter.Close();
                    }
                    var httpResponse = (System.Net.HttpWebResponse)request.GetResponse();

                    StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());

                    string resp = streamReader.ReadToEnd();

                    sJsonReceiver jj = Newtonsoft.Json.JsonConvert.DeserializeObject<sJsonReceiver>(resp);
                    result = jj.d;
                    
                }
                catch (System.Net.WebException e)
                {
                    this.Message = "Couldn't Find The Server";
                    string pageContent = new StreamReader(e.Response.GetResponseStream()).ReadToEnd().ToString();

                }
            }

            if (reset)
            {
                result = "";
            }

            ISystem sysLoaded = null;
            if (result.Length > 0)
            {
                string jsonSys = result;
                
                sysLoaded = SystemBase.Objectify(jsonSys);
                this.Message = "System : " + sysLoaded.systemSettings.systemName + "\nis loaded" + "\nLoadCase: " + sysLoaded.systemSettings.currentCase;
            }
            else
            {
                this.Message = "System NULL";
            }


            DA.SetData(0, sysLoaded);
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("d524f924-e6d8-4c45-93dd-bf2f930cbf07"); }
        }

        protected override System.Drawing.Bitmap Internal_Icon_24x24
        {
            get
            {
                return Resources.downSystem;
            }
        }


    }
}
