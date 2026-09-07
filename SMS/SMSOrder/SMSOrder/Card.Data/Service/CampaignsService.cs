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
    public class CampaignsService : ICampaignsService
    {

        public Campaigns Get(int Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Campaigns>("SP_Campaign_Get",
                                                                                                   new SqlParameter("@Id", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Campaigns();
            }
        }
        public Campaigns Get(string name)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Campaigns>("SP_Campaign_GetByName",
                                                                                                   new SqlParameter("@name", name));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Campaigns();
            }
        }
        /// <summary>
        /// chỉ campaign khởi tạo mới xóa
        /// </summary>
        /// <param name="functionId"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        public int Delete(int functionId,string Username)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@Id", functionId);
                pars[2] = new SqlParameter("@UserName", Username);
                pars[1] = new SqlParameter("@ErrorCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Campaign_Delete", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        /// <summary>
        /// chỉ campaign khởi tạo
        /// </summary>
        /// <param name="functionId"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        public int Send(int functionId, string Username)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@Id", functionId);
                pars[2] = new SqlParameter("@UserName", Username);
                pars[1] = new SqlParameter("@ErrorCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Campaign_Send", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        /// <summary>
        /// chỉ campaign khởi tạo hoặc chờ gửi
        /// </summary>
        /// <param name="functionId"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        public int Lock(int functionId, string Username)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@Id", functionId);
                pars[2] = new SqlParameter("@UserName", Username);
                pars[1] = new SqlParameter("@ErrorCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Campaign_Lock", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int UnLock(int functionId, string Username)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@Id", functionId);
                pars[2] = new SqlParameter("@UserName", Username);
                pars[1] = new SqlParameter("@ErrorCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Campaign_UnLock", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int Confirm(int functionId, string Username)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@Id", functionId);
                pars[2] = new SqlParameter("@UserName", Username);
                pars[1] = new SqlParameter("@ErrorCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Campaign_Confirm", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public List<Campaigns> GetFilter( string keyword, string createUser, int page, int pageSize, ref int total)
        {
            var select = "*";
            var order = "Id DESC";
            var where = "";
           
            if (!string.IsNullOrEmpty(createUser))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " [CreatedUser]='" + createUser + "'";
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                if (!string.IsNullOrEmpty(where))
                    where += " AND ";
                where += " (Name LIKE N'%" + keyword + "%' ) ";
            }

            return GetList(select, where, order, page, pageSize, ref total);
        }
        public List<Campaigns> GetList(string select, string where, string order, int page, int pageSize, ref int total)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@SelectQuery", select);
                pars[1] = new SqlParameter("@WhereCondition", where);
                pars[2] = new SqlParameter("@OrderByExpression", order);
                pars[3] = new SqlParameter("@PageIndex", page);
                pars[4] = new SqlParameter("@PageSize", pageSize);
                pars[5] = new SqlParameter("@TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var data = new DBHelper(Config.MainConnectionString).GetListSP<Campaigns>("sp_Campaign_SelectPagedDynamic", pars);
                total = Convert.ToInt32(pars[5].Value);
                return data;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                total = 0;
                return new List<Campaigns>();
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
        public int InsertUpdate(Campaigns functions)
        {
            try
            {
                var pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@_Id", functions.Id);
                pars[1] = new SqlParameter("@_Name", functions.Name+"");
                pars[2] = new SqlParameter("@_Contents", functions.Contents);
                pars[3] = new SqlParameter("@_StartTime", functions.StartTime);
                pars[4] = new SqlParameter("@_CreatedUser", functions.CreatedUser);
                pars[5] = new SqlParameter("@_Group", functions.Group+"");
              
                pars[6] = new SqlParameter("@_Confirm", functions.Confirm);
                pars[7] = new SqlParameter("@_Type", functions.Type);
                pars[8] = new SqlParameter("@_Status", functions.Status);
                
                pars[9] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Campaign_Update", pars);
                return Convert.ToInt32(pars[9].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int SetData(int id,int delete,string username,int priority)
        {
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_Id", id);
                pars[1] = new SqlParameter("@_Delete", delete);
                pars[3] = new SqlParameter("@_CreatedUser", username);
                pars[4] = new SqlParameter("@_Priority", priority);
                pars[2] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Campaign_SetData", pars);
                return Convert.ToInt32(pars[2].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public string Preview(string group, string content)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_Groups", group);
                pars[1] = new SqlParameter("@_Contents", content);
                pars[2] = new SqlParameter("@_SMSContent", SqlDbType.NVarChar,4000) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Campaign_PreviewData", pars);
                return pars[2].Value.ToString();
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return "";
            }
        }



        public int DeleteDynamic(string where)
        {
            try
            {
                var pars = new SqlParameter[1];
                pars[0] = new SqlParameter("@WhereCondition", where);

                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_Campaign_DeleteDynamic", pars);
                return 1;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int UpdateDynamic(string where, string updatest)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@UpdateCondition", updatest);
                pars[1] = new SqlParameter("@WhereCondition", where);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Campaign_UpdateDynamic", pars);
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

