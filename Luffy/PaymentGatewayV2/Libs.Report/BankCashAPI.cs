using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;

namespace Libs.Report
{
    public class BankCashAPI
    {
        public long TransactionID { get; set; }
        public int PartnerID { get; set; }
        public string PartnerCode { get; set; }
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
        public string ProviderCode { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Note { get; set; }
        public string RefCode { get; set; }
        public string BankCode { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public int ReturnValue { get; set; }
        public Decimal ReturnTotalValue { get; set; }
        public int Type { get; set; }
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

        // Lấy danh sách
        public DataTable GetTable(int top, string partnerCodes, string provider, DateTime requestTime, int status, string bankCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            return db.GetDataTableSP("sp_BankCashAPI_SelectList"
                , new SqlParameter("@Top", top)
                 , string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode)
                , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                , status == 0 ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status)
               , string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider)
                , new SqlParameter("@RequestTime", requestTime)
                );
        }
        // Lấy danh sách
        public DataTable GetTableV2(int top, string partnerCodes, string provider, DateTime requestTime, DateTime endtTime, int status, string bankCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            return db.GetDataTableSP("sp_BankCashAPI_SelectListV2"
                , new SqlParameter("@Top", top)
                 , string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode)
                , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                , status == -9 ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status)
               , string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider)
                , new SqlParameter("@RequestTime", requestTime)
                , new SqlParameter("@EndTime", endtTime)
                );
        }
        // 2014-05-08: Lấy danh sách
        public DataTable Search(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_BankCashAPI_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        // 2014-04-08: Tra cứu
        public DataTable Search(int top, string refCode, string orderNo, string mobile,  string bankCode, string partnerCodes, string providerCode, DateTime requestTime)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_BankCashAPI_Search"
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(refCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", refCode)
                , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
              
                , string.IsNullOrEmpty(mobile) ? new SqlParameter("@BankAccountNumber", DBNull.Value) : new SqlParameter("@BankAccountNumber", mobile)
               
                , string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode)
                , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                , string.IsNullOrEmpty(providerCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", providerCode)
                , new SqlParameter("@CreatedTime", requestTime)
                );
        }

        // 2014-04-08: Tra cứu
        public DataTable Search(int top, string refCode, string orderNo,  string mobile,  string bankCode, string partnerCodes, string providerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            return db.GetDataTableSP("sp_BankCashAPI_Search"
                 , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(refCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", refCode)
                , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
                
                , string.IsNullOrEmpty(mobile) ? new SqlParameter("@BankAccountNumber", DBNull.Value) : new SqlParameter("@BankAccountNumber", mobile)
                
                , string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode)
                , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                , string.IsNullOrEmpty(providerCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", providerCode)
                , new SqlParameter("@CreatedTime", DBNull.Value)
                );
        }

        // Lấy báo cáo
        public DataTable Report(string bankCode, string partnerCodes, string provider, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
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
        public DataTable ReportBankCode(string provider, string partnerCodes, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[2] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[3] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[4] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[5] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[6] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            DataTable dt = db.GetDataTableSP("sp_BankCashAPI_ReportGroupByBankCode", pars);
            totalTransaction = Convert.ToInt32(pars[4].Value);
            totalAmount = Convert.ToInt64(pars[5].Value);
            return dt;
        }
        // Lấy báo cáo theo đối tác
        public DataTable ReportPartner(string bankCode, string partnerCodes, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[2] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[3] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[4] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[5] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[6] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
            DataTable dt = db.GetDataTableSP("sp_BankCashAPI_ReportGroupByPartner", pars);
            totalTransaction = Convert.ToInt32(pars[4].Value);
            totalAmount = Convert.ToInt64(pars[5].Value);
            return dt;
        }

        // Lấy báo cáo theo dịch vụ
        public DataTable ReportService(string bankCode, string provider, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[1] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[2] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[3] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[4] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[5] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[6] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
            DataTable dt = db.GetDataTableSP("sp_BankCashAPI_ReportGroupByService", pars);
            totalTransaction = Convert.ToInt32(pars[4].Value);
            totalAmount = Convert.ToInt64(pars[5].Value);
            return dt;
        }
        /// <summary>
        ///  Lấy báo cáo theo nhà cung cấp
        /// </summary> 
        /// <param name="TypeSelect">1== partnerID   2= provider</param>
        /// <returns></returns>
        public List<BankCashAPI> ReportDoiSoat(string partnerCodes, string provider, string bankCode, DateTime beginTime, DateTime endTime, int TypeSelect = 1)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
            pars[3] = new SqlParameter("@BeginTime", beginTime);
            pars[4] = new SqlParameter("@EndTime", endTime);
            pars[5] = new SqlParameter("@TypeSelect", TypeSelect);
            return db.GetListSP<BankCashAPI>("sp_BankCashAPI_ReportDS", pars);
        }
        public List<BankCashAPI> ListReportDoiSoat(string partnerCodes, string provider, DateTime beginTime, DateTime endTime, int TypeSelect)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = new SqlParameter("@BeginTime", beginTime);
            pars[3] = new SqlParameter("@EndTime", endTime);
            pars[4] = new SqlParameter("@TypeSelect", TypeSelect);
            var data = db.GetListSP<BankCashAPI>("sp_BankCashAPI_ListReportDS", pars);

            return data;
        }
        public List<BankCashAPI> ReportDoiSoatDaily(string partnerCodes, string provider, DateTime beginTime, DateTime endTime, int TypeSelect)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);

            pars[3] = new SqlParameter("@BeginTime", beginTime);
            pars[4] = new SqlParameter("@EndTime", endTime);
            pars[2] = new SqlParameter("@TypeSelect", TypeSelect);
            var data = db.GetListSP<BankCashAPI>("sp_BankCashAPI_ReportDSDaily", pars);

            return data;
        }
        public List<BankCashAPI> GetBankCode()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<BankCashAPI>("sp_BankCashAPI_BankCode");
        }
        public List<BankCashAPI> GetBankCodeCache()
        {
            string KeyCache = "BankCashCode";
            try
            {
                var result = DataCaching.GetCache<List<BankCashAPI>>(KeyCache);
                if (result == null)
                {
                    result = GetBankCode();
                    result = DataCaching.SetCache(KeyCache, result, 60 * 60 * 6);
                }
                return result;
            }
            catch (Exception ex)
            {
                //   ExceptionHandler.Handle(ex, "Partners", "Get:" + KeyCache);
                return null;
            }
        }
            //public void UpdateService(long transactionID, int serviceID)
            //{
            //    DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            //    SqlParameter[] pars = new SqlParameter[3];
            //    pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //    pars[1] = new SqlParameter("@TransactionID", transactionID);
            //    pars[2] = new SqlParameter("@ServiceID", serviceID);

            //    db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateServiceID", pars);
            //    ReturnValue = Convert.ToInt32(pars[0].Value);
            //}

            //public void UpdateStatus()
            //{
            //    DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            //    SqlParameter[] pars = new SqlParameter[4];
            //    pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //    pars[1] = new SqlParameter("@TransactionID", TransactionID);
            //    pars[2] = new SqlParameter("@Status", Status);
            //    pars[3] = new SqlParameter("@LogContent", LogContent);

            //    db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateStatus", pars);
            //    ReturnValue = Convert.ToInt32(pars[0].Value);
            //}

            public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent);
            pars[4] = new SqlParameter("@LastTime", LastTime);
            pars[5] = new SqlParameter("@TotalAmount", TotalAmount);
            pars[6] = new SqlParameter("@Mobile", Mobile);
            pars[7] = new SqlParameter("@OrderInfo", OrderInfo);

            db.ExecuteNonQuerySP("sp_BankCashAPI_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public void UpdateStatusCheckStatus()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent);
            pars[4] = new SqlParameter("@LastTime", LastTime);
            pars[5] = new SqlParameter("@TotalAmount", TotalAmount);
            pars[6] = new SqlParameter("@RefCode", RefCode);
            pars[7] = new SqlParameter("@PartnerID", PartnerID);
            pars[8] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[9] = new SqlParameter("@BankAccountName", BankAccountName);
            pars[10] = new SqlParameter("@BankAccountNumber", BankAccountNumber);
            db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateStatusCheckStatus", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
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
            ReturnValue = Convert.ToInt32(pars[0].Value);
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
            ReturnValue  = Convert.ToInt32(pars[0].Value);
        }
        public DataTable ReportProfit(string partnerCodes, string bankCode, string provider, int year, int month, int day, ref int totalTransaction, ref long totalAmount, ref long totalProfit)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[9];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
            pars[2] = string.IsNullOrEmpty(provider) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", provider);
            pars[3] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[4] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[5] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[6] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[7] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[8] = new SqlParameter("@TotalProfit", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_BankCashAPI_Report_Profit", pars);
            totalTransaction = Convert.ToInt32(pars[6].Value);
            totalAmount = Convert.ToInt64(pars[7].Value);
            totalProfit = Convert.ToInt64(pars[8].Value);
            return dt;
        }
    }
}
