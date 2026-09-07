using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Libs.Db;

namespace Libs.BankCash
{
    public class BankCashAPI
    {
        public long TransactionID { get; set; }
        public int PartnerID { get; set; }
        public string PartnerCode { get; set; }
        public string ProviderCode { get; set; }
        public string OrderNo { get; set; }
        public string OrderInfo { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public string ReturnUrl { get; set; }
        public long RequestTime { get; set; }
        public string Signature { get; set; }
        public int Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastTime { get; set; }
        public string LogContent { get; set; }
        public string BankCode { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string RefCode { get; set; }
        public string ApproveUser { get; set; }
        public string Note { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public long ReturnValue { get; set; }

        public long Fee { get; set; }

        public long Reward { get; set; }

        public BankCashAPI()
        {

        }

        public BankCashAPI Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankCashAPI>("sp_BankCashAPI_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public BankCashAPI Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankCashAPI>("sp_BankCashAPI_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public BankCashAPI Get(string orderNo)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankCashAPI>("sp_BankCashAPI_Select_OrderNo"
                , new SqlParameter("@OrderNo", orderNo));
        }
        public BankCashAPI GetByRefcode(string refcode, string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@PartnerCode", partnerCode);
            pars[1] = new SqlParameter("@RefCode", refcode);
            return db.GetInstanceSP<BankCashAPI>("sp_BankCashAPI_Select_RefCode", pars);


        }
        public long Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[20];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@PartnerID", PartnerID);
            pars[2] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[3] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[4] = new SqlParameter("@OrderNo", OrderNo);
            pars[5] = new SqlParameter("@OrderInfo", OrderInfo);
            pars[6] = new SqlParameter("@Amount", Amount);
            pars[7] = new SqlParameter("@TotalAmount", TotalAmount);
            pars[8] = new SqlParameter("@Currency", Currency == null ? "" : Currency);
            pars[9] = new SqlParameter("@ReturnUrl", ReturnUrl);
            pars[10] = new SqlParameter("@RequestTime", RequestTime);
            pars[11] = new SqlParameter("@Signature", Signature);
            pars[12] = new SqlParameter("@LogContent", LogContent);
            pars[13] = new SqlParameter("@BankCode", BankCode);
            pars[14] = new SqlParameter("@FullName", FullName);
            pars[15] = new SqlParameter("@Mobile", Mobile);
            pars[16] = new SqlParameter("@RefCode", RefCode);
            pars[17] = new SqlParameter("@BankAccountName", BankAccountName);
            pars[18] = new SqlParameter("@BankAccountNumber", BankAccountNumber);
            pars[19] = new SqlParameter("@Note", Note);
            //BankCashAPI transaction = db.GetInstanceSP<BankCashAPI>("sp_BankCashAPI_Insert", pars);
            //ReturnValue = Convert.ToInt32(pars[0].Value);
            //return transaction;
            db.ExecuteNonQuerySP("sp_BankCashAPI_Insert", pars);
            ReturnValue = TransactionID = Convert.ToInt32(pars[0].Value);
            return ReturnValue;
        }

        public void UpdateStatus()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent);

            db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateStatus", pars);
            ReturnValue = TransactionID = Convert.ToInt32(pars[0].Value);
        }

        public void UpdateStatus(bool checkStatus)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent);

            if (checkStatus)
            {
                db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateStatusCheckStatus", pars);
            }
            else
            {
                db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateStatus", pars);
            }
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void UpdateService(long transactionID, int serviceID, string serviceCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", transactionID);
            pars[2] = new SqlParameter("@ServiceID", serviceID);
            pars[3] = new SqlParameter("@ServiceCode", serviceCode);

            db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateServiceID", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public int UpdateFail()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[12];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent);
            pars[4] = new SqlParameter("@LastTime", LastTime);
            pars[5] = new SqlParameter("@TotalAmount", TotalAmount);
            pars[6] = new SqlParameter("@OrderInfo", OrderInfo);
            pars[7] = new SqlParameter("@Fee", Fee);
            pars[8] = new SqlParameter("@Note", Note);
            pars[9] = new SqlParameter("@ApproveUser", ApproveUser);
            pars[10] = new SqlParameter("@Mobile", Mobile);
            pars[11] = new SqlParameter("@BankAccountName", BankAccountName);
            db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateFail", pars);
           return  Convert.ToInt32(pars[0].Value);
        }
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[13];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent);
            pars[4] = new SqlParameter("@LastTime", LastTime);
            pars[5] = new SqlParameter("@TotalAmount", TotalAmount);
            pars[6] = new SqlParameter("@OrderInfo", OrderInfo);
            pars[7] = new SqlParameter("@Fee", Fee);
            pars[8] = new SqlParameter("@Note", Note);
            pars[9] = new SqlParameter("@ApproveUser", ApproveUser);
            pars[10] = new SqlParameter("@Mobile", Mobile);
            pars[11] = new SqlParameter("@BankAccountName", BankAccountName);
            pars[12] = new SqlParameter("@Reward", Reward);
            db.ExecuteNonQuerySP("sp_BankCashAPI_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
    }
}
