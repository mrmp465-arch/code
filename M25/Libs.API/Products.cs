using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using System.Linq;

namespace Libs.API
{
      

    public class Products
    {
        public int Id{ get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int SubType { get; set; }

        public int ReturnValue { get; set; }
        public Products()
        {

        }

        public Products Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Products>("sp_Products_Select"
                , new SqlParameter("@Id", Id));
        }

        public Products Get(int Id)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Products>("sp_Products_Select"
                , new SqlParameter("@Id", Id));
        }

        public Products Get(string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Products>("sp_Products_SelectByPartnerCode"
                , new SqlParameter("@PartnerCode", partnerCode));
        }

        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            db.ExecuteNonQuerySP("sp_Products_Delete"
                , new SqlParameter("@Id", Id));
        }

        public void Delete(int Id)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            db.ExecuteNonQuerySP("sp_Products_Delete"
                , new SqlParameter("@Id", Id));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Name", Name);
            pars[2] = new SqlParameter("@Code", Code);
            pars[3] = new SqlParameter("@Status", Status);
            pars[4] = new SqlParameter("@Type", Type);
            pars[5] = new SqlParameter("@SubType", SubType);

            db.ExecuteNonQuerySP("sp_Products_Insert", pars);
            Id = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = new SqlParameter("@Name", Name);
            pars[3] = new SqlParameter("@Code", Code);
            pars[4] = new SqlParameter("@Status", Status);
            pars[5] = new SqlParameter("@Type", Type);
            pars[6] = new SqlParameter("@SubType", SubType);
            db.ExecuteNonQuerySP("sp_Products_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public List<Products> GetList(int? type=null, int? status = null)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            var lst=db.GetListSP<Products>("sp_Products_SelectList");
            if (lst != null)
            {
                if (status != null)
                    lst = lst.Where(e => e.Status == status).ToList();
                if (type != null)
                    lst = lst.Where(e => e.Type == type).ToList(); 
            }
            return lst;
        }

        public DataTable GetTable()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetDataTableSP("sp_Products_SelectList");
        } 
    }
}
