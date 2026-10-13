using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CMS.Data.Service;
using CMS.Data.DTO;
using CMS.Utility;


namespace CMS.Data.Service
{
    public class UsersLogService : IUsersLogService
    {
        /// <summary>
        /// Ghi log
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="userId"></param>
        /// <param name="functionId"></param>
        /// <param name="desription"></param>
        /// <param name="paygateName"></param>
        /// <returns></returns>
        public int InsertUsersLog(UsersLog log)
        {
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_UserID", log.UserID);
                pars[1] = new SqlParameter("@_FunctionCode", log.FunctionCode);
                pars[2] = new SqlParameter("@_Description", log.Description);
                pars[3] = new SqlParameter("@_LogType", log.LogType);
                pars[4] = new SqlParameter("@_ClientIP", log.ClientIP);
                pars[5] = new SqlParameter("@_UserName", log.UserName);
                pars[6] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_UserLogs_Insert", pars);
                return Convert.ToInt32(pars[6].Value);
            }
            catch (Exception ex)
            {
                 NLogLogger.PublishException(ex);
                return -99;
            }
        }


        /// <summary>
        /// Lấy danh sách UserLog
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userId"></param>
        /// <param name="functionId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<UsersLog> GetListUsersLog(string fromDate, string toDate, int userId, string functioncode, string keyword,int pageNumber, int pageSize, ref int totalrecord)
        {
            try
            {
                var pars = new SqlParameter[8];
                pars[0] = !string.IsNullOrEmpty(fromDate)
                               ? new SqlParameter("@_Fromdate", fromDate)
                               : new SqlParameter("@_Fromdate", DBNull.Value);
                pars[1] = !string.IsNullOrEmpty(toDate)
                              ? new SqlParameter("@_Todate", toDate)
                              : new SqlParameter("@_Todate", DBNull.Value);
                pars[2] = new SqlParameter("@_UserID", userId);
                pars[3] = new SqlParameter("@_FunctionCode", functioncode);
                pars[7] = new SqlParameter("@_KeyWord", keyword);

                pars[4] = new SqlParameter("@_CurrPage", pageNumber);
                pars[5] = new SqlParameter("@_RecordPerPage", pageSize);
                pars[6] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };

                var list_result = new DBHelper(Config.MainConnectionString).GetListSP<UsersLog>("SP_UserLogs_GetPages", pars);
                totalrecord = Convert.ToInt32(pars[6].Value);
                if (list_result == null || list_result.Count <= 0)
                    return new List<UsersLog>();
                return list_result;
            }
            catch (Exception ex)
            {
                 NLogLogger.PublishException(ex);
                totalrecord = 0;
                return new List<UsersLog>();
            }
        }



        /// <summary>
        /// Xóa UserLog
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userId"></param>
        /// <param name="functionId"></param>
        /// <param name="paygateName"></param>
        /// <returns></returns>
        public int DeleteUsersLog(string fromDate, string toDate, int userId, int functionId)
        {
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_Fromdate", fromDate);
                pars[1] = new SqlParameter("@_Todate", toDate);
                pars[2] = new SqlParameter("@_UserID", userId);
                pars[3] = new SqlParameter("@_FunctionID", functionId);
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_UserLogs_Delete", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception ex)
            {
                 NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public UsersLog GetLogOrder(long transId)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<UsersLog>("SP_UserLogs_Get",
                                                                                                 new SqlParameter("@_TransId", transId.ToString()));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new UsersLog();
            }
        }
    }
}
