using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.Entity
{
    public class GppTopupRequest
    {
        public string maKhachHang { get; set; }
        public string maTheCao { get; set; }
        public string serial { get; set; }
    }
}