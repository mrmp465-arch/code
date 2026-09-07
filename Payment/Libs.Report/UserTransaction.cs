using Libs.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Libs.Report
{
    public class UserTransaction
    {
        public DateTime CreatedTime { get; set; }

        public string PartnerCode { get; set; }
        public string Note { get; set; }
        public string UserName { get; set; }
        //1- topup 2 deduct 3 widraw
        public int Type { get; set; }
        public long Id { get; set; }
        public long Amount { get; set; }
        public long Balance { get; set; }
        public long BalanceBefore { get; set; }

        public string RefCode { get; set; }
        public List<UserTransaction> GetList(string UserName,string partnercode,string note,int type, DateTime beginTime, DateTime endTime,int top)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@UserName", UserName);
            pars[1] = new SqlParameter("@Type", type);
            pars[2] = new SqlParameter("@Note", note);
            pars[3] = new SqlParameter("@BeginTime", beginTime);
            pars[4] = new SqlParameter("@EndTime", endTime);
            pars[5] = new SqlParameter("@Top", top);
            pars[6] = new SqlParameter("@PartnerCode", partnercode);
            return db.GetListSP<UserTransaction>("sp_UserTransaction_Select", pars);
        }
    }
}
