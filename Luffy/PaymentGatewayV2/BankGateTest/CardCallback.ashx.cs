namespace BankGateTest
{
    using Libs.Utils;
    using RestSharp;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Web;
    using System.Web.Script.Serialization;

    public class CardCallback : IHttpHandler
    {
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        public void ProcessRequest(HttpContext context)
        {
            context.Request.ContentType = "application/json";
            context.Response.ContentType = "application/json";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str = string.Empty;
            string s = string.Empty;
            context.Request.InputStream.Position = 0L;
            try
            {
                using (StreamReader reader = new StreamReader(context.Request.InputStream))
                {
                    str = reader.ReadToEnd();
                }
                string[] list = new string[] { "APITiger", "Callback", str };
                NLogLogger.Info(list);
                if (!string.IsNullOrEmpty(str))
                {
                    DataCallback callback = serializer.Deserialize<DataCallback>(str);
                    RestClient client = new RestClient("https://viettelai.vn/tts/speech_synthesis")
                    {
                        Timeout = -1
                    };
                    callback.text = callback.text.Replace("//n", "");
                    RestRequest request = new RestRequest
                    {
                        Method = Method.POST
                    };
                    request.AddHeader("Content-Type", "application/json");
                    string[] textArray2 = new string[] { "{\"text\": \"", callback.text, "\",\"voice\": \"", callback.type, "\",\"speed\": 1,\"tts_return_option\":3,\"token\": \"0be5461c83986433f331c5c81cc6e7bf\",\"without_filter\":false}" };
                    request.AddParameter("application/json", string.Concat(textArray2), ParameterType.RequestBody);
                    string path = callback.path;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    IRestResponse response = client.Execute(request);
                    if (response.Content.Length < 600)
                    {
                        string[] textArray3 = new string[] { "APITiger", "respone", response.Content };
                        NLogLogger.Info(textArray3);
                    }
                    File.WriteAllBytes(callback.path + callback.filename, response.RawBytes);
                    context.Response.Write("1");
                }
                else
                {
                    context.Response.Write("-99");
                    return;
                }
            }
            catch (Exception exception)
            {
                string[] list = new string[] { "APITiger", "Callback", "Error", exception.Message };
                NLogLogger.Info(list);
                context.Response.Write("-1");
            }
            context.Response.Write(s);
        }

        public bool IsReusable =>
            false;

        public class DataCallback
        {
            public string path { get; set; }

            public string filename { get; set; }

            public string text { get; set; }

            public string type { get; set; }
        }
    }
}
