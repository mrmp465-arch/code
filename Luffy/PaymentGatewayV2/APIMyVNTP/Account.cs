using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace APIMyVNTP
{
    public class Account
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public int? CountCharge { get; set; }
        public int? CountCheck { get; set; }
        public int? Status { get; set; }
        public int Type { get; set; }
        public string Source { get; set; }
        public long ReturnValue { get; set; }
        public Account()
        {

        }


        public void Update()
        {
            DBHelper db = new DBHelper(Configs.CaptChaConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = CountCharge == null ? new SqlParameter("@CountCharge", DBNull.Value) : new SqlParameter("@CountCharge", CountCharge);
            pars[3] = CountCheck == null ? new SqlParameter("@CountCheck", DBNull.Value) : new SqlParameter("@CountCheck", CountCheck);
            pars[4] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            db.ExecuteNonQuerySP("sp_MyVNPAccount_Update", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
        }

        public long Insert()
        {
            DBHelper db = new DBHelper(Configs.CaptChaConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@AccountName", AccountName);
            pars[2] = new SqlParameter("@Password", Password);
            pars[3] = new SqlParameter("@Type", Type);
            pars[4] = new SqlParameter("@Source", Source);
            db.ExecuteNonQuerySP("sp_MyVNPAccount_Insert", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }

        public Account GetAccount(int type)
        {
            DBHelper db = new DBHelper(Configs.CaptChaConnectionStrings);
            return db.GetInstanceSP<Account>("sp_Get_AccountVNP", new SqlParameter("@Type", type));
        }

        public Account GetAccountCheck(int type)
        {
            DBHelper db = new DBHelper(Configs.CaptChaConnectionStrings);
            return db.GetInstanceSP<Account>("sp_Check_AccountVNP", new SqlParameter("@Type", type));
        }
    }
}
