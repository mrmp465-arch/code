using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyVNTP.Entity
{
    public class MyVNTPAppLoginResponse
    {
        public string error_code { get; set; }
        public string session { get; set; }
        public string msisdn { get; set; }
        public string message { get; set; }
    }

    public class Result
    {
        public string text_value { get; set; }
        public string text_date { get; set; }
        public string TOTAL_SPEND_LIMIT { get; set; }
        public int REMAIN { get; set; }
        public string text_right { get; set; }
        public string NEXT_SPEND_LIMIT { get; set; }
        public string BALANCE_NAME { get; set; }
        public string ACC_EXPIRATION { get; set; }
        public string text_left { get; set; }
        public string BAL_NAME { get; set; }
        public string BALANCE { get; set; }
    }

    public class MyVNTPAppBalanceResponse
    {
        public string error_code { get; set; }
        public string message { get; set; }
        public List<Result> result { get; set; }
    }

    public class MyVNTPAppRechargeResponse
    {
        public string error_code { get; set; }
        public string message { get; set; }
        public string result { get; set; }
    }

    public class MyVNTPAppFcmResponse
    {
        public string error_code { get; set; }
        public string message { get; set; }
    }


    public class Balance
    {
        public string origin_acc_name { get; set; }
        public string account_name_vi { get; set; }
        public string account_name_en { get; set; }
        public string comment_vi { get; set; }
        public string comment_en { get; set; }
        public string value { get; set; }
        public string unit { get; set; }
    }

    public class BalanceInfoResponse
    {
        public string error_code { get; set; }
        public string expired_date { get; set; }
        public List<Balance> balance { get; set; }
    }

    public class BalaceCharge
    {
        private DateTime expired_date { get; set; }
        public int Ezpay_Core_Before { get; set; }
        public int Ezpay_Core_After { get; set; }
        public int Hot_Charge_Before { get; set; }
        public int Hot_Charge_After { get; set; }

    }
}