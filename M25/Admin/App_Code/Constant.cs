using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

/// <summary>
/// Summary description for Constant
/// </summary>
public class Constant
{
    public static string ADMIN_PATH = ConfigurationSettings.AppSettings["ADMIN_PATH"];
    public static string MEDIA_PATH = ConfigurationSettings.AppSettings["MEDIA_PATH"];
    public static string MEDIA_URL = ConfigurationSettings.AppSettings["MEDIA_URL"];
    public static string MEDIA = ConfigurationSettings.AppSettings["MEDIA"];
    public static string TITLE = "Hệ thống quản trị nội dung";

    public Constant()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}
