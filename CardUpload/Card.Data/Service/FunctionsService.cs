using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Card.Data.Service;
using Card.Data.DTO;
using Card.Utility;



namespace Card.Data.Service
{
	public class FunctionsService : IFucntionsService
	{

        public Functions GetFunctionByFunctionCode(string code)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Functions>("SP_Functions_GetByFunctionCode",
                                                                                                   new SqlParameter("@_FunctionCode", code));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Functions();
            }
        }


        /// <summary>
        /// Lấy một Function theo FunctionID
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        public Functions GetFunctionByFunctionID(int functionId)
		{
			try
			{
				return new DBHelper(Config.MainConnectionString).GetInstanceSP<Functions>("SP_Functions_GetByFunctionID",
																								   new SqlParameter("@_FunctionID", functionId));
			}
			catch (Exception ex)
			{
                 NLogLogger.PublishException(ex);
				return new Functions();
			}
		}
        public List<Functions> GetListFunctionBySystemID(int systemId)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetListSP<Functions>("SP_Functions_SelectSystemID",
                                                                                                   new SqlParameter("@_SystemID", systemId));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<Functions>();
            }
        }

        /// <summary>
        /// Lấy danh sách funtion theo UserID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Functions> GetListFunctionByUserID(int userId)
		{
			try
			{

                return new DBHelper(Config.MainConnectionString).GetListSP<Functions>("sp_Functions_SelectByUserID",
																								   new SqlParameter("@_UserID", userId));
			}
			catch (Exception ex)
			{
                 NLogLogger.PublishException(ex);
				return new List<Functions>();
			}
		}

		/// <summary>
		/// Lây danh sách Fucntion theo các tham số truyền vào, Có phân trang
		/// </summary>
		/// <param name="functionName"></param>
		/// <param name="isAcitve"></param>
		/// <param name="systemId"></param>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
        public List<Functions> GetListFunctions(string Keyword, int isAcitve, int pageNumber, int pageSize, ref int TotalRecord)
        {
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_Keyword", string.IsNullOrEmpty(Keyword) ? string.Empty : Keyword);
                pars[1] = new SqlParameter("@_Status", isAcitve);
                pars[2] = new SqlParameter("@_CurrPage", pageNumber);
                pars[3] = new SqlParameter("@_RecordPerPage", pageSize);
                pars[4] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var list = new DBHelper(Config.MainConnectionString).GetListSP<Functions>("SP_Functions_GetPage", pars);
                TotalRecord = Convert.ToInt32(pars[4].Value);
                return list;
            }
            catch (Exception ex)
            {
                 NLogLogger.PublishException(ex);
                TotalRecord = 0;
                return new List<Functions>();
            }
        }

        public List<Functions> GetListFunctionsByFather(int FatherID)
        {
            try
            {
                var list = new DBHelper(Config.MainConnectionString).GetListSP<Functions>("SP_Functions_GetFather", 
                    new SqlParameter("@_FatherID", FatherID));
                if(list == null || list.Count <= 0)
                    return new List<Functions>();
                return list;
            }
            catch (Exception ex)
            {
                 NLogLogger.PublishException(ex);
                return new List<Functions>();
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
		public int InsertUpdateFunction(Functions functions)
		{
			try
			{
				var pars = new SqlParameter[11];
				pars[0] = new SqlParameter("@_FunctionID", functions.FunctionID);
				pars[1] = new SqlParameter("@_FunctionName", functions.FunctionName);
				pars[2] = new SqlParameter("@_Url", functions.Url);
				pars[3] = new SqlParameter("@_UrlDisplay", functions.UrlDisplay);
				pars[4] = new SqlParameter("@_IsDisplay", functions.IsDisplay);
				pars[5] = new SqlParameter("@_IsActive", functions.IsActive);
				pars[6] = new SqlParameter("@_FatherID", functions.FatherID);
				pars[7] = new SqlParameter("@_Order", functions.Order);
                pars[8] = new SqlParameter("@_IconId", functions.IconId);
                pars[9] = new SqlParameter("@_FunctionCode", functions.FunctionCode);
                pars[10] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Functions_Update", pars);
				return Convert.ToInt32(pars[10].Value);
			}
			catch (Exception ex)
			{
                 NLogLogger.PublishException(ex);
				return -99;
			}
		}


		/// <summary>
		/// Xóa Functions
		/// </summary>
		/// <param name="functionId"></param>
		/// <returns></returns>
		public int DelleteFunction(int functionId)
		{
			try
			{
				var pars = new SqlParameter[2];
				pars[0] = new SqlParameter("@_FunctionID", functionId);
				pars[1] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Functions_Delete", pars);
				return Convert.ToInt32(pars[1].Value);
			}
			catch (Exception ex)
			{
                 NLogLogger.PublishException(ex);
				return -99;
			}
		}

        public List<Functions> SelectAllFunctionID(int fatherID, string name, int isactive, int isdisplay)
        {
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_FatherID", fatherID);
                pars[1] = new SqlParameter("@_FunctionName", name);
                pars[2] = new SqlParameter("@_IsActive", isactive);
                pars[3] = new SqlParameter("@_IsDisplay", isdisplay);
                var result = new DBHelper(Config.MainConnectionString).GetListSP<Functions>("SP_Functions_SelectByCondition", pars);
                return result;
            }
            catch (Exception ex)
            {
                 NLogLogger.PublishException(ex);
                return new List<Functions>();
            }

        }

        public int UpdateOrder(int FunctionID, int ParentID, int Order)
        {
            try
            {
                var pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@FunctionID", FunctionID);
                pars[1] = new SqlParameter("@FatherID", ParentID);
                pars[2] = new SqlParameter("@Order", Order);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Function_UpdateSortOrder", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                 NLogLogger.PublishException(ex);
                return -99;
            }
        }
	}
}

