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

namespace BankSocket
{
    public class MomoImage : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            //foreach (string key in context.Request.Headers)
            //{
            //    NLogLogger.Info(new string[] { "Header", key, context.Request.Headers[key] });
            //}
            if (context.IsWebSocketRequest)
            {
               
                NLogLogger.Info(new string[] { "MomoImage", "Socket Connect", GetIP()
                    });

                context.AcceptWebSocketRequest(HandleWebSocketAsync);
              

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



            NLogLogger.Info(new string[] { "MomoImage", "WebSocketState", GetIP(), socket.State.ToString() });


            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    NLogLogger.Info(new string[] { "MomoImage", "Socket Close", GetIP()
                    });
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                }
                else
                {

                    string returnMessage = "test";
                    byte[] output = Encoding.UTF8.GetBytes(returnMessage);


                    NLogLogger.Info(new string[] { "BankImage", "Send Mess", GetIP() });
                    await socket.SendAsync(new ArraySegment<byte>(output), WebSocketMessageType.Text, true, CancellationToken.None);

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
