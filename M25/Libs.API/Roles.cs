using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;
using System.Linq;
namespace Libs.API
{ 

    public class Roles
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int Group { get; set; }
        public string GroupName { get; set; }
        public int OrderNo { get; set; }
        public bool IsMenu { get; set; }
        public int ParentId { get; set; }
        public int Status { get; set; }
        public int ReturnValue { get; set; }
        

       
        public Roles Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Roles>("sp_Roles_Select"
                , new SqlParameter("@Id", Id));
        } 
       
        public Roles Get(string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Roles>("sp_Roles_SelectByPartnerCode"
                , new SqlParameter("@PartnerCode", partnerCode));
        }
 
        public Roles GetSms(string partnerCommand)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Roles>("sp_Roles_Select_SmsPlusCommand"
                , new SqlParameter("@SMSPlusCommand", partnerCommand));
        }

   
        public void Delete()
        { 
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            db.ExecuteNonQuerySP("sp_Roles_Delete"
                , new SqlParameter("@Id", Id));
             
        }
        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Name", Name);
            pars[2] = new SqlParameter("@Code", Code);
            pars[3] = new SqlParameter("@Description", Description);
            pars[4] = new SqlParameter("@Url", Url);
            pars[5] = new SqlParameter("@Group", Group);
            pars[6] = new SqlParameter("@GroupName", GroupName);
            pars[7] = new SqlParameter("@OrderNo", OrderNo);
            pars[8] = new SqlParameter("@IsMenu", IsMenu);
            pars[9] = new SqlParameter("@ParentId", ParentId); 
            pars[10] = new SqlParameter("@Status", Status);  
            db.ExecuteNonQuerySP("sp_Roles_Insert", pars);
            Id = Convert.ToInt32(pars[0].Value);
        } 
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[12];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Name", Name);
            pars[2] = new SqlParameter("@Code", Code);
            pars[3] = new SqlParameter("@Description", Description);
            pars[4] = new SqlParameter("@Url", Url);
            pars[5] = new SqlParameter("@Group", Group);
            pars[6] = new SqlParameter("@GroupName", GroupName);
            pars[7] = new SqlParameter("@OrderNo", OrderNo);
            pars[8] = new SqlParameter("@IsMenu", IsMenu);
            pars[9] = new SqlParameter("@ParentId", ParentId);
            pars[10] = new SqlParameter("@Status", Status);
            pars[11] = new SqlParameter("@Id", Id);
            db.ExecuteNonQuerySP("sp_Roles_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
             
        } 
        public List<Roles> GetListByUserId(int UserId)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@UserId", UserId);
            return db.GetListSP<Roles>("sp_Roles_SelectListByUserId", pars);
        }
        public List<Roles> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            var lst= db.GetListSP<Roles>("sp_Roles_SelectList");
            if (lst != null)
                lst = lst.OrderBy(e => e.OrderNo).ToList();
            return lst;
        }
        public DataTable GetTable()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_Roles_SelectList");
        }
    }
}
