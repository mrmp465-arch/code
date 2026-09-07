using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.DTO
{
    public class SMSDictionary
    {
        public int Id { get; set; }
        public string Key   { get; set; }
        public string Value { get; set; }
        public string CreatedUser { get; set; }
    }
}
