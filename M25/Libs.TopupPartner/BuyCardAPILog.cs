using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Libs.Db;

namespace Libs.TopupPartner
{
    public class BuyCardAPILog
    {
        public long TransactionID { get; set; }
        public string TransactionNo { get; set; }
        public int PartnerID { get; set; }
        public string PartnerCode { get; set; }
        public string ProviderCode { get; set; }
        public string AccountName { get; set; }
        public long AccountId { get; set; }
        public string OrderNo { get; set; }
        public long RequestTime { get; set; }
        public string Provider { get; set; }
        public int Amount { get; set; }
        public int Quantity { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string ListCards { get; set; }
        public string LogContent { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastTime { get; set; }
        public int Status { get; set; }
        public string SimTarget { get; set; }
        public int ReturnValue { get; set; }

        public BuyCardService Get()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<BuyCardService>("sp_BuyCard_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public BuyCardService Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<BuyCardService>("sp_BuyCard_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public BuyCardAPILog GetByOrderNoPartnerCode(string orderNo, string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetInstanceSP<BuyCardAPILog>("sp_BuyCard_SelectByOrderNo_PartnerCode"
                , new SqlParameter("@OrderNo", orderNo)
                , new SqlParameter("@PartnerCode", partnerCode));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[16];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@TransactionNo", TransactionNo);
            pars[3] = new SqlParameter("@PartnerID", PartnerID);
            pars[4] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[5] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[6] = new SqlParameter("@AccountName", AccountName);
            pars[7] = new SqlParameter("@AccountId", AccountId);
            pars[8] = new SqlParameter("@OrderNo", OrderNo);
            pars[9] = new SqlParameter("@RequestTime", RequestTime);
            pars[10] = new SqlParameter("@Provider", Provider);
            pars[11] = new SqlParameter("@Amount", Amount);
            pars[12] = new SqlParameter("@Quantity", Quantity);
            pars[13] = new SqlParameter("@LogContent", LogContent);
            pars[14] = new SqlParameter("@Status", Status);
            pars[15] = new SqlParameter("@SimTarget", SimTarget);

            db.ExecuteNonQuerySP("sp_BuyCard_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[9];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[3] = new SqlParameter("@ErrorCode", ErrorCode);
            pars[4] = new SqlParameter("@Message", Message);
            pars[5] = new SqlParameter("@LogContent", LogContent);
            pars[6] = new SqlParameter("@ListCards", ListCards);
            pars[7] = new SqlParameter("@Status", Status);
            pars[8] = new SqlParameter("@AccountId", AccountId);
            db.ExecuteNonQuerySP("sp_BuyCard_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

    }



}
