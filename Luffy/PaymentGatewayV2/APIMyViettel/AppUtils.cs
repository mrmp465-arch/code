using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace APIMyViettel
{
    public class AppUtils
    {
        private static HttpSessionState session { get { return HttpContext.Current.Session; } }
        //public static bool Tokent (string name)
        //{
        //    get
        //    {
        //        return Convert.ToBoolean(session["Tokent"]);
        //    }
        //    set { session["ProviderTH"] = value; }
        //}
    }
}