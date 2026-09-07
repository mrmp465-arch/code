using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.Report
{
    public class BankCashAPIReport
    {
        public string PartnerCode { get; set; }
        public int Type { get; set; }
        public int TotalTrans { get; set; }

        public int? TotalTransProcess { get; set; }

        
        public int TotalTransSuccess { get; set; }
        public long TotalFee { get; set; }
        public long TotalAmountSuccess { get; set; }
    }
    public class BankCashReportProfit
    {
        public long Time { get; set; }
        public long TotalTransaction { get; set; }
        public long TotalAmount { get; set; }
        public long Fee { get; set; }
        public long Reward { get; set; }

        public long Profit { get; set; }

    }
    public class BankDashboardReport
    {
        public string PartnerCode { get; set; }
        public string Type { get; set; }
        public long TotalAmountSuccess { get; set; }
        public long TotalFee { get; set; }
        public int TotalTrans { get; set; }
        public int TotalTransSuccess { get; set; }


        public long TotalAmountSuccessCash { get; set; }
        public long TotalFeeCash { get; set; }
        public int TotalTransCash { get; set; }
        public int TotalTransSuccessCash { get; set; }

        public string Fit { get; set; }
        public long TotalDeduct { get; set; }
        public long TotalTopup { get; set; }
    }
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
        public string ApproveUser { get; set; }
        public string RefCode { get; set; }
        public string BankCode { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public int ReturnValue { get; set; }
        public long Fee { get; set; }
        public long Reward { get; set; }
        public Decimal ReturnTotalValue { get; set; }

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
        public BankCashAPI GetByRefcode(string refcode, string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@PartnerCode", partnerCode);
            pars[1] = new SqlParameter("@RefCode", refcode);
            return db.GetInstanceSP<BankCashAPI>("sp_BankCashAPI_Select_RefCode", pars);


        }
        // Lấy danh sách
        public DataTable GetTable(int top, string partnerCodes, string Mobile, DateTime fromdate, DateTime requestTime, int? status, string bankCode, string refCode, long? amount = null, string ProviderCode = null)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            return db.GetDataTableSP("sp_BankCashAPI_SelectList"
                , new SqlParameter("@Top", top)
                 , string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode)
                 , string.IsNullOrEmpty(refCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", refCode)
                , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                 , string.IsNullOrEmpty(ProviderCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", ProviderCode)
                , status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status)
                 , amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", amount)
               , string.IsNullOrEmpty(Mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", Mobile)
               , new SqlParameter("@FromDate", fromdate)
                , new SqlParameter("@RequestTime", requestTime)
                );
        }
        public DataTable GetTableV2(int top, string partnerCodes, string Mobile, DateTime fromdate, DateTime requestTime, int? status, string bankCode, string refCode, long? amount = null)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_BankCashAPI_SelectList"
                , new SqlParameter("@Top", top)
                 , string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode)
                 , string.IsNullOrEmpty(refCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", refCode)
                , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                , status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status)
                 , amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", amount)
               , string.IsNullOrEmpty(Mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", Mobile)
               , new SqlParameter("@FromDate", fromdate)
                , new SqlParameter("@RequestTime", requestTime)
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
        public DataTable Search(int top, string refCode, string orderNo, string mobile, string bankCode, string partnerCodes, string providerCode, DateTime requestTime)
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
                , new SqlParameter("@CreatedTime", requestTime)
                );
        }

        // 2014-04-08: Tra cứu
        public DataTable Search(int top, string refCode, string orderNo, string mobile, string bankCode, string partnerCodes, string providerCode)
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
        public DataTable Report(string bankCode, string partnerCodes, string provider, int type, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[9];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[8] = new SqlParameter("@Type", type);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[7] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
            DataTable dt = db.GetDataTableSP("sp_BankCashAPI_Report", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }
        public DataTable ReportCheck(string bankCode, string partnerCodes, string bankaccount, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = new SqlParameter("@BankAccountNumber", bankaccount);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[7] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
            DataTable dt = db.GetDataTableSP("sp_BankCashAPI_ReportCheck", pars);
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
        public List<BankCashAPIReport> ReportDashboard(string partnerCodes, DateTime beginTime, DateTime endTime, string Mobile = "")
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);

            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);
            pars[3] = string.IsNullOrEmpty(Mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", Mobile);
            return db.GetListSP<BankCashAPIReport>("sp_BankCashAPI_ReportDB", pars);
        }
        public List<BankCashAPIReport> ReportDashboardV2(string partnerCodes, DateTime beginTime, DateTime endTime, string provider, bool isApp)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);

            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);
            pars[3] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[4] = !isApp ? new SqlParameter("@IsApp ", false) : new SqlParameter("@IsApp ", isApp);
            return db.GetListSP<BankCashAPIReport>("sp_BankCashAPI_ReportDBV2", pars);
        }
        public List<BankCashAPIReport> ReportDashboardPartner(string partnerCodes, DateTime beginTime, DateTime endTime)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);

            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);

            return db.GetListSP<BankCashAPIReport>("sp_BankCashAPI_ReportDBPartner", pars);
        }
        public List<BankCashAPI> GetBankCode()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<BankCashAPI>("sp_BankCashAPI_BankCode");
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
        public void UpdateV2()
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

            pars[10] = new SqlParameter("@Reward", Reward);
            pars[11] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[12] = new SqlParameter("@Mobile", Mobile);
            db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateV2", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public void Update()
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
            pars[11] = new SqlParameter("@Mobile", Mobile);
            pars[10] = new SqlParameter("@Reward", Reward);
            db.ExecuteNonQuerySP("sp_BankCashAPI_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public int UpdateApp()
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
            pars[11] = new SqlParameter("@Mobile", Mobile);
            pars[10] = new SqlParameter("@Reward", Reward);
            db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateApp", pars);
            return Convert.ToInt32(pars[0].Value);

        }
        public int UpdateApp2()
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
            pars[11] = new SqlParameter("@Mobile", Mobile);
            pars[10] = new SqlParameter("@Reward", Reward);
            db.ExecuteNonQuerySP("sp_BankCashAPI_UpdateApp2", pars);
            return Convert.ToInt32(pars[0].Value);

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
        //public DataTable ReportProfit(string partnerCodes, string bankCode, string provider, int year, int month, int day, ref int totalTransaction, ref long totalAmount, ref long totalProfit)
        //{
        //    DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
        //    SqlParameter[] pars = new SqlParameter[9];
        //    pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
        //    pars[1] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
        //    pars[2] = string.IsNullOrEmpty(provider) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", provider);
        //    pars[3] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
        //    pars[4] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
        //    pars[5] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
        //    pars[6] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
        //    pars[7] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
        //    pars[8] = new SqlParameter("@TotalProfit", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

        //    DataTable dt = db.GetDataTableSP("sp_BankCashAPI_Report_Profit", pars);
        //    totalTransaction = Convert.ToInt32(pars[6].Value);
        //    totalAmount = Convert.ToInt64(pars[7].Value);
        //    totalProfit = Convert.ToInt64(pars[8].Value);
        //    return dt;
        //}
        public List<BankCashReportProfit> ReportProfit(string group, string partnerCodes, DateTime beginTime, DateTime endTime)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);
            pars[3] = string.IsNullOrEmpty(group) ? new SqlParameter("@Group", DBNull.Value) : new SqlParameter("@Group", group);
            return db.GetListSP<BankCashReportProfit>("sp_BankCashAPI_Report_ProfitV2", pars);

            //return dt;
        }
    }
}
