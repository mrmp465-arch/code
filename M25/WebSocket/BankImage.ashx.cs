using Libs.Utils;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;
using WebGrease.Css.Ast.Selectors;
using System.Web.Script.Serialization;
using System.Net.Http;
using System.Linq;
using System.Security.Cryptography;

namespace BankSocket
{
    public class BankImage : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //foreach (string key in context.Request.Headers)
            //{
            //    NLogLogger.Info(new string[] { "Header", key, context.Request.Headers[key] });
            //}
            if (context.IsWebSocketRequest)
            {
                string origin = context.Request.Headers["Origin"];
                NLogLogger.Info(new string[] { "BankImage", origin
                    });
                //string[] allowedOrigins = { ""https://another.com" };
                //// Kiểm tra Origin
                //if (allowedOrigins.Contains(origin))
                //{
                NLogLogger.Info(new string[] { "BankImage", "Socket Connect", GetIP()
                    });

                    context.AcceptWebSocketRequest(HandleWebSocketAsync);
                //}
                //else
                //{
                //    NLogLogger.Info(new string[] { "403", origin
                //    });
                //    context.Response.StatusCode = 403;
                //    context.Response.End();
                //}
                
            }
            else
            {
                NLogLogger.Info(new string[] { "400" });

                context.Response.StatusCode = 400;
                context.Response.Write("WebSocket requests only.");
            }
        }

        private async Task HandleWebSocketAsync(AspNetWebSocketContext context)
        {
            var socket = context.WebSocket;
            var buffer = new ArraySegment<byte>(new byte[1024]);



            NLogLogger.Info(new string[] { "BankImage", "WebSocketState", GetIP(), socket.State.ToString() });


            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    NLogLogger.Info(new string[] { "BankImage", "Socket Close", GetIP()
                    });
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                }
                else
                {
                    NLogLogger.Info(new string[] { "BankImage", "Begin Send", GetIP() });
                   
                    //var base64String = "test";
                    JavaScriptSerializer serializer = new JavaScriptSerializer();

                    for (int i = 0; i < 1000; i++)
                    {
                        System.Threading.Thread.Sleep(5000);
                        string id = context.Headers["id"];
                        var client = new HttpClient();

                        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:1593/ServiceHandler/GetDeviceImage.ashx?AppDeviceId=" + id);
                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        var base64String = await response.Content.ReadAsStringAsync();
                        var sendData = new CommandSend
                        {
                            command = "INFO",
                            extra = base64String
                        };
                        string returnMessage = serializer.Serialize(sendData);
                        byte[] output = Encoding.UTF8.GetBytes(returnMessage);

                       
                        NLogLogger.Info(new string[] { "BankImage", "Send Mess", GetIP(), id , Libs.Utils.Encrypts.MD5( base64String)});
                        await socket.SendAsync(new ArraySegment<byte>(output), WebSocketMessageType.Text, true, CancellationToken.None);
                    }


                }
            }
        }
        public class CommandSend
        {
            public String command;
            public String extra;
        }

    
        public static string GetHeader(string name)
        {
            string header = "";
           
            if (HttpContext.Current.Request.ServerVariables[name] != null)
            {
                header = HttpContext.Current.Request.ServerVariables[name];
                return header;
            }
            return header;
        }
        public static string GetIP()
        {
            string IP = "";
            //return IP;
            if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
                return IP;
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                return IP;
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED"];
                return IP;
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
                return IP;
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED_FOR"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED_FOR"];
                return IP;
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED"];
                return IP;
            }

            if (IP == "")
            {
                IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return IP;
        }
        public bool IsReusable => false;
    }
}
