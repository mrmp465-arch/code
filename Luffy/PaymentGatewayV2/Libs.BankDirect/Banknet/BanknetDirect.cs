using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using Libs.Db;
using Libs.Utils;

namespace Libs.BankDirect.Banknet
{
    public class BanknetDirect
    {
        public int ReturnValue { get; set; }
        public long TransactionID { get; set; }
        public string Selected_bank { get; set; }
        public string Trans_id { get; set; }
        public int Step { get; set; }
        public string CardNumber { get; set; }
        public string FullName { get; set; }
        public int CardMonth { get; set; }
        public int CardYear { get; set; }
        public string OTPType { get; set; }
        public string OTP { get; set; }
        public DateTime LastTime { get; set; }

        
        public BanknetDirect()
        {

        }

        public BanknetDirect Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BanknetDirect>("sp_pay_BanknetDirect_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public BanknetDirect Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BanknetDirect>("sp_pay_BanknetDirect_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Selected_bank", Selected_bank);

            db.ExecuteNonQuerySP("sp_pay_BanknetDirect_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[10];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Trans_id", Trans_id);
            pars[3] = new SqlParameter("@Step", Step);
            pars[4] = new SqlParameter("@CardNumber", CardNumber == null ? "" : CardNumber);
            pars[5] = new SqlParameter("@FullName", FullName == null ? "" : FullName);
            pars[6] = new SqlParameter("@CardMonth", CardMonth);
            pars[7] = new SqlParameter("@CardYear", CardYear);
            pars[8] = new SqlParameter("@OTPType", OTPType == null ? "" : OTPType);
            pars[9] = new SqlParameter("@OTP", OTP == null ? "" : OTP);

            db.ExecuteNonQuerySP("sp_pay_BanknetDirect_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

    }
}
