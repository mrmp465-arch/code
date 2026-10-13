using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.Report
{
    public class TopupMobile3rdLog
    {
        public long Id { get; set; }
        public long? RequestNo { get; set; }
        public long TransactionId { get; set; }
        public string PartnerCode { get; set; }
        public string ProviderCode { get; set; }
        public string Telco { get; set; }
        public string Sim { get; set; }
        public string SimTarget { get; set; }
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public string LogContent { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastTime { get; set; }
        public int? Amount { get; set; }
        public int AmountUser { get; set; }
        public int? Status { get; set; }
        public string ClientId { get; set; }
        public int Slot { get; set; }
        public string Core { get; set; }
        public decimal? BidRate { get; set; }
        public int? CallbackProviderStatus { get; set; }
        public int? CallbackPartnerStatus { get; set; }
        public int ReturnValue { get; set; }
        public TopupMobile3rdLog()
        {
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[15];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@RequestNo", RequestNo);
            pars[2] = new SqlParameter("@TransactionId", TransactionId);
            pars[3] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[4] = new SqlParameter("@ProviderCode", ProviderCode);
            pars[5] = new SqlParameter("@Telco", Telco);
            pars[6] = new SqlParameter("@Sim", Sim ?? "");
            pars[7] = new SqlParameter("@SimTarget", SimTarget ?? "");
            pars[8] = new SqlParameter("@Amount", Amount);
            pars[9] = new SqlParameter("@CardSerial", CardSerial);
            pars[10] = new SqlParameter("@CardCode", CardCode);
            pars[11] = new SqlParameter("@ClientId", ClientId ?? "");
            pars[12] = new SqlParameter("@Slot", Slot);
            pars[13] = new SqlParameter("@Core", Core);
            pars[14] = new SqlParameter("@BidRate", BidRate);
            db.ExecuteNonQuerySP("sp_TopupMobile3rdLog_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = Amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
            pars[3] = Status== null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            pars[4] = string.IsNullOrEmpty(LogContent) ? new SqlParameter("@LogContent", DBNull.Value) : new SqlParameter("@LogContent", LogContent);
            pars[5] = string.IsNullOrEmpty(Sim) ? new SqlParameter("@Sim", DBNull.Value) : new SqlParameter("@Sim", Sim);
            pars[6] = RequestNo == null ? new SqlParameter("@RequestNo", DBNull.Value) : new SqlParameter("@RequestNo", RequestNo);
            pars[7] = string.IsNullOrEmpty(SimTarget) ? new SqlParameter("@SimTarget", DBNull.Value) : new SqlParameter("@SimTarget", SimTarget);
            db.ExecuteNonQuerySP("sp_TopupMobile3rdLog_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void UpdateCallback(long id, int? callbackProviderStatus, int? callbackPartnerStatus)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", id);
            pars[2] = callbackProviderStatus == null ? new SqlParameter("@CallbackProviderStatus", DBNull.Value) : new SqlParameter("@CallbackProviderStatus", callbackProviderStatus);
            pars[3] = callbackPartnerStatus == null ? new SqlParameter("@CallbackPartnerStatus", DBNull.Value) : new SqlParameter("@CallbackPartnerStatus", callbackPartnerStatus);
            db.ExecuteNonQuerySP("sp_TopupMobile3rdLog_Update_Callback", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void UpdateByTransaction()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionId", TransactionId);
            pars[2] = Amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
            pars[3] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            pars[4] = string.IsNullOrEmpty(LogContent) ? new SqlParameter("@LogContent", DBNull.Value) : new SqlParameter("@LogContent", LogContent);
            db.ExecuteNonQuerySP("sp_TopupMobile3rdLog_Update_Transaction", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void UpdateByReviewByRequestNo()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@RequestNo", RequestNo);
            db.ExecuteNonQuerySP("sp_TopupMobile3rdLog_Update_Review", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
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
            return db.GetListSP<CardAPILog>("sp_TopupMobile3rdLog_SelectList", pars);
        }

        public TopupMobile3rdLog Get()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupMobile3rdLog>("sp_TopupMobile3rdLog_Select", new SqlParameter("@Id", Id));
        }

        public TopupMobile3rdLog GetByTransactionId(long transactionId)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupMobile3rdLog>("sp_TopupMobile3rdLog_Select_TransactionId", new SqlParameter("@TransactionId", transactionId));
        }

        public TopupMobile3rdLog GetByTransactionIdSuccess(long transactionId)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupMobile3rdLog>("sp_TopupMobile3rdLog_Select_TransactionId_Success", new SqlParameter("@TransactionId", transactionId));
        }

        public List<TopupMobile3rdLog> GetListByTransactionId(long transactionId)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetListSP<TopupMobile3rdLog>("sp_TopupMobile3rdLog_Select_TransactionId", new SqlParameter("@TransactionId", transactionId));
        }

        public List<TopupMobile3rdLog> GetListMissCallback()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetListSP<TopupMobile3rdLog>("sp_TopupMobile3rdLog_Select_MissCallback");
        }

        public void DeleteTransactionId()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            db.ExecuteNonQuerySP("sp_TopupMobile3rdLog_Delete", new SqlParameter("@TransactionID", TransactionId));
        }
    }
}
