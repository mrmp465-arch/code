
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
using System.Linq;
using Libs.Utils;
using Libs.API;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace MomoSocket
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
          

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            if(socket.State == WebSocketState.Open)
            {
                var data = new MomoAccounts().GetList().OrderBy(x => x.Id).ToList();
                if (data.Exists(x => x.StatusDetection == 0))
                {
                    var dataSend = new List<CommandSend>();

                    foreach (var item in data.Where(x => x.StatusDetection == 0))
                    {
                        var _Momo = new MomoAccounts();
                        _Momo.Id = item.Id;
                        _Momo = _Momo.Get();
                        if (!string.IsNullOrEmpty(_Momo.ProfileImage))
                        {
                            var lstProfileImage = serializer.Deserialize<List<MomoProfileImage>>(_Momo.ProfileImage);
                            if (lstProfileImage.Exists(x => !x.Base64.Contains("/cmspay")))
                            {
                                foreach (var img in lstProfileImage.Where(x => !x.Base64.Contains("/cmspay")))
                                {
                                    var itemSend = new CommandSend
                                    {
                                        Base64 = img.Base64,
                                        ImgName = item.MomoId + "_" + img.ImgName
                                    };
                                    dataSend.Add(itemSend);
                                }
                            }
                        }

                    }
                    if (dataSend.Count() > 0)
                    {
                        foreach (var item in dataSend)
                        {
                            string returnMessage = serializer.Serialize(item);
                            byte[] output = Encoding.UTF8.GetBytes(returnMessage);


                            NLogLogger.Info(new string[] { "MomoImage", "Send Mess", item.ImgName });
                            await socket.SendAsync(new ArraySegment<byte>(output), WebSocketMessageType.Text, true, CancellationToken.None);
                            await Task.Delay(100);
                        }
                    }
                }
            }    
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                string receivedMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count).Trim();
                NLogLogger.Info(new string[] { "MomoImage", "receivedMessage: ", receivedMessage });
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    NLogLogger.Info(new string[] { "MomoImage", "Socket Close", GetIP()
                    });
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                }
                else
                {
                    //if(receivedMessage== "hello")
                    //{
                        
                    //}
                    
                    if (receivedMessage.Contains("Detection"))
                    {
                        try
                        {
                            var receivedData = serializer.Deserialize<CommandRecieve>(receivedMessage);
                            var momoId = receivedData.ImgName.Split('_')[0];
                            var imgName = int.Parse(receivedData.ImgName.Split('_')[1]);


                            var momo = new MomoAccounts().Get(momoId);
                            var lstProfileImage = serializer.Deserialize<List<MomoProfileImage>>(momo.ProfileImage);
                            lstProfileImage[imgName - 1].Detection = receivedData.Detection;
                            momo.ProfileImage = serializer.Serialize(lstProfileImage);
                            if (!lstProfileImage.Exists(x=>!x.Base64.Contains("/cmspay")&& string.IsNullOrEmpty(x.Detection)))
                            {
                                momo.StatusDetection = 1;
                            }
                            momo.Update();
                        }
                        catch (Exception e)
                        {
                            NLogLogger.Info(new string[] { "MomoImage", "Exeption", e.Message });
                            //return string.Empty;
                        }


                    }





                }



            
                //if (receivedMessage.Contains("ping"))
                //{
                //    string returnMessage = "pong";
                //    byte[] output = Encoding.UTF8.GetBytes(returnMessage);


                //    NLogLogger.Info(new string[] { "MomoImage", "Send Mess", GetIP() });
                //    await socket.SendAsync(new ArraySegment<byte>(output), WebSocketMessageType.Text, true, CancellationToken.None);
                //    await Task.Delay(100);
              
                

            }
        }
        public class CommandSend
        {
            public String Base64;
            public String ImgName;
        }
        public class CommandRecieve
        {
            public String Detection;
            public String ImgName;
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
