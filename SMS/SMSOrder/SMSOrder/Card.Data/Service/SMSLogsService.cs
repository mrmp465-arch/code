using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SMS.Data.Service;
using SMS.Data.DTO;
using SMS.Utility;
using System.Linq;
using System.Globalization;

namespace SMS.Data.Service
{
    public class SMSLogsService : ISMSLogsService
    {

      
        public List<SMSLogs> GetTopSMS(int top, int Telco)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@Top", top);
                pars[1] = new SqlParameter("@Telco", Telco);
                //pars[2] = new SqlParameter("@OrderByExpression", order);
                //var data = Config.MainConnectionString;
                var lstdata = new DBHelper(Config.MainConnectionString).GetListSP<SMSLogs>("SP_SMSLog_GetTop", pars);
                return lstdata;

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSLogs>();
            }
        }
        //public List<SMSLogs> GetTopSMS(int top,int Telco )
        //{
        //    var select = "Top("+ top + ") * ";
        //    var order = "Priority,StartTime  ";
        //    var where = "Status=2 AND ResponseStatus=0 And CampaignId>0 And Confirm=0 And Telco=" + Telco;


        //    return GetDynamic(select, where, order);
        //}

        public SMSLogs GetSMS(int id)
        {
            var select = " * ";
            var order = "";
            var where = "Id=" + id;


            return GetDynamic(select, where, order).FirstOrDefault();
        }
        public List<SMSLogs> GetFilter(Users user, int cId, string keyword,string port,string telco,int status, int page, int pageSize, ref int total)
        {
            var select = "*";
            var order = "Id DESC";
            var where = "";
            if (cId > 0)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";

                where += " [CampaignId]=" + cId.ToString();
            }
            if (!string.IsNullOrEmpty(port))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";

                where += " [Port]=" + port;
            }
            if (!string.IsNullOrEmpty(telco))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                if(telco=="4")
                {
                    where += " ([Number] like '092%' or [Number] like '058%' or [Number] like '056%' or [Number] like '052%') ";
                }
                else
                {
                    where += " [Telco]=" + telco;
                }
                
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " ( Number LIKE N'%" + keyword + "%' OR ResponseMessage LIKE N'%" + keyword + "%' OR Sender LIKE N'%" + keyword + "%' OR  Contents LIKE N'%" + keyword + "%') ";
            }
            if(status>=0)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                if(status==0)
                {
                    where += " [ResponseStatus]<2" ;
                }
                if (status == 1)
                {
                    where += " [ResponseStatus]=2";
                }
            }
            if (user.Type ==2)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " CreatedUser = '" + user.Username + "'";
            }
            //NLogLogger.DebugMessage(where);
            return GetList(select, where, order, page, pageSize, ref total);
        }
        public List<SMSLogs> GetList(string select, string where, string order, int page, int pageSize, ref int total)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                pars[3] = new SqlParameter("@PageIndex", page);
                pars[4] = new SqlParameter("@PageSize", pageSize);
                pars[5] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var data = new DBHelper(Config.MainConnectionString).GetListSP<SMSLogs>("sp_SMSLog_SelectPagedDynamic", pars);
                total = Convert.ToInt32(pars[5].Value);
                return data;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                total = 0;
                return new List<SMSLogs>();
            }
        }
        public List<SMSReport> GetReportDaily(Users user, string fromdate, string todate)
        {
            try
            {
                var select = "CONVERT(VARCHAR(8),   ResponseTime, 3) as Data,  Count([Id]) as Total,Telco";

                var where = "ResponseStatus>1 ";
                var orderBy = "CONVERT(VARCHAR(8),   ResponseTime, 3) ASC";
                var groupBy = "CONVERT(VARCHAR(8),   ResponseTime, 3), Telco";

                if (!string.IsNullOrEmpty(fromdate) || !string.IsNullOrEmpty(todate))
                {
                    var culture = new CultureInfo("fr-FR", true);
                    var _FormDate = new DateTime(1900, 1, 1);
                    var _ToDate = new DateTime(9999, 1, 1);
                    if (!string.IsNullOrEmpty(fromdate))
                        _FormDate = DateTime.Parse(fromdate, culture).Date;
                    if (!string.IsNullOrEmpty(todate))
                        _ToDate = DateTime.Parse(todate, culture).Date.AddDays(1).AddSeconds(-1);

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where +=
                        " (convert(nvarchar(23),ResponseTime,121) between '" + _FormDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + _ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";

                }
                



                if (user.Type ==2)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " CreatedUser = '" + user.Username + "'";
                }
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                NLogLogger.DebugMessage(where);
                return new DBHelper(Config.MainConnectionString).GetListSP<SMSReport>("SP_SMSLog_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSReport>();
            }
        }
        public List<SMSReport> GetReporHour(Users user, string fromdate, string telco)
        {
            try
            {
                var select = "CONVERT(varchar(10), DATEPART(HOUR, ResponseTime)) as Data,  Count([Id]) as Total,Telco";

                var where = "ResponseStatus>1 ";
                var orderBy = "DATEPART(HOUR, ResponseTime)  ASC";
                var groupBy = "DATEPART(HOUR, ResponseTime), Telco";

                if (!string.IsNullOrEmpty(fromdate) )
                {
                    var culture = new CultureInfo("fr-FR", true);
                    var _FormDate = new DateTime(1900, 1, 1);
                    var _ToDate = new DateTime(9999, 1, 1);
                    if (!string.IsNullOrEmpty(fromdate))
                        _FormDate = DateTime.Parse(fromdate, culture).Date;
                    
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where +=
                           " (convert(nvarchar(10),ResponseTime,103)) = '" + fromdate + "'";

                }
                if (!string.IsNullOrEmpty(telco))
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " Telco = " + telco;
                }
                if (user.Type ==2)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " CreatedUser = '" + user.Username + "'";
                }
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<SMSReport>("SP_SMSLog_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSReport>();
            }
        }
        public List<SMSReport> GetReporRevenue(string fromdate, string todate,Users user)
        {
            try
            {
                var select = "CONVERT(VARCHAR(8),   ResponseTime, 1) as Data,CreatedUser,  Count([Id]) as Total";

                var where = "ResponseStatus>1 And CreatedUser<>'admin' ";
                //var orderBy = "CONVERT(VARCHAR(8),   ResponseTime, 3)";
                var orderBy = "CONVERT(VARCHAR(8),   ResponseTime, 1)";
                var groupBy = "CreatedUser, CONVERT(VARCHAR(8),   ResponseTime, 1)";

                if (!string.IsNullOrEmpty(fromdate) || !string.IsNullOrEmpty(todate))
                {
                    var culture = new CultureInfo("fr-FR", true);
                    var _FormDate = new DateTime(1900, 1, 1);
                    var _ToDate = new DateTime(9999, 1, 1);
                    if (!string.IsNullOrEmpty(fromdate))
                        _FormDate = DateTime.Parse(fromdate, culture).Date;
                    if (!string.IsNullOrEmpty(todate))
                        _ToDate = DateTime.Parse(todate, culture).Date.AddDays(1).AddSeconds(-1);

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where +=
                        " (convert(nvarchar(23),ResponseTime,121) between '" + _FormDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + _ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";

                }

                if (user.Type > 1)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " CreatedUser = '" + user.Username + "'";
                }
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<SMSReport>("SP_SMSLog_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSReport>();
            }
        }
        public List<SMSReport> GetReporTopup(DateTime fromdate, DateTime todate)
        {
            try
            {
                var select = "CONVERT(VARCHAR(8),   [Time], 3) as Data,  Sum([Amount]) as Total";

                var where = "[Type]=1 ";
                var orderBy = "CONVERT(VARCHAR(8),   [Time], 3)";
                var groupBy = " CONVERT(VARCHAR(8),   [Time], 3)";

                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where +=
                    " (convert(nvarchar(23),[Time],121) between '" + fromdate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + todate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";


               
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<SMSReport>("SP_TransactionSim_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSReport>();
            }
        }
        public List<SMSReport> GetReporLockSim(DateTime fromdate, DateTime todate)
        {
            try
            {
                var select = "CONVERT(VARCHAR(8),   [LockDate], 3) as Data,  Count([Id]) as Total";

                var where = "Status=0 ";
                var orderBy = "CONVERT(VARCHAR(8),   [LockDate], 3)";
                var groupBy = " CONVERT(VARCHAR(8),   [LockDate], 3)";

                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where +=
                    " (convert(nvarchar(23),[LockDate],121) between '" + fromdate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + todate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";



                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<SMSReport>("SP_Sim_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSReport>();
            }
        }
        public List<SMSReport> GetReporSim(string fromdate, string telco)
        {
            try
            {
                var select = "Sender as Data,  Count([Id]) as Total";

                var where = "ResponseStatus>1 ";
                var orderBy = "Total  DESC";
                var groupBy = "Sender";

                if (!string.IsNullOrEmpty(fromdate))
                {
                    var culture = new CultureInfo("fr-FR", true);
                    var _FormDate = new DateTime(1900, 1, 1);
                    var _ToDate = new DateTime(9999, 1, 1);
                    if (!string.IsNullOrEmpty(fromdate))
                        _FormDate = DateTime.Parse(fromdate, culture).Date;

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where +=
                           " (convert(nvarchar(10),ResponseTime,103)) = '" + fromdate + "'";

                }
                if (!string.IsNullOrEmpty(telco))
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " Telco = " + telco;
                }

                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<SMSReport>("SP_SMSLog_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSReport>();
            }
        }
        public List<SMSReport> GetReporUser( string fromdate, string telco)
        {
            try
            {
                var select = "CONVERT(varchar(10), DATEPART(HOUR, ResponseTime)) as Data,CreatedUser,  Count([Id]) as Total";

                var where = "ResponseStatus>1 ";
                var orderBy = "DATEPART(HOUR, ResponseTime)  ASC";
                var groupBy = "CreatedUser, DATEPART(HOUR, ResponseTime)";

                if (!string.IsNullOrEmpty(fromdate))
                {
                    var culture = new CultureInfo("fr-FR", true);
                    var _FormDate = new DateTime(1900, 1, 1);
                    var _ToDate = new DateTime(9999, 1, 1);
                    if (!string.IsNullOrEmpty(fromdate))
                        _FormDate = DateTime.Parse(fromdate, culture).Date;

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where +=
                           " (convert(nvarchar(10),ResponseTime,103)) = '" + fromdate + "'";

                }
                if (!string.IsNullOrEmpty(telco))
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " Telco = " + telco;
                }
                
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<SMSReport>("SP_SMSLog_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSReport>();
            }
        }
        private List<SMSLogs> GetDynamic(string select, string where, string order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                //var data = Config.MainConnectionString;
                var lstdata= new DBHelper(Config.MainConnectionString).GetListSP<SMSLogs>("SP_SMSLog_SelectDynamic", pars);
                return lstdata;

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSLogs>();
            }
        }
        public int Add(string Number,string Content,string Username, int Telco,int CampaignId,int price)
        {
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_Number", Number);
                pars[1] = new SqlParameter("@_Contents", Content);
                pars[2] = new SqlParameter("@_CreatedUser", Username);
                pars[3] = new SqlParameter("@_Telco", Telco);
                pars[5] = new SqlParameter("@_CampaignId", CampaignId);
                pars[6] = new SqlParameter("@_Price", price);
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_SMSLog_Add", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int UpdateTime(int Id,DateTime StartTime)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Id", Id);
                pars[1] = new SqlParameter("@_StartTime", StartTime);
                pars[2] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_SMSLog_UpdateTime", pars);
                return Convert.ToInt32(pars[2].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public void UpdateMultiRespone(List<long> lstId, int CampaignId, int Status, string Message)
        {
            try
            {
                var ids = String.Join(",", lstId);
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@Ids", ids);
                pars[1] = new SqlParameter("@CampaignId", CampaignId);
                pars[2] = new SqlParameter("@Status", Status);
                pars[3] = new SqlParameter("@Message", Message);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_SMSLog_UpdateResponeMuti", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return;
            }
        }
        public void UpdateRespone(long Id,int CampaignId, int Status,string Message)
        {
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@Id", Id);
                pars[1] = new SqlParameter("@CampaignId", CampaignId);
                pars[2] = new SqlParameter("@Status", Status);
                pars[3] = new SqlParameter("@Message", Message);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_SMSLog_UpdateRespone", pars);
               
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return;
            }
        }
        public int UpdateDynamic(string where, string updatest)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@UpdateCondition", updatest);
                pars[1] = new SqlParameter("@WhereCondition", where);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_SMSLog_UpdateDynamic", pars);
                return 1;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }

    }
}

