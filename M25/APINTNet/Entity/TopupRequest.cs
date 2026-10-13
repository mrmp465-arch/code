using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APINTNet.Entity
{
    public class TopupRequest
    {

        public string usercode { get; set; }
        public string telco { get; set; }
        public string cardcode { get; set; }
        public string cardseri { get; set; }
        public string refcode { get; set; }
        public int amount { get; set; }
        public string acconame { get; set; }
        public string appcode { get; set; }
        public string callurl { get; set; }
        public string sign { get; set; }
    }
}