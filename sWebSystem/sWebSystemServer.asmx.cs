using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using sNStatSystem;

using sDataObject.sElement;

namespace sWebSystem
{
    [ScriptService]
    [WebService(Namespace = "ASKSGH_WebAppBase")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 

    #region HTTP
    public class sWebSystemServer : System.Web.Services.WebService
    {
        //Update DataBase
        public static string latestSystem = "";

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ReceiveFromClient(string sysFromClient)
        {
            string status = "StandBy";
            try
            {
                this.CalculatesSystem(sysFromClient);
                status = "Uploaded To ASKSGH";
            }
            catch (Exception e)
            {
                status = e.Message;
                status = "Failed";
            }
            return status;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ReceiveFromServer(string ClientMess)
        {
            //string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            //AppPath += "\\jsons\\latestSystem.json";

            //string sysJson = File.ReadAllText(AppPath);

            if (latestSystem.Length > 0)
            {
                return latestSystem;
            }
            else
            {
                return "invalid system";
            }
        }

        public void CalculatesSystem(string jsonSystem)
        {
           // sSystem ssys = sSystem.Objectify(jsonSystem);
           //
           // sStatSystem stSys = new sStatSystem();
           // stSys.BuildSystem(ssys);
           // stSys.SolveSystemByCaseName(stSys.systemSettings.currentCase);
           //
           // sSystem csys = stSys.ToISystem();
           // 
           // latestSystem = csys.Jsonify();
           // FeedSystemToClient(csys.Jsonify(true));
        }
        //to call signalR outside of scope, make it as static


        public static void FeedSystemToClient(string jsonSystem)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<sDataCommunication>();

            context.Clients.All.receiveSystemFromServer(jsonSystem);
        }
    }
    #endregion
    
    #region SignalR
    [HubName("sDataCommunication")]
    public class sDataCommunication : Hub
    {
        //for some reasons, signalR / JS side function name should be in lower cases...



    }
    #endregion
}
