using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using Libs.Utils;

namespace APIMyVNTP
{
    /// <summary>
    /// Summary description for TestService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TestService : System.Web.Services.WebService
    {

        [WebMethod]
        public string PostTelegram()
        {
            var t = Task.Run(() => TelegramClient.TelegramSendMessage(-280811434, "Có thẻ nghi vấn, các anh check ngay nhé (^_^) !"));
            t.Wait();
            return "Success";
        }
    }
}
