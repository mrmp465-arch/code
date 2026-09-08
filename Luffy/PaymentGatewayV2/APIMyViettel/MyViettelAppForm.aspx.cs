using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using APIMyViettel.Entity;
using APIMyViettel.Service;
using HtmlAgilityPack;
using Libs.API;

namespace APIMyViettel
{
    public partial class MyViettelAppForm : System.Web.UI.Page
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnOk_Click(object sender, EventArgs e)
        {

            //var result = serializer.Serialize(CheckSerial.CheckCard(txtCardSerial.Text));
            var result = serializer.Serialize(MyViettelService.CheckCard(txtCardSerial.Text));
            Response.Write(result);
        }

        protected void btnTopup_Click(object sender, EventArgs e)
        {
            //var result = serializer.Serialize(MyViettelService.TopupCard(txtCardSerial.Text, txtCode.Text, txtMobie.Text, 1));
            //Response.Write(result);
        }

        protected void Button3_Click(object sender, EventArgs e)
        {


            var res = Task.Run(() => UtilsSmas.GetTask("https://smas.edu.vn/Home/LogOn", new CookieContainer())).Result;
            //Response.Write(serializer.Serialize(res));


            //var url = "https://smas.edu.vn/Home/LogOn";
            //var uri = new Uri(url);
            //var cookieContainer = new CookieContainer();
            //var smasCookie = new SmasCookie();

            //var httpClientHandler = new HttpClientHandler() { CookieContainer = new CookieContainer() };
            //var httpClient = new HttpClient(httpClientHandler);
            //var response = httpClient.GetAsync(uri).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var responseContent = response.Content.ReadAsStringAsync().Result;
            //    var doc = new HtmlDocument();
            //    doc.LoadHtml(responseContent);
            //    var input = doc.DocumentNode.SelectSingleNode("//*[@name='__RequestVerificationToken']");
            //    var token = input.Attributes["value"].Value;
            //    if (doc.GetElementbyId("captcha") != null)
            //        smasCookie.CaptChaLink = doc.GetElementbyId("captcha").GetAttributeValue("src", "");
            //    smasCookie.RequestVerificationToken = token;
            //    smasCookie.CookieContainer = cookieContainer;
            //    smasCookie.HtmlContent = responseContent;

            //}

            Libs.Utils.SharedCache.Add("smas", res);

            var ck = (SmasCookie)Libs.Utils.SharedCache.Get("smas");

            Response.Write(serializer.Serialize(ck));

        }
    }
}