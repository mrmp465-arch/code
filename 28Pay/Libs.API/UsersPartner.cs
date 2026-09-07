using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Libs.API
{
    public class UserPartner
    {
        public int UserPartnerId { get; set; }
        public int UserId { get; set; }
        public int PartnerId { get; set; }
        public string PartnerCode { get; set; }
        public int ReturnValue { get; set; } 
        public UserPartner()
        {

        }  
        public UserPartner Get(int UserId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<UserPartner>("sp_UserPartner_Select"
                , new SqlParameter("@UserId", UserId));
        }

        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            var oCommand = new SqlCommand("sp_UserPartner_Delete");
            oCommand.CommandType = CommandType.StoredProcedure;
            oCommand.Parameters.Add(new SqlParameter("@UserPartnerId", this.UserPartnerId));
            db.ExecuteNonQuery(oCommand);
        }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserId", UserId);
            pars[2] = new SqlParameter("@PartnerId", PartnerId);
            pars[3] = new SqlParameter("@PartnerCode", PartnerCode); 

            db.ExecuteNonQuerySP("sp_UserPartner_Insert", pars);
            UserPartnerId = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserId", UserId);  
            pars[2] = new SqlParameter("@PartnerId", PartnerId);
            pars[3] = new SqlParameter("@PartnerCode", PartnerCode); 
            db.ExecuteNonQuerySP("sp_UserPartner_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }
        public List<UserPartner> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<UserPartner>("sp_UserPartner_SelectList");
        }

        public List<UserPartner> GetListByUser(int UserId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<UserPartner>("sp_UserPartner_Select" , new SqlParameter("@UserId", UserId));
        }
    }
}
