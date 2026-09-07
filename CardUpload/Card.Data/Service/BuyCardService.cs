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
    public class BuyCardService : IBuyCardService
    {
        
        public List<BuyCardReport> GetReportDaily( string parentname, string username,string telco,string fromdate, string todate)
        {
            try
            {
                var select = "CONVERT(VARCHAR(8),   CreatedDate, 1) as Data,  Count([Id]) as TotalTrans,  SUM(Money) as TotalRevenue, SUM([MoneyReward]) as TotalReward";
               
                var orderBy = "CONVERT(VARCHAR(8),   CreatedDate, 1) ASC";
                var groupBy = "CONVERT(VARCHAR(8),   CreatedDate, 1)";


                var where = "Status>0";
                if (!string.IsNullOrEmpty(telco))
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " [Telco]  ='" + telco + "' ";
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
            
            var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", orderBy);
                pars[3] = new SqlParameter("@GroupbByExpression", groupBy);
                return new DBHelper(Config.MainConnectionString).GetListSP<BuyCardReport>("SP_BuyCard_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<BuyCardReport>();
            }
        }
        public BuyCard GetDetail(long Id)
        {
            var select = "Top 1 *";
            var order = "Id DESC";
            var where = "Id="+Id;
            return GetFilter(select, where, order).FirstOrDefault();
        }
         public List<BuyCard> GetList(int top,string parentname,string username, string telco,string cardseri,int status,long? id,string fromdate,string todate)
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
            if (!string.IsNullOrEmpty(telco))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [Telco]  ='" + telco + "' ";
            }
            if (!string.IsNullOrEmpty(cardseri))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [CardData] like N'%" + cardseri + "%' ";
               
            }
            if(id.HasValue)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";


                where += "Id=" + id.GetValueOrDefault();
            }
            else
            {
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
            }
         
            if (status > -1000)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                if(status==1)
                where += " Status=1" ;
                if (status == -1)
                    where += " Status<1";
            }
            return GetFilter(select, where, order);
        }
        public List<BuyCard> GetFilter(string select, string where, string order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
               // pars[3] = new SqlParameter("@GroupbByExpression", group);
                
                return new DBHelper(Config.MainConnectionString).GetListSP<BuyCard>("SP_BuyCard_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<BuyCard>();
            }
        }
        
        public int UpdateDynamic(string where, string updatest)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@UpdateCondition", updatest);
                pars[1] = new SqlParameter("@WhereCondition", where);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_BuyCard_UpdateDynamic", pars);
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
        public long Add(BuyCard functions)
        {
            try
            {
                var pars = new SqlParameter[11];
                pars[0] = new SqlParameter("@_Type", functions.Type);
                pars[1] = new SqlParameter("@_CardNumber", functions.CardNumber);
                pars[2] = new SqlParameter("@_Fee", functions.Fee);
                pars[4] = new SqlParameter("@_CardValue", functions.CardValue);
                pars[5] = new SqlParameter("@_Telco", functions.Telco);
                pars[6] = new SqlParameter("@_Reward", functions.Reward);
                pars[7] = new SqlParameter("@_Amount", functions.Amount);
                pars[8] = new SqlParameter("@_UserName", functions.UserName);
                pars[9] = new SqlParameter("@_ParrentName", functions.ParrentName);
                pars[10] = new SqlParameter("@_RefCode", functions.RefCode);
                //pars[11] = new SqlParameter("@_MoneyReward", functions.MoneyReward);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_BuyCard_Add", pars);
                return Convert.ToInt64(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }





    }
}

