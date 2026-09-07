using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.Report
{
    public class TopupMobileTransactionLog
    {

        public long Id { set; get; }
        public long IdOrder { set; get; }
        public long TransactionID { set; get; }
        public string UserName { set; get; }
        public string OrderNo { set; get; }
        public string FullName { set; get; }
        public string Telco { set; get; }
        public string Mobile { set; get; }
        public int Amount { set; get; }
        public int AmountUser { set; get; }
        public System.DateTime CreateTime { set; get; }
        public System.DateTime LastTime { set; get; }
        public int Status { set; get; }
        private string CardSerial { get; set; }
        private string CardCode{ get; set; }
        private string Core { get; set; }
        public TopupMobileTransactionLog()
        {
        }

        //public TopupMobileLog Get()
        //{
        //    DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
        //    return db.GetInstanceSP<TopupMobileLog>("sp_TopupMobileLog_Select"
        //        , new SqlParameter("@TransactionID", TransactionID));
        //}

        public DataTable GetTable(int top, string userIds, string telco, string requestNo, string mobile, DateTime createdTime, int? status, string cardSerial, string cardCode, string orderNo, string core)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_TopupMobileLogTransaction_SelectList"
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(userIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", userIds)
                , string.IsNullOrEmpty(telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", telco)
                , string.IsNullOrEmpty(requestNo) ? new SqlParameter("@RequestNo", DBNull.Value) : new SqlParameter("@RequestNo", requestNo)
                , string.IsNullOrEmpty(mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", mobile)
                , new SqlParameter("@CreatedTime", createdTime)
                , status == null ? new SqlParameter("@status", DBNull.Value) : new SqlParameter("@status", status)
                , string.IsNullOrEmpty(cardSerial) ? new SqlParameter("@CardSerial", DBNull.Value) : new SqlParameter("@CardSerial", cardSerial)
                , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
                , string.IsNullOrEmpty(cardCode) ? new SqlParameter("@CardCode", DBNull.Value) : new SqlParameter("@CardCode", cardCode)
                , string.IsNullOrEmpty(core) ? new SqlParameter("@Core", DBNull.Value) : new SqlParameter("@Core", core)
                );
        }

        //public DataTable Search(int top, string UserIds, string Telco, string RequestNo, string Mobile, int Amount, DateTime CreatedTime)
        //{
        //    DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
        //    return db.GetDataTableSP("sp_TopupMobileLog_Search"
        //        , new SqlParameter("@Top", top)
        //          , string.IsNullOrEmpty(UserIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", UserIds)
        //         , string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco)
        //        , string.IsNullOrEmpty(RequestNo) ? new SqlParameter("@RequestNo", DBNull.Value) : new SqlParameter("@RequestNo", RequestNo)
        //        , string.IsNullOrEmpty(Mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", Mobile)
        //        , Amount == 0 ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount)
        //        , new SqlParameter("@CreatedTime", CreatedTime)
        //        );
        //}
        /// <summary>
        /// 
        /// </summary> 
        /// <param name="providerCode">provider</param>
        /// <param name="provider"> provider=>CardType</param>  
        public DataTable Report(string UserIds, string Telco, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(UserIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", UserIds);
            pars[1] = string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            DataTable dt = db.GetDataTableSP("sp_TopupMobileLogTransaction_Report", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }

        // Lấy báo cáo theo loại thẻ
        public DataTable ReportTelco(string UserIds, string Telco, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(UserIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", UserIds);
            pars[1] = string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            DataTable dt = db.GetDataTableSP("sp_TopupMobileLogTransaction_ReportTelco", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }

        // Lấy báo cáo theo Order
        public DataTable ReportOrder(string userIds, string telCo, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(userIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", userIds);
            pars[1] = string.IsNullOrEmpty(telCo) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", telCo);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            DataTable dt = db.GetDataTableSP("sp_TopupMobileLogTransaction_ReportOrder", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }

        public List<TopupMobileLog> ReportDoiSoat(string UserIds, string Telco, DateTime beginTime, DateTime endTime)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = string.IsNullOrEmpty(UserIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", UserIds);
            pars[1] = string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco);
            pars[2] = new SqlParameter("@BeginTime", beginTime);
            pars[3] = new SqlParameter("@EndTime", endTime);
            return db.GetListSP<TopupMobileLog>("sp_TopupMobileLogTransaction_ReportDS", pars);
        }
        //public DataTable GetList(int? Top, int? UserId, string Telco, string Mobile, int? Amount, int? Status)
        //{
        //    DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
        //    SqlParameter[] pars = new SqlParameter[6];
        //    pars[0] = Top == 0 ? new SqlParameter("@Top", DBNull.Value) : new SqlParameter("@Top", Top);
        //    pars[1] = UserId == 0 ? new SqlParameter("@UserId", DBNull.Value) : new SqlParameter("@UserId", UserId);
        //    pars[2] = string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco);
        //    pars[3] = string.IsNullOrEmpty(Mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", Mobile);
        //    pars[4] = Amount == 0 ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
        //    pars[5] = Status == 0 ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
        //    return db.GetDataTableSP("sp_TopupMobileLog_SelectList", pars);
        //}

    }
}
