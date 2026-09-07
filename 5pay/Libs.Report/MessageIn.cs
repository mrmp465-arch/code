using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.Report
{
    public class MessageIn
    {
        public long Id { get; set; }
        public string Provider { get; set; }
        public int Type { get; set; }
        public long SenderId { get; set; }
        public string SenderNumber { get; set; }
        public long ReceiverId { get; set; }
        public string ReceiverNumber { get; set; }
        public string Subject { get; set; }
        public int PartnerId { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerCommand { get; set; }
        public string Content { get; set; }
        public DateTime SentTime { get; set; }
        public DateTime ReceivedTime { get; set; }
        public string RefTranId { get; set; }
        public long Amount { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public int ReturnValue { get; set; }
        public MessageIn()
        {

        }

        public MessageIn Get()
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetInstanceSP<MessageIn>("sp_MessageIn_Select"
                , new SqlParameter("@Id", Id));
        }
         

        // Lấy danh sách
        /// <summary>
        /// 
        /// </summary> 
        /// <param name="status">null==all</param> 
        /// <returns></returns>
        public DataTable GetTable(int top, string PartnerCodes, DateTime CreatedTime, int? status, string SenderNumber, string provider)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_MessageIn_SelectList"
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(PartnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", PartnerCodes)
                , new SqlParameter("@CreatedTime", CreatedTime)
                , status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status)
                , string.IsNullOrEmpty(SenderNumber) ? new SqlParameter("@SenderNumber", DBNull.Value) : new SqlParameter("@SenderNumber", SenderNumber)
                , string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider)
                );
        }

        // 2014-0408: Tìm kiếm giao dich
        public DataTable Search(string PartnerCodes, int top, string SenderNumber, string subject, DateTime CreatedTime)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_MessageIn_Search" 
                , string.IsNullOrEmpty(PartnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", PartnerCodes)
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(subject) ? new SqlParameter("@ReceiverNumber", DBNull.Value) : new SqlParameter("@ReceiverNumber", subject)
                , string.IsNullOrEmpty(SenderNumber) ? new SqlParameter("@SenderNumber", DBNull.Value) : new SqlParameter("@SenderNumber", SenderNumber)
                , new SqlParameter("@CreatedTime", CreatedTime)
                );
        }
        public DataTable Search(string PartnerCodes, int top, string SenderNumber, string Subject)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_MessageIn_Search"
                  , string.IsNullOrEmpty(PartnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", PartnerCodes)
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(SenderNumber) ? new SqlParameter("@SenderNumber", DBNull.Value) : new SqlParameter("@SenderNumber", SenderNumber)
                , string.IsNullOrEmpty(Subject) ? new SqlParameter("@ReceiverNumber", DBNull.Value) : new SqlParameter("@ReceiverNumber", Subject)
                , new SqlParameter("@Subject", DBNull.Value)
                );
        }
        /// <summary>
        /// 
        /// </summary> 
        /// <param name="Provider">provider</param>
        /// <param name="provider"> provider=>ReceiverNumber</param>  
        public DataTable Report(string PartnerCodes, string Provider, string ReceiverNumber, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = string.IsNullOrEmpty(PartnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", PartnerCodes); 
            pars[1] = string.IsNullOrEmpty(ReceiverNumber) ? new SqlParameter("@ReceiverNumber", DBNull.Value) : new SqlParameter("@ReceiverNumber", ReceiverNumber);
            pars[2] = string.IsNullOrEmpty(Provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", Provider);
            pars[3] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[4] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[5] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[6] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[7] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_MessageIn_Report", pars);
            totalTransaction = Convert.ToInt32(pars[6].Value);
            totalAmount = Convert.ToInt64(pars[7].Value);
            return dt;
        }

        // Lấy báo cáo theo loại thẻ
        public DataTable ReportReceiverNumber(string PartnerCodes, string Provider,string ReceiverNumber, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = string.IsNullOrEmpty(PartnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", PartnerCodes);
            pars[1] = string.IsNullOrEmpty(ReceiverNumber) ? new SqlParameter("@ReceiverNumber", DBNull.Value) : new SqlParameter("@ReceiverNumber", ReceiverNumber);
            pars[2] = string.IsNullOrEmpty(Provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", Provider); 
            pars[3] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[4] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[5] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[6] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[7] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_MessageIn_ReportGroupByCardType", pars);
            totalTransaction = Convert.ToInt32(pars[6].Value);
            totalAmount = Convert.ToInt64(pars[7].Value);
            return dt;
        }
        // Lấy báo cáo theo nhà cung cấp
        public DataTable ReportPartner(string PartnerCode, string ReceiverNumber, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(ReceiverNumber) ? new SqlParameter("@ReceiverNumber", DBNull.Value) : new SqlParameter("@ReceiverNumber", ReceiverNumber);
            pars[1] = string.IsNullOrEmpty(PartnerCode) ? new SqlParameter("@PartnerCode", DBNull.Value) : new SqlParameter("@PartnerCode", PartnerCode);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_MessageIn_ReportGroupByPartner", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }
        // Lấy báo cáo theo nhà cung cấp
        public DataTable ReportProvider(string ReceiverNumber, string Provider, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(ReceiverNumber) ? new SqlParameter("@ReceiverNumber", DBNull.Value) : new SqlParameter("@ReceiverNumber", ReceiverNumber);
            pars[1] = string.IsNullOrEmpty(Provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", Provider);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            DataTable dt = db.GetDataTableSP("sp_MessageIn_ReportGroupByProvider", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        } 
        // Lấy báo cáo theo nhà cung cấp 
        /// <param name="TypeSelect">1= select theo partner ; 2 select theo provider code</param>
        /// <returns></returns>
        public List<MessageIn> ReportDoiSoat(string partnerCodes, string provider, string ReceiverNumber, DateTime beginTime, DateTime endTime, int TypeSelect = 1)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = string.IsNullOrEmpty(partnerCodes) ? new SqlParameter("@PartnerCodes", DBNull.Value) : new SqlParameter("@PartnerCodes", partnerCodes);
            pars[1] = string.IsNullOrEmpty(provider) ? new SqlParameter("@Provider", DBNull.Value) : new SqlParameter("@Provider", provider);
            pars[2] = string.IsNullOrEmpty(ReceiverNumber) ? new SqlParameter("@ReceiverNumber", DBNull.Value) : new SqlParameter("@ReceiverNumber", ReceiverNumber);
            pars[3] = new SqlParameter("@BeginTime", beginTime);
            pars[4] = new SqlParameter("@EndTime", endTime);
            pars[5] = new SqlParameter("@TypeSelect", TypeSelect);
            return db.GetListSP<MessageIn>("sp_MessageIn_ReportDS", pars);
        }

        // Lấy báo cáo theo nhà cung cấp
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@Id", Id);
            pars[1] = new SqlParameter("@Status", Status);
            pars[2] = new SqlParameter("@Amount", Amount);
            pars[4] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };

            db.ExecuteNonQuerySP("sp_MessageIn_Update", pars);
            ReturnValue = Convert.ToInt32(pars[4].Value);
        }
        public List<MessageIn> GetReceiverNumber()
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetListSP<MessageIn>("sp_MessageIn_CardType");
        } 
    }
}
