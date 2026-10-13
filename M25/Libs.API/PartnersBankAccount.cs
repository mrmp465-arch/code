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
    public class PartnersBankAccount
    {
        public int Id { get; set; }
        public string BankCode { get; set; }
        public string AccountName { get; set; }
        public int Status { get; set; }
        public int Number { get; set; }
        public string AccountNumber { get; set; }
        public string ParnerCode { get; set; }
        public string IP { get; set; }
        public DateTime Time { get; set; }
        
        public PartnersBankAccount()
        {

        }
        public void Delete(int Id)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            db.ExecuteNonQuerySP("sp_PartnersBankAccount_CMS_Delete"
                , new SqlParameter("@Id", Id));
        }
        public List<PartnersBankAccount> GetLis()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
                return db.GetListSP<PartnersBankAccount>("sp_PartnersBankAccount_CMS_SelectList");
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }

        public int Add(PartnersBankAccount p)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
                SqlParameter[] pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@BankCode", p.BankCode);
                pars[2] = new SqlParameter("@AccountName", p.AccountName);
                pars[3] = new SqlParameter("@Status", p.Status);
                pars[4] = new SqlParameter("@AccountNumber", p.AccountNumber);
                pars[5] = new SqlParameter("@IP", p.IP);
                pars[6] = new SqlParameter("@ParnerCode", p.ParnerCode);
                db.ExecuteNonQuerySP("sp_PartnersBankAccount_CMS_Add", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", ex.Message.Replace("\n", " ") });
                return -99;
            }
        }
        public int Update(PartnersBankAccount p)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
                SqlParameter[] pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", p.Id);
                pars[2] = new SqlParameter("@Status", p.Status);
                pars[3] = new SqlParameter("@Number", p.Number);
                db.ExecuteNonQuerySP("sp_PartnersBankAccount_CMS_Update", pars);
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
