using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Libs.Utils;

namespace Libs.BankGate
{
    public static class BankGateFactory
    {
        static Dictionary<string, IBankGateHandler> dicts = new Dictionary<string, IBankGateHandler>();

        static BankGateFactory()
        {

            //Unknown class
            Register("unknown", new Unknown.UnknownBank());
            Register("megabank", new MegaBank.MegaBank());


            //---------------------------

        }


        public static void Register(string providerCode, IBankGateHandler hander)
        {

            switch (providerCode)
            {

                case "megabank":
                    {
                        dicts.Add("megabank", hander);
                        break;
                    }

                default:
                    {
                        dicts.Add("unknown".ToLower(), hander);
                        break;
                    }
            }

        }

        public static IBankGateHandler GetHandler(string providerCode)
        {
            try
            {
                switch (providerCode)
                {
                    case "megabank":
                        return dicts["megabank"];
                    default:
                        return dicts["unknown"];
                }


            }

            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
    }
}
