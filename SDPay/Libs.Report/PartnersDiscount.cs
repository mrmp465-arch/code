using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Libs.API;
using Libs.Db;
using Libs.Utils;

namespace Libs.Report
{
    public class PartnersDiscount
    {
        public int Time { get; set; }
        public string PartnerCode { get; set; }
        public DateTime Date { get; set; }
        public decimal DiscountVTT { get; set; }
        public decimal RewardVTT { get; set; }
        public decimal DiscountVNP { get; set; }
        public decimal RewardVNP { get; set; }
        public decimal DiscountVMS { get; set; }
        public decimal RewardVMS { get; set; }
        public decimal DiscountGATE { get; set; }
        public decimal RewardGATE { get; set; }
        public decimal DiscountZING { get; set; }
        public decimal RewardZING { get; set; }
        public decimal DiscountGARENA { get; set; }
        public decimal RewardGARENA { get; set; }
        public decimal DiscountVCOIN { get; set; }
        public decimal RewardVCOIN { get; set; }
        public decimal DiscountGOSU { get; set; }
        public decimal RewardGOSU { get; set; }
        public decimal DiscountBIT { get; set; }
        public decimal RewardBIT { get; set; }
        public decimal DiscountDZO { get; set; }
        public decimal RewardDZO { get; set; }
        public decimal DiscountBANKTRANFER { get; set; }
        public decimal RewardBANKTRANFER { get; set; }
        public decimal DiscountMOMO { get; set; }
        public decimal RewardMOMO { get; set; }


        public decimal DiscountMOMOOUT { get; set; }
        public decimal RewardMOMOOUT { get; set; }

        public decimal DiscountBANKOUTTRANFER { get; set; }
        public decimal RewardBANKOUTTRANFER { get; set; }


        public decimal DiscountVTTOUT { get; set; }
        public decimal RewardVTTOUT { get; set; }
        public decimal DiscountVMSOUT { get; set; }
        public decimal RewardVMSOUT { get; set; }

        public decimal DiscountVNPOUT { get; set; }
        public decimal RewardVNPOUT { get; set; }
       


        public List<PartnersDiscount> GetList(string partnerCode, int year, int month)
        {
            //DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            //SqlParameter[] pars = new SqlParameter[3];
            //pars[0] = new SqlParameter("@PartnerCodes", partnerCode);
            //pars[1] = new SqlParameter("@Year", year);
            //pars[2] = new SqlParameter("@Month", month);           
            ////DataTable dt = db.GetDataTableSP("sp_PartnersDiscount_Select", pars);           
            ////return dt;
            //return db.GetListSP<PartnersDiscount>("sp_PartnersDiscount_Select", pars);
            string KeyCache = string.Format("{0}:{1}", "PartnersDiscount", partnerCode);
            try
            {
                var result = DataCaching.GetCache<List<PartnersDiscount>>(KeyCache);
                if (result == null)
                {
                    result = GetListPrivae(partnerCode, year, month);
                    result = DataCaching.SetCache(KeyCache, result,60);
                }
                return result;
            }
            catch (Exception ex)
            {
                //   ExceptionHandler.Handle(ex, "Partners", "Get:" + KeyCache);
                return null;
            }

            //return GetListPrivae(partnerCode, year, month);
        }
        private List<PartnersDiscount> GetListPrivae(string partnerCode, int year, int month)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@PartnerCodes", partnerCode);
            pars[1] = new SqlParameter("@Year", year);
            pars[2] = new SqlParameter("@Month", month);
            //DataTable dt = db.GetDataTableSP("sp_PartnersDiscount_Select", pars);           
            //return dt;
            return db.GetListSP<PartnersDiscount>("sp_PartnersDiscount_Select", pars);
        }
        public DataTable GetTablePartnersDiscount(string partnerCode, int year, int month)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@PartnerCodes", partnerCode);
            pars[1] = new SqlParameter("@Year", year);
            pars[2] = new SqlParameter("@Month", month);
            DataTable dt = db.GetDataTableSP("sp_PartnersDiscount_Select", pars);
            return dt;

        }
        public void DeleteCache(string partnerCode)
        {
            string KeyCache = string.Format("{0}:{1}", "PartnersDiscount", partnerCode);
            DataCaching.RemoveCache(KeyCache);
        }
        public long Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[27];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };            
            pars[1] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[2] = new SqlParameter("@Date", Date);
            pars[3] = new SqlParameter("@DiscountVTT", DiscountVTT);
            pars[4] = new SqlParameter("@RewardVTT", RewardVTT);
            pars[5] = new SqlParameter("@DiscountVNP", DiscountVNP);
            pars[6] = new SqlParameter("@RewardVNP", RewardVNP);
            pars[7] = new SqlParameter("@DiscountVMS", DiscountVMS);
            pars[8] = new SqlParameter("@RewardVMS", RewardVMS);
            pars[9] = new SqlParameter("@DiscountGATE", DiscountGATE);
            pars[10] = new SqlParameter("@RewardGATE", RewardGATE);
            pars[11] = new SqlParameter("@DiscountZING", DiscountZING);
            pars[12] = new SqlParameter("@RewardZING", RewardZING);
            pars[13] = new SqlParameter("@DiscountVTTOUT", DiscountVTTOUT);
            pars[14] = new SqlParameter("@RewardVTTOUT", RewardVTTOUT);
            pars[15] = new SqlParameter("@DiscountBANKOUTTRANFER", DiscountBANKOUTTRANFER);
            pars[16] = new SqlParameter("@RewardBANKOUTTRANFER", RewardBANKOUTTRANFER);
            pars[17] = new SqlParameter("@DiscountVNPOUT", DiscountVNPOUT);
            pars[18] = new SqlParameter("@RewardVNPOUT", RewardVNPOUT);
            pars[19] = new SqlParameter("@DiscountVMSOUT", DiscountVMSOUT);
            pars[20] = new SqlParameter("@RewardVMSOUT", RewardVMSOUT);
            pars[21] = new SqlParameter("@DiscountMOMOOUT", DiscountMOMOOUT);
            pars[22] = new SqlParameter("@RewardMOMOOUT", RewardMOMOOUT);
            pars[23] = new SqlParameter("@DiscountBANKTRANFER", DiscountBANKTRANFER);
            pars[24] = new SqlParameter("@RewardBANKTRANFER", RewardBANKTRANFER);
            pars[25] = new SqlParameter("@DiscountMOMO", DiscountMOMO);
            pars[26] = new SqlParameter("@RewardMOMO", RewardMOMO);

            db.ExecuteNonQuerySP("sp_PartnersDiscount_Insert", pars);
            return Convert.ToInt64(pars[0].Value);
        }


    }
}
