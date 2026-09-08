using Libs.API;
using Libs.BankCash.Jav;
using Libs.Utils;
using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace BankGateV2
{
    public partial class rutbankcoroach : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                string str = string.Empty;
                using (StreamReader reader = new StreamReader(base.Request.InputStream))
                {
                    str = reader.ReadToEnd();
                }
                if (!string.IsNullOrEmpty(str))
                {
                    string[] list = new string[] { "HynBankCalback", "Callback", str };
                    NLogLogger.Info(list);
                    JavBankLib.CallbackResponse response = new JavaScriptSerializer().Deserialize<JavBankLib.CallbackResponse>(str);
                    APIResponse response2 = new JavBank().Callback(response);
                }
            }
            catch (Exception exception)
            {
                string[] list = new string[] { "HynBankCalback", "ProcessRequest", exception.Message };
                NLogLogger.Info(list);
            }
        }
    }
}