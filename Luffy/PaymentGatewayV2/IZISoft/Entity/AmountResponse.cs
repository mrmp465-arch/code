using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IZISoft.Entity
{
    public class DataAmout
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int value { get; set; }
    }

    public class AmountResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public List<DataAmout> data { get; set; }
    }
}