using Libs.API;
using Libs.BankDirect.HynBank;
using Libs.Utils;
using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI;
namespace BankGateV2
{
    public partial class napbankcoroach : System.Web.UI.Page
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
                    JavaScriptSerializer serializer2 = new JavaScriptSerializer();
                    HynBankLib.Callback callback = serializer2.Deserialize<HynBankLib.Callback>(serializer2.Deserialize<HynBankLib.CallbackResponse>(str).ResponseContent);
                    APIResponse response2 = new Libs.BankDirect.HynBank.HynBank().CallbackV3(callback);
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