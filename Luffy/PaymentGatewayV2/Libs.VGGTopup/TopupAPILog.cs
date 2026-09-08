using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.API;

namespace Libs.VGGTopup
{
    public class TopupAPILog
    {
        public long TransactionID { get; set; }
        public int PartnerID { get; set; }
        public string RequestID { get; set; }
        public string AccountName { get; set; }
        public long AccountID { get; set; }
        public long Amount { get; set; }
        public DateTime CreatTime { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public DateTime UpdateTime { get; set; }
        public int ReturnValue { get; set; }

        public TopupAPILog()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public TopupAPILog Get()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupAPILog>("sp_TopupAPILog_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public TopupAPILog Get(int transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupAPILog>("sp_TopupAPILog_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public TopupAPILog Check(int partnerID, string requestID)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<TopupAPILog>("sp_TopupAPILog_Check"
                , new SqlParameter("@PartnerID", partnerID)
                , new SqlParameter("@RequestID", requestID)
                );
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[8];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@PartnerID", PartnerID);
            pars[3] = new SqlParameter("@RequestID", RequestID);
            pars[4] = new SqlParameter("@AccountName", AccountName);
            pars[5] = new SqlParameter("@AccountID", AccountID);
            pars[6] = new SqlParameter("@Amount", Amount);
            pars[7] = new SqlParameter("@Description", Description);

            db.ExecuteNonQuerySP("sp_TopupAPILog_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@Status", Status);
            pars[3] = new SqlParameter("@Description", Description);
            db.ExecuteNonQuerySP("sp_TopupAPILog_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
    }
}
