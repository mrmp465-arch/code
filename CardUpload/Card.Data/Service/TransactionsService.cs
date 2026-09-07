using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Card.Data.Service;
using Card.Data.DTO;
using Card.Utility;
using System.Globalization;

namespace Card.Data.Service
{
	public class TransactionService : ITransactionsService
    {
        public List<Transactions> GetListHold(string Username, int Type, int pageNumber, int pageSize, ref int TotalRecord)
        {
            try
            {
                var orderby = "Id DESC";
                var select = " *";
                var where = "";
                if (!string.IsNullOrEmpty(Username))
                {

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Username =" + "'" + Username + "'";


                }
                if (Type > -1)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " Type=" + Type;
                }
                var pars = new SqlParameter[6];
                pars[5] = new SqlParameter("@SelectQuery", select);
                pars[0] = new SqlParameter("@WhereCondition ", where);
                pars[1] = new SqlParameter("@OrderByExpression", orderby);
                pars[2] = new SqlParameter("@PageIndex", pageNumber);
                pars[3] = new SqlParameter("@PageSize", pageSize);
                pars[4] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var list = new DBHelper(Config.MainConnectionString).GetListSP<Transactions>("sp_TransactionHold_SelectPagedDynamic", pars);
                TotalRecord = Convert.ToInt32(pars[4].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                TotalRecord = 0;
                return new List<Transactions>();
            }
        }
        public List<Transactions> GetList(string Username,int Type, int pageNumber, int pageSize, ref int TotalRecord, ref long TotalDeduct, ref long TotalTopup, int ActionType=-1,string ReferenceId="",string keyword= "", string FromDate="", string ToDate="")




        {
            try
            {
                var orderby = "Id DESC";
                var select = " *";
                var where = "";
                if (!string.IsNullOrEmpty(Username))
                {

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Username =" + "'" + Username + "'";


                }
                if (Type > -1)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " Type=" + Type;
                }
                if (ActionType > -1)
                {
                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";
                    where += " ActionType=" + ActionType;
                }
                if (!string.IsNullOrEmpty(ReferenceId))
                {

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " ReferenceId =" + "'" + ReferenceId + "'";


                }
                if (!string.IsNullOrEmpty(keyword))
                {

                    if (!string.IsNullOrEmpty(where))
                        where += " AND ";

                    where += " Description like N'%" + keyword + "%' ";


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
                        " (convert(nvarchar(23),Time,121) between '" + _FormDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' and '" + _ToDate.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')";

                }
                var pars = new SqlParameter[6];
                pars[5] = new SqlParameter("@SelectQuery", select);
                pars[0] = new SqlParameter("@WhereCondition ", where);
                pars[1] = new SqlParameter("@OrderByExpression", orderby);
                pars[2] = new SqlParameter("@PageIndex", pageNumber);
                pars[3] = new SqlParameter("@PageSize", pageSize);
                pars[4] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
               // pars[6] = new SqlParameter("@TotalDeduct", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                //pars[7] = new SqlParameter("@TotalTopup", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
                var list = new DBHelper(Config.MainConnectionString).GetListSP<Transactions>("sp_Transaction_SelectPagedDynamic", pars);
                TotalRecord = Convert.ToInt32(pars[4].Value);
                //Int64.TryParse(pars[6].Value.ToString(), out TotalDeduct);
                //Int64.TryParse(pars[7].Value.ToString(), out TotalTopup);
                //TotalDeduct = Convert.ToInt64(pars[6].Value);
                //TotalTopup = Convert.ToInt64(pars[7].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                TotalRecord = 0;
                return new List<Transactions>();
            }
        }

        public int Topup(string UserName, int Amount, string Description, string ReferenceId = "",int ActionType=2)
        {
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_Description", Description);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[6] = new SqlParameter("@_Note", "");
                pars[3] = new SqlParameter("@_ActionType", ActionType);
                pars[5] = new SqlParameter("@_ReferenceId", ReferenceId);
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Transaction_Topup", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int Deduct(string UserName, int Amount, string Description, string ReferenceId = "")
        {
            if (Amount <= 0)
                return -99;
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_Description", Description);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[3] = new SqlParameter("@_Note", "");
                pars[5] = new SqlParameter("@_ReferenceId", ReferenceId);
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Transaction_Deduct", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int TopupHold(string UserName, int Amount, string Description,string ReferenceId="")
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_Description", Description);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[3] = new SqlParameter("@_Note", "");
                pars[5] = new SqlParameter("@_ReferenceId", ReferenceId);
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Transaction_TopupHold", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int DeductHold(string UserName, int Amount, string Description,string ReferenceId="")
        {
            if (Amount <= 0)
                return -99;
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_Description", Description);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[3] = new SqlParameter("@_Note", "");
                pars[5] = new SqlParameter("@_ReferenceId", ReferenceId);
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Transaction_DeductHold", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int BuyCard(string UserName, string ParrentName, int Money, int MoneyReward, string Telco, int CardValue,int CardNumber,long RefCode)
        {
            try
            {
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_ParrentName", ParrentName);
                pars[2] = new SqlParameter("@_Money", Money);
                pars[3] = new SqlParameter("@_MoneyReward", MoneyReward);
                pars[6] = new SqlParameter("@_Telco", Telco);
                pars[7] = new SqlParameter("@_CardValue", CardValue);
                pars[8] = new SqlParameter("@_CardNumber", CardNumber);
                pars[9] = new SqlParameter("@_RefCode", RefCode);
                pars[5] = new SqlParameter("@_ClientIP", Config.GetIP());
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Transaction_BuyCard", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int Confrim(string UserName, string ParrentName, int AmountParrent, int AmountSuccess, int AmountTopupHold, string ReferenceId)
        {
            try
            {
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_ParrentName", ParrentName);
                pars[2] = new SqlParameter("@_AmountParrent", AmountParrent);
                pars[3] = new SqlParameter("@_AmountSuccess", AmountSuccess);
                pars[6] = new SqlParameter("@_AmountTopupHold", AmountTopupHold);
                pars[7] = new SqlParameter("@_ReferenceId", ReferenceId);
                pars[5] = new SqlParameter("@_ClientIP", Config.GetIP());
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Transaction_Confirm", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int DeductBid(string UserName, int AmountSuccess,long OrderId,string Description,string Note)
        {
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_AmountSuccess", AmountSuccess);
                pars[4] = new SqlParameter("@_ReferenceId", OrderId.ToString());
                pars[5] = new SqlParameter("@_Description", Description);
                pars[6] = new SqlParameter("@_Note", Note);
                pars[2] = new SqlParameter("@_OrderId", OrderId);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Transaction_DeductBidV2", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int DeductCard(string UserName, int AmountSuccess, long OrderId, string Description, string Note)
        {
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_AmountSuccess", AmountSuccess);
                pars[4] = new SqlParameter("@_ReferenceId", OrderId.ToString());
                pars[5] = new SqlParameter("@_Description", Description);
                pars[6] = new SqlParameter("@_Note", Note);
                pars[2] = new SqlParameter("@_OrderId", OrderId);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Transaction_DeductCard", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
    }
}

