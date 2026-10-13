using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.API
{
    public abstract class APIService
    {
        public APIService()
        {

        }

        public abstract APIResponse Request(APITransaction transaction);

   }
}
