using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.API
{
    public class BankReportV2
    {
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string Type { get; set; }
        public string BankId { get; set; }
        public long TotalIn { get; set; }
        public long TotalAmountIn { get; set; }
      
    }
    public class BankReport
    {
        public long Time { get; set; }
        public long TotalIn { get; set; }
        public long TotalAmountIn { get; set; }
        public long TotalOut { get; set; }
        public long TotalAmountOut { get; set; }

        public long TotalTranfer { get; set; }
        public long TotalAmountTranfer { get; set; }

       
    }
}
