using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.DTO
{
    public class SMSOutbox
    {
        public int Id { get; set; }
        public string Number   { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }

        public int Port { get; set; }

        public DateTime Time { get; set; }
    }
}
