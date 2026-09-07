using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Card.Data.Service;
using Card.Data.DTO;
using Card.Utility;
using System.Linq;

namespace Card.Data.Service
{
    public class ContentsService : IContentsService
    {

        public Contents Get(int Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Contents>("SP_Content_Get",
                                                                                                   new SqlParameter("@Id", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Contents();
            }
        }

        public Contents GetHot()
        {
            var select = "*";
            var order = "PublishDate DESC";
            var where = "Status=1 And IsHot=1";
            return GetList(select, where, order).FirstOrDefault();
        }
        public List<Contents> GetTop(int top, string usename)
        {
            var select = "TOP(" + top + ") *";
            var order = "IsHot DESC, PublishDate DESC";
            var where = "Status=1 AND "+ " [CreatedUser]='" + usename + "'";
            return GetList(select, where, order);
        }
        public List<Contents> GetFilter(int status, string keyword,string usename)
        {
            var select = "*";
            var order = "PublishDate DESC";
            var where = "[CreatedUser]='"+ usename + "'";
            if (status > 0)
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";

                where += " Status=" + status.ToString();
            }
            if (!string.IsNullOrEmpty(keyword))

            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " Title LIKE N'%" + keyword + "%' ";
            }

            return GetList(select, where, order);
        }
        public List<Contents> GetList(string select, string where, string order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                return new DBHelper(Config.MainConnectionString).GetListSP<Contents>("SP_Content_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<Contents>();
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
        public int InsertUpdate(Contents functions)
        {
            try
            {
                var pars = new SqlParameter[9];
                pars[0] = new SqlParameter("@_Id", functions.Id);
                pars[1] = new SqlParameter("@_Title", functions.Title);
                pars[2] = new SqlParameter("@_Description", functions.Description);
                pars[4] = new SqlParameter("@_Content", functions.Content);
                pars[5] = new SqlParameter("@_IsHot", functions.IsHot);
                pars[6] = new SqlParameter("@_Status", functions.Status);
                pars[7] = new SqlParameter("@_PublishDate", functions.PublishDate);
                pars[8] = new SqlParameter("@_CreatedUser", functions.CreatedUser);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Content_Update", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }


        /// <summary>
        /// Xóa Contents
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
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Content_Delete", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int UpdateUserDynamic(string where, string updatest)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@UpdateCondition", updatest);
                pars[1] = new SqlParameter("@WhereCondition", where);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Content_UpdateDynamic", pars);
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

