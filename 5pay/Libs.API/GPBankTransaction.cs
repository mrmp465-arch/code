using Libs.Db;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Libs.API
{
    [Serializable]
    public class GPBankTransaction
    {
        public long Id { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerBankCode { get; set; }
        public string PartnerBankId { get; set; }
        public string RefCode { get; set; }
        public string CommandCode { get; set; }
        //public string RequestContent { get; set; }
        public string BankTransId { get; set; }
        public string BankCode { get; set; }
        public string BankId { get; set; }
        public string Comment { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdateTime { get; set; }
     
        public string CallbackUrl { get; set; }
        public int? Amount { get; set; }
        public int? Status { get; set; }
        public string Description { get; set; }

        public string CommentOrg { get; set; }
        public long ReturnValue { get; set; }
        public GPBankTransaction()
        {

        }
        public GPBankTransaction Get()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<GPBankTransaction>("sp_GPTransaction_Select", new SqlParameter("@Id", Id));
        }
        public List<GPBankTransaction> GetList(int top, long? id, string partnerCode, string partnerBankId, string bankTransId, string partnerBankCode, string bankId, string commandCode, DateTime fromDate,DateTime creatTime, int? status, int? amount, string refCode, string comment, bool isComentNull = false)
        {
            //int? amount = null;
            //if (isByPass10k)
            //    amount = 10000;
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[15];
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
                return db.GetListSP<GPBankTransaction>("sp_GPTransaction_CMS_SelectList", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
        public long Add()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            SqlParameter[] pars = new SqlParameter[13];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = string.IsNullOrEmpty(PartnerCode) ? new SqlParameter("@PartnerCode", DBNull.Value) : new SqlParameter("@PartnerCode", PartnerCode);
            pars[3] = string.IsNullOrEmpty(RefCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", RefCode);
            pars[4] = string.IsNullOrEmpty(CommandCode) ? new SqlParameter("@CommandCode", DBNull.Value) : new SqlParameter("@CommandCode", CommandCode);
            pars[5] = Amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
            pars[6] = string.IsNullOrEmpty(PartnerBankId) ? new SqlParameter("@PartnerBankId", " ") : new SqlParameter("@PartnerBankId", PartnerBankId);
            pars[7] = new SqlParameter("@BankTransId", BankTransId);
            pars[8] = string.IsNullOrEmpty(Comment) ? new SqlParameter("@Comment", " ") : new SqlParameter("@Comment", Comment);
            pars[9] = string.IsNullOrEmpty(CommentOrg) ? new SqlParameter("@CommentOrg", " ") : new SqlParameter("@CommentOrg", CommentOrg);
            pars[10] = string.IsNullOrEmpty(CallbackUrl) ? new SqlParameter("@CallbackUrl", DBNull.Value) : new SqlParameter("@CallbackUrl", CallbackUrl);
            pars[11] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            pars[12] = string.IsNullOrEmpty(PartnerBankCode) ? new SqlParameter("@PartnerBankCode", " ") : new SqlParameter("@PartnerBankCode", PartnerBankCode);
            db.ExecuteNonQuerySP("sp_GPTransaction_Insert", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }
        public List<BankReport> ReportDaily(DateTime beginTime, DateTime endTime)
        {
            DBHelper db = new DBHelper(Configs.VPGBankReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@BeginTime", beginTime);
            pars[1] = new SqlParameter("@EndTime", endTime);
            return db.GetListSP<BankReport>("sp_GPTransaction_ReportDaily", pars);

        }
        public long UpdateComment()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = string.IsNullOrEmpty(Comment) ? new SqlParameter("@Comment", DBNull.Value) : new SqlParameter("@Comment", Comment);

            db.ExecuteNonQuerySP("sp_GPTransaction_UpdateComment", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }
        public long UpdateCode(string bankTransId, string code)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@BankTransId", bankTransId);
            pars[2] = string.IsNullOrEmpty(code) ? new SqlParameter("@Comment", DBNull.Value) : new SqlParameter("@Comment", code);

            db.ExecuteNonQuerySP("sp_GPTransactionCMS_UpdateCode", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }
    }
}
