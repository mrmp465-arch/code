using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SMS.Data.Service;
using SMS.Data.DTO;
using SMS.Utility;



namespace SMS.Data.Service
{
	public class GroupsService : IGroupsService
	{

        public Groups Get(int Id)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Groups>("SP_Group_GetByGroupID",
                                                                                                   new SqlParameter("@GroupID", Id));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Groups();
            }
        }


      
        public List<Groups> GetList()
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetListSP<Groups>("SP_Group_SelectAll");

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<Groups>();
            }
        }
        public Groups GetByName(string name, string username)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@Name", name);
                pars[1] = new SqlParameter("@CreatedUser", username);
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Groups>("SP_Group_GetByName", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return null;
            }
        }
        public List<Groups> GetList(int type,string username)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@Type", type);
                pars[1] = new SqlParameter("@CreatedUser", username);
                return new DBHelper(Config.MainConnectionString).GetListSP<Groups>("SP_Group_GetList", pars);

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<Groups>();
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
        public int InsertUpdate(Groups functions)
		{
			try
			{
				var pars = new SqlParameter[5];
				pars[0] = new SqlParameter("@GroupID", functions.GroupID);
				pars[1] = new SqlParameter("@Name", functions.Name);
				pars[2] = new SqlParameter("@IsActive", functions.IsActive);
                pars[4] = new SqlParameter("@Alias", functions.Alias);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Group_Update", pars);
				return Convert.ToInt32(pars[3].Value);
			}
			catch (Exception ex)
			{
                 NLogLogger.PublishException(ex);
				return -99;
			}
		}


		/// <summary>
		/// Xóa Groups
		/// </summary>
		/// <param name="functionId"></param>
		/// <returns></returns>
		public int Delete(int functionId,string Username)
		{
			try
			{
				var pars = new SqlParameter[3];
				pars[0] = new SqlParameter("@GroupID", functionId);
                pars[2] = new SqlParameter("@UserName", Username);
                pars[1] = new SqlParameter("@ErrorCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_Group_Delete", pars);
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

