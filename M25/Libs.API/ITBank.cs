using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;

namespace Libs.API
{
    public class ITBank
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string BankId { get; set; }
        public int Status { get; set; }

        public string PartnerCode { get; set; }
        public string Type { get; set; }
        public ITBank()
        {

        }
        public ITBank Get()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<ITBank>("sp_ITBank_CMS_Select", new SqlParameter("@Id", Id));
        }
        public void Delete(int Id)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            db.ExecuteNonQuerySP("sp_ITBank_CMS_Delete"
                , new SqlParameter("@Id", Id));
        }
        public int Add()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@BankCode", BankCode);
                pars[2] = new SqlParameter("@BankName", BankName);
                pars[3] = new SqlParameter("@Status", Status);
                pars[4] = new SqlParameter("@BankId", BankId);
                pars[5] = new SqlParameter("@Type", Type);
                db.ExecuteNonQuerySP("sp_ITBank_CMS_Add", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", ex.Message.Replace("\n", " ") });
                return -99;
            }
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
                db.ExecuteNonQuerySP("sp_ITBank_CMS_Update", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                return -99;
            }
        }
        public List<ITBank> GetList()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                return db.GetListSP<ITBank>("sp_ITBank_CMS_SelectList");
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
    }
}
