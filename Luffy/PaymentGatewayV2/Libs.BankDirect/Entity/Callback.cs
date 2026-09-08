using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.BankDirect
{
    public class DataCallback
    {
        public string TransId { get; set; }
        public int Amount { get; set; }
        public string Content { get; set; }
        public string Signature { get; set; }
        public string BankCode { get; set; }
        public string Mobile { get; set; }
        public string mTransId { get; set; }

        public string Result { get; set; }
    }
}
