using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libs.Db;
using Libs.Utils;
namespace Libs.API
{
    [Serializable]
    public class BankDaily
    {
        public string BankCode { get; set; }

        public string BankId { get; set; }
        public string BankName { get; set; }
        public string BankType { get; set; }

        public long Balance { get; set; }

        public long BalanceBefore { get; set; }
        
        public long TotalIn { get; set; }
        public long TotalOut { get; set; }
        public long Time { get; set; }

        public int Status { get; set; }
        public int Type { get; set; }
        public List<BankDaily> GetList(string BankId,string BankType,int Type,int Status, long Time)
        {
            //int? amount = null;
            //if (isByPass10k)
            //    amount = 10000;
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@Time", Time);
                pars[1] = string.IsNullOrEmpty(BankId) ? new SqlParameter("@BankId", DBNull.Value) : new SqlParameter("@BankId", BankId);
                pars[2] = string.IsNullOrEmpty(BankType) ? new SqlParameter("@BankType", DBNull.Value) : new SqlParameter("@BankType", BankType);
                pars[3] = new SqlParameter("@Type", Type);
                pars[4] = new SqlParameter("@Status", Status);
                return db.GetListSP<BankDaily>("sp_BankDaily_SelectList", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
    }
}
