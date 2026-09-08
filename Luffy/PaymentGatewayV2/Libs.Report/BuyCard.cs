using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using DocumentFormat.OpenXml.Math;
using Libs.API;

namespace Libs.Report
{
    public class BuyCard
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
        public long Amount { get; set; }
        public long Quantity { get; set; }
        public long ErrorCode { get; set; }
        public string Message { get; set; }
        public string LogContent { get; set; }
        public string ListCards { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastTime { get; set; }
        public int Status { get; set; }
        public int ReturnValue { get; set; }
        public BuyCard()
        {

        }

        public BuyCard Get()
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetInstanceSP<BuyCard>("sp_BuyCard_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public BuyCard Get(int transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetInstanceSP<BuyCard>("sp_BuyCard_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        // Lấy danh sách
        /// <summary>
        /// 
        /// </summary> 
        /// <param name="status">null==all</param> 
        /// <returns></returns>
        public DataTable GetTable(int top, string partnerIds, DateTime CreatedTime, int? status, string TransactionNo, string provider)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_BuyCard_SelectList"
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(partnerIds) ? new SqlParameter("@partnerIds", DBNull.Value) : new SqlParameter("@partnerIds", partnerIds)
                , new SqlParameter("@CreatedTime", CreatedTime)
                , status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status)
                , string.IsNullOrEmpty(TransactionNo) ? new SqlParameter("@TransactionNo", DBNull.Value) : new SqlParameter("@TransactionNo", TransactionNo)
                , string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider)
                );
        }
        public List<BuyCard> GetTableListV2(int top, string partnerIds, DateTime creatTime, DateTime endTime, int? status, string TransactionNo)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@Top", top);
            pars[1] = string.IsNullOrEmpty(partnerIds) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerIds);
            pars[2] = new SqlParameter("@CreatTime", creatTime);
            pars[5] = new SqlParameter("@EndTime", endTime);
            pars[3] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
            pars[4] = string.IsNullOrEmpty(TransactionNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", TransactionNo);
          
            return db.GetListSP<BuyCard>("sp_BuyCard_SelectListV2", pars);
        }
        public List<BuyCard> ReportDoiSoatDaily(string partnerCodes, string provider,DateTime beginTime, DateTime endTime, int TypeSelect)
        {
            return new DBHelper(Configs.VPGLogReportConnectionStrings).GetListSP<BuyCard>("sp_BuyCard_ReportDSDaily", new SqlParameter[5]
            {
            string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes),
            string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider),
       
            new SqlParameter("@BeginTime", beginTime),
            new SqlParameter("@EndTime", endTime),
            new SqlParameter("@TypeSelect", TypeSelect)
            });
        }
        // 2014-0408: Tìm kiếm giao dich
        public DataTable Search(string partnerIds, int top, string accountName, string OrderNo, DateTime CreatedTime)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_BuyCard_Search"
                  , string.IsNullOrEmpty(partnerIds) ? new SqlParameter("@partnerIds", DBNull.Value) : new SqlParameter("@partnerIds", partnerIds)
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(accountName) ? new SqlParameter("@AccountName", DBNull.Value) : new SqlParameter("@AccountName", accountName)
                , string.IsNullOrEmpty(OrderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", OrderNo)
                , new SqlParameter("@CreatedTime", CreatedTime)
                );
        }

        // 2014-0408: Tìm kiếm giao dich
        public DataTable Search(string partnerIds, int top, string accountName, string OrderNo)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_BuyCard_Search"
                  , string.IsNullOrEmpty(partnerIds) ? new SqlParameter("@partnerIds", DBNull.Value) : new SqlParameter("@partnerIds", partnerIds)
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(accountName) ? new SqlParameter("@AccountName", DBNull.Value) : new SqlParameter("@AccountName", accountName)
                , string.IsNullOrEmpty(OrderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", OrderNo)
                , new SqlParameter("@OrderNo", DBNull.Value)
                );
        }

        /// <summary>
        /// 
        /// </summary> 
        /// <param name="providerCode">provider</param>
        /// <param name="provider"> provider=>CardType</param>  
        public DataTable Report(string partnerCodes, string providerCode, string CardType, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(providerCode) ? new SqlParameter("@providerCode", DBNull.Value) : new SqlParameter("@providerCode", providerCode);
            pars[2] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", CardType);
            pars[3] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[4] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[5] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[6] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[7] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_BuyCard_Report", pars);
            totalTransaction = Convert.ToInt32(pars[6].Value);
            totalAmount = Convert.ToInt64(pars[7].Value);
            return dt;
        }

        // Lấy báo cáo theo loại thẻ
        public DataTable ReportCardType(string partnerCodes, string providerCode,string CardType, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(providerCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", providerCode);
            pars[2] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", CardType);
            pars[3] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[4] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[5] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[6] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[7] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_BuyCard_ReportGroupByCardType", pars);
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

            DataTable dt = db.GetDataTableSP("sp_BuyCard_ReportGroupByPartner", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }
        // Lấy báo cáo theo nhà cung cấp
        public DataTable ReportProvider(string CardType, string Provider, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(Provider) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", Provider);
            pars[1] = string.IsNullOrEmpty(CardType) ? new SqlParameter("@CardType", DBNull.Value) : new SqlParameter("@CardType", CardType);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_BuyCard_ReportGroupByProvider", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        } 
        // Lấy báo cáo theo nhà cung cấp
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="provider"></param>
        /// <param name="ProviderCode"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="TypeSelect">1= select theo partner ; 2 select theo provider code</param>
        /// <returns></returns>
        public List<BuyCard> ReportDoiSoat(string partnerIds, string provider, string ProviderCode, DateTime beginTime, DateTime endTime,int TypeSelect=1)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = string.IsNullOrEmpty(partnerIds) ? new SqlParameter("@partnerIds", DBNull.Value) : new SqlParameter("@partnerIds", partnerIds); 
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = string.IsNullOrEmpty(ProviderCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", ProviderCode);
            pars[3] = new SqlParameter("@BeginTime", beginTime);
            pars[4] = new SqlParameter("@EndTime", endTime);
            pars[5] = new SqlParameter("@TypeSelect", TypeSelect);
            return db.GetListSP<BuyCard>("sp_BuyCard_ReportDS", pars);
        }

        // Lấy báo cáo theo nhà cung cấp
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@TransactionID", TransactionID);
            pars[1] = new SqlParameter("@Status", Status);
            pars[2] = new SqlParameter("@Amount", Amount);
            pars[4] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };

            db.ExecuteNonQuerySP("sp_BuyCard_Update", pars);
            ReturnValue = Convert.ToInt32(pars[4].Value);
        }
        public List<BuyCard> GetCardType()
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetListSP<BuyCard>("sp_BuyCard_CardType");
        } 
    }
}
