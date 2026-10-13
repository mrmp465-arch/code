using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.API
{
    public class MomoReport
    {
        public long Time { get; set; }
        public int TotalIn { get; set; }
        public long TotalAmountIn { get; set; }
        public int TotalOut { get; set; }
        public long TotalAmountOut { get; set; }

        public int TotalCash { get; set; }
        public long TotalAmountCash { get; set; }

        public int TotalTranferInternal { get; set; }
        public long TotalAmountTranferInternal { get; set; }
        public int TotalTranferOutside { get; set; }
        public long TotalAmountTranferOutside { get; set; }
    }
}
