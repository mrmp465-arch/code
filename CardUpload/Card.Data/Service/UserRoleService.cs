using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Card.Data.Service;
using Card.Data.DTO;
using Card.Utility;


namespace Card.Data.Service
{
    public class UserRoleService : IUserRoleService
    {
       
        public UserFunction CheckPermission(int UserID, int FunctionID)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_UserID", UserID);
                pars[1] = new SqlParameter("@_FunctionID", FunctionID);
                var result = new DBHelper(Config.MainConnectionString).GetInstanceSP<UserFunction>("SP_CheckPermissionForUser", pars);
                return result;
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return null;
            }
        }
        public int UserFunctionInsert(UserFunction RoleFunction)
        {
            try
            {
                var pars = new SqlParameter[8];
                pars[0] = new SqlParameter("@_UserID", RoleFunction.UserID);
                pars[1] = new SqlParameter("@_FunctionID", RoleFunction.FunctionID);
                pars[2] = new SqlParameter("@_IsInsert", RoleFunction.IsInsert);
                pars[3] = new SqlParameter("@_IsUpdate", RoleFunction.IsUpdate);
                pars[4] = new SqlParameter("@_IsDelete", RoleFunction.IsDelete);
                pars[5] = new SqlParameter("@_FunctionCode", RoleFunction.FunctionCode);
                pars[6] = new SqlParameter("@_IsFullControl", RoleFunction.IsFullControl);
                pars[7] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_UserFunctions_Insert", pars);
                return Convert.ToInt32(pars[7].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int UserFunctionInsertList(int UserID, string ListRole)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_UserID", UserID);
                pars[1] = new SqlParameter("@_UserFunctionData", ListRole);
                pars[2] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_UserFunctions_InsertList", pars);
                return Convert.ToInt32(pars[2].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int GroupFunctionInsertList(int UserID, string ListRole)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_UserID", UserID);
                pars[1] = new SqlParameter("@_UserFunctionData", ListRole);
                pars[2] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_GroupFunctions_InsertList", pars);
                return Convert.ToInt32(pars[2].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int UserFunctionInsertListV2(int UserID, string ListRole)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_UserID", UserID);
                pars[1] = new SqlParameter("@_UserFunctionData", ListRole);
                pars[2] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_UserFunctions_InsertListV2", pars);
                return Convert.ToInt32(pars[2].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int UserFunctionDelete(int UserID, int FunctionID)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@_UserID", UserID);
                pars[1] = new SqlParameter("@_FunctionID", FunctionID);
                pars[2] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_UserFunctions_Delete", pars);
                return Convert.ToInt32(pars[2].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int UserFunctionDeleteAll(int UserID)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_UserID", UserID);
                pars[1] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_UserFunctions_DeleteAll_ByUserID", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public List<UserFunction> UserFunction_GetByUserID(int UserID)
        {
            try
            {
                var list = new DBHelper(Config.MainConnectionString).GetListSP<UserFunction>("SP_UserFunctions_GetByUserID",
                    new SqlParameter("@_UserID", UserID));
                if (list == null || list.Count <= 0)
                    return new List<UserFunction>();
                return list;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<UserFunction>();
            }
        }
        public List<UserFunction> GroupFunction_GetByID(int id)
        {
            try
            {
                var list = new DBHelper(Config.MainConnectionString).GetListSP<UserFunction>("[SP_GroupFunctions_GetByID]",
                    new SqlParameter("@_GroupID", id));
                if (list == null || list.Count <= 0)
                    return new List<UserFunction>();
                return list;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<UserFunction>();
            }
        }
        /// <summary>
        /// Lấy danh sách funtion theo Group
        /// </summary>
        /// <param name=")"></param>
        /// <returns></returns>
        public List<Functions> GetListFunctionByID(int id)
        {
            try
            {

                return new DBHelper(Config.MainConnectionString).GetListSP<Functions>("sp_Functions_SelectByGroupID",
                                                                                                   new SqlParameter("@_ID", id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<Functions>();
            }
        }
    }
}
