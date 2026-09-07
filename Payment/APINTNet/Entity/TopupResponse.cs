using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APINTNet.Entity
{

   public class TopupResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public string cardcode { get; set; }
        public string cardseri { get; set; }
        public int amount { get; set; }
        public int id_tran { get; set; }
        public string refcode { get; set; }
        public string creadate { get; set; }
        public string lastdate { get; set; }
    }

}