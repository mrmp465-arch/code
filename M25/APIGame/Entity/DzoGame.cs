using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIGame.Entity
{
    public class DzoGame
    {
        public class TopupData
        {
            public object currency { get; set; }
            public double balances { get; set; }
            public double balances_bonus { get; set; }
            public double balanceAdd { get; set; }
            public object transDate { get; set; }
            public double amount { get; set; }
            public object urlCallback { get; set; }
        }

        public class TopupResponse
        {
            public string msg { get; set; }
            public string errorMsg { get; set; }
            public string status { get; set; }
            public object transactionID { get; set; }
            public object accountID { get; set; }
            public object mTransID { get; set; }
            public TopupData data { get; set; }
            public string callBackEvent { get; set; }
        }

        public class Login
        {
            public string msg { get; set; }
            public object errorMsg { get; set; }
            public int status { get; set; }
            public string data { get; set; }
        }

        public class CardRequest
        {
            public string serialCode { get; set; }
            public string serialNumber { get; set; }
            public string partner { get; set; }
            public string rate { get; set; }
            public string capchar { get; set; }
        }
    }
}