using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Libs.BankGate.MegaBank
{

    public static class BankCode
    {

        public static Dictionary<String, String> Bank()
        {
            Dictionary<String, String> bank = new Dictionary<string, string>
            {
                {"vcb", "999999"}, //Vietcombank
                

            };

            return bank;
        }

        //public static string KeyByValue(Dictionary<string, string> dict, string val)
        //{
        //    string key = null;
        //    foreach (KeyValuePair<string, string> pair in dict)
        //    {
        //        if (pair.Value == val)
        //        {
        //            key = pair.Key;
        //            break;
        //        }
        //    }
        //    return key;
        //}
    }

}
