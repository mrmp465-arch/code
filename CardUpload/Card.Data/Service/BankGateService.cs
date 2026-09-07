using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Card.Data.Service;
using Card.Data.DTO;
using Card.Utility;
using System.Linq;
using System.Globalization;

namespace Card.Data.Service
{
    public class BankGateService : IBankGateService
    {


        public BankGateAPI Get(int Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<BankGateAPI>("sp_BankGateAPI_Select",
                                                                                                   new SqlParameter("@TransactionID", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return  null;
            }
        }
        public List<BankGateAPI> GetList(string select, string where, string order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                return new DBHelper(Config.MainConnectionString).GetListSP<BankGateAPI>("SP_BankGateAPI_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<BankGateAPI>();
            }
        }
        public List<BankGateAPI> GetFilter(int UserId, int Top,string OrderNo,int Amount,int Status, string FromDate = "", string ToDate = "")
        {
            try
            {
                var orderby = "TransactionID DESC";
                var select = $"Top {Top} *";
                var where = "";
               
                if (UserId > 0)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " UserId=" + UserId;
                }
                if (!string.IsNullOrEmpty(OrderNo))
                {

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " OrderNo =" + "'" + OrderNo + "'";


                }
                if (Amount > 0)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " TotalAmount=" + Amount;
                }
                if (Status != -99)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " Status=" + Status;
                }
                if (!string.IsNullOrEmpty(FromDate) || !string.IsNullOrEmpty(ToDate))
                {
                    var culture = new CultureInfo("fr-FR", true);
                    var _FormDate = new DateTime(1900, 1, 1);
                    var _ToDate = new DateTime(9999, 1, 1);
                    if (!string.IsNullOrEmpty(FromDate))
                        _FormDate = DateTime.Parse(FromDate, culture).Date;
                    if (!string.IsNullOrEmpty(ToDate))
                        _ToDate = DateTime.Parse(ToDate, culture).Date.AddDays(1).AddSeconds(-1);

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where +=
                        " (convert(nvarchar(23),CreatedTime,121) between '" + _FormDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + _ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";

                }
                //NLogLogger.DebugMessage(select);
                //NLogLogger.DebugMessage(where);
                //NLogLogger.DebugMessage(orderby);
                return GetList(select,where,orderby);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                
                return new List<BankGateAPI>();
            }
        }

        // Lấy báo cáo
        public List<BankGateReport> Report(int UserId, int year, int month, int day, ref int totalTransaction, ref long totalAmount)
        {
            //NLogLogger.DebugMessage(year.ToString());
            DBHelper db = new DBHelper(Config.MainConnectionString);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@UserId", UserId);
            pars[1] = year == 0 ? new SqlParameter("@Year", DBNull.Value) : new SqlParameter("@Year", year);
            pars[2] = month == 0 ? new SqlParameter("@Month", DBNull.Value) : new SqlParameter("@Month", month);
            pars[3] = day == 0 ? new SqlParameter("@Day", DBNull.Value) : new SqlParameter("@Day", day);
            pars[4] = new SqlParameter("@TotalTransaction", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[5] = new SqlParameter("@TotalAmount", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
           
            var dt = db.GetListSP<BankGateReport>("sp_BankGateAPI_Report", pars);
            totalTransaction = Convert.ToInt32(pars[4].Value);
            totalAmount = Convert.ToInt64(pars[5].Value);
            return dt;
        }
        public List<BankGateAPI> ReportDoiSoat(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect = 1)
        {
            DBHelper db = new DBHelper(Config.MainConnectionString);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@UserId", UserId);
            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);
            pars[3] = new SqlParameter("@TypeSelect", TypeSelect);
            return db.GetListSP<BankGateAPI>("sp_BankGateAPI_ReportDS", pars);
        }

        public List<BankGateAPI> ListReportDoiSoat(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect)
        {
            DBHelper db = new DBHelper(Config.MainConnectionString);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@UserId", UserId);
            pars[2] = new SqlParameter("@BeginTime", beginTime);
            pars[3] = new SqlParameter("@EndTime", endTime);
            pars[1] = new SqlParameter("@TypeSelect", TypeSelect);
            var data = db.GetListSP<BankGateAPI>("sp_BankGateAPI_ListReportDS", pars);

            return data;
        }

        public List<BankGateAPI> ReportDoiSoatDaily(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect)
        {
            DBHelper db = new DBHelper(Config.MainConnectionString);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@UserId", UserId);
            pars[3] = new SqlParameter("@BeginTime", beginTime);
            pars[1] = new SqlParameter("@EndTime", endTime);
            pars[2] = new SqlParameter("@TypeSelect", TypeSelect);
            var data = db.GetListSP<BankGateAPI>("sp_BankGateAPI_ReportDSDaily", pars);

            return data;
        }
        public int Add(BankGateAPI functions)
        {
            try
            {
                var pars = new SqlParameter[12];
                pars[0] = new SqlParameter("@UserId", functions.UserId);
                pars[1] = new SqlParameter("@UserName", functions.UserName);
                pars[2] = new SqlParameter("@OrderNo", functions.OrderNo);
                pars[4] = new SqlParameter("@OrderInfo", functions.OrderInfo);
                pars[5] = new SqlParameter("@Amount", functions.Amount);
                pars[6] = new SqlParameter("@Status", functions.Status);
                pars[7] = new SqlParameter("@TotalAmount", functions.TotalAmount);
                pars[8] = new SqlParameter("@BankCode", functions.BankCode);
                pars[9] = new SqlParameter("@Mobile", functions.Mobile);
                pars[10] = new SqlParameter("@BankAccountName", functions.BankAccountName);
                pars[11] = new SqlParameter("@BankAccountNumber", functions.BankAccountNumber);
                pars[3] = new SqlParameter("@ReturnValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_BankGateAPI_Insert", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }

        public int UpdateBank(BankGateAPI functions)
        {
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@TransactionID", functions.TransactionID);
                pars[1] = new SqlParameter("@OrderNo", functions.OrderNo);
                pars[2] = new SqlParameter("@BankAccountName", functions.BankAccountName);
                pars[4] = new SqlParameter("@BankAccountNumber", functions.BankAccountNumber);
                pars[3] = new SqlParameter("@ReturnValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_BankGateAPI_UpdateBank", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }

        public int Update(BankGateAPI functions)
        {
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@TransactionID", functions.TransactionID);
                pars[1] = new SqlParameter("@Status", functions.Status);
                pars[2] = new SqlParameter("@Amount", functions.Amount);
                pars[4] = new SqlParameter("@OrderInfo", functions.OrderInfo);
                pars[5] = new SqlParameter("@TotalAmount", functions.TotalAmount);
                pars[6] = new SqlParameter("@Mobile", functions.Mobile);
                pars[3] = new SqlParameter("@ReturnValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_BankGateAPI_Update", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
    }
}

