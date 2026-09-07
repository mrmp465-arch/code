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
    public class BankCashService : IBankCashService
    {

        public BankCashAPI Get(int Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<BankCashAPI>("sp_BankCashAPI_Select",
                                                                                                   new SqlParameter("@TransactionID", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return null;
            }
        }
        public List<BankCashAPI> GetList(string select, string where, string order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                return new DBHelper(Config.MainConnectionString).GetListSP<BankCashAPI>("SP_BankCashAPI_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<BankCashAPI>();
            }
        }
        public List<BankCashAPI> GetFilter(int UserId, int Top, string OrderNo, int Amount, int Status, string FromDate = "", string ToDate = "")
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
                return GetList(select, where, orderby);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);

                return new List<BankCashAPI>();
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

            var dt = db.GetListSP<BankGateReport>("sp_BankCashAPI_Report", pars);
            totalTransaction = Convert.ToInt32(pars[4].Value);
            totalAmount = Convert.ToInt64(pars[5].Value);
            return dt;
        }
        public List<BankCashAPI> ReportDoiSoat(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect = 1)
        {
            DBHelper db = new DBHelper(Config.MainConnectionString);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@UserId", UserId);
            pars[1] = new SqlParameter("@BeginTime", beginTime);
            pars[2] = new SqlParameter("@EndTime", endTime);
            pars[3] = new SqlParameter("@TypeSelect", TypeSelect);
            return db.GetListSP<BankCashAPI>("sp_BankCashAPI_ReportDS", pars);
        }

        public List<BankCashAPI> ListReportDoiSoat(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect)
        {
            DBHelper db = new DBHelper(Config.MainConnectionString);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@UserId", UserId);
            pars[2] = new SqlParameter("@BeginTime", beginTime);
            pars[3] = new SqlParameter("@EndTime", endTime);
            pars[1] = new SqlParameter("@TypeSelect", TypeSelect);
            var data = db.GetListSP<BankCashAPI>("sp_BankCashAPI_ListReportDS", pars);

            return data;
        }

        public List<BankCashAPI> ReportDoiSoatDaily(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect)
        {
            DBHelper db = new DBHelper(Config.MainConnectionString);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@UserId", UserId);
            pars[3] = new SqlParameter("@BeginTime", beginTime);
            pars[1] = new SqlParameter("@EndTime", endTime);
            pars[2] = new SqlParameter("@TypeSelect", TypeSelect);
            var data = db.GetListSP<BankCashAPI>("sp_BankCashAPI_ReportDSDaily", pars);

            return data;
        }
        /// <summary>
        /// Insert Fucntion
        /// </summary>
        /// <param name="functions"></param>
        /// <returns> >0 : thanh cong
        ///			-1: da ton tai
        ///			-99: loi he thong
        /// </returns>
        public int Add(BankCashAPI functions)
        {
            try
            {
                var pars = new SqlParameter[11];
                pars[0] = new SqlParameter("@UserId", functions.UserId);
                pars[1] = new SqlParameter("@UserName", functions.UserName);
                pars[4] = new SqlParameter("@OrderInfo", functions.OrderInfo);
                pars[5] = new SqlParameter("@Amount", functions.Amount);
                pars[6] = new SqlParameter("@Status", functions.Status);
                pars[7] = new SqlParameter("@TotalAmount", functions.TotalAmount);
                pars[8] = new SqlParameter("@BankCode", functions.BankCode);
                pars[9] = new SqlParameter("@Note", functions.Note);
                pars[10] = new SqlParameter("@BankAccountName", functions.BankAccountName);
                pars[2] = new SqlParameter("@BankAccountNumber", functions.BankAccountNumber);
                pars[3] = new SqlParameter("@ReturnValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_BankCashAPI_Insert", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }

        public int Update(BankCashAPI functions)
        {
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@TransactionID", functions.TransactionID);
                pars[1] = new SqlParameter("@Status", functions.Status);
                pars[4] = new SqlParameter("@OrderInfo", functions.OrderInfo);
                pars[2] = new SqlParameter("@TotalAmount", functions.TotalAmount);
                pars[3] = new SqlParameter("@ReturnValue", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_BankCashAPI_Update", pars);
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

