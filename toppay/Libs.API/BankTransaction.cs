using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;
using System.Net;

namespace Libs.API
{
    [Serializable]
    public class BankTransaction
    {
        public long Id { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerBankCode { get; set; }
        public string PartnerBankId { get; set; }
        public string RefCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string BankTransId { get; set; }
        public string BankCode { get; set; }
        public string BankId { get; set; }
        public string Comment { get; set; }
        public DateTime? TimeBankSuccess { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string IpAddress { get; set; }
        public string CallbackUrl { get; set; }
        public int? Amount { get; set; }
        public int? Status { get; set; }
        public string Description { get; set; }

        public string BankName { get; set; }

        public string CommentOrg { get; set; }
        public long ReturnValue { get; set; }
        public BankTransaction()
        {

        }
        public BankTransaction Get()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<BankTransaction>("sp_APITransaction_CMS_Select", new SqlParameter("@Id", Id));
        }
        public BankTransaction GetByRefcode(string Refcode)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<BankTransaction>("sp_APITransaction_CMS_SelectByRefCode", new SqlParameter("@RefCode", Refcode));
        }
        public List<BankTransaction> GetListCallback()
        {
            //int? amount = null;
            //if (isByPass10k)
            //    amount = 10000;
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                
                return db.GetListSP<BankTransaction>("sp_APITransaction_CMS_SelectFix");
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
        public long UpdateCode(string bankTransId, string code)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@BankTransId", bankTransId);
            pars[2] = string.IsNullOrEmpty(code) ? new SqlParameter("@Comment", DBNull.Value) : new SqlParameter("@Comment", code);

            db.ExecuteNonQuerySP("sp_APITransactionCMS_UpdateCode", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }
        public long UpdateCodeById(long bankTransId, string code)
        {
            if(bankTransId>0)
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", bankTransId);
                pars[2] = string.IsNullOrEmpty(code) ? new SqlParameter("@Comment", DBNull.Value) : new SqlParameter("@Comment", code);

                db.ExecuteNonQuerySP("sp_APITransactionCMS_UpdateCodeById", pars);
                ReturnValue = Convert.ToInt64(pars[0].Value);
                return ReturnValue;
            }
            return -1;
        }
        public List<BankTransaction> GetList(int top, long? id, string partnerCode, string partnerBankId, string bankTransId, string partnerBankCode, string bankId, string commandCode, DateTime creatTime, int? status, int? amount, int? minamount, int? maxamount, string refCode, string comment, bool isComentNull=false, DateTime? fromDate = null)
        {
            //int? amount = null;
            //if (isByPass10k)
            //    amount = 10000;
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankReportConnectionStrings);
                SqlParameter[] pars = new SqlParameter[17];
                pars[0] = new SqlParameter("@Top", top);
                pars[1] = id == null ? new SqlParameter("@Id", DBNull.Value) : new SqlParameter("@Id", id);
                pars[2] = string.IsNullOrEmpty(partnerCode) ? new SqlParameter("@PartnerCode", DBNull.Value) : new SqlParameter("@PartnerCode", partnerCode);
                pars[3] = new SqlParameter("@CreatedTime", creatTime);
                pars[4] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
                pars[5] = string.IsNullOrEmpty(partnerBankId) ? new SqlParameter("@PartnerBankId", DBNull.Value) : new SqlParameter("@PartnerBankId", partnerBankId);
                pars[6] = string.IsNullOrEmpty(bankTransId) ? new SqlParameter("@BankTransId", DBNull.Value) : new SqlParameter("@BankTransId", bankTransId);
                pars[7] = string.IsNullOrEmpty(partnerBankCode) ? new SqlParameter("@PartnerBankCode", DBNull.Value) : new SqlParameter("@PartnerBankCode", partnerBankCode);
                pars[8] = string.IsNullOrEmpty(bankId) ? new SqlParameter("@BankId", DBNull.Value) : new SqlParameter("@BankId", bankId);
                pars[9] = string.IsNullOrEmpty(commandCode) ? new SqlParameter("@CommandCode", DBNull.Value) : new SqlParameter("@CommandCode", commandCode);
                pars[10] = amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", amount.Value);
                pars[11] = string.IsNullOrEmpty(refCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", refCode);
                pars[12] = string.IsNullOrEmpty(comment) ? new SqlParameter("@Comment", DBNull.Value) : new SqlParameter("@Comment", comment);
                pars[13] = !isComentNull ? new SqlParameter("@IsCommentNull", false) : new SqlParameter("@IsCommentNull", isComentNull);
                pars[14] = new SqlParameter("@FromDate", fromDate);
                pars[15] = minamount == null ? new SqlParameter("@MinAmount", DBNull.Value) : new SqlParameter("@MinAmount", minamount.Value);
                pars[16] = maxamount == null ? new SqlParameter("@MaxAmount", DBNull.Value) : new SqlParameter("@MaxAmount", maxamount.Value);
                return db.GetListSP<BankTransaction>("sp_APITransaction_CMS_SelectList", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
        public List<BankReport> Report(int year, int month, int day,string partnerBankId)
        {
            DBHelper db = new DBHelper(Configs.VPGBankReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[1] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[2] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[3] = string.IsNullOrEmpty(partnerBankId) ? new SqlParameter("@PartnerBankId", DBNull.Value) : new SqlParameter("@PartnerBankId", partnerBankId);
            return db.GetListSP<BankReport>("sp_APITransaction_Report", pars);

        }
        public List<BankReport> ReportDaily(DateTime beginTime, DateTime endTime)
        {
            DBHelper db = new DBHelper(Configs.VPGBankReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@BeginTime", beginTime);
            pars[1] = new SqlParameter("@EndTime", endTime);
            return db.GetListSP<BankReport>("sp_APITransaction_ReportDaily", pars);

        }
        public long UpdateComment()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = string.IsNullOrEmpty(Comment) ? new SqlParameter("@Comment", DBNull.Value) : new SqlParameter("@Comment", Comment);
            
            db.ExecuteNonQuerySP("sp_APITransaction_UpdateComment", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }
        public long Update()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            SqlParameter[] pars = new SqlParameter[12];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = string.IsNullOrEmpty(PartnerCode) ? new SqlParameter("@PartnerCode", DBNull.Value) : new SqlParameter("@PartnerCode", PartnerCode);
            pars[3] = string.IsNullOrEmpty(RefCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", RefCode);
            pars[4] = string.IsNullOrEmpty(CommandCode) ? new SqlParameter("@CommandCode", DBNull.Value) : new SqlParameter("@CommandCode", CommandCode);
            pars[5] = Amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
            pars[6] = string.IsNullOrEmpty(RequestContent) ? new SqlParameter("@RequestContent", DBNull.Value) : new SqlParameter("@RequestContent", RequestContent);
            pars[7] = string.IsNullOrEmpty(IpAddress) ? new SqlParameter("@IpAddress", DBNull.Value) : new SqlParameter("@IpAddress", IpAddress);
            pars[8] = CreatedTime == null ? new SqlParameter("@CreatedTime", DBNull.Value) : new SqlParameter("@CreatedTime", CreatedTime);
            pars[9] = UpdateTime == null ? new SqlParameter("@UpdateTime", DBNull.Value) : new SqlParameter("@UpdateTime", UpdateTime);
            pars[10] = string.IsNullOrEmpty(CallbackUrl) ? new SqlParameter("@CallbackUrl", DBNull.Value) : new SqlParameter("@CallbackUrl", CallbackUrl);
            pars[11] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);

            db.ExecuteNonQuerySP("sp_APITransaction_Update", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }
    }
}
