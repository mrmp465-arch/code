using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CMS.Data.Service;
using CMS.Data.DTO;
using CMS.Utility;



namespace CMS.Data.Service
{
	public class UserPartnersService : IUserPartnersService
	{

        

      
        public List<UserPartner> GetList(int UserId)
        {
            try
            {
                //return new DBHelper(Config.MainConnectionString).GetListSP<UserPartner>("sp_UserPartner_Select");
				var list = new DBHelper(Config.MainConnectionString).GetListSP<UserPartner>("sp_UserPartner_Select",
				  new SqlParameter("@UserId", UserId));
				if (list == null || list.Count <= 0)
					return new List<UserPartner>();
				return list;
			}
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<UserPartner>();
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
		public int Add(UserPartner functions)
		{
			try
			{
				var pars = new SqlParameter[4];
				pars[0] = new SqlParameter("@UserId", functions.UserId);
				pars[1] = new SqlParameter("@PartnerId", functions.PartnerId);
				pars[2] = new SqlParameter("@PartnerCode", functions.PartnerCode);
                pars[3] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_UserPartner_Insert", pars);
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
		public int Delete(int functionId)
		{
			try
			{
				var pars = new SqlParameter[1];
				pars[0] = new SqlParameter("@UserPartnerId", functionId);
				//pars[1] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_UserPartner_Delete", pars);
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

