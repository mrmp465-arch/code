using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Libs.Db;

namespace Libs.TopupPartner
{
    public class TopupMobile
    {
        public long TransactionID { get; set; }
        public string TransactionNo { get; set; }
        public int PartnerID { get; set; }
        public string Telco { get; set; }
        public string RequestNo { get; set; }
        public long RequestTime { get; set; }
        public string Provider { get; set; }
        public string Mobile { get; set; }
        public int Amount { get; set; }
        public int TopupType { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string LogContent { get; set; }
        public int AmountTopupSuccess { get; set; }
        public int AmountPending { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastTime { get; set; }
        public int ReturnValue { get; set; }

        public TopupMobile()
        {

        }

        public TopupMobile Get()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupMobile>("sp_TopupMobile_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public TopupMobile Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupMobile>("sp_TopupMobile_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@PartnerID", PartnerID);
            pars[3] = new SqlParameter("@Telco", Telco);
            pars[4] = new SqlParameter("@RequestNo", RequestNo);
            pars[5] = new SqlParameter("@RequestTime", RequestTime);
            pars[6] = new SqlParameter("@Provider", Provider);
            pars[7] = new SqlParameter("@Mobile", Mobile);
            pars[8] = new SqlParameter("@Amount", Amount);
            pars[9] = new SqlParameter("@TopupType", TopupType);
            pars[10] = new SqlParameter("@LogContent", LogContent);

            db.ExecuteNonQuerySP("sp_TopupMobile_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@ErrorCode", ErrorCode);
            pars[3] = new SqlParameter("@Message", Message);
            pars[4] = new SqlParameter("@LogContent", LogContent);
            pars[5] = new SqlParameter("@AmountTopupSuccess", AmountTopupSuccess);
            pars[6] = new SqlParameter("@AmountPending", AmountPending);

            db.ExecuteNonQuerySP("sp_TopupMobile_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
    }
}
