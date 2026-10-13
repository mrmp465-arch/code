using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IZISoft.Entity
{
    public class DataTelco
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }

    public class TelcoResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public List<DataTelco> data { get; set; }
    }
}