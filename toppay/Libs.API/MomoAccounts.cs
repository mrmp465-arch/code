using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.Utils;

namespace Libs.API
{
    public class MomoProfileImage
    {
        public string Base64 { get; set; }
        public string ImgName { get; set; }
        public string Detection { get; set; }
    }
    public class MomoExtra
    {
        public string MomoId { get; set; }
        public string Description { get; set; }

    }
        [Serializable]
    public class MomoAccounts
    {
        public int Id { get; set; }
        public string MomoName { get; set; }
        public string MomoId { get; set; }
        public string MomoPass { get; set; }
        public int Status { get; set; }
        public int StatusOver { get; set; }
        public int StatusOverOut { get; set; }
        public string Type { get; set; }
        public int BalanceDayIn { get; set; }
        public int BalanceMonthIn { get; set; }
        public int BalanceDayOut { get; set; }
        public int BalanceMonthOut { get; set; }
        public int BalanceTotal { get; set; }
        public int BalanceMaxDay { get; set; }
        public int BalanceMaxMonth { get; set; }
        public int CashTimeMonth { get; set; }
        public int CashTotalMonth { get; set; }
        public string Solution { get; set; }
        public int StatusExtra { get; set; }
        public string PartnerName { get; set; }
        public string Source { get; set; }

        public string ProfileImage { get; set; }
        public int StatusDetection { get; set; }

        public DateTime StopScanAt { get; set; }

        public DateTime? CreatedAt { get; set; }
        public int ByPass20M { get; set; }
        public MomoAccounts()
        {

        }
        public MomoAccounts Get()
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            return db.GetInstanceSP<MomoAccounts>("sp_Momo_CMS_Select", new SqlParameter("@Id", Id));
        }
        public MomoAccounts Get(int partnerID)
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            return db.GetInstanceSP<MomoAccounts>("sp_Momo_CMS_Select", new SqlParameter("@Id", partnerID));
        }

        public MomoAccounts Get(string MomoId)
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            return db.GetInstanceSP<MomoAccounts>("sp_Momo_Select_By_MomoId", new SqlParameter("@MomoId", MomoId));
        }
        public MomoExtra GetExt(string MomoId)
        {
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            return db.GetInstanceSP<MomoExtra>("sp_MomoExtra_Select_By_MomoId", new SqlParameter("@MomoId", MomoId));
        }
        public int Add()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                SqlParameter[] pars = new SqlParameter[10];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@MomoName", MomoName);
                pars[2] = new SqlParameter("@MomoId", MomoId);
                pars[3] = new SqlParameter("@Status", Status);
                pars[4] = new SqlParameter("@Type", Type);
                pars[5] = new SqlParameter("@MomoPass", MomoPass);
                pars[6] = new SqlParameter("@BalanceMaxDay", BalanceMaxDay);
                pars[7] = new SqlParameter("@BalanceMaxMonth", BalanceMaxMonth);
                pars[8] = new SqlParameter("@Solution", Solution);
                pars[9] = new SqlParameter("@Source", Source);
                db.ExecuteNonQuerySP("sp_Momo_CMS_Add", pars);
                Id = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", ex.Message.Replace("\n", " ") });
                Id = -99;
            }
            return Id;
        }
        public void Reset()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                SqlParameter[] pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@Id", Id);
                pars[1] = new SqlParameter("@BalanceMaxDay", BalanceMaxDay);
                db.ExecuteNonQuerySP("sp_Momo_CMS_Reset", pars);
                //Id = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                //Id = -99;
            }

        }
        public void Update()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                SqlParameter[] pars = new SqlParameter[13];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@MomoName", MomoName);
                pars[2] = new SqlParameter("@MomoId", MomoId);
                pars[3] = new SqlParameter("@Status", Status);
                pars[4] = new SqlParameter("@Type", Type);
                pars[5] = new SqlParameter("@MomoPass", MomoPass);
                pars[6] = new SqlParameter("@Id", Id);
                pars[7] = new SqlParameter("@BalanceMaxDay", BalanceMaxDay);
                pars[8] = new SqlParameter("@BalanceMaxMonth", BalanceMaxMonth);
                pars[9] = new SqlParameter("@Solution", Solution);
                pars[10] = new SqlParameter("@ProfileImage", ProfileImage);
                pars[11] = new SqlParameter("@StatusDetection", StatusDetection);
                pars[12] = new SqlParameter("@ByPass20M", ByPass20M);
                db.ExecuteNonQuerySP("sp_Momo_CMS_Update", pars);
                //Id = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                //Id = -99;
            }

        }
        public void Stop()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                SqlParameter[] pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", Id);
                db.ExecuteNonQuerySP("sp_Momo_CMS_Stop", pars);
                //Id = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                //Id = -99;
            }

        }
        public void StopScanAt_Update(int MomoId, DateTime Time)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", MomoId);
                pars[2] = new SqlParameter("@StopScanAt", Time);
                db.ExecuteNonQuerySP("sp_Momo_StopScanAt_Update", pars);
                //Id = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                //Id = -99;
            }

        }
        public void UpdateExtra(string MomoId, int status, DateTime Time, DateTime lastUpdate, string momoName, string description)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                SqlParameter[] pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@MomoId", MomoId);

                pars[3] = new SqlParameter("@Status", status);
                pars[4] = new SqlParameter("@LastUpdate", lastUpdate);
                pars[5] = new SqlParameter("@MomoName", momoName);
                pars[2] = new SqlParameter("@Description", momoName);
                db.ExecuteNonQuerySP("sp_MomoExtra_Update_Status", pars);
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
            DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
            db.ExecuteNonQuerySP("sp_Momo_CMS_Delete"
                , new SqlParameter("@Id", Id));
        }
        public List<MomoAccounts> GetList()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGMOMOConnectionStrings);
                return db.GetListSP<MomoAccounts>("sp_Momo_CMS_SelectList");
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
    }
}
