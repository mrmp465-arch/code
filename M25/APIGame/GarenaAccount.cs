using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Libs.Db;

namespace APIGame
{
    public class GarenaAccount
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public int? CountCharge { get; set; }
        public int? CountCheck { get; set; }
        public int? Status { get; set; }
        public int Type { get; set; }
        public string Source { get; set; }
        public string LastToken { get; set; }
        public string ProductCode { get; set; }
        public DateTime LastChangePass { get; set; }
        public long ReturnValue { get; set; }
        public GarenaAccount()
        {

        }


        public void Update()
        {

            DBHelper db = new DBHelper(Configs.CaptChaConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            //pars[2] = CountCharge == null ? new SqlParameter("@CountCharge", DBNull.Value) : new SqlParameter("@CountCharge", CountCharge);
            //pars[3] = CountCheck == null ? new SqlParameter("@CountCheck", DBNull.Value) : new SqlParameter("@CountCheck", CountCheck);
            pars[2] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            //pars[5] = Password == null ? new SqlParameter("@Password", DBNull.Value) : new SqlParameter("@Password", Password);
            //pars[6] = LastToken == null ? new SqlParameter("@LastToken", DBNull.Value) : new SqlParameter("@LastToken", LastToken);
            //pars[7] = ProductCode == null ? new SqlParameter("@ProductCode", DBNull.Value) : new SqlParameter("@ProductCode", ProductCode);
            //pars[8] = new SqlParameter("@LastChangePass", LastChangePass);
            db.ExecuteNonQuerySP("sp_MyGarenaAccount_Update", pars);
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
            db.ExecuteNonQuerySP("sp_MyGarenaAccount_Insert", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }

        public GarenaAccount GetAccount()
        {
            DBHelper db = new DBHelper(Configs.CaptChaConnectionStrings);
            return db.GetInstanceSP<GarenaAccount>("sp_Get_AccountGarena");
        }


    }
}
