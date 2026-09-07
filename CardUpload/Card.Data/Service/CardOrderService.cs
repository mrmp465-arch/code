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
    public class CardOrderService : ICardOrderService
    {
        public List<CardOrderReport2> GetReport()
        {
            try
            {
                var select = "Telco, Amount,  Count([Id]) as TotalTrans, SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) AS TotalQueue ";

                var orderBy = "Telco ASC";
                var groupBy = "Telco, Amount";


                var where = "";
                
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupbByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<CardOrderReport2>("SP_CardOrder_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<CardOrderReport2>();
            }
        }
        public List<CardOrderReport> GetReportDaily( string parentname, string username, string orderNo, string fromdate, string todate)
        {
            try
            {
                var select = "CONVERT(VARCHAR(8),   CreatedDate, 1) as Data,  Count([Id]) as TotalTrans, SUM([AmountSuccess]) as TotalAmount, SUM(Money) as TotalRevenue, SUM([MoneyReward]) as TotalReward";
               
                var orderBy = "CONVERT(VARCHAR(8),   CreatedDate, 1) ASC";
                var groupBy = "CONVERT(VARCHAR(8),   CreatedDate, 1)";


                var where = "Status>0";
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
            if (!string.IsNullOrEmpty(parentname))

            {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " ParrentName  ='" + parentname + "' ";
            }
            if (!string.IsNullOrEmpty(username))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " UserName  ='" + username + "' ";
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [OrderNo] like N'%" + orderNo + "%' ";
            }
            var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupbByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<CardOrderReport>("SP_CardOrder_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<CardOrderReport>();
            }
        }
        public List<CardOrderGroup> GetListGroup(int top, string parentname, string username, string orderNo,string telco)
        {
            var select = $"Top {top} OrderNo";
            select +=", MAX(CreatedDate) AS CreatedDate, MAX(UpdateDate) AS UpdateDate";
            select += ", MAX(UserName) AS UserName";
            select += ", MAX(ParrentName) AS ParrentName";
            select += ", COUNT(Id) AS TotalTrans";
            select += " , SUM(CAST(Amount AS BIGINT)) AS TotalAmount";
            select += ", SUM(AmountSuccess) AS TotalAmountSuccess";
            select += ", SUM(Money) AS TotalMoney";
            select += ", SUM(MoneyReward) AS TotalMoneyReward";
            select += " , SUM(CASE WHEN Status > 0 THEN 1 ELSE 0 END) AS TotalSuccess";
            select += ", SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) AS TotalQueue";
            select += "  , SUM(CASE WHEN Status < 0 THEN 1 ELSE 0 END) AS TotalFail";
            select += "  , SUM(CASE WHEN IsConfirm = 1 THEN 1 ELSE 0 END) AS TotalConfirm";
            var order = "CreatedDate DESC";
            var where = "";
            if (!string.IsNullOrEmpty(parentname))

            {
                where += " ParrentName  ='" + parentname + "' ";
            }
            if (!string.IsNullOrEmpty(username))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " UserName  ='" + username + "' ";
            }
            if (!string.IsNullOrEmpty(telco))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " Telco  ='" + telco + "' ";
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [OrderNo] like N'%" + orderNo + "%' ";
            }
            
           
            return GetGroupFilter(select, where, order, "OrderNo");
        }
        public List<CardOrder> GetByOrderNo( string orderNo)
        {
            var select = $" *";
            var order = "Id DESC";
            var where = "";
            
           
            if (!string.IsNullOrEmpty(orderNo))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [OrderNo]  ='" + orderNo + "' ";
            }
            
            return GetFilter(select, where, order);
        }
        public List<CardOrder> GetByRefCode(long refcode)
        {
            var select = $" *";
            var order = "Id DESC";
            var where = "";


            where += " [RefCode]  =" + refcode + " ";

            return GetFilter(select, where, order);
        }
        public List<CardOrder> Search( string orderNo, string cardseri,string telco,int amount)
        {
            var select = $"Top 500 *";
            var order = "Id DESC";
            var where = "";
           
            if (!string.IsNullOrEmpty(orderNo))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [OrderNo] like N'%" + orderNo + "%' ";
            }
            if (!string.IsNullOrEmpty(cardseri))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += "( [CardSerial]  ='" + cardseri + "'  OR";
                where += " [CardCode]  ='" + cardseri + "' )";
            }
            if (!string.IsNullOrEmpty(telco))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " Telco  ='" + telco + "' ";
            }
            if (amount>0)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [Amount]  =" + amount + " ";
            }
            return GetFilter(select, where, order);
        }
        public List<CardOrder> GetList(int top,string parentname,string username, string orderNo,string cardseri,string cardcode,int status=-1000)
        {
            var select = $"Top {top} *";
            var order = "Id DESC";
            var where = "";
            if (!string.IsNullOrEmpty(parentname))

            {
                where += " ParrentName  ='" + parentname + "' ";
            }
            if (!string.IsNullOrEmpty(username))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " UserName  ='" + username + "' ";
            }
            if (!string.IsNullOrEmpty(orderNo))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [OrderNo] like N'%" + orderNo + "%' ";
            }
            if (!string.IsNullOrEmpty(cardseri))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [CardSerial]  ='" + cardseri + "' ";
            }
            if (!string.IsNullOrEmpty(cardcode))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [CardCode]  ='" + cardcode + "' ";
            }
            if (status > -1000)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " Status=" + status;
            }
            return GetFilter(select, where, order);
        }
        public List<CardOrder> GetFilter(string select, string where, string order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
               // pars[3] = new SqlParameter("@GroupbByExpression", group);
                
                return new DBHelper(Config.MainConnectionString).GetListSP<CardOrder>("SP_CardOrder_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<CardOrder>();
            }
        }
        public List<CardOrderGroup> GetGroupFilter(string select, string where, string order, string group)
        {
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                pars[3] = new SqlParameter("@GroupbByExpression", group);

                return new DBHelper(Config.MainConnectionString).GetListSP<CardOrderGroup>("SP_CardOrder_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<CardOrderGroup>();
            }
        }
        public int UpdateDynamic(string where, string updatest)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@UpdateCondition", updatest);
                pars[1] = new SqlParameter("@WhereCondition", where);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_CardOrder_UpdateDynamic", pars);
                return 1;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        /// <summary>
        /// Insert Fucntion
        /// </summary>
        /// <param name="functions"></param>
        /// <returns> >0 : thanh cong
        ///			-1: da ton tai
        ///			-99: loi he thong
        /// </returns>
        public int Add(CardOrder functions)
        {
            try
            {
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_CardCode", functions.CardCode);
                pars[1] = new SqlParameter("@_CardSerial", functions.CardSerial);
                pars[2] = new SqlParameter("@_Fee", functions.Fee);
                pars[4] = new SqlParameter("@_OrderNo", functions.OrderNo);
                pars[5] = new SqlParameter("@_Telco", functions.Telco);
                pars[6] = new SqlParameter("@_Reward", functions.Reward);
                pars[7] = new SqlParameter("@_Amount", functions.Amount);
                pars[8] = new SqlParameter("@_UserName", functions.UserName);
                pars[9] = new SqlParameter("@_ParrentName", functions.ParrentName);

                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_CardOrder_Add", pars);
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

