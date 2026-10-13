using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.API
{
   public interface IServiceHandler
    {
        APIResponse Request(APITransaction transaction);
    }
}
