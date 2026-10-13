using System;
using System.Web;
using System.Web.UI;

/// <summary>
/// Summary description for Message
/// </summary>
public class Message
{
    public static void Alert(Page page, string message)
    {
        ScriptManager.RegisterStartupScript(page, typeof(Page), new Guid().ToString(), "window.setTimeout(\"alert('" + message + "')\",100);", true);
    }

    public static void AlertAndRedirect(Page page, string message, string url)
    {
        ScriptManager.RegisterStartupScript(page, typeof(Page), new Guid().ToString(), "alert('" + message + "'); window.location='" + url + "';", true);
    }
}
