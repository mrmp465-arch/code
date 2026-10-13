using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.Report
{
    public class TopupMobileLog
    {
        public long TransactionID { get; set; }
        public string OrderNo { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Telco { get; set; }
        public string RequestNo { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public int Amount { get; set; }
        public int AmountUser { get; set; }
        public int? TopupType { get; set; }
        public string LogContent { get; set; }
        public int? AmountTopupSuccess { get; set; }
        public int? AmountPending { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastTime { get; set; }
        public string Partners { get; set; }
        public string Providers { get; set; }
        public int? AmountMin { get; set; }
        public int? Priority { get; set; }
        public long TotalAmountSuccess { get; set; }
        public long TotalAmount { get; set; }
        public int TotalTranSuccess { get; set; }
        public int TotalTrans { get; set; }
        public int Lock { get; set; }
        public int Queue { get; set; }
        public int Success { get; set; }
        public int Disable { get; set; }
        public int Ignore { get; set; }
        public int? IsConfirm { get; set; }
        public int? AmountMinAll { get; set; }
        public int Stt { get; set; }
        public string State { get; set; }
        public string LastStatus { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public int Dup { get; set; }
        public int AmountABS { get; set; }
        public int CountCharge { get; set; }
        public int ReturnValue { get; set; }
        public int? Ussd { get; set; }
        public string CallbackUrl { get; set; }
        public decimal? BidRate { get; set; }
        public int? BidFee { get; set; }
        public string ExtData { get; set; }
        public string SubUser { get; set; }
        public int? StatusCash { get; set; }


        public TopupMobileLog()
        {
        }
        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            db.ExecuteNonQuerySP("sp_TopupMobileLog_Delete"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public TopupMobileLog Get()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupMobileLog>("sp_TopupMobileLog_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public TopupMobileLog Get(long transactionId)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupMobileLog>("sp_TopupMobileLog_Select"
                , new SqlParameter("@TransactionID", transactionId));
        }

        public TopupMobileLog GetProcess(string telco, int amount, string partnerCode, string providerCode, string listSim = "", int ussdOnly = -1)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupMobileLog>("sp_TopupMobileLog_Select_Process"
                , new SqlParameter("@Telco", telco)
                , new SqlParameter("@Amount", amount)
                , string.IsNullOrEmpty(partnerCode) ? new SqlParameter("@PartnerCode", DBNull.Value) : new SqlParameter("@PartnerCode", partnerCode)
                , string.IsNullOrEmpty(providerCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", providerCode)
                , string.IsNullOrEmpty(listSim) ? new SqlParameter("@ListSim", DBNull.Value) : new SqlParameter("@ListSim", listSim)
                , new SqlParameter("@OnlyUSSD", ussdOnly)
                );
        }
        public TopupMobileLog GetProcess(string telco, int amount, string providerCode, int quanity)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupMobileLog>("sp_TopupMobileLog_Cash_Select_Process"
                , new SqlParameter("@Telco", telco)
                , new SqlParameter("@Amount", amount)
                , new SqlParameter("@ProviderCode", providerCode)
                , new SqlParameter("@Quanity", quanity)
            );
        }

        public TopupMobileLog GetProcess(string telco, string userName, int type, ref string mobileDes, ref Int64 idDes)
        {

            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@Telco", telco);
            pars[1] = new SqlParameter("@UserName", userName);
            pars[2] = new SqlParameter("@Type", type);
            pars[3] = new SqlParameter("@MobileDes", SqlDbType.VarChar, 50) { Direction = ParameterDirection.Output };
            pars[4] = new SqlParameter("@TransactionIdDes", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            var dt = db.GetInstanceSP<TopupMobileLog>("sp_TopupMobileLog_TranferBalance_Select_Process", pars);
            if (dt != null)
            {
                mobileDes = Convert.ToString(pars[3].Value);
                idDes = Convert.ToInt64(pars[4].Value);
            }
            return dt;

        }

        public TopupMobileLog GetProcess_Test(string telco, int amount, string partnerCode, string providerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupMobileLog>("sp_TopupMobileLog_Select_Process_Test_All"
                , new SqlParameter("@Telco", telco)
                , new SqlParameter("@Amount", amount)
                , string.IsNullOrEmpty(partnerCode) ? new SqlParameter("@PartnerCode", DBNull.Value) : new SqlParameter("@PartnerCode", partnerCode)
                , string.IsNullOrEmpty(providerCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", providerCode)
            );
        }

        public DataTable CountWaiting(string userIds)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetDataTableSP("sp_TopupMobileLog_Order_CountWaiting"
                , new SqlParameter("@UserIds", userIds)
            );
        }
        //public TopupMobileLog GetProcess(string telco, int amount, string partnerCode, string providerCode, string listSim)
        //{
        //    DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
        //    return db.GetInstanceSP<TopupMobileLog>("sp_TopupMobileLog_Select_Process"
        //        , new SqlParameter("@Telco", telco)
        //        , new SqlParameter("@Amount", amount)
        //        , string.IsNullOrEmpty(partnerCode) ? new SqlParameter("@PartnerCode", DBNull.Value) : new SqlParameter("@PartnerCode", partnerCode)
        //        , string.IsNullOrEmpty(providerCode) ? new SqlParameter("@ProviderCode", DBNull.Value) : new SqlParameter("@ProviderCode", providerCode)
        //        , string.IsNullOrEmpty(listSim) ? new SqlParameter("@ListSim", DBNull.Value) : new SqlParameter("@ListSim", listSim)
        //    );
        //}
        // Lấy danh sách
        /// <summary>
        /// 
        /// </summary> 
        /// <param name="status">null==all</param> 
        /// <returns></returns>
        public DataTable GetTable(int top, string userIds, string telco, string requestNo, string mobile, int amount, DateTime createdTime, int? status,
            int? priority, string orderNo, int? topuptype, int? isConfirm, string fullName, int? ussd, string subUser)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetDataTableSP("sp_TopupMobileLog_SelectList"
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(userIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", userIds)
                , string.IsNullOrEmpty(telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", telco)
                , string.IsNullOrEmpty(requestNo) ? new SqlParameter("@RequestNo", DBNull.Value) : new SqlParameter("@RequestNo", requestNo)
                , string.IsNullOrEmpty(mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", mobile)
                , amount == 0 ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", amount)
                , new SqlParameter("@CreatedTime", createdTime)
                , status == null ? new SqlParameter("@status", DBNull.Value) : new SqlParameter("@status", status)
                , priority == null ? new SqlParameter("@Priority", DBNull.Value) : new SqlParameter("@Priority", priority)
                , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
                , topuptype == null ? new SqlParameter("@Topuptype", DBNull.Value) : new SqlParameter("@Topuptype", topuptype)
                , isConfirm == null ? new SqlParameter("@IsConfirm", DBNull.Value) : new SqlParameter("@IsConfirm", isConfirm)
                , string.IsNullOrEmpty(fullName) ? new SqlParameter("@FullName", DBNull.Value) : new SqlParameter("@FullName", fullName)
                , ussd == null ? new SqlParameter("@Ussd", DBNull.Value) : new SqlParameter("@Ussd", ussd)
                , string.IsNullOrEmpty(subUser) ? new SqlParameter("@SubUser", DBNull.Value) : new SqlParameter("@SubUser", subUser)
                );
        }

        public DataTable GetTableOrder(int top, string userIds, string telco, string orderNo, DateTime createdTime, int? status, string subUser, int? ussd, ref long totalSuccess, ref long totalRequest, ref long totalWaiting)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = new SqlParameter("@Top", top);
            pars[1] = string.IsNullOrEmpty(userIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", userIds);
            pars[2] = string.IsNullOrEmpty(telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", telco);
            pars[3] = new SqlParameter("@CreatedTime", createdTime);
            pars[4] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
            pars[5] = string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo);
            pars[6] = string.IsNullOrEmpty(subUser) ? new SqlParameter("@SubUser", DBNull.Value) : new SqlParameter("@SubUser", subUser);
            pars[7] = new SqlParameter("@TotalSuccess", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[8] = new SqlParameter("@TotalRequest", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[9] = new SqlParameter("@TotalWaiting", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            pars[10] = ussd == null ? new SqlParameter("@Ussd", DBNull.Value) : new SqlParameter("@Ussd", ussd);
            DataTable dt = db.GetDataTableSP("sp_TopupMobileLog_Order_SelectList", pars);
            totalSuccess = Convert.ToInt64(pars[7].Value);
            totalRequest = Convert.ToInt64(pars[8].Value);
            totalWaiting = Convert.ToInt64(pars[9].Value);
            return dt;


            //DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            //return db.GetDataTableSP("sp_TopupMobileLog_Order_SelectList"
            //    , new SqlParameter("@Top", top)
            //    , string.IsNullOrEmpty(userIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", userIds)
            //    , string.IsNullOrEmpty(telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", telco)
            //    , new SqlParameter("@CreatedTime", createdTime)
            //    , status == null ? new SqlParameter("@status", DBNull.Value) : new SqlParameter("@status", status)
            //    , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
            //    , new SqlParameter("@TotalSuccess", SqlDbType.Int) { Direction = ParameterDirection.Output },
            //    new SqlParameter("@TotalRequest", SqlDbType.BigInt) { Direction = ParameterDirection.Output }
            //);
        }

        public DataTable GetTableTransactionExport(string userIds, string orderNo, int? transactionId, DateTime? beginTime, DateTime? endTime)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_TopupMobileLogTransaction_Export"
                , string.IsNullOrEmpty(userIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", userIds)
                , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
                , transactionId == null ? new SqlParameter("@TransactionId", DBNull.Value) : new SqlParameter("@TransactionId", transactionId)
                , beginTime == null ? new SqlParameter("@BeginTime", DBNull.Value) : new SqlParameter("@BeginTime", beginTime)
                , endTime == null ? new SqlParameter("@EndTime", DBNull.Value) : new SqlParameter("@EndTime", endTime)
            );

        }
        public DataTable GetTableOrderExport(string userIds, string orderNo)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_TopupMobileLog_SelectList_Export"
                , string.IsNullOrEmpty(userIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", userIds)
                , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
            );

        }
        public DataTable GetTableOrderConfirm(int top, string userIds, string telco, string orderNo, DateTime createdTime, int? status, string subUser)
        {
            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            return db.GetDataTableSP("sp_TopupMobileLog_Order_Confirm_SelectList"
                , new SqlParameter("@Top", top)
                , string.IsNullOrEmpty(userIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", userIds)
                , string.IsNullOrEmpty(telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", telco)
                , new SqlParameter("@CreatedTime", createdTime)
                , status == null ? new SqlParameter("@status", DBNull.Value) : new SqlParameter("@status", status)
                , string.IsNullOrEmpty(orderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", orderNo)
                , string.IsNullOrEmpty(subUser) ? new SqlParameter("@SubUser", DBNull.Value) : new SqlParameter("@SubUser", subUser)
            );
        }

        public DataTable Search(int top, string UserIds, string Telco, string RequestNo, string Mobile, int Amount, DateTime CreatedTime)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetDataTableSP("sp_TopupMobileLog_Search"
                , new SqlParameter("@Top", top)
                  , string.IsNullOrEmpty(UserIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", UserIds)
                 , string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco)
                , string.IsNullOrEmpty(RequestNo) ? new SqlParameter("@RequestNo", DBNull.Value) : new SqlParameter("@RequestNo", RequestNo)
                , string.IsNullOrEmpty(Mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", Mobile)
                , Amount == 0 ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount)
                , new SqlParameter("@CreatedTime", CreatedTime)
                );
        }
        /// <summary>
        /// 
        /// </summary> 
        /// <param name="providerCode">provider</param>
        /// <param name="provider"> provider=>CardType</param>  
        public DataTable Report(string UserIds, string Telco, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(UserIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", UserIds);
            pars[1] = string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            DataTable dt = db.GetDataTableSP("sp_TopupMobileLog_Report", pars);
            totalTransaction = Convert.ToInt32(pars[5].Value);
            totalAmount = Convert.ToInt64(pars[6].Value);
            return dt;
        }

        // Lấy báo cáo theo loại thẻ
        public DataTable ReportTelco(string UserIds, string Telco, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = string.IsNullOrEmpty(UserIds) ? new SqlParameter("@UserIds", DBNull.Value) : new SqlParameter("@UserIds", UserIds);
            pars[1] = string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco);
            pars[2] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[3] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[4] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[5] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[6] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            DataTable dt = db.GetDataTableSP("sp_TopupMobileLog_ReportTelco", pars);
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
            return db.GetListSP<TopupMobileLog>("sp_TopupMobileLog_ReportDS", pars);
        }

        public List<TopupMobileLog> GetListDuplicate(string orderNo, int userId)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@OrderNo", orderNo);
            pars[1] = new SqlParameter("@UserId", userId);
            return db.GetListSP<TopupMobileLog>("sp_TopupMobileLog_Check_Duplicate", pars);
        }

        public DataTable GetList(int? Top, int? UserId, string Telco, string Mobile, int? Amount, int? Status)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = Top == 0 ? new SqlParameter("@Top", DBNull.Value) : new SqlParameter("@Top", Top);
            pars[1] = UserId == 0 ? new SqlParameter("@UserId", DBNull.Value) : new SqlParameter("@UserId", UserId);
            pars[2] = string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco);
            pars[3] = string.IsNullOrEmpty(Mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", Mobile);
            pars[4] = Amount == 0 ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
            pars[5] = Status == 0 ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            return db.GetDataTableSP("sp_TopupMobileLog_SelectList", pars);
        }

        public int Add()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[24];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserId", UserId);
            pars[2] = new SqlParameter("@UserName", UserName);
            pars[3] = new SqlParameter("@Telco", Telco);
            pars[4] = new SqlParameter("@RequestNo", RequestNo);
            pars[5] = new SqlParameter("@Mobile", Mobile);
            pars[6] = new SqlParameter("@Amount", Amount);
            pars[7] = new SqlParameter("@TopupType", TopupType);
            pars[8] = new SqlParameter("@LogContent", LogContent);
            pars[9] = new SqlParameter("@AmountTopupSuccess", AmountTopupSuccess);
            pars[10] = new SqlParameter("@Status", Status);
            pars[11] = new SqlParameter("@FullName", FullName);
            pars[12] = new SqlParameter("@Partners", Partners);
            pars[13] = new SqlParameter("@AmountMin", AmountMin);
            pars[14] = new SqlParameter("@Priority", Priority);
            pars[15] = new SqlParameter("@OrderNo", OrderNo);
            pars[16] = new SqlParameter("@Providers", Providers);
            pars[17] = new SqlParameter("@AmountMinAll", AmountMinAll);
            pars[18] = new SqlParameter("@AccountName", AccountName);
            pars[19] = new SqlParameter("@Password", Password);
            pars[20] = new SqlParameter("@Ussd", Ussd);
            pars[21] = new SqlParameter("@CallbackUrl", CallbackUrl);
            pars[22] = new SqlParameter("@SubUser", SubUser);
            pars[23] = string.IsNullOrEmpty(ExtData) ? new SqlParameter("@ExtData", DBNull.Value) : new SqlParameter("@ExtData", ExtData);
            db.ExecuteNonQuerySP("sp_TopupMobileLog_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
            return ReturnValue;
        }

        public int Update()
        {

            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[27];
            pars[0] = new SqlParameter("@ReturnValue", ReturnValue) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = UserId == 0 ? new SqlParameter("@UserId", DBNull.Value) : new SqlParameter("@UserId", UserId);
            pars[3] = string.IsNullOrEmpty(Telco) ? new SqlParameter("@Telco", DBNull.Value) : new SqlParameter("@Telco", Telco);
            pars[4] = string.IsNullOrEmpty(RequestNo) ? new SqlParameter("@RequestNo", DBNull.Value) : new SqlParameter("@RequestNo", RequestNo);
            pars[5] = string.IsNullOrEmpty(Mobile) ? new SqlParameter("@Mobile", DBNull.Value) : new SqlParameter("@Mobile", Mobile);
            pars[6] = Amount == 0 ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
            pars[7] = TopupType == null ? new SqlParameter("@TopupType", DBNull.Value) : new SqlParameter("@TopupType", TopupType);
            pars[8] = LogContent == null ? new SqlParameter("@LogContent", DBNull.Value) : new SqlParameter("@LogContent", LogContent);
            pars[9] = AmountTopupSuccess == null ? new SqlParameter("@AmountTopupSuccess", DBNull.Value) : new SqlParameter("@AmountTopupSuccess", AmountTopupSuccess);
            pars[10] = AmountPending == null ? new SqlParameter("@AmountPending", DBNull.Value) : new SqlParameter("@AmountPending", AmountPending);
            pars[11] = CreatedTime == null ? new SqlParameter("@CreatedTime", DBNull.Value) : new SqlParameter("@CreatedTime", CreatedTime);
            pars[12] = LastTime == null ? new SqlParameter("@LastTime", DBNull.Value) : new SqlParameter("@LastTime", LastTime);
            pars[13] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            pars[14] = string.IsNullOrEmpty(UserName) ? new SqlParameter("@UserName", DBNull.Value) : new SqlParameter("@UserName", UserName);
            pars[15] = FullName == null ? new SqlParameter("@FullName", DBNull.Value) : new SqlParameter("@FullName", FullName);
            pars[16] = AmountMin == null ? new SqlParameter("@AmountMin", DBNull.Value) : new SqlParameter("@AmountMin", AmountMin);
            pars[17] = Priority == null ? new SqlParameter("@Priority", DBNull.Value) : new SqlParameter("@Priority", Priority);
            pars[18] = string.IsNullOrEmpty(OrderNo) ? new SqlParameter("@OrderNo", DBNull.Value) : new SqlParameter("@OrderNo", OrderNo);
            pars[19] = IsConfirm == null ? new SqlParameter("@IsConfirm", DBNull.Value) : new SqlParameter("@IsConfirm", IsConfirm);
            pars[20] = AmountMinAll == null ? new SqlParameter("@AmountMinAll", DBNull.Value) : new SqlParameter("@AmountMinAll", AmountMinAll);
            pars[21] = string.IsNullOrEmpty(AccountName) ? new SqlParameter("@AccountName", DBNull.Value) : new SqlParameter("@AccountName", AccountName);
            pars[22] = Password == null ? new SqlParameter("@Password", DBNull.Value) : new SqlParameter("@Password", Password);
            pars[23] = Ussd == null ? new SqlParameter("@Ussd", DBNull.Value) : new SqlParameter("@Ussd", Ussd);
            pars[24] = BidRate == null ? new SqlParameter("@BidRate", DBNull.Value) : new SqlParameter("@BidRate", BidRate);
            pars[25] = BidFee == null ? new SqlParameter("@BidFee", DBNull.Value) : new SqlParameter("@BidFee", BidFee);
            pars[26] = string.IsNullOrEmpty(ExtData) ? new SqlParameter("@ExtData", DBNull.Value) : new SqlParameter("@ExtData", ExtData);
            db.ExecuteNonQuerySP("sp_TopupMobileLog_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
            return ReturnValue;
        }

        public int UpdateOrder(string orderNo, int? priority, int? status, int userId, int? isConfirm, int currentIsConfirm)//, string userName)
        {

            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", ReturnValue) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@OrderNo", orderNo);
            pars[2] = priority == null ? new SqlParameter("@Priority", DBNull.Value) : new SqlParameter("@Priority", priority);
            pars[3] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
            pars[4] = new SqlParameter("@UserId", userId);
            pars[5] = isConfirm == null ? new SqlParameter("@IsConfirm", DBNull.Value) : new SqlParameter("@IsConfirm", isConfirm);
            pars[6] = new SqlParameter("@CurrentIsConfirm", currentIsConfirm);
            //pars[3] = new SqlParameter("@UserName", userName);
            db.ExecuteNonQuerySP("sp_TopupMobileLog_Update_Order", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
            return ReturnValue;
        }

        public int UpdateOrderByUser(int userId, string userName, int status)// để khỏa bên bán lẻ
        {

            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", ReturnValue) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserName", userName);
            pars[2] = new SqlParameter("@UserId", userId);
            pars[3] = new SqlParameter("@Status", status);
            db.ExecuteNonQuerySP("sp_TopupMobileLog_Update_Order_By_User", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
            return ReturnValue;
        }

        public void UpdateProviders(string providers, int userId)//, string userName)
        {

            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", ReturnValue) { Direction = ParameterDirection.Output };
            pars[1] = string.IsNullOrEmpty(providers) ? new SqlParameter("@Providers", DBNull.Value) : new SqlParameter("@Providers", providers);
            pars[2] = new SqlParameter("@UserId", userId);
            db.ExecuteNonQuerySP("sp_TopupMobileLog_Update_Providers", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }


        public void Topup(int status, int amount, decimal? bidRate = 0)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Amount", amount);
            pars[3] = new SqlParameter("@Status", status);
            pars[4] = new SqlParameter("@BidRate", bidRate);
            db.ExecuteNonQuerySP("sp_TopupMobileLog_UpdateTopup", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Cash(int status, int amount, decimal? bidRate = 0)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Amount", amount);
            pars[3] = new SqlParameter("@StatusCash", status);
            pars[4] = new SqlParameter("@BidRate", bidRate);
            db.ExecuteNonQuerySP("sp_TopupMobileLog_UpdateCash", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void TranferAccountConfirm(Int64 idSource, Int64 idDes, int amount, int status, int statusIn, int statusOut)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionIDSource", idSource);
            pars[2] = new SqlParameter("@TransactionIDDes", idDes);
            pars[3] = new SqlParameter("@Amount", amount);
            pars[4] = new SqlParameter("@Status", status);
            pars[5] = new SqlParameter("@StatusIn", statusIn);
            pars[6] = new SqlParameter("@StatusOut", statusOut);
            db.ExecuteNonQuerySP("sp_TopupMobileLog_UpdateTranferAccount", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public string GenOrderCode()
        {
            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,1,2,3,4,5,6,7,8,9").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i <= 7; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }
            var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return tmp.ToUpper() + timeSpan.ToString();
        }
    }
}
