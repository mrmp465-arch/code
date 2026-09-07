using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.Report
{
    public class CardAPILogReport
    {
        public string CardType { get; set; }
        public string PartnerCode { get; set; }
        public long TotalAmount { get; set; }

        public int Time { get; set; }
    }
    public class CardAPILogReportDB
    {
        public string PartnerCode { get; set; }
        //public int Type { get; set; }
        public int TotalTrans { get; set; }
        public int? TotalTransSuccess { get; set; }
        public long TotalFee { get; set; }
        public long TotalAmountSuccess { get; set; }
    }
    public class CardAPILog
    {
        public long TransactionID { get; set; }
        public int PartnerID { get; set; }
        public string PartnerCode { get; set; }
        public string RequestNo { get; set; }
        public string AccountName { get; set; }
        public long AccountID { get; set; }
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public long Amount { get; set; }
        public long AmountUser { get; set; }
        public string CardType { get; set; }
        public string Provider { get; set; }
        public DateTime CreatTime { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public DateTime LastTime { get; set; }
        public string CallbackUrl { get; set; }
        public long AmountReal { get; set; }
        public int ReturnValue { get; set; }
        public long Fee { get; set; }
        public long Reward { get; set; }
        public CardAPILog()
        {

        }
       
        public CardAPILog Get()
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetInstanceSP<CardAPILog>("sp_CardAPILog_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public CardAPILog Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<CardAPILog>("sp_CardAPILog_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        /// <summary>
        /// Lấy danh sách
        /// </summary>
        /// <param name="PartnerCodes">vd: 1,3,4</param>
        /// <param name="status">vd: null=all,0,1,2,3</param>
        public DataTable GetTable(int top, string partnerCodes, DateTime creatTime, int? status, string cardType, string provider)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_CardAPILog_SelectList"
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                , new SqlParameter("@CreatTime", creatTime)
                , status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status)
                , string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", cardType)
                , string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider)
                );
        }

        public List<CardAPILog> GetTableList(int top, string PartnerCodes, DateTime creatTime, int? status, string cardType, string provider)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@Top", top);
            pars[1] = string.IsNullOrEmpty(PartnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", PartnerCodes);
            pars[2] = new SqlParameter("@CreatTime", creatTime);
            pars[3] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
            pars[4] = string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", cardType);
            pars[5] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            return db.GetListSP<CardAPILog>("sp_CardAPILog_SelectList", pars);
        }
        public List<CardAPILog> GetTableListV2(int top, string PartnerCodes, DateTime creatTime, DateTime endTime, int? status, string cardType, string provider,string key)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = new SqlParameter("@Top", top);
            pars[1] = string.IsNullOrEmpty(PartnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", PartnerCodes);
            pars[2] = new SqlParameter("@CreatTime", creatTime);
            pars[6] = new SqlParameter("@EndTime", endTime);
            pars[7] = new SqlParameter("@Key", key);
            pars[3] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
            pars[4] = string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", cardType);
            pars[5] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            return db.GetListSP<CardAPILog>("sp_CardAPILog_SelectListV2", pars);
        }
        public List<CardAPILogReportDB> ReportDashboard(string partnerCodes, DateTime beginTime, DateTime endTime, string bankId = "", int type = 1)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);

            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);
          
            return db.GetListSP<CardAPILogReportDB>("sp_CardAPILog_ReportDB", pars);
        }
        public List<CardAPILogReportDB> ReportDashboardPartner(string partnerCodes, DateTime beginTime, DateTime endTime)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);

            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);

            return db.GetListSP<CardAPILogReportDB>("sp_CardAPILog_ReportDBPartner", pars);
        }
        public List<CardAPILog> ListReportDoiSoat(string partnerCodes, string provider, string cardType, DateTime beginTime, DateTime endTime, int TypeSelect)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", cardType);
            pars[3] = new SqlParameter("@BeginTime", beginTime);
            pars[4] = new SqlParameter("@EndTime", endTime);
            pars[5] = new SqlParameter("@TypeSelect", TypeSelect);

            var data = db.GetListSP<CardAPILog>("sp_CardAPILog_ListReportDS", pars);

            return data;
        }
        // 2014-0408: Tìm kiếm giao dich
        public DataTable Search(string partnerCodes, int top, string accountName, string cardSerial, string cardCode, string requestNo, DateTime creatTime, string providerCodes)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_CardAPILog_Search"
                , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(accountName) ? new SqlParameter("@AccountName", DBNull.Value) : new SqlParameter("@AccountName", accountName)
                , string.IsNullOrEmpty(cardSerial) ? new SqlParameter("@CardSerial", DBNull.Value) : new SqlParameter("@CardSerial", cardSerial)
                , string.IsNullOrEmpty(requestNo) ? new SqlParameter("@RequestNo", DBNull.Value) : new SqlParameter("@RequestNo", requestNo)
                , new SqlParameter("@CreatTime", creatTime)
                , string.IsNullOrEmpty(providerCodes) ? new SqlParameter("@ProviderCodes", DBNull.Value) : new SqlParameter("@ProviderCodes", providerCodes)
                , string.IsNullOrEmpty(cardCode) ? new SqlParameter("@CardCode", DBNull.Value) : new SqlParameter("@CardCode", cardCode)
                );
        }

        // 2014-0408: Tìm kiếm giao dich
        public DataTable Search(string partnerCodes, int top, string accountName, string cardSerial, string cardCode, string requestNo, string providerCodes)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_CardAPILog_Search"
                , string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes)
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(accountName) ? new SqlParameter("@AccountName", DBNull.Value) : new SqlParameter("@AccountName", accountName)
                , string.IsNullOrEmpty(cardSerial) ? new SqlParameter("@CardSerial", DBNull.Value) : new SqlParameter("@CardSerial", cardSerial)
                , string.IsNullOrEmpty(requestNo) ? new SqlParameter("@RequestNo", DBNull.Value) : new SqlParameter("@RequestNo", requestNo)
                , new SqlParameter("@CreatTime", DBNull.Value)
                , string.IsNullOrEmpty(providerCodes) ? new SqlParameter("@ProviderCodes", DBNull.Value) : new SqlParameter("@ProviderCodes", partnerCodes)
                , string.IsNullOrEmpty(cardCode) ? new SqlParameter("@CardCode", DBNull.Value) : new SqlParameter("@CardCode", cardCode)
                );
        }

        // Lấy báo cáo
        public DataTable Report(string partnerCodes, string cardType, string provider, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", cardType);
            pars[2] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[3] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[4] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[5] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[6] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[7] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_CardAPILog_Report", pars);
            totalTransaction = Convert.ToInt32(pars[6].Value);
            totalAmount = Convert.ToInt64(pars[7].Value);
            return dt;
        }

        public DataTable ReportProfit(string partnerCodes, string cardType, string provider, int year, int month, int day, ref int totalTransaction, ref long totalAmount, ref long totalProfit)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[9];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", cardType);
            pars[2] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[3] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[4] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[5] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[6] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[7] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[8] = new SqlParameter("@TotalProfit", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_CardAPILog_Report_Profit", pars);
            totalTransaction = Convert.ToInt32(pars[6].Value);
            totalAmount = Convert.ToInt64(pars[7].Value);
            totalProfit = Convert.ToInt64(pars[8].Value);
            return dt;
        }

        // Lấy báo cáo theo loại thẻ
        public DataTable ReportCardType(string partnerCodes, string Provider, string CardType, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(Provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", Provider);
            pars[2] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", CardType);
            pars[3] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[4] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[5] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[6] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[7] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_CardAPILog_ReportGroupByCardType", pars);
            totalTransaction = Convert.ToInt32(pars[6].Value);
            totalAmount = Convert.ToInt64(pars[7].Value);
            return dt;
        }
        // Lấy báo cáo theo nhà cung cấp
        public DataTable ReportPartner(string partnerCodes, string CardType, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", CardType);
            pars[1] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);

            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_CardAPILog_ReportGroupByPartner", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }

        public List<CardAPILogReport> ReportPartner2(string partnerCodes, string CardType, int year, int month, int day)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", CardType);
            pars[1] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);

            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            //DataTable dt = db.GetDataTableSP("sp_CardAPILog_ReportGroupByPartnerV2", pars);
            return db.GetListSP<CardAPILogReport>("sp_CardAPILog_ReportGroupByPartnerV2", pars);
            
        }
        public List<CardAPILogReport> ReportCardType2(string partnerCodes, string CardType, int year, int month, int day)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", CardType);
            pars[1] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);

            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            //DataTable dt = db.GetDataTableSP("sp_CardAPILog_ReportGroupByPartnerV2", pars);
            return db.GetListSP<CardAPILogReport>("sp_CardAPILog_ReportGroupByCardTypeV2", pars);

        }
        // Lấy báo cáo theo nhà cung cấp
        public DataTable ReportProvider(string Provider, string CardType, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", CardType);
            pars[1] = string.IsNullOrEmpty(Provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", Provider);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_CardAPILog_ReportGroupByProvider", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }

        /// <summary>
        ///  Lấy báo cáo theo nhà cung cấp
        /// </summary> 
        /// <param name="TypeSelect">1== partnerID   2= provider</param>
        /// <returns></returns>
        public List<CardAPILog> ReportDoiSoat(string partnerCodes, string provider, string cardType, DateTime beginTime, DateTime endTime, int TypeSelect, ref int AmountResidual)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = string.IsNullOrEmpty(cardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", cardType);
            pars[3] = new SqlParameter("@BeginTime", beginTime);
            pars[4] = new SqlParameter("@EndTime", endTime);
            pars[5] = new SqlParameter("@TypeSelect", TypeSelect);
            pars[6] = new SqlParameter("@AmountResidual", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var data= db.GetListSP<CardAPILog>("sp_CardAPILog_ReportDS", pars);
            AmountResidual = Convert.ToInt32(pars[6].Value);
            return data;
        }

        // Lấy báo cáo theo nhà cung cấp
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = new SqlParameter("@TransactionID", TransactionID);
            pars[1] = new SqlParameter("@Status", Status);
            pars[2] = new SqlParameter("@Amount", Amount);
            pars[3] = new SqlParameter("@Description", Description);
            pars[5] = new SqlParameter("@AccountID", AccountID);
            pars[4] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@Fee", Fee);
            pars[7] = new SqlParameter("@Reward", Reward);
            db.ExecuteNonQuerySP("sp_CardAPILog_Update", pars);
            ReturnValue = Convert.ToInt32(pars[4].Value);
        }
        public List<CardAPILog> GetCardType()
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetListSP<CardAPILog>("sp_CardAPILog_CardType");
        }
    }
}
