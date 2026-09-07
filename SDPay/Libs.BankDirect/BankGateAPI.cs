using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Libs.Db;

namespace Libs.BankDirect
{
    public class BankGateAPI
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
        public string Email { get; set; }
        public string Group { get; set; }
        public string RefCode { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public long ReturnValue { get; set; }
        public long Fee { get; set; }

        public int FeeProvider { get; set; }

        
        public long Reward { get; set; }
        
        public BankGateAPI()
        {

        }

        public BankGateAPI Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public BankGateAPI Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public BankGateAPI Get(string orderNo)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select_OrderNo"
                , new SqlParameter("@OrderNo", orderNo));
        }
        public BankGateAPI GetByOrderInfo(string orderNo)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            return db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select_OrderInfo"
                , new SqlParameter("@OrderInfo", orderNo));
        }
        public BankGateAPI GetByRefcode(string refcode, string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@PartnerCode", partnerCode);
            pars[1] = new SqlParameter("@RefCode", refcode);
            return db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select_RefCode", pars);


        }
        public BankGateAPI GetByRefcodeV2(string refcode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[1];
          
            pars[0] = new SqlParameter("@RefCode", refcode);
            return db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select_RefCodeV2", pars);


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
            pars[19] = new SqlParameter("@Group", Group);
            //BankGateAPI transaction = db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Insert", pars);
            //ReturnValue = Convert.ToInt32(pars[0].Value);
            //return transaction;
            db.ExecuteNonQuerySP("sp_BankGateAPI_Insert", pars);
            ReturnValue = TransactionID = Convert.ToInt32(pars[0].Value);
            return ReturnValue;
        }
        public long AddV2()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[23];
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
            pars[19] = new SqlParameter("@Fee", Fee);
            pars[20] = new SqlParameter("@Reward", Reward);
            pars[21] = new SqlParameter("@Group", Group);
            pars[22] = new SqlParameter("@FeeProvider", FeeProvider);
            //BankGateAPI transaction = db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Insert", pars);
            //ReturnValue = Convert.ToInt32(pars[0].Value);
            //return transaction;
            db.ExecuteNonQuerySP("sp_BankGateAPI_InsertV2", pars);
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
            pars[3] = new SqlParameter("@LogContent", LogContent == null ? "" : LogContent);

            db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateStatus", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void UpdateStatus(bool checkStatus)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent == null ? "" : LogContent);

            if (checkStatus)
            {
                db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateStatusCheckStatus", pars);
            }
            else
            {
                db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateStatus", pars);
            }
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void UpdateService(long transactionID, int serviceID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", transactionID);
            pars[2] = new SqlParameter("@ServiceID", serviceID);

            db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateServiceID", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public void UpdateBank()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@BankAccountName", BankAccountName);
            pars[3] = new SqlParameter("@BankAccountNumber", BankAccountNumber);
            pars[4] = new SqlParameter("@OrderNo", OrderNo);
            db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateBank", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public void UpdateBankV2()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@BankAccountName", BankAccountName);
            pars[3] = new SqlParameter("@BankAccountNumber", BankAccountNumber);
            pars[4] = new SqlParameter("@OrderNo", OrderNo);
            pars[5] = new SqlParameter("@BankCode", BankCode);
            db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateBankV2", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public long Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent);
            pars[4] = new SqlParameter("@LastTime", LastTime);
            pars[5] = new SqlParameter("@TotalAmount", TotalAmount);
            pars[6] = new SqlParameter("@Mobile", Mobile);
            pars[7] = new SqlParameter("@OrderInfo", OrderInfo);
            pars[8] = new SqlParameter("@Fee", Fee);
            pars[9] = new SqlParameter("@Reward", Reward);
            pars[10] = new SqlParameter("@FeeProvider", FeeProvider);
            try
            {
                db.ExecuteNonQuerySP("sp_BankGateAPI_Update", pars);
                ReturnValue = Convert.ToInt32(pars[0].Value);

            }
            catch (SqlException ex)
            {
                ReturnValue = -99;
                if (ex.Number == 1205) // Mã lỗi deadlock
                {
                    db.ExecuteNonQuerySP("sp_BankGateAPI_Update", pars);
                    ReturnValue = Convert.ToInt32(pars[0].Value);
                }

            }
            return ReturnValue;
        }
    }
}
