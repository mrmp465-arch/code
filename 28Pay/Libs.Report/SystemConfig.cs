using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Libs.Db;

namespace Libs.Report
{
    public class SystemConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Feature { get; set; }
        public int? Status { get; set; }
        public int ReturnValue { get; set; }

        public List<SystemConfig> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetListSP<SystemConfig>("sp_SystemConfig_SelectList");
        }

        public SystemConfig Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<SystemConfig>("sp_SystemConfig_Select", new SqlParameter("@Id", Id));
        }
        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@Id", Id);
            pars[1] = string.IsNullOrEmpty(Name) ? new SqlParameter("@Name", DBNull.Value) : new SqlParameter("@Name", Name);
            pars[2] = string.IsNullOrEmpty(Code) ? new SqlParameter("@Code", DBNull.Value) : new SqlParameter("@Code", Code);
            pars[3] = string.IsNullOrEmpty(Description) ? new SqlParameter("@Description", DBNull.Value) : new SqlParameter("@Description", Description);
            pars[4] = string.IsNullOrEmpty(Feature) ? new SqlParameter("@Feature", DBNull.Value) : new SqlParameter("@Feature", Feature);
            pars[5] = Status == null ? new SqlParameter("@Status", DBNull.Value) : new SqlParameter("@Status", Status);
            pars[6] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            db.ExecuteNonQuerySP("sp_SystemConfig_Update", pars);
            ReturnValue = Convert.ToInt32(pars[6].Value);
        }

    }
    public class WServiceAutoBuyCardService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Status { get; set; }

    }

}
