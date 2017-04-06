using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sDataObject;
using sDataObject.sGeometry;
using sDataObject.sElement;
using SAP2000v17;


namespace sSAP17System
{
    public class sSAP17Converter
    {


        public void ToSAPElement(sObject sobj)
        {
            if(sobj is sBeam)
            {

            }
            else if(sobj is sPointLoad)
            {

            }
            else if(sobj is sPointSupport)
            {

            }
        }
    }
}
