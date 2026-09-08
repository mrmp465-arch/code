using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Libs.Db;
using Libs.Utils;

namespace Libs.BankGate.VTCPay
{
    public class VTCPay
    {
        public long TransactionID { get; set; }
        public int WebsiteID { get; set; }
        public string ReceiveAccount { get; set; }
        public string ParamExtend { get; set; }
        public int ResponseAmount { get; set; }
        public int ResponCode { get; set; }
        public int ReturnValue { get; set; }

        private string UrlCheckOut = ConfigurationManager.AppSettings["VTCPay_Url"];
        private string SecretKey = ConfigurationManager.AppSettings["VTCPay_SecretKey"];

        public VTCPay()
        {
            WebsiteID = Convert.ToInt32(ConfigurationManager.AppSettings["VTCPay_WebsiteID"]);
            ReceiveAccount = ConfigurationManager.AppSettings["VTCPay_ReceiveAccount"];
            ParamExtend = "";
        }

        public VTCPay Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<VTCPay>("sp_pay_VTCPay_Select"
                , new SqlParameter("@TransactionID", TransactionID));
        }

        public VTCPay Get(long transactionID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<VTCPay>("sp_pay_VTCPay_Select"
                , new SqlParameter("@TransactionID", transactionID));
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@WebsiteID", WebsiteID);
            pars[3] = new SqlParameter("@ReceiveAccount", ReceiveAccount);
            pars[4] = new SqlParameter("@ParamExtend", ParamExtend);

            db.ExecuteNonQuerySP("sp_pay_VTCPay_Insert", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[4];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@TransactionID", TransactionID);
            pars[2] = new SqlParameter("@ResponseAmount", ResponseAmount);
            pars[3] = new SqlParameter("@ResponCode", ResponCode);

            db.ExecuteNonQuerySP("sp_pay_VTCPay_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        }

        public string GetUrlCheckOut(BankGateAPI bankGateAPI)
        {
            string sign = WebsiteID.ToString() + "-" +
                "1" + "-" +
                 bankGateAPI.TransactionID.ToString() + "-" +
                 bankGateAPI.TotalAmount.ToString() + "-" +
                 ReceiveAccount + "-" +
                 ParamExtend + "-" +
                 SecretKey;

            sign = Encrypts.SHA256(sign);

            return UrlCheckOut + "?website_id=" + WebsiteID.ToString() +
                "&payment_method=" + "1" +
                "&order_code=" + bankGateAPI.TransactionID.ToString() +
                "&amount=" + bankGateAPI.TotalAmount + 
                "&receiver_acc=" + ReceiveAccount + 
                "&customer_name=" +bankGateAPI.FullName +
                "&customer_mobile=" + bankGateAPI.Mobile +
                "&order_des=" + bankGateAPI.OrderInfo + 
                "&param_extend=" + ParamExtend + 
                "&sign=" + sign;
        }


        public string GetUrlCheckOut(BankGateAPI bankGateAPI, string paymentType)
        {
            ParamExtend = paymentType;

            string sign = WebsiteID.ToString() + "-" +
                "1" + "-" +
                 bankGateAPI.TransactionID.ToString() + "-" +
                 bankGateAPI.TotalAmount.ToString() + "-" +
                 ReceiveAccount + "-" +
                 ParamExtend + "-" +
                 SecretKey;

            sign = Encrypts.SHA256(sign);

            return UrlCheckOut + "?website_id=" + WebsiteID.ToString() +
                "&payment_method=" + "1" +
                "&order_code=" + bankGateAPI.TransactionID.ToString() +
                "&amount=" + bankGateAPI.TotalAmount +
                "&receiver_acc=" + ReceiveAccount +
                "&customer_name=" + bankGateAPI.FullName +
                "&customer_mobile=" + bankGateAPI.Mobile +
                "&order_des=" + bankGateAPI.OrderInfo +
                "&param_extend=" + ParamExtend +
                "&sign=" + sign;
        }
    }
}
