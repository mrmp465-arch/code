using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;

namespace Libs.API
{
    public class CasBank
    {
        public int Id { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string BankId { get; set; }
        public int Status { get; set; }

        public string PartnerCode { get; set; }

        public CasBank()
        {

        }
        public GPBank Get()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<GPBank>("sp_CasBank_CMS_Select", new SqlParameter("@Id", Id));
        }
        public int UpdateStatus(int id, int status)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", id);
                pars[2] = new SqlParameter("@Status", status);
                db.ExecuteNonQuerySP("sp_CasBank_CMS_Update", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                return -99;
            }
        }
        public List<CasBank> GetList()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                return db.GetListSP<CasBank>("sp_GPBank_CMS_SelectList");
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
    }
}
