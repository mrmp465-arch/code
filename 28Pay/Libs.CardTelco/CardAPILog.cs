using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.API;
using Libs.Utils;

namespace Libs.CardTelco
{
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
        public int ReturnValue { get; set; }

        public CardAPILog()
        {

        }

        public CardAPILog Get()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<CardAPILog>("sp_CardAPILog_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public CardAPILog Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<CardAPILog>("sp_CardAPILog_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public CardAPILog Check(int partnerID, string cardSerial, string cardType)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<CardAPILog>("sp_CardAPILog_Check"
                , new SqlParameter("@PartnerID", partnerID)
                , new SqlParameter("@CardSerial", cardSerial)
                , new SqlParameter("@CardType", cardType)
                );
        }

        public List<CardAPILog> CheckTrans(int partnerID, string refCode)
        {
            //NLogLogger.Info(new string[] { "CardTelco", "CheckTrans", partnerID.ToString(), refCode });
            //DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            //return db.GetList<CardAPILog>("sp_CardAPILog_CheckTrans"
            //    , new SqlParameter("@PartnerID", partnerID)
            //    , new SqlParameter("@RefCode", refCode)
            //);

            DBHelper db = new DBHelper(Configs.VPGLogReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@PartnerID", partnerID);
            pars[1] = new SqlParameter("@RequestNo", refCode);
            return db.GetListSP<CardAPILog>("sp_CardAPILog_CheckTrans", pars);
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[16];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@PartnerID", PartnerID);
            pars[3] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[4] = new SqlParameter("@AccountName", AccountName);
            pars[5] = new SqlParameter("@AccountID", AccountID);
            pars[6] = new SqlParameter("@CardSerial", CardSerial);
            pars[7] = new SqlParameter("@CardCode", CardCode);
            pars[8] = new SqlParameter("@Amount", Amount);
            pars[9] = new SqlParameter("@CardType", CardType);
            pars[10] = new SqlParameter("@Provider", Provider);
            pars[11] = new SqlParameter("@Status", Status);
            pars[12] = new SqlParameter("@Description", Description);
            pars[13] = new SqlParameter("@RequestNo", string.IsNullOrEmpty(RequestNo) ? "" : RequestNo);
            pars[14] = new SqlParameter("@CallbackUrl", string.IsNullOrEmpty(CallbackUrl) ? "" : CallbackUrl);
            pars[15] = new SqlParameter("@AmountUser", AmountUser);
            db.ExecuteNonQuerySP("sp_CardAPILog_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@Amount", Amount);
            pars[4] = new SqlParameter("@Description", Description);
            pars[5] = new SqlParameter("@AccountID", AccountID);
            db.ExecuteNonQuerySP("sp_CardAPILog_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
    }
}
