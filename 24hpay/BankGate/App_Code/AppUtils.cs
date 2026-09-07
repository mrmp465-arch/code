using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Libs.API;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for AppUtils
/// </summary>
public class AppUtils
{
	public AppUtils()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static int Request(string name)
    {
        try
        {
            return Convert.ToInt32(HttpContext.Current.Request[name]);
        }
        catch
        {
            return 0;
        }
    }

    // Thông báo
    public static void Alert(Page page, string message)
    {
        ScriptManager.RegisterStartupScript(page, typeof(Page), "scriptkey", "window.setTimeout(\"alert('" + message + "')\",100);", true);
    }

    public static bool CheckAZ09(string s, int min, int max)
    {
        s = s.ToLower();
        if (s.Length < min || s.Length > max) return false;

        string data = "abcdefghijklmnopqrstuvwxyz0123456789";

        for (int i = 0; i < s.Length; i++)
        {
            if (data.IndexOf(s[i]) < 0) return false;
        }
        return true;
    }

    public static bool CheckRegular(string data, string pattern)
    {
        return new Regex(pattern).Match(data).Success;
    }
}