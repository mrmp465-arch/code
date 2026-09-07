using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Card.Data.Service;
using Card.Data.DTO;
using Card.Utility;
using System.Linq;
using Card.Data.Api;
using System.Globalization;

namespace Card.Data.Service
{
    public class OrderReportsService : IOrderReportsService
    {
        public int UpdateAmount(long Id, int AmountMin)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@Id", Id);
                pars[1] = new SqlParameter("@AmountMin", AmountMin);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_OrderReport_SetAmountMin", pars);
                return 1;
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int Confirm(long Id)
        {
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@Id", Id);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_OrderReport_Confirm", pars);
                return 1;
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int UpdateStatus(long Id, int Status, string Description = "")
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@Id", Id);
                pars[1] = new SqlParameter("@Status", Status);
                pars[2] = new SqlParameter("@Description", Description);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_OrderReport_UpdateStatus", pars);
                return 1;
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int UpdateStatusOrder(string Order, int Status)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@Order", Order);
                pars[1] = new SqlParameter("@Status", Status);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_OrderReport_UpdateStatusOrder", pars);
                return 1;
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int UnConfirm(long Id)
        {
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@Id", Id);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_OrderReport_UnConfirm", pars);
                return 1;
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public OrderReport Get(long Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<OrderReport>("SP_OrderReport_Get",
                                                                                                   new SqlParameter("@Id", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new OrderReport();
            }
        }
        public OrderReportHistory GetByCode(string Code)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<OrderReportHistory>("SP_OrderReportHistory_Get",
                                                                                                   new SqlParameter("@CardCode", Code));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return null;
            }
        }


        public long Add(OrderReport functions)
        {
            try
            {
                var pars = new SqlParameter[19];
                pars[0] = new SqlParameter("@_OrderId", functions.OrderId);
                pars[1] = new SqlParameter("@_Mobile", functions.Mobile);
                pars[2] = new SqlParameter("@_UserName", functions.UserName);
                pars[4] = new SqlParameter("@_OrderNo", functions.OrderNo);
                pars[5] = new SqlParameter("@_Telco", functions.Telco.ToUpper());
                pars[6] = new SqlParameter("@_TopupType", functions.TopupType);
                pars[7] = new SqlParameter("@_Amount", functions.Amount);
                pars[8] = new SqlParameter("@_UserApi", functions.UserApi);
                pars[9] = new SqlParameter("@_Type", functions.Type);
                pars[10] = new SqlParameter("@_Ussd", functions.Ussd);
                pars[11] = new SqlParameter("@_ParrentName", functions.ParrentName);
                pars[12] = new SqlParameter("@_Percent", functions.Percent);
                pars[13] = new SqlParameter("@_PercentParrent", functions.PercentParrent);
                pars[14] = new SqlParameter("@_Priority", functions.Priority);
                pars[15] = new SqlParameter("@_PasswordApi", functions.PasswordApi);
                pars[16] = new SqlParameter("@_C1Name", functions.C1Name);
                pars[17] = new SqlParameter("@_PercentC1", functions.PercentC1);
                pars[18] = new SqlParameter("@_AmountMin", functions.AmountMin);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_OrderReport_Add", pars);
                return Convert.ToInt64(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public List<OrderReport> OrderSearch(string mobile)
        {
            try
            {
                var select = "*";

                var where = string.Empty;
                var orderBy = String.Empty;


                if (!string.IsNullOrEmpty(mobile))

                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " Mobile LIKE N'%" + mobile + "%'  ";

                }

                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);

                return new DBHelper(Config.MainConnectionString).GetListSP<OrderReport>("SP_OrderReport_SelectDynamic", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderReport>();
            }
        }
        public List<OrderReportHistoryItem> GetGroupMaxBid(string fromdate, string telco)
        {
            try
            {
                var select = " CONVERT(varchar(10), DATEPART(HOUR, CreatedDate)) as Data, BidRate, Sum([Amount]) as TotalAmount";

                var where = "[BidRate]>0";

                var orderBy = " DATEPART(HOUR, CreatedDate)  ASC";
                var groupBy = " BidRate, DATEPART(HOUR, CreatedDate) ";

                if (!string.IsNullOrEmpty(fromdate))
                {
                    var culture = new CultureInfo("fr-FR", true);
                    var _FormDate = new DateTime(1900, 1, 1);


                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where +=
                        " (convert(nvarchar(10),CreatedDate,103)) = '" + fromdate + "'";

                }
                if (!string.IsNullOrEmpty(telco))

                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Telco='" + telco.ToUpper() + "'";
                }

                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@GroupByExpression", groupBy);
                pars[3] = new SqlParameter("@OrderByExpression", orderBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<OrderReportHistoryItem>("SP_OrderReportHistory_SelectDynamic", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderReportHistoryItem>();
            }
        }
        public List<OrderReport> GetMaxBid()
        {
            try
            {
                var select = "Max(BidRate) as BidRate, Telco";

                var where = " ([Amount]-[AmoutSuccess]) >=50000 And IsConfirm=0 And [BidRate]>0  AND [AmoutSuccess]< [Amount] And CreatedDate>=DATEADD(day, -2, getdate()) AND Status=1";


                var groupby = "[Telco] ";

                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@GroupByExpression", groupby);

                return new DBHelper(Config.MainConnectionString).GetListSP<OrderReport>("SP_OrderReport_SelectDynamic", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderReport>();
            }
        }
        public List<OrderReport> GetTopBid(int top, string username, string telco, int BidRate)
        {
            try
            {
                var select = "TOP(" + top + ") *";

                var where = " IsConfirm =0   And [AmoutSuccess] < [Amount] And [BidRate]>0  And CreatedDate>=DATEADD(day, -60, getdate()) AND Status=1";
                if (!string.IsNullOrEmpty(username))
                {
                    where += " And [UserApi]='" + username + "'";
                }
                if (!string.IsNullOrEmpty(telco))
                {

                    where += " And Telco='" + telco.ToUpper() + "'";
                }
                if (BidRate > 0)
                {

                    where += " And BidRate=" + BidRate;
                }
                var orderBy = "[BidRate] DESC";




                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);

                return new DBHelper(Config.MainConnectionString).GetListSP<OrderReport>("SP_OrderReport_SelectDynamic", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderReport>();
            }
        }
        public List<OrderGroupBid> GetGroupBid(int top, string username, string telco, int AmountMin)
        {
            try
            {
                var select = "[BidRate] , Sum ([Amount]) As TotalAmount , Sum ([AmoutSuccess]) As TotalAmountSuccess";

                var where = "  IsConfirm = 0   And [AmoutSuccess] < [Amount] And [BidRate]>0  And CreatedDate>=DATEADD(day, -60, getdate()) AND Status=1";
                if (!string.IsNullOrEmpty(username))
                {
                    where += " And [UserApi]='" + username + "'";
                }
                if (!string.IsNullOrEmpty(telco))
                {

                    where += " And Telco='" + telco.ToUpper() + "'";
                }
                if (AmountMin > 0)
                {

                    where += " And ([Amount]-[AmoutSuccess]) >=" + AmountMin;
                }
                var orderBy = " [BidRate] DESC";


                var groupby = " [BidRate]";

                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupby);
                return new DBHelper(Config.MainConnectionString).GetListSP<OrderGroupBid>("SP_OrderReport_SelectDynamic", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderGroupBid>();
            }
        }
        public List<OrderReportHistory> OrderHistory(int orderId)
        {
            try
            {
                var select = "*";

                var where = string.Empty;
                var orderBy = String.Empty;


                if (orderId > 0)

                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " OrderId=" + orderId.ToString();

                }

                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);

                return new DBHelper(Config.MainConnectionString).GetListSP<OrderReportHistory>("SP_OrderReportHistory_SelectDynamic", pars);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderReportHistory>();
            }
        }
        public List<UserWarning> ListUserWarning()
        {
            try
            {
                var select = "UserName, Min(UpdateDate) AS LastTime";

                var where = "[IsConfirm]=0";
                var orderBy = "LastTime ASC";
                var groupBy = "[UserName]";

                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<UserWarning>("SP_OrderReport_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<UserWarning>();
            }
        }
        public List<OrderGroup> ListGroupFinish(Users user, string fromdate, string todate, string telco)
        {
            try
            {
                var select = "[OrderNo], Sum(Amount) as TotalAmount,Sum(AmoutSuccess) as TotalAmountSuccess, Max(UpdateDate) AS LastTime, Max(Telco) AS Telco";

                var where = "[IsConfirm]=1";
                var orderBy = "Max(UpdateDate) DESC";
                var groupBy = "[OrderNo]";

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
                        " (convert(nvarchar(23),CreatedDate,121) between '" + _FormDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + _ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";

                }
                if (!string.IsNullOrEmpty(telco))

                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Telco='" + telco.ToUpper() + "'";
                }



                if (user.Type > 1)
                {
                    where += " AND  UserApi = '" + user.UserAPI + "'";
                    if (user.Type == 3)
                    {
                        where += " AND  ParrentName = '" + user.Username + "'";
                    }
                    if (user.Type == 4)
                    {
                        where += " AND UserName = '" + user.Username + "'";
                    }
                }
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<OrderGroup>("SP_OrderReport_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderGroup>();
            }
        }
        public List<OrderReportHistoryDS> GetReportDS(Users user, string fromdate, string todate)
        {
            try
            {
                var select = "CONVERT(VARCHAR(8),   CreatedDate, 3) as Data, SUM([Amount]) as TotalAmount, SUM([BidFee]) as TotalBidFee,"
                    + "SUM(CASE WHEN Telco = 'VNP' THEN[Amount] ELSE 0 END) TotalVNP,"
                    + "SUM(CASE WHEN Telco = 'Garena' THEN[Amount] ELSE 0 END) TotalGarena,"
                    + "SUM(CASE WHEN Telco = 'VMS' THEN[Amount] ELSE 0 END) TotalVMS,"
                    + "SUM(CASE WHEN Telco = 'Zing' THEN[Amount] ELSE 0 END) TotalZing,"
                    + "SUM(CASE WHEN Telco = 'VTT' AND Type = 1 THEN[Amount] ELSE 0 END) TotalMyVTT,"
                    + "SUM(CASE WHEN Telco = 'VTT' AND Type = 0 THEN[Amount] ELSE 0 END) TotalVTT";

                var where = string.Empty;
                var orderBy = "CONVERT(VARCHAR(8),   CreatedDate, 3) ASC";
                var groupBy = "CONVERT(VARCHAR(8),   CreatedDate, 3)";

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
                        " (convert(nvarchar(23),CreatedDate,121) between '" + _FormDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + _ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";

                }


                if (user.Type > 1)
                {
                    where += " AND  UserApi = '" + user.UserAPI + "'";

                }
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<OrderReportHistoryDS>("SP_OrderReportHistory_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderReportHistoryDS>();
            }
        }
        public List<OrderReportHistoryItem> GetReportDaily(Users user, string fromdate, string todate, string telco, int type, string OrderNo)
        {
            try
            {
                var select = "CONVERT(VARCHAR(8),   CreatedDate, 1) as Data,  Count([Id]) as TotalTrans, SUM([Amount]) as TotalAmount, SUM([Amount]/100 *[Percent]) as TotalRevenue, SUM([BidFee]) as TotalBidFee";
                if (user.Type == 3)
                    select = "CONVERT(VARCHAR(8),   CreatedDate, 1) as Data,  Count([Id]) as TotalTrans, SUM([Amount]) as TotalAmount, SUM([Amount]/100 *[PercentC1]) as TotalRevenue, SUM([BidFee]) as TotalBidFee";
                if (user.Type == 4)
                    select = "CONVERT(VARCHAR(8),   CreatedDate, 1) as Data,  Count([Id]) as TotalTrans, SUM([Amount]) as TotalAmount, SUM([Amount]/100 *[PercentParrent]) as TotalRevenue, SUM([BidFee]) as TotalBidFee";
                var where = string.Empty;
                var orderBy = "CONVERT(VARCHAR(8),   CreatedDate, 1) ASC";
                var groupBy = "CONVERT(VARCHAR(8),   CreatedDate, 1)";

                if (!string.IsNullOrEmpty(OrderNo))

                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " OrderNo='" + OrderNo + "'";
                }
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
                        " (convert(nvarchar(23),CreatedDate,121) between '" + _FormDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + _ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";

                }
                if (!string.IsNullOrEmpty(telco))

                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Telco='" + telco.ToUpper() + "'";
                }


                if (type > -1)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Type=" + type.ToString();
                }
                if (user.Type > 1)
                {
                    where += " AND  UserApi = '" + user.UserAPI + "'";
                    if (user.Type == 3)
                    {
                        where += " AND  C1Name = '" + user.Username + "'";
                    }
                    if (user.Type == 4)
                    {
                        where += " AND  ParrentName = '" + user.Username + "'";
                    }
                    if (user.Type == 5)
                    {
                        where += " AND  UserName = '" + user.Username + "'";
                    }
                }
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<OrderReportHistoryItem>("SP_OrderReportHistory_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderReportHistoryItem>();
            }
        }
        public List<OrderReportHistoryItem> GetReportUserAdmin(string fromdate, string todate, string telco, int type)
        {
            try
            {
                var select = "UserApi AS UserName ,CONVERT(VARCHAR(8),   CreatedDate, 3) as Data,  Count([Id]) as TotalTrans, SUM([Amount]) as TotalAmount, SUM([BidFee]) as TotalBidFee";

                var where = string.Empty;
                var orderBy = "CONVERT(VARCHAR(8),   CreatedDate, 3) ASC";
                var groupBy = "UserApi,CONVERT(VARCHAR(8),   CreatedDate, 3)";

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
                        " (convert(nvarchar(23),CreatedDate,121) between '" + _FormDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + _ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";

                }
                if (!string.IsNullOrEmpty(telco))

                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Telco='" + telco.ToUpper() + "'";
                }


                if (type > -1)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Type=" + type.ToString();
                }

                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<OrderReportHistoryItem>("SP_OrderReportHistory_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderReportHistoryItem>();
            }
        }
        public List<OrderReportHistoryItem> GetReportUser(string UserAPI, string fromdate, string todate, string telco, int type)
        {
            try
            {
                var select = "C1Name AS UserName ,CONVERT(VARCHAR(8),   CreatedDate, 3) as Data,  Count([Id]) as TotalTrans, SUM([Amount]) as TotalAmount, SUM([Amount]/100 *[Percent]) as TotalRevenue,  SUM([Amount]/100 *[PercentC1]) as AmountRevenueParrent, SUM([BidFee]) as TotalBidFee";

                var where = string.Empty;
                var orderBy = "CONVERT(VARCHAR(8),   CreatedDate, 3) ASC";
                var groupBy = "C1Name,CONVERT(VARCHAR(8),   CreatedDate, 3)";

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
                        " (convert(nvarchar(23),CreatedDate,121) between '" + _FormDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + _ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";

                }
                if (!string.IsNullOrEmpty(telco))

                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Telco='" + telco.ToUpper() + "'";
                }


                if (type > -1)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Type=" + type.ToString();
                }
                if (!string.IsNullOrEmpty(UserAPI))
                {
                    where += " AND  UserApi = '" + UserAPI + "'";

                }
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<OrderReportHistoryItem>("SP_OrderReportHistory_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderReportHistoryItem>();
            }
        }
      
        public List<OrderReportHistoryItem> GetReportOur(Users user, string fromdate, string telco, int type)
        {
            try
            {
                var select = "  CONVERT(varchar(10), DATEPART(HOUR, CreatedDate)) as Data,  Count([Id]) as TotalTrans, SUM([Amount]) as TotalAmount, SUM([Amount]/100 *[Percent]) as TotalRevenue, SUM([BidFee]) as TotalBidFee";
                if (user.Type == 3)
                {
                    select = "  CONVERT(varchar(10), DATEPART(HOUR, CreatedDate)) as Data,  Count([Id]) as TotalTrans, SUM([Amount]) as TotalAmount, SUM([Amount]/100 *[PercentC1]) as TotalRevenue, SUM([BidFee]) as TotalBidFee";
                }
                if (user.Type == 4)
                {
                    select = "  CONVERT(varchar(10), DATEPART(HOUR, CreatedDate)) as Data,  Count([Id]) as TotalTrans, SUM([Amount]) as TotalAmount, SUM([Amount]/100 *[PercentParrent]) as TotalRevenue, SUM([BidFee]) as TotalBidFee";
                }
                var where = string.Empty;
                var orderBy = " DATEPART(HOUR, CreatedDate)  ASC";
                var groupBy = " DATEPART(HOUR, CreatedDate) ";

                if (!string.IsNullOrEmpty(fromdate))
                {
                    var culture = new CultureInfo("fr-FR", true);
                    var _FormDate = new DateTime(1900, 1, 1);


                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where +=
                        " (convert(nvarchar(10),CreatedDate,103)) = '" + fromdate + "'";

                }
                if (!string.IsNullOrEmpty(telco))

                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Telco='" + telco.ToUpper() + "'";
                }


                if (type > -1)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Type=" + type.ToString();
                }
                if (user.Type > 1)
                {
                    where += " AND  UserApi = '" + user.UserAPI + "'";
                    if (user.Type == 3)
                    {
                        where += " AND  C1Name = '" + user.Username + "'";
                    }
                    if (user.Type == 4)
                    {
                        where += " AND  ParrentName = '" + user.Username + "'";
                    }
                    if (user.Type == 5)
                    {
                        where += " AND  UserName = '" + user.Username + "'";
                    }
                }
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<OrderReportHistoryItem>("SP_OrderReportHistory_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderReportHistoryItem>();
            }
        }
        public long AddHistory(OrderReportHistory functions)
        {
            try
            {
                var pars = new SqlParameter[23];
                pars[0] = new SqlParameter("@_OrderId", functions.OrderId);
                pars[1] = new SqlParameter("@_Mobile", functions.Mobile);
                pars[2] = new SqlParameter("@_UserName", functions.UserName);
                pars[4] = new SqlParameter("@_OrderNo", functions.OrderNo);
                pars[5] = new SqlParameter("@_Telco", functions.Telco);
                pars[6] = new SqlParameter("@_TopupType", functions.TopupType);
                pars[7] = new SqlParameter("@_Amount", functions.Amount);
                pars[8] = new SqlParameter("@_UserApi", functions.UserApi);
                pars[9] = new SqlParameter("@_Type", functions.Type);
                pars[10] = new SqlParameter("@_Ussd", functions.Ussd);
                pars[11] = new SqlParameter("@_ParrentName", functions.ParrentName);
                pars[12] = new SqlParameter("@_Percent", functions.Percent);
                pars[13] = new SqlParameter("@_PercentParrent", functions.PercentParrent);
                pars[14] = new SqlParameter("@_CreatedDate", functions.CreatedDate);
                pars[15] = new SqlParameter("@_CardSerial", functions.CardSerial);
                pars[16] = new SqlParameter("@_CardCode", functions.CardCode);
                pars[17] = new SqlParameter("@_Priority", functions.Priority);
                pars[18] = new SqlParameter("@_BidFee", functions.BidFee.GetValueOrDefault());
                pars[19] = new SqlParameter("@_C1Name", functions.C1Name);
                pars[20] = new SqlParameter("@_PercentC1", functions.PercentC1);
                pars[21] = new SqlParameter("@_PercentRoot", functions.PercentRoot);
                pars[22] = new SqlParameter("@_BidRate", functions.BidRate);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_OrderReportHistory_Add", pars);
                return Convert.ToInt64(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }

        public int UpdateBid(long id, decimal bidRate, int bidFeeHold)
        {
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_Id", id);
                pars[1] = new SqlParameter("@_BidRate", bidRate);
                pars[2] = new SqlParameter("@_BidFeeHold", bidFeeHold);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_OrderReport_UpdateBid", pars);
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

