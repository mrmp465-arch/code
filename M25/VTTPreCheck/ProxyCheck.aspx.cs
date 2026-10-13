using APIMyViettel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIMyViettel.Entity;
using Libs.Utils;

namespace VTTPreCheck
{
    public partial class ProxyCheck : System.Web.UI.Page
    {
        private static bool isStar = true;
        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnStart_Click(object sender, EventArgs e)
        {
            var isStar = 0;
            while (isStar <= 20)
            {
                txtResult.Text = txtResult.Text + Task.Run(() => GetTask("http://lumtest.com/myip.json")).Result;
                isStar++;
                Thread.Sleep(1000);
            }
        }

        public static async Task<string> GetTask(string url)
        {
            var session_id = new Random().Next().ToString();
            var uri = new Uri(url);
            var credentials = new NetworkCredential("lum-customer-hl_37347aa4-zone-static-country-vn" + "-session-" + session_id, "fuxjiv7btklo");
            var handler = new HttpClientHandler
            {
                UseProxy = true,
                Proxy = new WebProxy("zproxy.lum-superproxy.io:22225", false, new string[] { }, credentials)
            };
            var httpClient = new HttpClient(handler);

            httpClient.Timeout = TimeSpan.FromSeconds(60);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
            try
            {
                var response = await httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "ProxyCheck", "GetTask", "Exception", url, e.Message, e.StackTrace });
            }
            httpClient.Dispose();
            return null;
        }
    }
}