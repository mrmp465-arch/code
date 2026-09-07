using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.DTO
{
    public class Profile
    {
        public long id { get; set; }
        public string username { get; set; }
        public string display_name { get; set; }
        public string phone { get; set; }
    }
}
