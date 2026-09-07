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
            Register("24hbankcash", new _24h._24hBank());
            //Register("kzmomocash", new KZ.KZBank());
            //Register("drumcash", new Drum.DrumBank());
            Register("drummomocash", new DrumV2.DrumV2Bank());
            Register("drumbankcash", new DrumV2.DrumV2Bank());
            Register("vnpaycash", new Jav.JavBank());
            Register("simexcash", new Simex.SimexBank());
            //---------------------------

        }


        public static void Register(string providerCode, IBankCashHandler hander)
        {

            switch (providerCode)
            {

                case "vnpaycash":
                    {
                        dicts.Add("vnpaycash", hander);
                        break;
                    }
                case "24hbankcash":
                    {
                        dicts.Add("24hbankcash", hander);
                        break;
                    }
                case "drumbankcash":
                    {
                        dicts.Add("drumbankcash", hander);
                        break;
                    }
                case "drumcash":
                    {
                        dicts.Add("drumcash", hander);
                        break;
                    }
                case "drummomocash":
                    {
                        dicts.Add("drummomocash", hander);
                        break;
                    }
                case "simexcash":
                    {
                        dicts.Add("simexcash", hander);
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
                    case "vnpaycash":
                        return dicts["vnpaycash"];
                    case "24hbankcash":
                        return dicts["24hbankcash"];
                    case "drumbankcash":
                        return dicts["drumbankcash"];

                    case "drumcash":
                        return dicts["drumcash"];
                    case "drummomocash":
                        return dicts["drummomocash"];
                    case "simexcash":
                        return dicts["simexcash"];
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
