using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;

namespace Libs.API
{
    [Serializable]
    public class UserDaily
    {
        public string UserName { get; set; }

        public long Balance { get; set; }
        public long TotalBankIn { get; set; }
        public long TotalBankOut { get; set; }

        public long TotalCardIn { get; set; }
        public long TotalCardOut { get; set; }
        public long TotalCash { get; set; }
        public long TotalRecharge { get; set; }
        public long BalanceBefore { get; set; }
        public long BalanceAfter { get; set; }
        public long Day { get; set; }
        public long Amount { get; set; }

        public long AmountBankin { get; set; }
        public long AmountBankout { get; set; }
        public List<UserDaily> GetList(string username, DateTime BeginTime, DateTime EndTime)
        {
            //int? amount = null;
            //if (isByPass10k)
            //    amount = 10000;
            try
            {
                DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@BeginTime", BeginTime);
                pars[2] = new SqlParameter("EndTime", EndTime);
                pars[1] = string.IsNullOrEmpty(username) ? new SqlParameter("@UserName", DBNull.Value) : new SqlParameter("@UserName", username);
                return db.GetListSP<UserDaily>("sp_UserDaily_SelectList", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
    }
   
}
