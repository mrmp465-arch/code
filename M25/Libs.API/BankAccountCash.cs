using Libs.Db;
using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.API
{
    public class BankAccountCash
    {
        public int Id { get; set; }
        public string BankCode { get; set; }
        public string AccountName { get; set; }
        public int Status { get; set; }
        public string AccountNumber { get; set; }
        public BankAccountCash()
        {

        }
        public void Delete(int Id)
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            db.ExecuteNonQuerySP("sp_BankAccountCash_CMS_Delete"
                , new SqlParameter("@Id", Id));
        }
        public List<BankAccountCash> GetLis()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                return db.GetListSP<BankAccountCash>("sp_BankAccountCash_CMS_SelectList");
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
      
        public int Add(BankAccountCash p)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                SqlParameter[] pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@BankCode", p.BankCode);
                pars[2] = new SqlParameter("@AccountName", p.AccountName);
                pars[3] = new SqlParameter("@Status", p.Status);
                pars[4] = new SqlParameter("@AccountNumber", p.AccountNumber);
                
                db.ExecuteNonQuerySP("sp_BankAccountCash_CMS_Add", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", ex.Message.Replace("\n", " ") });
                return -99;
            }
        }
        public int Update(BankAccountCash p)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", p.Id);
                pars[2] = new SqlParameter("@Status", p.Status);
                db.ExecuteNonQuerySP("sp_BankAccountCash_CMS_Update", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", ex.Message.Replace("\n", " ") });
                return -99;
            }
        }
    }
}
