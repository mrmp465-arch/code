using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyMobi.Entity
{
    public class Error
    {
        public string code { get; set; }
        public string impact { get; set; }
        public string message { get; set; }

    }

    public class TopupResponse
    {
        public bool data { get; set; }
        public List<Error> errors { get; set; }
        public string card_value { get; set; }
        public int status { get; set; }
    }
}