using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.API
{
    public class UsersProvider
    {
        public int UserProviderId { get; set; }
        public int UserId { get; set; }
        public int ProviderId { get; set; }
        public string ProviderCode { get; set; }
        public int ReturnValue { get; set; } 
        public UsersProvider()
        {

        }  
        public UsersProvider Get(int UserId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<UsersProvider>("sp_UserProvider_Select"
                , new SqlParameter("@UserId", UserId));
        }

        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            var oCommand = new SqlCommand("sp_UserProvider_Delete");
            oCommand.CommandType = CommandType.StoredProcedure;
            oCommand.Parameters.Add(new SqlParameter("@UserProviderId", this.UserProviderId));
            db.ExecuteNonQuery(oCommand);
        }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserId", UserId);
            pars[2] = new SqlParameter("@ProviderId", ProviderId);
            pars[3] = new SqlParameter("@ProviderCode", ProviderCode); 

            db.ExecuteNonQuerySP("sp_UserProvider_Insert", pars);
            UserProviderId = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserId", UserId);  
            pars[2] = new SqlParameter("@ProviderId", ProviderId);
            pars[3] = new SqlParameter("@ProviderCode", ProviderCode); 
            db.ExecuteNonQuerySP("sp_UserProvider_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public List<UsersProvider> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<UsersProvider>("sp_UserProvider_SelectList");
        }

        public List<UsersProvider> GetListByUser(int UserId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<UsersProvider>("sp_UserProvider_Select" , new SqlParameter("@UserId", UserId));
        }
    }
}
