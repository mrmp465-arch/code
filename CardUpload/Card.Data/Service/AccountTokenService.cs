using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Card.Data.Service;
using Card.Data.DTO;
using Card.Utility;



namespace Card.Data.Service
{
	public class AccountTokenService : IAccountTokenService
    {

        public AccountToken Get(String Mobile)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<AccountToken>("SP_AcountToken_Get",
                                                                                                   new SqlParameter("@Mobile", Mobile));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return null;
            }
        }


      
       
		/// </returns>
		public int InsertUpdate(AccountToken functions)
		{
			try
			{
				var pars = new SqlParameter[3];
				pars[0] = new SqlParameter("@Mobile", functions.Mobile);
				pars[1] = new SqlParameter("@Token", functions.Token);
                pars[2] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_AccountToken_Update", pars);
				return Convert.ToInt32(pars[2].Value);
			}
			catch (Exception ex)
			{
                 NLogLogger.PublishException(ex);
				return -99;
			}
		}


		

      
	}
}

