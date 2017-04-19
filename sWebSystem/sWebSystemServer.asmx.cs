using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using sDataObject.sBuildingElement;
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

        [WebMethod]
        public string WebServiceFunction()
        {
            return "ASKSGH";
        }

        //to call signalR outside of scope, make it as static
        public static void CallSignalRFunctionOutside()
        {

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
