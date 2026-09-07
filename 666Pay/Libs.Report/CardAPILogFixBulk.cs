using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using System.Linq;

namespace Libs.Report
{
    public class CardAPILogFixBulk
    {
        public long Id { get; set; }
        public long TransactionID { get; set; } 
        public string CardSerial { get; set; } 
        public long Amount { get; set; } 
        public int Status { get; set; }
        public int ReturnValue { get; set; }
        public CardAPILogFixBulk()
        {

        }
        public CardAPILogFixBulk Get()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            return db.GetInstanceSP<CardAPILogFixBulk>("sp_CardAPILogFixBulk_Select", new SqlParameter("@Id", Id));
        }
        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            db.ExecuteNonQuerySP("sp_CardAPILogFixBulk_Delete"
                , new SqlParameter("@Id", Id));
        } 
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@Id", Id);
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@CardSerial", CardSerial);
            pars[3] = new SqlParameter("@Amount", Amount);
            pars[4] = new SqlParameter("@Status", Status);
            pars[5] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };

            db.ExecuteNonQuerySP("sp_CardAPILogFixBulk_Update", pars);
            ReturnValue = Convert.ToInt32(pars[4].Value);
        }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@CardSerial", CardSerial);
            pars[3] = new SqlParameter("@Amount", Amount);
            pars[4] = new SqlParameter("@Status", Status); 

            db.ExecuteNonQuerySP("sp_CardAPILogFixBulk_Insert", pars);
            Id = Convert.ToInt32(pars[0].Value);
        }
        public List<CardAPILogFixBulk> GetList(int? status, long? transactionID, string cardSerial)
        {
            int TotalRecord = 0;
            return GetListPage(status, transactionID, cardSerial,null, null,out TotalRecord);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status">null =all</param>
        /// <param name="transactionID">null =all</param>
        /// <param name="cardSerial">null =all</param>
        /// <returns></returns>
        public List<CardAPILogFixBulk> GetListPage(int? status, long? transactionID, string cardSerial, int? PageNumber, int? PageSize,out int TotalRecord)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];  
            pars[0] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
            pars[1] = transactionID== null ? new SqlParameter("@TransactionID", DBNull.Value) : new SqlParameter("@TransactionID", transactionID);
            pars[2] = string.IsNullOrEmpty(cardSerial) ? new SqlParameter("@CardSerial", DBNull.Value) : new SqlParameter("@CardSerial", cardSerial);
            pars[3] = PageNumber == null ? new SqlParameter("@PageNumber", 1) : new SqlParameter("@PageNumber", PageNumber);
            pars[4] = PageSize == null ? new SqlParameter("@PageSize", int.MaxValue) : new SqlParameter("@PageSize", PageSize);  
            pars[5] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var data= db.GetListSP<CardAPILogFixBulk>("sp_CardAPILogFixBulk_SelectList", pars); 
            TotalRecord = Convert.ToInt32(pars[5].Value);
            return data;
        }
        public void Delete(int Id)
        {
            DBHelper db = new DBHelper(Configs.VPGLogConnectionStrings);
            db.ExecuteNonQuerySP("sp_CardAPILogFixBulk_Delete"
                , new SqlParameter("@Id", Id));
        }

    }
}
