using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using Libs.API;
using Libs.Utils;

namespace Libs.VGGTopup
{
    public class VGGTopup
    {
        public int ServiceID { get; set; }
        public string ServiceKey { get; set; }
        public string AccessToken { get; set; }
        public string AccountName { get; set; }
        public long AccountID { get; set; }
        public long Amount { get; set; }
        public string PartnerTransactionID { get; set; }
        public long ReferenceID { get; set; }
        public string Description { get; set; }
        public string ClientIP { get; set; }
        public long Balance { get; set; }
        public long TotalBalance { get; set; }
        public long Gift { get; set; }
        public long ReturnValue { get; set; }

        public VGGTopup()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public void TopupFromIntecom()
        {
            DBHelper db = new DBHelper(Configs.VGGBillingAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[13];
            pars[0] = new SqlParameter("@_ServiceID", ServiceID);
            pars[1] = new SqlParameter("@_ServiceKey", ServiceKey);
            pars[2] = new SqlParameter("@_AccessToken", AccessToken);
            pars[3] = new SqlParameter("@_AccountName", AccountName);
            pars[4] = new SqlParameter("@_Amount", Amount);
            pars[5] = new SqlParameter("@_IntecomTranId", PartnerTransactionID);
            pars[6] = new SqlParameter("@_ReferenceID", ReferenceID);
            pars[7] = new SqlParameter("@_Description", Description);
            pars[8] = new SqlParameter("@_ClientIP", ClientIP);
            pars[9] = new SqlParameter("@_Vcoin", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[10] = new SqlParameter("@_Gift", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[11] = new SqlParameter("@_TotalVcoin", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[12] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            db.ExecuteNonQuerySP("SP_TopupAccount_Intecom", pars);
            ReturnValue = Convert.ToInt64(pars[12].Value);
            if (ReturnValue >= 0)
            {
                Balance = Convert.ToInt32(pars[9].Value);
                Gift = Convert.ToInt32(pars[10].Value);
                TotalBalance = Convert.ToInt32(pars[11].Value);
            }
        }

        public void TopupFromSmartlink()
        {
            DBHelper db = new DBHelper(Configs.VGGBillingAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[13];
            pars[0] = new SqlParameter("@_ServiceID", ServiceID);
            pars[1] = new SqlParameter("@_ServiceKey", ServiceKey);
            pars[2] = new SqlParameter("@_AccessToken", AccessToken);
            pars[3] = new SqlParameter("@_AccountName", AccountName);
            pars[4] = new SqlParameter("@_Amount", Amount);
            pars[5] = new SqlParameter("@_BankTranId", PartnerTransactionID);
            pars[6] = new SqlParameter("@_ReferenceID", ReferenceID);
            pars[7] = new SqlParameter("@_Description", Description);
            pars[8] = new SqlParameter("@_ClientIP", ClientIP);
            pars[9] = new SqlParameter("@_Vcoin", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[10] = new SqlParameter("@_Gift", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[11] = new SqlParameter("@_TotalVcoin", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[12] = new SqlParameter("@_ResponseStatus", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            db.ExecuteNonQuerySP("SP_TopupAccount_FromSmartLink", pars);
            ReturnValue = Convert.ToInt64(pars[12].Value);
            if (ReturnValue >= 0)
            {
                Balance = Convert.ToInt32(pars[9].Value);
                Gift = Convert.ToInt32(pars[10].Value);
                TotalBalance = Convert.ToInt32(pars[11].Value);
            }
        }

        // insert transaction log   27/01/2014
        public void AddLog(long referenceID, int serviceID, long accountID, long amount, string description, string ipAddress, int status)
        {
            float currentBalance = 0;

            DBHelper db = new DBHelper(Configs.VGGBillingLogConnectionStrings);
            SqlParameter[] pars = new SqlParameter[12];
            pars[0] = new SqlParameter("@ReferenceId", referenceID);
            pars[1] = new SqlParameter("@ServiceId", serviceID);
            pars[2] = new SqlParameter("@AccountId", accountID);
            pars[3] = new SqlParameter("@MoneyTranfer", amount);
            pars[4] = new SqlParameter("@CurrentBalance", currentBalance);
            pars[5] = new SqlParameter("@Description", description);
            pars[6] = new SqlParameter("@CreateTime", DateTime.Now);
            pars[7] = new SqlParameter("@Field0", ipAddress);
            pars[8] = new SqlParameter("@Field1", "");
            pars[9] = new SqlParameter("@Field2", "");
            pars[10] = new SqlParameter("@Status", status);
            pars[11] = new SqlParameter("@Id", SqlDbType.BigInt) { Direction = ParameterDirection.Output };

            db.ExecuteNonQuerySP("sp_TransactionLog_Insert", pars);
            ReturnValue = Convert.ToInt64(pars[11].Value);
        }

        public string GetAccessToken(int serviceId, string accountName)
        {
            try
            {
                DBHelper db = new DBHelper(Configs.VGGProfileAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@_SysPartnerKey", Constant.PartnerKey);
            pars[1] = new SqlParameter("@_ServiceID", serviceId);
            pars[2] = new SqlParameter("@_RequestToken", "");
            pars[3] = new SqlParameter("@_AccountName", accountName);
            pars[4] = new SqlParameter("@_AccessToken", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output };
            pars[5] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };

            db.ExecuteNonQuerySP("SP_Account_GenerateAccessToken", pars);
            return Convert.ToString(pars[4].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.Info(string.Format("VGGTopup::GetAccessToken(serviceId={0}, accountName={1}, requestToken={2}):Error={3}"
                    , serviceId, accountName, "", ex.Message));
                return "";
            }
        }

    }
}
