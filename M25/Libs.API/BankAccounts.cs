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
    public class BankProfileImage
    {
        public string Base64 { get; set; }
        public string ImgName { get; set; }
    }
    public class BankExtra
    {
        public string BankCode { get; set; }
        public string Description { get; set; }
        public string BankId { get; set; }

    }

    [Serializable]
    public class BankAccounts
    {
        public int Id { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string BankId { get; set; }
        public string BankPass { get; set; }
        public int Status { get; set; }
        public int StatusOverIn { get; set; }
        public int StatusOverOut { get; set; }
        public string Type { get; set; }
        public long BalanceDayIn { get; set; }
        public long BalanceMonthIn { get; set; }
        public long BalanceDayOut { get; set; }
        public long BalanceMonthOut { get; set; }
        public long BalanceTotal { get; set; }
        public int BalanceMaxDay { get; set; }
        public int BalanceMaxMonth { get; set; }
        public string Solution { get; set; }
        public int StatusExtra { get; set; }
        public string Computer { get; set; }
        public string PhoneDevice { get; set; }
        public string PinOtp { get; set; }
        public string BankType { get; set; }
        public string Source { get; set; }
        public string ProfileImage { get; set; }
        public string AppDeviceId { get; set; }
        public string CloudPhoneId { get; set; }
        public string PartnerName { get; set; }


        public DateTime StopScanAt { get; set; }
        public BankAccounts()
        {

        }
        public BankAccounts Get()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<BankAccounts>("sp_Bank_CMS_Select", new SqlParameter("@Id", Id));
        }
       
        public BankAccounts Get(int bankId)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<BankAccounts>("sp_Bank_CMS_Select", new SqlParameter("@Id", bankId));
        }

        public BankAccounts Get(string bankId)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<BankAccounts>("sp_Bank_Select_By_BankId", new SqlParameter("@BankId", bankId));
        }
        public void StopScanAt_Update(int id, DateTime Time)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", id);
                pars[2] = new SqlParameter("@StopScanAt", Time);
                db.ExecuteNonQuerySP("sp_Bank_StopScanAt_Update", pars);
                //Id = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                //Id = -99;
            }

        }
        public BankAccounts Get(string bankId, string bankCode)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            SqlParameter[] pars = new SqlParameter[2];
            pars[0] = new SqlParameter("@BankId", bankId);
            pars[1] = new SqlParameter("@BankCode", bankCode);
            return db.GetInstanceSP<BankAccounts>("sp_Bank_Select_By_BankId", pars);
        }
        public BankAccounts GetByAppDeviceId(string bankId)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<BankAccounts>("sp_Bank_Select_By_AppDeviceId", new SqlParameter("@AppDeviceId", bankId));
        }
        public BankAccounts GetByAppDeviceIdImage(string bankId)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<BankAccounts>("sp_Bank_SelectProfile_By_AppDeviceId", new SqlParameter("@AppDeviceId", bankId));
        }
        public BankAccounts GetProfile()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<BankAccounts>("sp_Bank_CMS_SelectProfile", new SqlParameter("@Id", Id));
        }
        public int UpdateProfile(int id, string ProfileImage)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", id);
                pars[2] = new SqlParameter("@ProfileImage", ProfileImage);
                db.ExecuteNonQuerySP("sp_Bank_CMS_Update_Image", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                return -99;
            }
        }

        public BankAccounts GetByAppDeviceIdCache(string bankId)
        {
            string KeyCache = string.Format("{0}:{1}", "RedisBankAccounts", bankId);
            try
            {
                var result = DataCaching.GetCache<BankAccounts>(KeyCache);
                if (result == null)
                {
                    result = GetByAppDeviceId(bankId);
                    result.ProfileImage = null;
                    result = DataCaching.SetCache(KeyCache, result, 60 * 60);
                }
                return result;
            }
            catch (Exception ex)
            {
                //   ExceptionHandler.Handle(ex, "Partners", "Get:" + KeyCache);
                return null;
            }
        }
        public void DeleteCache()
        {

            DataCaching.RemoveByPattern(@"^(RedisBankAccounts:)(\w+)");
        }
        public int Add()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[16];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@BankCode", BankCode);
                pars[2] = new SqlParameter("@BankAccount", BankAccount);
                pars[3] = new SqlParameter("@BankName", BankName);
                pars[4] = new SqlParameter("@BankId", BankId);
                pars[5] = new SqlParameter("@Status", Status);
                pars[6] = new SqlParameter("@Type", Type);
                pars[7] = new SqlParameter("@BankPass", BankPass);
                pars[8] = new SqlParameter("@BalanceMaxDay", BalanceMaxDay);
                pars[9] = new SqlParameter("@BalanceMaxMonth", BalanceMaxMonth);
                pars[10] = new SqlParameter("@Solution", Solution);
                pars[11] = new SqlParameter("@Computer", Computer);
                pars[12] = new SqlParameter("@PhoneDevice", PhoneDevice);
                pars[13] = new SqlParameter("@PinOtp", PinOtp);
                pars[14] = new SqlParameter("@BankType", BankType);
                pars[15] = new SqlParameter("@AppDeviceId", AppDeviceId);

                db.ExecuteNonQuerySP("sp_Bank_CMS_Add", pars);
                Id = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Add Error", ex.Message.Replace("\n", " ") });
                Id = -99;
            }
            return Id;
        }

        public void Update()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[18];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", Id);
                pars[2] = new SqlParameter("@BankCode", BankCode);
                pars[3] = new SqlParameter("@BankName", BankName);
                pars[4] = new SqlParameter("@BankId", BankId);
                pars[5] = new SqlParameter("@Status", Status);
                pars[6] = new SqlParameter("@Type", Type);
                pars[7] = new SqlParameter("@BankPass", BankPass);
                pars[8] = new SqlParameter("@BalanceMaxDay", BalanceMaxDay);
                pars[9] = new SqlParameter("@BalanceMaxMonth", BalanceMaxMonth);
                pars[10] = new SqlParameter("@Solution", Solution);
                pars[11] = new SqlParameter("@Computer", Computer);
                pars[12] = new SqlParameter("@PhoneDevice", PhoneDevice);
                pars[13] = new SqlParameter("@PinOtp", PinOtp);
                pars[14] = new SqlParameter("@BankType", BankType);
                pars[15] = new SqlParameter("@ProfileImage", ProfileImage);
                pars[16] = new SqlParameter("@AppDeviceId", AppDeviceId);
                pars[17] = new SqlParameter("@CloudPhoneId", CloudPhoneId);
                db.ExecuteNonQuerySP("sp_Bank_CMS_Update", pars);
                //Id = Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                //Id = -99;
            }
        }

        public int UpdateBalance(int id, int balance)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                SqlParameter[] pars = new SqlParameter[3];
                pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[1] = new SqlParameter("@Id", id);
                pars[2] = new SqlParameter("@BalanceTotal", balance);
                db.ExecuteNonQuerySP("sp_Bank_CMS_Update_Balance", pars);
                return Convert.ToInt32(pars[0].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Update Error", ex.Message.Replace("\n", " ") });
                return -99;
            }
        }
        public void Reset()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            db.ExecuteNonQuerySP("sp_Bank_CMS_Reset"
                , new SqlParameter("@Id", Id));
        }
        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            db.ExecuteNonQuerySP("sp_Bank_CMS_Delete"
                , new SqlParameter("@Id", Id));
        }
        public List<BankAccounts> GetList()
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
                return db.GetListSP<BankAccounts>("sp_Bank_CMS_SelectList");
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "GetList Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
        public BankExtra GetExt(string BankId, string BankCode)
        {
            DBHelper db = new DBHelper(Configs.VPGBankConnectionStrings);
            return db.GetInstanceSP<BankExtra>("sp_BankExtra_Select_By_BankId", new SqlParameter("@BankId", BankId), new SqlParameter("@BankCode", BankCode));
        }
    }
}
