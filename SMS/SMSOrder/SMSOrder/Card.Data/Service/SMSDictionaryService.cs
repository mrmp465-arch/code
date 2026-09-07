using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SMS.Data.Service;
using SMS.Data.DTO;
using SMS.Utility;
using System.Linq;

namespace SMS.Data.Service
{
    public class SMSDictionaryService : ISMSDictionaryService
    {

        public SMSDictionary Get(int Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<SMSDictionary>("SP_SMSDictionary_Get",
                                                                                                   new SqlParameter("@Id", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new SMSDictionary();
            }
        }


        public List<SMSDictionary> GetAll(string CreatedUser)
        {
            var select = " *";
            var order = "[Key] ASC";
            var where = "";
            if(!string.IsNullOrEmpty(CreatedUser))
            {
                where = "CreatedUser='"+ CreatedUser+ "' Or CreatedUser='Admin'";
            }
            return GetList(select, where, order);
        }
       
        public List<SMSDictionary> GetList(string select, string where, string order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                return new DBHelper(Config.MainConnectionString).GetListSP<SMSDictionary>("SP_SMSDictionary_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<SMSDictionary>();
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
        public int InsertUpdate(SMSDictionary functions)
        {
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_Id", functions.Id);
                pars[1] = new SqlParameter("@_Key", functions.Key);
                pars[2] = new SqlParameter("@_Value", functions.Value);
                pars[4] = new SqlParameter("@_CreatedUser", functions.CreatedUser);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_SMSDictionary_Update", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }


        public int Delete(int functionId)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_Id", functionId);
                pars[1] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_SMSDictionary_Delete", pars);
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

