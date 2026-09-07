using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.DTO
{
    [Serializable]
    public class AccountToken
    {
        public int Id { get; set; }
        public string Mobile { get; set; }
        public string Token { get; set; }
        public DateTime Time { get; set; }
    }
}
