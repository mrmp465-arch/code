using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Utils;

namespace Libs.BankCash
{
    public static class BankCashFactory
    {
        static Dictionary<string, IBankCashHandler> dicts = new Dictionary<string, IBankCashHandler>();

        static BankCashFactory()
        {

            //Unknown class
            Register("unknown", new Unknown.UnknownBankV2());
           
            //Register("drumcash", new Drum.DrumBank());
            Register("drummomocash", new DrumV2.DrumV2Bank());
            Register("drumbankcash", new DrumV2.DrumV2Bank());

            Register("fastpaybankcash", new FastPay.fastbank());
            //Register("vnpaycash", new Jav.JavBank());
            //---------------------------

        }


        public static void Register(string providerCode, IBankCashHandler hander)
        {

            switch (providerCode)
            {

                case "fastpaybankcash":
                    {
                        dicts.Add("fastpaybankcash", hander);
                        break;
                    }
                case "drumbankcash":
                    {
                        dicts.Add("drumbankcash", hander);
                        break;
                    }
               
                case "drummomocash":
                    {
                        dicts.Add("drummomocash", hander);
                        break;
                    }

                default:
                    {
                        dicts.Add("unknown".ToLower(), hander);
                        break;
                    }
            }

        }

        public static IBankCashHandler GetHandler(string providerCode)
        {
            try
            {
                switch (providerCode)
                {
                  
                    case "fastpaybankcash":
                        return dicts["fastpaybankcash"];

                    case "drumbankcash":
                        return dicts["drumbankcash"];
                    case "drummomocash":
                        return dicts["drummomocash"];
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
