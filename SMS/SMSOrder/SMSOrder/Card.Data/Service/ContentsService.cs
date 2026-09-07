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
    public class CommentsService : ICommentsService
    {

        public Comments Get(int Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Comments>("SP_Comment_Get",
                                                                                                   new SqlParameter("@Id", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Comments();
            }
        }

      
        public List<Comments> GetTop(int top,int NewsId)
        {
            var select = "TOP(" + top + ") *";
            var order = " newid() ";
            var where = "Status=1 And NewsId="+ NewsId;
            return GetList(select, where, order);
        }
        public List<Comments> GetFilter(int status, string keyword)
        {
            var select = "*";
            var order = "PublishDate DESC";
            var where = "";
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
                where += " (Title LIKE N'%" + keyword + "%' OR CreatedUser LIKE N'%" + keyword + "%' ) ";
            }

            return GetList(select, where, order);
        }
        public List<Comments> GetList(string select, string where, string order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                return new DBHelper(Config.MainConnectionString).GetListSP<Comments>("SP_Comment_SelectDynamic", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<Comments>();
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
        public int InsertUpdate(Comments functions)
        {
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_Id", functions.Id);
                pars[1] = new SqlParameter("@_Title", functions.Title);
                pars[2] = new SqlParameter("@_Description", functions.Description);
                pars[4] = new SqlParameter("@_Status", functions.Status);
                pars[5] = new SqlParameter("@_CreatedUser", functions.CreatedUser);
                pars[6] = new SqlParameter("@_NewsId", functions.NewsId);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Comment_Update", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }


        /// <summary>
        /// Xóa Comments
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
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Comment_Delete", pars);
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
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Comment_UpdateDynamic", pars);
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

