using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.API;

namespace Libs.Report
{
    public class BankGateAPIReport
    {
        public string PartnerCode { get; set; }
        public int Type { get; set; }
        public int TotalTrans { get; set; }
        public int TotalTransSuccess { get; set; }
        public long TotalFee { get; set; }
        public long TotalAmountSuccess { get; set; }
    }
    public class BankGateAPILogReportPartner
    {
        //public string BankCode { get; set; }
        public string PartnerCode { get; set; }
        public decimal TotalAmount { get; set; }

        public int Time { get; set; }
    }
    public class BankReportProfit
    {
        public long Time { get; set; }
        public long TotalTransaction { get; set; }
        public long TotalAmount { get; set; }
        public long Fee { get; set; }
        public long Reward { get; set; }

        public long Profit { get; set; }
        public long FeeProvider { get; set; }

    }
    public class BankReportProfitV3
    {
        public string PartnerCode { get; set; }
        public long TotalTransaction { get; set; }
        public long TotalAmount { get; set; }
        public long Fee { get; set; }
        public long Reward { get; set; }

        public long Profit { get; set; }
        public long FeeProvider { get; set; }

    }
    public class BankGateAPI
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
        public string Email { get; set; }
        public string RefCode { get; set; }
        public string BankCode { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public long ReturnValue { get; set; }
        public Decimal ReturnTotalValue { get; set; }
        public long Fee { get; set; }
        public long Reward { get; set; }

        
        public BankGateAPI()
        {

        }

        public BankGateAPI Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            return db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }
        public BankGateAPI Get(string orderNo)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select_OrderNo"
                , new SqlParameter("@OrderNo", orderNo));
        }
        public BankGateAPI Get(string orderNo,string refcode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select_OrderNoRefCode"
                , new SqlParameter("@OrderNo", orderNo), new SqlParameter("@RefCode", refcode));
        }
        public BankGateAPI Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }
        public BankGateAPI GetByOrderInfo(string orderNo)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
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

        // Lấy danh sách
        public DataTable GetTable(int top, string partnerCodes, string provider, DateTime fromdate, DateTime requestTime, int? status, string bankCode,string refCode, string orderNo, string orderInfo,long? amount=null,string bankid="",int type=1)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            return db.GetDataTableSP("sp_BankGateAPI_Search"
                , new SqlParameter("@Top", top)
                 , string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode)
                , string.IsNullOrEmpty(bankid) ? new SqlParameter("@BankAccountNumber", DBNull.Value) : new SqlParameter("@BankAccountNumber", bankid)
                 , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                , status == null? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status)
                 , amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", amount)
               , string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider)
                , string.IsNullOrEmpty(refCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", refCode)
                , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
                 , string.IsNullOrEmpty(orderInfo) ? new SqlParameter("@OrderInfo", DBNull.Value) : new SqlParameter("@OrderInfo", orderInfo)
               , new SqlParameter("@RequestTime", requestTime)
                 , new SqlParameter("@FromDate", fromdate)
                 , new SqlParameter("@Type", type)
                );
        }

        // 2014-05-08: Lấy danh sách
        public DataTable Search(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_BankGateAPI_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        // 2014-04-08: Tra cứu
        public DataTable Search(int top, string refCode, string orderNo, string orderInfo, string mobile, string email, string bankCode, string partnerCodes, string providerCode, DateTime requestTime)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            return db.GetDataTableSP("sp_BankGateAPI_Search"
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(refCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", refCode)
                , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
                , string.IsNullOrEmpty(orderInfo) ? new SqlParameter("@OrderInfo", DBNull.Value) : new SqlParameter("@OrderInfo", orderInfo)
                , string.IsNullOrEmpty(mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", mobile)
                , string.IsNullOrEmpty(email) ? new SqlParameter("@Email", DBNull.Value) : new SqlParameter("@Email", email)
                , string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode)
                , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                , string.IsNullOrEmpty(providerCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", providerCode)
                , new SqlParameter("@CreatedTime", requestTime)
                );
        }

        // 2014-04-08: Tra cứu
        public DataTable Search(int top, string refCode, string orderNo, string orderInfo, string mobile, string email, string bankCode, string partnerCodes, string providerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            return db.GetDataTableSP("sp_BankGateAPI_Search"
                 , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(refCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", refCode)
                , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
                 , string.IsNullOrEmpty(orderInfo) ? new SqlParameter("@OrderInfo", DBNull.Value) : new SqlParameter("@OrderInfo", orderInfo)
                , string.IsNullOrEmpty(mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", mobile)
                , string.IsNullOrEmpty(email) ? new SqlParameter("@Email", DBNull.Value) : new SqlParameter("@Email", email)
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
            DataTable dt = db.GetDataTableSP("sp_BankGateAPI_Report", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }
        public List<BankGateAPIReport> ReportDashboardV2(string partnerCodes, DateTime beginTime, DateTime endTime, string bankId = "", int type = 1, string providercode = "")
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);

            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);
            pars[3] = string.IsNullOrEmpty(bankId) ? new SqlParameter("@BankAccountNumber", DBNull.Value) : new SqlParameter("@BankAccountNumber", bankId);
            pars[4] = new SqlParameter("@Type", type);
            pars[5] = string.IsNullOrEmpty(providercode) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", providercode);
            return db.GetListSP<BankGateAPIReport>("sp_BankGateAPI_ReportDBV2", pars);
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
            DataTable dt = db.GetDataTableSP("sp_BankGateAPI_ReportGroupByBankCode", pars);
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
            DataTable dt = db.GetDataTableSP("sp_BankGateAPI_ReportGroupByPartner", pars);
            totalTransaction = Convert.ToInt32(pars[4].Value);
            totalAmount = Convert.ToInt64(pars[5].Value);
            return dt;
        }
        public List<BankGateAPILogReportPartner> ReportPartner2(string bankCode, string partnerCodes, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
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
            var dt = db.GetListSP<BankGateAPILogReportPartner>("sp_BankGateAPI_ReportGroupByPartnerV2", pars);
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
            DataTable dt = db.GetDataTableSP("sp_BankGateAPI_ReportGroupByService", pars);
            totalTransaction = Convert.ToInt32(pars[4].Value);
            totalAmount = Convert.ToInt64(pars[5].Value);
            return dt;
        }
        /// <summary>
        ///  Lấy báo cáo theo nhà cung cấp
        /// </summary> 
        /// <param name="TypeSelect">1== partnerID   2= provider</param>
        /// <returns></returns>
        public List<BankGateAPI> ReportDoiSoat(string partnerCodes, string provider, string bankCode, DateTime beginTime, DateTime endTime, int TypeSelect = 1)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
            pars[3] = new SqlParameter("@BeginTime", beginTime);
            pars[4] = new SqlParameter("@EndTime", endTime);
            pars[5] = new SqlParameter("@TypeSelect", TypeSelect);
            return db.GetListSP<BankGateAPI>("sp_BankGateAPI_ReportDS", pars);
        }
        public List<BankGateAPI> ListReportDoiSoat(string partnerCodes, string provider, DateTime beginTime, DateTime endTime, int TypeSelect)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = new SqlParameter("@BeginTime", beginTime);
            pars[3] = new SqlParameter("@EndTime", endTime);
            pars[4] = new SqlParameter("@TypeSelect", TypeSelect);
            var data = db.GetListSP<BankGateAPI>("sp_BankGateAPI_ListReportDS", pars);

            return data;
        }

        public List<BankGateAPIReport> ReportDashboard(string partnerCodes, DateTime beginTime, DateTime endTime,string bankId="",int type=1,string provider="")
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);

            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);
            pars[3] = string.IsNullOrEmpty(bankId) ? new SqlParameter("@BankAccountNumber", DBNull.Value) : new SqlParameter("@BankAccountNumber", bankId);
            pars[4] = new SqlParameter("@Type", type);
            pars[5] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            return db.GetListSP<BankGateAPIReport>("sp_BankGateAPI_ReportDB", pars);
        }
        public List<BankGateAPIReport> ReportDashboardPartner(string partnerCodes, DateTime beginTime, DateTime endTime)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);

            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);

            return db.GetListSP<BankGateAPIReport>("sp_BankGateAPI_ReportDBPartner", pars);
        }
        public List<BankGateAPI> GetBankCode()
        {
            //DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            //return db.GetListSP<BankGateAPI>("sp_BankGateAPI_BankCode");
            var data = new List<BankGateAPI>();
            data.Add(new BankGateAPI { BankCode="MOMO"});
            data.Add(new BankGateAPI { BankCode = "ACB" });
            data.Add(new BankGateAPI { BankCode = "VPB" });
            data.Add(new BankGateAPI { BankCode = "VCB" });
            data.Add(new BankGateAPI { BankCode = "NAB" });
            data.Add(new BankGateAPI { BankCode = "MB" });
            data.Add(new BankGateAPI { BankCode = "ICB" });
            data.Add(new BankGateAPI { BankCode = "TIMO" });
            data.Add(new BankGateAPI { BankCode = "SEAB" });
            data.Add(new BankGateAPI { BankCode = "OCB" });
            return data;
        }
        //public void UpdateService(long transactionID, int serviceID)
        //{
        //    DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
        //    SqlParameter[] pars = new SqlParameter[3];
        //    pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //    pars[1] = new SqlParameter("@TransactionID", transactionID);
        //    pars[2] = new SqlParameter("@ServiceID", serviceID);

        //    db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateServiceID", pars);
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

        //    db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateStatus", pars);
        //    ReturnValue = Convert.ToInt32(pars[0].Value);
        //}

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[9];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent);
            pars[4] = new SqlParameter("@LastTime", LastTime);
            pars[5] = new SqlParameter("@TotalAmount", TotalAmount);
            pars[6] = new SqlParameter("@Mobile", Mobile);
            pars[7] = new SqlParameter("@OrderInfo", OrderInfo);
            pars[8] = new SqlParameter("@Fee", Fee);

            db.ExecuteNonQuerySP("sp_BankGateAPI_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public void UpdateCMS()
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
            pars[9] = new SqlParameter("@Signature", Signature);
            pars[10] = new SqlParameter("@Reward", Reward);
            db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateCMS", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public void UpdateContent()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@OrderNo", OrderNo);
            pars[3] = new SqlParameter("@PartnerID", PartnerID);
            pars[4] = new SqlParameter("@PartnerCode", PartnerCode);
            db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateContent", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public void UpdateStatusCheckStatus()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[12];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@LogContent", LogContent);
            pars[4] = new SqlParameter("@LastTime", LastTime);
            pars[5] = new SqlParameter("@TotalAmount", TotalAmount);
            pars[6] = new SqlParameter("@RefCode", RefCode);
            pars[7] = new SqlParameter("@PartnerID", PartnerID);
            pars[8] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[9] = new SqlParameter("@OrderInfo", OrderInfo);
            pars[10] = new SqlParameter("@Mobile", Mobile);
            pars[11] = new SqlParameter("@Fee", Fee);
            db.ExecuteNonQuerySP("sp_BankGateAPI_UpdateStatusCheckStatus", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public List<BankReportProfit> ReportProfit(string group,string partnerCodes, DateTime beginTime, DateTime endTime)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);
            pars[3] = string.IsNullOrEmpty(group) ? new SqlParameter("@Group", DBNull.Value) : new SqlParameter("@Group", group);
            return db.GetListSP<BankReportProfit>("sp_BankGateAPI_Report_ProfitV2", pars);
          
            //return dt;
        }
        public List<BankReportProfitV3> ReportProfitV3(string group, string partnerCodes, DateTime beginTime, DateTime endTime)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);
            pars[3] = string.IsNullOrEmpty(group) ? new SqlParameter("@Group", DBNull.Value) : new SqlParameter("@Group", group);
            return db.GetListSP<BankReportProfitV3>("sp_BankGateAPI_Report_ProfitV3", pars);

            //return dt;
        }
    }
}
