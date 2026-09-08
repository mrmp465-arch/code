using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.Entity
{

    public class ReturnValue
    {
        public string ma_khach_hang { get; set; }
        public string ma_cong_tac_vien { get; set; }
        public string ma_the_cao { get; set; }
        public string so_seri { get; set; }
        public double menh_gia { get; set; }
        public double so_du_tai_khoan { get; set; }
        public string ngay_nap { get; set; }
    }

    public class Result
    {
        public int code { get; set; }
        public string errorCode { get; set; }
        public string message { get; set; }
        public ReturnValue returnValue { get; set; }
    }

    public class GppTopupResponse
    {
        public Result result { get; set; }
        public object targetUrl { get; set; }
        public bool success { get; set; }
        public object error { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
    }
}