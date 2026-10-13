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
    public class UserDeposit
    {
        public DateTime CreatedTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Note { get; set; }

        public long Money { get; set; }
        public string UserName { get; set; }
        public string Admin { get; set; }
        public long Id { get; set; }
        public long Amount { get; set; }
        
        public int Status { get; set; }
        public List<UserDeposit> GetList(int Top, string username, int status, DateTime FromDate, DateTime ToDate)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@UserName", username);
            pars[1] = new SqlParameter("@Status", status);
            pars[2] = new SqlParameter("@FromDate", FromDate);
            pars[3] = new SqlParameter("@ToDate", ToDate);
            pars[4] = new SqlParameter("@Top", Top);


            return db.GetListSP<UserDeposit>("sp_UserDeposit_Select", pars);

        }
        public long Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserName", UserName);
            pars[2] = new SqlParameter("@Amount", Amount);
            pars[3] = new SqlParameter("@Note", Note);
            pars[4] = new SqlParameter("@Money", Money);
            pars[5] = new SqlParameter("@Admin", Admin);
            db.ExecuteNonQuerySP("sp_UserDeposit_Insert", pars);
            return Convert.ToInt64(pars[0].Value);
        }
        public long Add2()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserName", UserName);
            pars[2] = new SqlParameter("@Amount", Amount);
            pars[3] = new SqlParameter("@Note", Note);
            pars[4] = new SqlParameter("@Money", Money);
            pars[5] = new SqlParameter("@Admin", Admin);
            db.ExecuteNonQuerySP("sp_UserDeposit_InsertV2", pars);
            return Convert.ToInt64(pars[0].Value);
        }
    }
}
