using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SMS.Data.Service;
using SMS.Data.DTO;
using SMS.Utility;



namespace SMS.Data.Service
{
	public class TransactionService : ITransactionsService
    {

        public List<Transactions> GetList(string Username,int Type, int pageNumber, int pageSize, ref int TotalRecord)
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
                var list = new DBHelper(Config.MainConnectionString).GetListSP<Transactions>("sp_Transaction_SelectPagedDynamic", pars);
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

        public int Topup(string UserName, int Amount, string Description)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_Description", Description);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[3] = new SqlParameter("@_Note", "");
                pars[5] = new SqlParameter("@_ClientIP", Config.GetIP());
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
        public int Deduct(string UserName, int Amount, string Description)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_Description", Description);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[3] = new SqlParameter("@_Note", "");
                pars[5] = new SqlParameter("@_ClientIP", Config.GetIP());
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
        public int TopupHold(string UserName, int Amount, string Description)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_Description", Description);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[3] = new SqlParameter("@_Note", "");
                pars[5] = new SqlParameter("@_ClientIP", Config.GetIP());
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
        public int DeductHold(string UserName, int Amount, string Description)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_Description", Description);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[3] = new SqlParameter("@_Note", "");
                pars[5] = new SqlParameter("@_ClientIP", Config.GetIP());
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
    }
}

