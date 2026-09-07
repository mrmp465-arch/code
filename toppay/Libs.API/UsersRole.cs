using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.API
{
    public class UsersRole
    {
       
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string UserName { get; set; }
        public string Url { get; set; }
        public int ReturnValue { get; set; } 
        
         
        public UsersRole Get(int UserId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<UsersRole>("sp_UserRole_Select"
                , new SqlParameter("@UserId", UserId));
        }

        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            var oCommand = new SqlCommand("sp_UserRole_Delete");
            oCommand.CommandType = CommandType.StoredProcedure;
            oCommand.Parameters.Add(new SqlParameter("@Id", this.Id));
            db.ExecuteNonQuery(oCommand);
        }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserId", UserId);
            pars[2] = new SqlParameter("@RoleId", RoleId);
            pars[3] = new SqlParameter("@UserName", UserName);
            pars[4] = new SqlParameter("@Url", Url);
            db.ExecuteNonQuerySP("sp_UserRole_Insert", pars);
            Id = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserId", UserId);  
            pars[2] = new SqlParameter("@RoleId", RoleId);
            pars[3] = new SqlParameter("@UserName", UserName);
            pars[4] = new SqlParameter("@Url", Url);
            pars[5] = new SqlParameter("@Id", Id);
            db.ExecuteNonQuerySP("sp_UserRole_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public List<UsersRole> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<UsersRole>("sp_UserRole_SelectList");
        }

        public List<UsersRole> GetListByUser(int UserId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<UsersRole>("sp_UserRole_Select" , new SqlParameter("@UserId", UserId));
        }
    }
}
