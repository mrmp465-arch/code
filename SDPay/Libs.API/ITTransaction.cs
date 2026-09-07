using Libs.Db;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.API
{
    public class ITTransaction
    {
        public long Id { get; set; }
        public string PartnerBankName { get; set; }
        public string PartnerBankCode { get; set; }
        public string PartnerBankId { get; set; }
        public string BankCode { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; }
        public string CommandCode { get; set; }
        //public string RequestContent { get; set; }
        public string BankTransId { get; set; }

        public string UserName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        public int? Amount { get; set; }
        public int? Status { get; set; }
        public string Description { get; set; }

        public string AppName { get; set; }
        public long ReturnValue { get; set; }
        public ITTransaction()
        {

        }
        public ITTransaction Get()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<ITTransaction>("sp_ITTransaction_CMS_Select", new SqlParameter("@Id", Id));

        }
        public long Update()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = string.IsNullOrEmpty(AppName) ? new SqlParameter("@AppName", DBNull.Value) : new SqlParameter("@AppName", AppName);
            pars[3] = new SqlParameter("@Status", Status);
            db.ExecuteNonQuerySP("sp_ITTransaction_Update", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }
        public List<ITTransaction> GetList(int top, long? id, string bankCode, string partnerBankId, string partnerBankCode, string bankId,  DateTime creatTime, DateTime endTime, int? status, int? amount, string refCode, string comment,string comandcode)
        {
            //int? amount = null;
            //if (isByPass10k)
            //    amount = 10000;
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@Top", top);
                pars[1] = id == null ? new SqlParameter("@Id", DBNull.Value) : new SqlParameter("@Id", id);
                pars[2] = string.IsNullOrEmpty(bankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", bankCode);
                pars[3] = new SqlParameter("@CreatedTime", creatTime);
                pars[4] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
                pars[5] = string.IsNullOrEmpty(partnerBankId) ? new SqlParameter("@PartnerBankId", DBNull.Value) : new SqlParameter("@PartnerBankId", partnerBankId);
                //pars[6] = string.IsNullOrEmpty(bankTransId) ? new SqlParameter("@BankTransId", DBNull.Value) : new SqlParameter("@BankTransId", bankTransId);
                pars[7] = string.IsNullOrEmpty(partnerBankCode) ? new SqlParameter("@PartnerBankCode", DBNull.Value) : new SqlParameter("@PartnerBankCode", partnerBankCode);
                pars[8] = string.IsNullOrEmpty(bankId) ? new SqlParameter("@BankId", DBNull.Value) : new SqlParameter("@BankId", bankId);
               
                pars[10] = amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", amount.Value);
                pars[11] = string.IsNullOrEmpty(refCode) ? new SqlParameter("@UserName", DBNull.Value) : new SqlParameter("@UserName", refCode);
                pars[12] = string.IsNullOrEmpty(comandcode) ? new SqlParameter("@CommandCode", DBNull.Value) : new SqlParameter("@CommandCode", comandcode);
                pars[9] = string.IsNullOrEmpty(comment) ? new SqlParameter("@Comment", DBNull.Value) : new SqlParameter("@Comment", comment);
                pars[6] = new SqlParameter("@EndTime", endTime);
                return db.GetListSP<ITTransaction>("sp_ITTransaction_CMS_SelectList", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
        public List<BankReport> ReportDaily(DateTime beginTime, DateTime endTime, string partnerBankId)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@BeginTime", beginTime);
            pars[1] = new SqlParameter("@EndTime", endTime);
            pars[2] = string.IsNullOrEmpty(partnerBankId) ? new SqlParameter("@PartnerBankId", DBNull.Value) : new SqlParameter("@PartnerBankId", partnerBankId);
            return db.GetListSP<BankReport>("sp_ITTransaction_ReportDaily", pars);

        }
      
        public long Add()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            SqlParameter[] pars = new SqlParameter[12];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            
            pars[2] = string.IsNullOrEmpty(PartnerBankName) ? new SqlParameter("@PartnerBankName", DBNull.Value) : new SqlParameter("@PartnerBankName", PartnerBankName);
            pars[3] = string.IsNullOrEmpty(BankCode) ? new SqlParameter("@BankCode", DBNull.Value) : new SqlParameter("@BankCode", BankCode);
            pars[4] = string.IsNullOrEmpty(CommandCode) ? new SqlParameter("@CommandCode", DBNull.Value) : new SqlParameter("@CommandCode", CommandCode);
            pars[5] = Amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", Amount);
            pars[6] = string.IsNullOrEmpty(PartnerBankId) ? new SqlParameter("@PartnerBankId", " ") : new SqlParameter("@PartnerBankId", PartnerBankId);
            pars[7] = new SqlParameter("@Description", Description);
            pars[8] = string.IsNullOrEmpty(UserName) ? new SqlParameter("@UserName", " ") : new SqlParameter("@UserName", UserName);
            pars[9] = string.IsNullOrEmpty(BankId) ? new SqlParameter("@BankId", " ") : new SqlParameter("@BankId", BankId);
            pars[10] = string.IsNullOrEmpty(BankName) ? new SqlParameter("@BankName ", DBNull.Value) : new SqlParameter("@BankName ", BankName);
            pars[11] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            pars[1] = string.IsNullOrEmpty(PartnerBankCode) ? new SqlParameter("@PartnerBankCode", " ") : new SqlParameter("@PartnerBankCode", PartnerBankCode);
            db.ExecuteNonQuerySP("sp_ITTransaction_Insert", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }

    }
}
