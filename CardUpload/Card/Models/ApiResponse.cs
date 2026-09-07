using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Card.CMS.Models

{
    public class ApiResponse<T>
    {
        public T Data { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        public string TransId { get; set; }
        public string OrderNo { get; set; }
    }
    public class UserInfo
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class CardInput
    {
        public string telco { get; set; }
        public int cardvalue { get; set; }

        public int cardnumber { get; set; }
        public string refcode { get; set; }
        public string sign { get; set; }
    }
    public class CardOut
    {
        public string pin { get; set; }
        public string seri { get; set; }
       
    }
    public class TopupInput
    {
        public string account { get; set; }
        public string orderNo { get; set; }
        public string telco { get; set; }
        public string topuptype { get; set; }
        public int priority { get; set; }
        public int amount { get; set; }
        public string sign { get; set; }
        public string refcode { get; set; }
        public string cardvalue { get; set; }
        public string password { get; set; }
        

        
    }
}