using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using System.ComponentModel;
using System.Linq;
using Libs.Utils;
using System.Threading.Tasks;
using System.Web;
namespace Libs.API
{
    public class UserLog
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
        public string ActionName { get; set; }
        public string Description { get; set; }
        public string Ip { get; set; }
        public DateTime Time { get; set; }

        public void Add()
        {
            try
            {


                Ip = GetIP();
                //if (UserName == "admin")
                //{
                //    Ip = "";
                //}

                DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
                SqlParameter[] pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@UserName", UserName);
                pars[2] = new SqlParameter("@Action", Action);
                pars[3] = new SqlParameter("@ActionName", ActionName);
                pars[4] = new SqlParameter("@Description", Description);
                pars[5] = new SqlParameter("@Ip", Ip);
                db.ExecuteNonQuerySP("sp_UserLog_Insert", pars);
                Id = Convert.ToInt64(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", ex.Message.Replace("\n", " ") });
                
            }
        }

        public List<UserLog> GetList(int Top, string Action, string UserName, string Keyword, DateTime FromDate, DateTime ToDate)
        {

            try
            {
                DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
                SqlParameter[] pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@Top", Top);
                pars[1] = string.IsNullOrEmpty(Action) ? new SqlParameter("@Action", DBNull.Value) : new SqlParameter("@Action", Action);
                pars[2] = string.IsNullOrEmpty(UserName) ? new SqlParameter("@UserName", DBNull.Value) : new SqlParameter("@UserName", UserName);
                pars[3] = new SqlParameter("@FromDate", System.Data.SqlDbType.DateTime);
                pars[4] = new SqlParameter("@ToDate", System.Data.SqlDbType.DateTime);
                pars[3].Value = FromDate;
                pars[4].Value = ToDate;
                pars[5] = string.IsNullOrEmpty(Keyword) ? new SqlParameter("@Keyword", DBNull.Value) : new SqlParameter("@Keyword", Keyword);

                return db.GetListSP<UserLog>("sp_UserLog_Select", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

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
    }


}
