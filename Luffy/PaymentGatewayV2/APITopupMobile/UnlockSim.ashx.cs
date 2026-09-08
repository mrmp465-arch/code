using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Libs.Report;
using Libs.Utils;

namespace APITopupMobile
{
    /// <summary>
    /// Summary description for UnlockSim
    /// </summary>
    public class UnlockSim : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                var data = context.Request.QueryString["data"];
                if (!string.IsNullOrEmpty(data))
                {
                    var id = Encrypts.Decrypt("pay", Encrypts.Base64Decode(data));
                    NLogLogger.Info(new string[] { "Unlock", data, id});
                    var mobile3Rd = new TopupMobile3rdLog()
                    {
                        Id = Convert.ToInt64(id)
                    };
                    var result = mobile3Rd.Get();
                    if (result != null)
                    {
                        var response = DataRequest.UpdateSim(result.ClientId, result.Slot, string.Empty, 1);
                        if (response == 0)
                        {
                            context.Response.Write("Unlock Success");
                        }
                    }
                }
                else
                    context.Response.Write("Unlock Failed");
            }
            catch (Exception e)
            {
                context.Response.Write("Unlock Failed");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}