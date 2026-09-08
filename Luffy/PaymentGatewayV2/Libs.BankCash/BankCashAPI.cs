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

        public string Note { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public long ReturnValue { get; set; }

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
        public BankCashAPI GetByRefcode(string refcode, string partnercode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankCashAPI>("sp_BankCashAPI_Select_RefCode"
                , new SqlParameter("@RefCode", refcode)
                , new SqlParameter("@PartnerCode", partnercode)
                );
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

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent);
            pars[4] = new SqlParameter("@LastTime", LastTime);
            pars[5] = new SqlParameter("@TotalAmount", TotalAmount);
            pars[6] = new SqlParameter("@OrderInfo", OrderInfo);
            db.ExecuteNonQuerySP("sp_BankCashAPI_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public DataTable Report(string bankCode, string partnerCodes, string provider, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[7] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
            DataTable dt = db.GetDataTableSP("sp_BankCashAPI_Report", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }
        public DataTable Report2(string bankCode, string partnerCodes, string provider, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[7] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
            DataTable dt = db.GetDataTableSP("sp_BankCashAPI_Report2", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }
    }
}
