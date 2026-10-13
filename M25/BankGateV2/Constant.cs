using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BankGateV2
{
    public class Constant
    {
        public static string HOME_ROOT = ConfigurationSettings.AppSettings["HOME_ROOT"];
    }
}