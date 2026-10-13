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
    public class MomoTransaction
    {
        public long Id { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerMomoId { get; set; }
        public string RefCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public long MomoTransId { get; set; }
        public string MomoId { get; set; }
        public string Comment { get; set; }
        public DateTime? TimeMomoSuccess { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string IpAddress { get; set; }
        public string CallbackUrl { get; set; }
        public int? Amount { get; set; }
        public int? Status { get; set; }
        public string Description { get; set; }
        public long ReturnValue { get; set; }
        public MomoTransaction()
        {

        }
        public MomoTransaction Get()
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            return db.GetInstanceSP<MomoTransaction>("sp_APITransaction_CMS_Select", new SqlParameter("@Id", Id));
        }
        public MomoTransaction GetByRefcode(string Refcode)
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            return db.GetInstanceSP<MomoTransaction>("sp_APITransaction_CMS_SelectByRefCode", new SqlParameter("@RefCode", Refcode));
        }
        public List<MomoTransaction> GetList(int Top, long? id, string PartnerCode, string PartnerMomoId, string MomoTransId, string MomoId, string CommandCode, DateTime creatTime, int? status, bool isByPass10k, string refCode, string comment,string source)
        {
            int? amount = null;
            if (isByPass10k)
                amount = 10000;
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOReportConnectionStrings);
                SqlParameter[] pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@Top", Top);
                pars[1] = id == null ? new SqlParameter("@Id", DBNull.Value) : new SqlParameter("@Id", id);
                pars[2] = string.IsNullOrEmpty(PartnerCode) ? new SqlParameter("@PartnerCode", DBNull.Value) : new SqlParameter("@PartnerCode", PartnerCode);
                pars[3] = new SqlParameter("@CreatedTime", creatTime);
                pars[4] = status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", status);
                pars[5] = string.IsNullOrEmpty(PartnerMomoId) ? new SqlParameter("@PartnerMomoId", DBNull.Value) : new SqlParameter("@PartnerMomoId", PartnerMomoId);
                pars[6] = string.IsNullOrEmpty(MomoTransId) ? new SqlParameter("@MomoTransId", DBNull.Value) : new SqlParameter("@MomoTransId", MomoTransId);
                pars[7] = string.IsNullOrEmpty(MomoId) ? new SqlParameter("@MomoId", DBNull.Value) : new SqlParameter("@MomoId", MomoId);
                pars[8] = string.IsNullOrEmpty(CommandCode) ? new SqlParameter("@CommandCode", DBNull.Value) : new SqlParameter("@CommandCode", CommandCode);
                pars[9] = amount == null ? new SqlParameter("@Amount", DBNull.Value) : new SqlParameter("@Amount", amount);
                pars[10] = string.IsNullOrEmpty(refCode) ? new SqlParameter("@RefCode", DBNull.Value) : new SqlParameter("@RefCode", refCode);
                pars[11] = string.IsNullOrEmpty(comment) ? new SqlParameter("@Comment", DBNull.Value) : new SqlParameter("@Comment", comment);
                pars[12] = string.IsNullOrEmpty(source) ? new SqlParameter("@Source", DBNull.Value) : new SqlParameter("@Source", source);
                return db.GetListSP<MomoTransaction>("sp_APITransaction_CMS_SelectList", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
        public List<MomoReport> Report(int year, int month, int day,string source)
        {
            // DBHelper db = new DBHelper(Configs.VPGMOMOReportConnectionStrings);
            DBHelper db = new DBHelper(Configs.VPGMOMOReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[1] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[2] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[3] = string.IsNullOrEmpty(source) ? new SqlParameter("@Source", DBNull.Value) : new SqlParameter("@Source", source);
            return db.GetListSP<MomoReport>("sp_APITransaction_Report", pars);
            
        }
        public List<MomoReport> ReportDaily(DateTime beginTime, DateTime endTime, string source)
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOReportConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@BeginTime", beginTime);
            pars[1] = new SqlParameter("@EndTime", endTime);
            pars[2] = string.IsNullOrEmpty(source) ? new SqlParameter("@Source", DBNull.Value) : new SqlParameter("@Source", source);
            return db.GetListSP<MomoReport>("sp_APITransaction_ReportDaily", pars);

        }
        public long Update()
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
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
        public long UpdateCash(int Amount,string MomoId)
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = new SqlParameter("@Amount", Amount);
            pars[3] = new SqlParameter("@MomoId", MomoId);

            db.ExecuteNonQuerySP("sp_APITransaction_UpdateCash", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
            return ReturnValue;
        }
    }
}
