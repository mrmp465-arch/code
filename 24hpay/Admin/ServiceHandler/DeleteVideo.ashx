<%@ WebHandler Language="C#" Class="DeleteVideo" %>

using System;
using System.IO;
using System.Web;

public class DeleteVideo : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        try
        {
            string fileName = context.Request["fileName"];

            if (string.IsNullOrWhiteSpace(fileName))
            {
                context.Response.Write("Thiếu tên file");
                return;
            }

            fileName = Path.GetFileName(fileName);
            var bankCode = AppUtils.RequestCode("bankcode");
            var bankId = AppUtils.RequestCode("bankId");
            string UploadFolderPhysical = Path.Combine(@"Z:\24HPAY", bankCode, bankId);
         

            string filePath = Path.Combine(UploadFolderPhysical, fileName);

            if (!File.Exists(filePath))
            {
                context.Response.Write("File không tồn tại");
                return;
            }

            File.Delete(filePath);
            context.Response.Write("1");
        }
        catch (Exception ex)
        {
            context.Response.Write(ex.Message);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }

   

}