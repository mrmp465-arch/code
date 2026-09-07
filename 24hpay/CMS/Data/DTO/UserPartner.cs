using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Data.DTO
{
    public class UserPartner
    {
        public int UserPartnerId { get; set; }
        public int UserId { get; set; }
        public int PartnerId { get; set; }
        public string PartnerCode { get; set; }
    }
}