using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Captcha.InhouseCaptcha.ApiResponse
{
   public class TaskResultResponse
    {
        public int status { get; set; }
        public string msg { get; set; }
        public string captcha { get; internal set; }
        
    }
}
