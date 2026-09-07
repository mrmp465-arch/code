using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;

namespace Libs.API
{
    [Serializable]
    public class BPartner
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public int Status { get; set; }
        public string Secret { get; set; }
        public string InCallbackUrl { get; set; }
    }
    [Serializable]
    public class PartnerBank
    {
        public int Id { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankId { get; set; }
        public int BId { get; set; }
        public int OrderNo { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int PartnerId { get; set; }
        public int Status { get; set; }
        public int StatusOverIn { get; set; }
        public int StatusOverOut { get; set; }
        public long BalanceDayIn { get; set; }
        public long BalanceMonthIn { get; set; }
        public long BalanceDayOut { get; set; }
        public long BalanceMonthOut { get; set; }
        public long BalanceTotal { get; set; }
        public int StatusExtra { get; set; }
        public string Solution { get; set; }
        public PartnerBank()
        {

        }
        public void DeletePartner(int partnerId)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            db.ExecuteNonQuerySP("sp_Partners_CMS_Delete"
                , new SqlParameter("@Id", partnerId));
        }
        public List<BPartner> GetListPartner()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                return db.GetListSP<BPartner>("sp_Partners_CMS_SelectList");
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
        public BPartner GetPartner(string partnerCode)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<BPartner>("sp_Partners_CMS_Select"
                , new SqlParameter("@Code", partnerCode));
        }
        public int AddPartner(BPartner p)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Code", p.Code);
                pars[2] = new SqlParameter("@Secret", p.Secret);
                pars[3] = new SqlParameter("@Status", p.Status);
                pars[4] = new SqlParameter("@InCallbackUrl", p.InCallbackUrl);
                pars[5] = new SqlParameter("@Name", p.Name);
                db.ExecuteNonQuerySP("sp_Partners_CMS_Add", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", ex.Message.Replace("\n", " ") });
                return -99;
            }
        }
        public int UpdatePartner(BPartner p)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", p.Id);
                pars[2] = new SqlParameter("@InCallbackUrl", p.InCallbackUrl);
                pars[3] = new SqlParameter("@Status", p.Status);

                db.ExecuteNonQuerySP("sp_Partners_CMS_Update", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", ex.Message.Replace("\n", " ") });
                return -99;
            }
        }
        public void Add()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@PartnerId", PartnerId);
                pars[2] = new SqlParameter("@BankId", BId);
                pars[3] = new SqlParameter("@Status", Status);
                pars[4] = new SqlParameter("@OrderNo", OrderNo);
                db.ExecuteNonQuerySP("sp_PartnerBank_CMS_Add", pars);
                Id = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", ex.Message.Replace("\n", " ") });
                Id = -99;
            }
        }

        public void Update()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Status", Status);
                pars[2] = new SqlParameter("@OrderNo", OrderNo);
                pars[3] = new SqlParameter("@Id", Id);
                db.ExecuteNonQuerySP("sp_PartnerBank_CMS_Update", pars);
                //Id = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                //Id = -99;
            }

        }
        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            db.ExecuteNonQuerySP("sp_PartnerBank_CMS_Delete"
                , new SqlParameter("@Id", Id));
        }
        public List<PartnerBank> GetList()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                return db.GetListSP<PartnerBank>("sp_PartnerBank_CMS_SelectList");
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
    }
}
