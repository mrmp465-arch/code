using System;
using System.Linq;
using System.Web;
using System.Globalization;
using Libs.API;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.SessionState;

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

    public static System.Data.DataTable ToDataTable<T>(IList<T> data)
    {
        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
        System.Data.DataTable table = new System.Data.DataTable();
        for (int i = 0; i < props.Count; i++)
        {
            PropertyDescriptor prop = props[i];
            table.Columns.Add(prop.Name, prop.PropertyType);
        }
        object[] values = new object[props.Count];
        foreach (T item in data)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = props[i].GetValue(item);
            }
            table.Rows.Add(values);
        }
        return table;
    }
    public static string AmountToPercent(string amount, string TotalTransaction)
    {
        if (!string.IsNullOrEmpty(amount) && !string.IsNullOrEmpty(TotalTransaction))
        {
            if (TotalTransaction == "0") return "0";
            return (Convert.ToInt64(amount.Replace(".", "")) * 100.0 / Convert.ToInt64(TotalTransaction.Replace(".", ""))).ToString("F2");
        }
        else
            return amount;

    }

    public static string AmountToPercentFit(string amount, string fit)
    {
        if (!string.IsNullOrEmpty(amount) && !string.IsNullOrEmpty(fit))
        {
            return (Convert.ToInt64(fit) * 100.0 / Convert.ToInt64(amount)).ToString();
        }
        else
            return "0";

    }


    public static string AmountToPercentFitDecimal(string amount, string fit)
    {
        if (!string.IsNullOrEmpty(amount) && !string.IsNullOrEmpty(fit))
        {
            return (Convert.ToDecimal(fit) * 100 / Convert.ToDecimal(amount)).ToString();
        }
        else
            return "0";

    }

    public static DateTime DateTimeParseExact(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                return DateTime.ParseExact(value, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
        return DateTime.Now;
    }
    public static DateTime ToDateTime(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                return Convert.ToDateTime(value);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
        return DateTime.Now;
    }
    public static DateTime ToDateTime(string value, out bool erro)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                erro = false;
                return Convert.ToDateTime(value);
            }
            catch (Exception)
            {
                erro = true;
                return DateTime.Now;
            }
        }
        erro = true;
        return DateTime.Now;
    }
    public static int ToInt32(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        return 0;
    }
    public static int ToInt32(string value, out bool erro)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                erro = false;
                return Convert.ToInt32(value);

            }
            catch (Exception)
            {
                erro = true;
                return 0;
            }
        }
        erro = true;
        return 0;
    }
    public static long ToInt64(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                return Convert.ToInt64(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        return 0;
    }
    public static long ToInt64(string value, out bool erro)
    {

        if (!string.IsNullOrEmpty(value))
        {
            try
            {
                erro = false;
                return Convert.ToInt64(value);
            }
            catch (Exception)
            {
                erro = true;
                return 0;
            }
        }
        erro = true;
        return 0;
    }
    public static long Request(string name)
    {
        try
        {
            return Convert.ToInt64(HttpContext.Current.Request[name]);
        }
        catch
        {
            return 0;
        }
    }

    public static string RequestCode(string name)
    {
        try
        {
            return HttpContext.Current.Request[name].ToString();
        }
        catch
        {
            return string.Empty;
        }
    }


    public static int UserID
    {
        get
        {
            return Convert.ToInt32(HttpContext.Current.Session["UserID"]);
        }
    }
    public static bool IsAdmin
    {
        get
        {
            return Convert.ToBoolean(HttpContext.Current.Session["IsAdmin"]);
        }
    }
    public static bool IsPartner
    {
        get
        {
            return Convert.ToBoolean(HttpContext.Current.Session["IsPartner"]);
        }
    }
    public static bool IsProvider
    {
        get
        {
            return Convert.ToBoolean(HttpContext.Current.Session["IsProvider"]);
        }
    }
    public static bool IsTopup
    {
        get
        {
            return Convert.ToBoolean(HttpContext.Current.Session["IsTopup"]);
        }
    }

    public static string UserName
    {
        get { return HttpContext.Current.Session["UserName"].ToString(); }
    }

    public static string LastestTime
    {
        get { return HttpContext.Current.Session["LastestTime"].ToString(); }
    }

    public static string AccessKey
    {
        get { return HttpContext.Current.Session["AccessKey"].ToString(); }
    }

    public static void CheckRoles(string Url)
    {
        if (HttpContext.Current.Session["UserID"] == null)
            HttpContext.Current.Response.Redirect(Constant.ADMIN_PATH + Resources.Url.SignOut + "?u=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString()));
        //else if (!IsAdmin)
        else if (UserID != 1)
        {
            var lst = new UsersRole().GetListByUser(UserID);
            if (!(lst != null && lst.Count(e => e.Url.ToLower() == Url.ToLower()) > 0))
                HttpContext.Current.Response.Redirect(Constant.ADMIN_PATH + Resources.Url.Default);
        }
    }

    public static bool CheckRolesPermission(string Url)
    {
        if (HttpContext.Current.Session["UserID"] == null)
            HttpContext.Current.Response.Redirect(Constant.ADMIN_PATH + Resources.Url.SignOut + "?u=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString()));
        //else if (!IsAdmin)
        else if (UserID != 1)
        {
            var lst = new UsersRole().GetListByUser(UserID);
            if (!(lst != null && lst.Count(e => e.Url.ToLower() == Url.ToLower()) > 0))
                return false;
        }

        return true;
    }
    public static void CheckLogin()
    {
        if (HttpContext.Current.Session["UserID"] == null)
        {
            HttpContext.Current.Response.Redirect(Constant.ADMIN_PATH + Resources.Url.SignOut + "?u=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString()));
        }
    }

    private static HttpSessionState session { get { return HttpContext.Current.Session; } }
    public static bool ProviderTH
    {
        get
        {
            return Convert.ToBoolean(session["ProviderTH"]);
        }
        set { session["ProviderTH"] = value; }
    }

    public static bool PartnerTH
    {
        get
        {
            return Convert.ToBoolean(session["PartnerTH"]);
        }
        set
        {
            session["PartnerTH"] = value;
        }
    }

    public static List<Partners> PartnerUser
    {
        get
        {
            return (List<Partners>)session["PartnerUser"];
        }
        set
        {
            session["PartnerUser"] = value;
        }
    }

    public static List<Providers> ProviderUser
    {
        get
        {
            return (List<Providers>)session["ProviderUser"];
        }
        set
        {
            session["ProviderUser"] = value;
        }
    }
}