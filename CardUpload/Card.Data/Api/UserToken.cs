using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.Api
{
    public class UserToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
   
    }
    public class TelcoInfo
    {
        public int Id { get; set; }
        public string Password { get; set; }
       
    }
}
