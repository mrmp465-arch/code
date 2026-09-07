using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Card.Data.Service;
using Card.Data.DTO;
using Card.Utility;
using System.Linq;
using Card.Data.Api;

namespace Card.Data.Service
{
    public class OrderTempsService : IOrderTempsService
    {

        public OrderInput Get(int Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<OrderInput>("SP_OrderTemp_Get",
                                                                                                   new SqlParameter("@Id", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new OrderInput();
            }
        }

      
       
        public List<OrderInput> GetFilter(string username)
        {
            var select = "*";
            var order = "Id DESC";
            var where = "";
           
            if (!string.IsNullOrEmpty(username))

            {
                where += " CreateUser  ='" + username + "' ";
            }

            return GetList(select, where, order);
        }
        public List<OrderInput> GetList(string select, string where, string order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                return new DBHelper(Config.MainConnectionString).GetListSP<OrderInput>("SP_OrderTemp_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<OrderInput>();
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
        public int InsertUpdate(OrderInput functions)
        {
            try
            {
                var pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@_Id", functions.Id);
                pars[1] = new SqlParameter("@_Mobile", functions.Mobile);
                pars[2] = new SqlParameter("@_FullName", functions.FullName);
                pars[4] = new SqlParameter("@_OrderNo", functions.OrderNo);
                pars[5] = new SqlParameter("@_Telco", functions.Telco);
                pars[6] = new SqlParameter("@_TopupType", functions.TopupType);
                pars[7] = new SqlParameter("@_Amount", functions.Amount);
                pars[8] = new SqlParameter("@_AccountName", functions.AccountName);
                pars[9] = new SqlParameter("@_AmountMin", functions.AmountMin);
                pars[10] = new SqlParameter("@_AmountMinAll", functions.AmountMinAll);
                pars[11] = new SqlParameter("@_Priority", functions.Priority);
                pars[12] = new SqlParameter("@_CreateUser", functions.CreateUser);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_OrderTemp_Update", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }


        /// <summary>
        /// Xóa OrderTemps
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        public int Delete(int functionId)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Id", functionId);
                pars[1] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_OrderTemp_Delete", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
      

    }
}

