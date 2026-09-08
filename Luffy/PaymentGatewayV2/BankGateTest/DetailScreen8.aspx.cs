using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;




namespace BankGateTest
{
    public partial class DetailScreen8 : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var orderNo = Request.QueryString["orderNo"];
                var refcode = Request.QueryString["refcode"];
                var chatid = Request.QueryString["chatid"];
                var message_id = Request.QueryString["message_id"].ToString();
                if (!string.IsNullOrEmpty(orderNo) && !string.IsNullOrEmpty(refcode) && !string.IsNullOrEmpty(chatid))
                {
                    await BillScreenshotBot.CaptureBillAndSendTelegramAsync(
                  botToken: "123456:ABC...",
                  chatId: chatid,
                  url: "https://info.airapp.me/Pages/Detail.aspx?orderNo=" + orderNo,
                  billSelector: "#bill",
                   message_id: message_id,
                  caption: refcode 
              );
                }


            }
        }
        public static class BillScreenshotBot
        {


            public static async Task CaptureBillAndSendTelegramAsync(
       string botToken,
       string chatId,
       string url,
       string billSelector, string message_id,   // ví dụ: "#bill", ".bill-container", "#ctl00_ContentPlaceHolder1_bill"
       string caption)
            {
                var chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

                // Tải Chromium (lần đầu)
                var launchOptions = new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = chromePath,
                    Timeout = 120000,
                    Args = new[]
      {
        "--no-sandbox",
        "--disable-setuid-sandbox",
        "--disable-dev-shm-usage",
        "--disable-gpu",
        "--no-first-run",
        "--no-zygote",
        "--user-data-dir=C:\\chrome-profile"
    }
                };

                using (var browser = await Puppeteer.LaunchAsync(launchOptions))
                using (var page = await browser.NewPageAsync())
                {
                    page.DefaultTimeout = 60000;
                    page.DefaultNavigationTimeout = 60000;

                    await page.SetViewportAsync(new ViewPortOptions
                    {
                        Width = 1280,
                        Height = 900,
                        DeviceScaleFactor = 2
                    });

                    await page.GoToAsync(url, new NavigationOptions
                    {
                        Timeout = 60000
                    });

                    await page.WaitForTimeoutAsync(1500);

                    await page.WaitForSelectorAsync("#bill", new WaitForSelectorOptions
                    {
                        Visible = true,
                        Timeout = 20000
                    });

                    var billElement = await page.QuerySelectorAsync("#bill");
                    if (billElement == null)
                        throw new Exception("Không tìm thấy #bill");

                    var png = await billElement.ScreenshotDataAsync();

                    await SendPhotoTelegramAsync(botToken, chatId, png, "bill.png", caption, message_id);
                }
            }

            private static async Task SendPhotoTelegramAsync(string botToken, string chatId, byte[] photo, string fileName, string caption, string message_id)
            {
                var apiUrl = "https://api.telegram.org/bot8870375130:AAHgU08FL8C4Ppt3fN0U2hmIiXk7OrHcG0c/sendPhoto";

                using (var http = new HttpClient())
                using (var form = new MultipartFormDataContent())
                {
                    form.Add(new StringContent(chatId), "chat_id");
                    if (!string.IsNullOrEmpty(caption))
                        form.Add(new StringContent(caption), "caption");

                    if (!string.IsNullOrEmpty(message_id))
                        form.Add(new StringContent(message_id), "reply_to_message_id");

                    var fileContent = new ByteArrayContent(photo);
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                    form.Add(fileContent, "photo", fileName);

                    var resp = await http.PostAsync(apiUrl, form).ConfigureAwait(false);
                    var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!resp.IsSuccessStatusCode)
                        throw new Exception("Telegram sendPhoto failed: " + body);
                }
            }
        }
    }
}