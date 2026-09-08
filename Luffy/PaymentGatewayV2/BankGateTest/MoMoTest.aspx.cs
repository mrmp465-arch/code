using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BankGateTest
{
    public partial class MoMoTest : System.Web.UI.Page
    {
        private const string urlBaseService = "https://ncpay.asia/api/request";
        private const string apiKey = "34723cedd9714389d32939d4d23281a7";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                Init();
        }
        private void Init()
        {
            var requestId = DateTime.Now.ToString("yyyyMMddHHmmss");
            var sign = Encrypts.MD5(apiKey + requestId);
            var url = urlBaseService + "?" + string.Format("key={0}&type=momo&option=2&amount=10000&refcode={1}&sign={2}", apiKey, requestId, sign);
            NLogLogger.Info(new string[] { "bank Test", "Request Core", url });
            var response = Task.Run(async () => await GetTask(url)).Result;
            NLogLogger.Info(new string[] { "Khoai", "GetBanks Response", response,
            });
           

        }
        public static async Task<string> GetTask(string url)
        {
            var uri = new Uri(url);
            HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(60);

            NLogLogger.Info(new string[] { "Khoai", "GetTask", url });

            try
            {
                var response = await client.GetAsync(uri);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    client.Dispose();
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Khoai", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;
        }
    }
}