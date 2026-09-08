using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.Entity
{
    public class Datum
    {
        public string CODE { get; set; }
        public int DAI_LY_ID { get; set; }
        public int LICH_SU_NAP_TIEN_DAI_LY_ID { get; set; }
        public string MA_CONG_TAC_VIEN { get; set; }
        public string MA_THE_CAO { get; set; }
        public string MESSAGE { get; set; }
        public DateTime NGAY_NAP { get; set; }
        public string SO_SERI { get; set; }
        public int SO_TIEN { get; set; }
        public string TEN_DANG_NHAP { get; set; }
        public string TOP_UP_RESULT { get; set; }
        public int LOAI_GIAO_DICH { get; set; }
    }

    public class TcVncdcTopupResponse
    {
        public int draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public List<Datum> data { get; set; }
    }

}