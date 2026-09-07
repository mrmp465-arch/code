using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Utils;

namespace Libs.CardTelco
{
    public static class CardTelcoFactory
    {
        static Dictionary<string, ICardTelcoHandler> dicts = new Dictionary<string, ICardTelcoHandler>();

        static CardTelcoFactory()
        {

            //Unknown class
            Register("unknown", new Unknow.UnknownCard());

            ////---Không hóa đơn open rem
            ////ABTPay
            //Register("abtpay15", new ABTPayCard.KHD.ABTPayCard15());
            //Register("abtpay5", new ABTPayCard.KHD.ABTPayCard5());
            //Register("abtpay10", new ABTPayCard.KHD.ABTPayCard10());

            ////Gate
            //Register("gate1", new Gate.KHD.GateCard1());
            //Register("c2c7", new C2C.KHD.C2CCard7());

            ////Home Driect
            //Register("paydirect1", new PayDirectCard.KHD.PayDirectCard1());
            //Register("paydirect5", new PayDirectCard.KHD.PayDirectCard5());
            //Register("paydirect15", new PayDirectCard.KHD.PayDirectCard15());


            ////XBOM
            //Register("xbom5", new XBomCard.KHD.XBomCard5());
            //Register("xbom10", new XBomCard.KHD.XBomCard10());
            //Register("xbom15", new XBomCard.KHD.XBomCard15());

            ////MTop
            //Register("mtop5", new MTop.KHD.MTopCard5());

            ////PaymentNow
            //Register("pnow15", new PaymentNow.KHD.PNowCard15());

            ////1Pay
            //Register("1pay15", new _1Pay.KHD._1PayCard15());

            ////VMG
            //Register("vmg", new VMG.KHD.VMGCard());

            //PayPlus USSD
            Register("ppussd", new PayPlusUSSD.PayPlusCard());
            Register("bigussd", new PayPlusUSSD.PayPlusCard());
            Register("glbussd", new PayPlusUSSD.PayPlusCard());
            Register("mrtussd", new PayPlusUSSD.PayPlusCard());

            //PayPlus APP VTT
            Register("glbappvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("datappvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("mrxappvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("kenappvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("thuyappvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("mraappvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("shoappvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("htoappvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("vanappvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("hocappvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("bigappvtt", new PayPlusAppV2VTT.PayPlusCard());

            //PayPlus Web
            Register("ppweb", new PayPlusWeb.PayPlusCard());
            Register("ppappvtt", new PayPlusAppV1VTT.PayPlusCard());
            Register("ppappvttv2", new PayPlusAppV2VTT.PayPlusCard());


            Register("glbxhr", new PayPlusAppVNTP.PayPlusCard());
            Register("mrxxhr", new PayPlusAppVNTP.PayPlusCard());
            Register("mrdxhr", new PayPlusAppVNTP.PayPlusCard());
            Register("mraxhr", new PayPlusAppVNTP.PayPlusCard());
            Register("datxhr", new PayPlusAppVNTP.PayPlusCard());
            Register("thuyxhr", new PayPlusAppVNTP.PayPlusCard());
            Register("bigxhr", new PayPlusAppVNTP.PayPlusCard());
            Register("htoxhr", new PayPlusAppVNTP.PayPlusCard());
            Register("vanxhr", new PayPlusAppVNTP.PayPlusCard());
            Register("hocxhr", new PayPlusAppVNTP.PayPlusCard());
            Register("ppxhrv2", new PayPlusAppVNTP.PayPlusCard());
            Register("ppvnpgsm", new PayPlusAppVNTP.PayPlusCard());
            Register("shovnp", new PayPlusAppVNTP.PayPlusCard());
            Register("kenvnp", new PayPlusAppVNTP.PayPlusCard());

            //Register("glbxhr", new PayPlusWeb.PayPlusCard());
            //Register("mrxxhr", new PayPlusWeb.PayPlusCard());
            //Register("mrdxhr", new PayPlusWeb.PayPlusCard());
            //Register("mraxhr", new PayPlusWeb.PayPlusCard());
            //Register("datxhr", new PayPlusWeb.PayPlusCard());
            //Register("thuyxhr", new PayPlusWeb.PayPlusCard());
            //Register("bigxhr", new PayPlusWeb.PayPlusCard());
            //Register("htoxhr", new PayPlusWeb.PayPlusCard());
            //Register("vanxhr", new PayPlusWeb.PayPlusCard());
            //Register("hocxhr", new PayPlusWeb.PayPlusCard());
            //Register("ppxhrv2", new PayPlusWeb.PayPlusCard());


            Register("ppappmobi", new PayPlusAppMobi.PayPlusCard());
            Register("datappmobi", new PayPlusAppMobi.PayPlusCard());
            Register("htoappmobi", new PayPlusAppMobi.PayPlusCard());
            Register("hocappmobi", new PayPlusAppMobi.PayPlusCard());
            Register("mrxappmobi", new PayPlusAppMobi.PayPlusCard());
            Register("vanappmobi", new PayPlusAppMobi.PayPlusCard());
            Register("bigappmobi", new PayPlusAppMobi.PayPlusCard());
            Register("shoappmobi", new PayPlusAppMobi.PayPlusCard());
            Register("mraappmobi", new PayPlusAppMobi.PayPlusCard());
            Register("thuyappmobi", new PayPlusAppMobi.PayPlusCard());
            Register("ppvmsgsm", new PayPlusAppMobi.PayPlusCard());
            Register("hubvms", new VinaHub.VinaHubCard());
            Register("chuappmobi", new PayPlusAppMobi.PayPlusCard());
            //Global
            Register("global", new Global.GlobalCard());

            //Gate
            Register("mobo", new MoBo.MoBoCard());

            //ezpay
            Register("ezpayvnp", new EZPay.EZPayCard());
            Register("ezpayvms", new EZPay.EZPayCard());
            Register("ezpayvtt", new EZPay.EZPayCard());

            //gosu
            Register("gosu", new PayPlusGame.PayPlusCard());
            Register("shogosu", new PayPlusGame.PayPlusCard());

            //Zing
            Register("ppzing", new PayPlusGame.PayPlusCard());
            Register("shozing", new PayPlusGame.PayPlusCard());
            Register("datzing", new PayPlusGame.PayPlusCard());
            Register("mrxzing", new PayPlusGame.PayPlusCard());
            Register("kenzing", new PayPlusGame.PayPlusCard());
            

            //DZO
            Register("ppdzo", new PayPlusGame.PayPlusCard());
            Register("shodzo", new PayPlusGame.PayPlusCard());

            //VTC
            Register("ppvtc", new PayPlusGame.PayPlusCard());
            Register("shovtc", new PayPlusGame.PayPlusCard());

            //Garena
            Register("shogarena", new PayPlusGame.PayPlusCard());
            Register("ppgarena", new PayPlusGame.PayPlusCard());
            Register("datgarena", new PayPlusGame.PayPlusCard());
            Register("mrxgarena", new PayPlusGame.PayPlusCard());

            //The24
            Register("the24vtt", new TheCao24.The24Card());
            Register("the24vms", new TheCao24.The24Card());
            Register("the24vnp", new TheCao24.The24Card());

            //NapTien247
            Register("ntnetvms", new NapTien247.NapTien247Card());
            Register("ntnetvtt", new NapTien247.NapTien247Card());
            Register("ntnetvnp", new NapTien247.NapTien247Card());

            //NapTien247
            Register("izivms", new IZISoft.IZISoftCard());
            Register("izivtt", new IZISoft.IZISoftCard());
            Register("izivnp", new IZISoft.IZISoftCard());

            //Push247
            Register("pushvms", new Push247.Push247Card());
            Register("pushvtt", new Push247.Push247Card());
            Register("pushvnp", new Push247.Push247Card());

            //Mon
            Register("monvms", new Mon.MonCard());
            Register("monvtt", new Mon.MonCard());
            Register("monvnp", new Mon.MonCard());

            //Pon
            Register("ponvms", new PayPlusAppMobi.PayPlusCard());
            Register("ponvtt", new PayPlusAppV2VTT.PayPlusCard());
            Register("ponvnp", new PayPlusAppVNTP.PayPlusCard());

            //Cr7
            Register("cr7vms", new Cr7.Cr7Card());
            Register("cr7vtt", new Cr7.Cr7Card());
            Register("cr7vnp", new Cr7.Cr7Card());

            //newCr7
            Register("p17vtt", new VinaPay.VinaPayCard());
            Register("p17vnp", new VinaPay.VinaPayCard());
            Register("p17vms", new VinaPay.VinaPayCard());

            //vitetool
            Register("vitevtt", new Vite.ViteCard());
            Register("vitevnp", new Vite.ViteCard());
            Register("vitevms", new Vite.ViteCard());

            //biden
            Register("shipvtt", new VinaPay.VinaPayCard());
            Register("shipvnp", new VinaPay.VinaPayCard());
            Register("shipvms", new VinaPay.VinaPayCard());

            //Khoai
            Register("khoaivms", new Khoai.KhoaiCard());
            Register("khoaivtt", new Khoai.KhoaiCard());
            Register("khoaivnp", new Khoai.KhoaiCard());
            Register("khoaizing", new Khoai.KhoaiCard());

            //Tiger
            Register("tigervms", new Tiger.TigerCard());
            Register("tigervtt", new Tiger.TigerCard());
            Register("tigervnp", new Tiger.TigerCard());
            //---------------------------
            Register("huyvms", new Huy.HuyCard());
            //---Có hóa đơn open rem
            Register("bicbiczing", new BicBic.BicBicCard());
            Register("bicbicvms", new BicBic.BicBicCard());
            Register("bicbicvtt", new BicBic.BicBicCard());
            Register("bicbicvnp", new BicBic.BicBicCard());
            Register("bicbicvcoin", new BicBic.BicBicCard());
            
            ////Gate
            Register("ppgate", new PayPlusGame.PayPlusCard());
            Register("b2bin", new PayPlusGame.PayPlusCard());

            ////Home Driect
            //Register("paydirect30", new PayDirectCard.CHD.PayDirectCard30());

            //Register("intecom30", new IntecomV2.CHD.IntecomV2Card30());

            //---------------------------

        }


        public static void Register(string providerCode, ICardTelcoHandler hander)
        {

            switch (providerCode)
            {
                //case "abtpay5":
                //    {
                //        dicts.Add("abtpay5", hander);
                //        break;
                //    }
                //case "abtpay10":
                //    {
                //        dicts.Add("abtpay10", hander);
                //        break;
                //    }
                //case "abtpay15":
                //    {
                //        dicts.Add("abtpay15", hander);
                //        break;
                //    }

                //case "gate1":
                //    {
                //        dicts.Add("gate1", hander);
                //        break;
                //    }

                //case "gate7":
                //    {
                //        dicts.Add("gate7", hander);
                //        break;
                //    }
                //case "gate15":
                //    {
                //        dicts.Add("gate15", hander);
                //        break;
                //    }
                //case "gatesb":
                //    {
                //        dicts.Add("gatesb", hander);
                //        break;
                //    }
                //case "gate30":
                //    {
                //        dicts.Add("gate30", hander);
                //        break;
                //    }
                //case "c2c7":
                //{
                //    dicts.Add("c2c7", hander);
                //    break;
                //}

                //case "paydirect1":
                //    {
                //        dicts.Add("paydirect1", hander);
                //        break;
                //    }
                //case "paydirect5":
                //    {
                //        dicts.Add("paydirect5", hander);
                //        break;
                //    }
                //case "paydirect15":
                //    {
                //        dicts.Add("paydirect15", hander);
                //        break;
                //    }
                //case "paydirect30":
                //    {
                //        dicts.Add("paydirect30", hander);
                //        break;
                //    }

                //case "xbom5":
                //    {
                //        dicts.Add("xbom5", hander);
                //        break;
                //    }
                //case "xbom10":
                //    {
                //        dicts.Add("xbom10", hander);
                //        break;
                //    }
                //case "xbom15":
                //    {
                //        dicts.Add("xbom15", hander);
                //        break;
                //    }
                //case "mtop5":
                //    {
                //        dicts.Add("mtop5", hander);
                //        break;
                //    }
                //case "pnow15":
                //    {
                //        dicts.Add("pnow15", hander);
                //        break;
                //    }
                //case "1pay15":
                //    {
                //        dicts.Add("1pay15", hander);
                //        break;
                //    }
                //case "vmg":
                //    {
                //        dicts.Add("vmg", hander);
                //        break;
                //    }
                case "ppussd":
                    {
                        dicts.Add("ppussd", hander);
                        break;
                    }
                case "bigussd":
                    {
                        dicts.Add("bigussd", hander);
                        break;
                    }
                case "glbussd":
                    {
                        dicts.Add("glbussd", hander);
                        break;
                    }
                case "mrtussd":
                    {
                        dicts.Add("mrtussd", hander);
                        break;
                    }

                case "glbxhr":
                    {
                        dicts.Add("glbxhr", hander);
                        break;
                    }
                case "mrxxhr":
                    {
                        dicts.Add("mrxxhr", hander);
                        break;
                    }
                case "mrdxhr":
                    {
                        dicts.Add("mrdxhr", hander);
                        break;
                    }
                case "mraxhr":
                    {
                        dicts.Add("mraxhr", hander);
                        break;
                    }
                case "datxhr":
                    {
                        dicts.Add("datxhr", hander);
                        break;
                    }
                case "thuyxhr":
                    {
                        dicts.Add("thuyxhr", hander);
                        break;
                    }
                case "bigxhr":
                    {
                        dicts.Add("bigxhr", hander);
                        break;
                    }
                case "htoxhr":
                    {
                        dicts.Add("htoxhr", hander);
                        break;
                    }
                case "vanxhr":
                    {
                        dicts.Add("vanxhr", hander);
                        break;
                    }
                case "hocxhr":
                    {
                        dicts.Add("hocxhr", hander);
                        break;
                    }
                case "shovnp":
                {
                    dicts.Add("shovnp", hander);
                    break;
                }
                case "kenvnp":
                {
                    dicts.Add("kenvnp", hander);
                    break;
                }
                case "ppweb":
                    {
                        dicts.Add("ppweb", hander);
                        break;
                    }
                case "ppxhrv2":
                    {
                        dicts.Add("ppxhrv2", hander);
                        break;
                    }
                case "ppvnpgsm":
                    {
                        dicts.Add("ppvnpgsm", hander);
                        break;
                    }

                case "ppappvtt":
                    {
                        dicts.Add("ppappvtt", hander);
                        break;
                    }
                case "glbappvtt":
                    {
                        dicts.Add("glbappvtt", hander);
                        break;
                    }
                case "datappvtt":
                    {
                        dicts.Add("datappvtt", hander);
                        break;
                    }
                case "mrxappvtt":
                    {
                        dicts.Add("mrxappvtt", hander);
                        break;
                    }
                case "kenappvtt":
                    {
                        dicts.Add("kenappvtt", hander);
                        break;
                    }
                case "thuyappvtt":
                    {
                        dicts.Add("thuyappvtt", hander);
                        break;
                    }
                case "mraappvtt":
                    {
                        dicts.Add("mraappvtt", hander);
                        break;
                    }
                case "shoappvtt":
                    {
                        dicts.Add("shoappvtt", hander);
                        break;
                    }
                case "htoappvtt":
                    {
                        dicts.Add("htoappvtt", hander);
                        break;
                    }
                case "vanappvtt":
                    {
                        dicts.Add("vanappvtt", hander);
                        break;
                    }
                case "hocappvtt":
                    {
                        dicts.Add("hocappvtt", hander);
                        break;
                    }
                case "bigappvtt":
                    {
                        dicts.Add("bigappvtt", hander);
                        break;
                    }

                case "ppappvttv2":
                    {
                        dicts.Add("ppappvttv2", hander);
                        break;
                    }

                case "ppappmobi":
                    {
                        dicts.Add("ppappmobi", hander);
                        break;
                    }
                case "datappmobi":
                    {
                        dicts.Add("datappmobi", hander);
                        break;
                    }
                case "htoappmobi":
                    {
                        dicts.Add("htoappmobi", hander);
                        break;
                    }
                case "chuappmobi":
                    {
                        dicts.Add("chuappmobi", hander);
                        break;
                    }
                case "hocappmobi":
                    {
                        dicts.Add("hocappmobi", hander);
                        break;
                    }
                case "mrxappmobi":
                    {
                        dicts.Add("mrxappmobi", hander);
                        break;
                    }
                case "mraappmobi":
                    {
                        dicts.Add("mraappmobi", hander);
                        break;
                    }
                case "vanappmobi":
                    {
                        dicts.Add("vanappmobi", hander);
                        break;
                    }
                case "bigappmobi":
                    {
                        dicts.Add("bigappmobi", hander);
                        break;
                    }
                case "shoappmobi":
                    {
                        dicts.Add("shoappmobi", hander);
                        break;
                    }
                case "thuyappmobi":
                    {
                        dicts.Add("thuyappmobi", hander);
                        break;
                    }
                case "ppvmsgsm":
                    {
                        dicts.Add("ppvmsgsm", hander);
                        break;
                    }

                case "global":
                    {
                        dicts.Add("global", hander);
                        break;
                    }

                case "mobo":
                    {
                        dicts.Add("mobo", hander);
                        break;
                    }

                case "ezpayvnp":
                    {
                        dicts.Add("ezpayvnp", hander);
                        break;
                    }
                case "ezpayvms":
                    {
                        dicts.Add("ezpayvms", hander);
                        break;
                    }
                case "ezpayvtt":
                    {
                        dicts.Add("ezpayvtt", hander);
                        break;
                    }

                case "gosu":
                    {
                        dicts.Add("gosu", hander);
                        break;
                    }
                case "shogosu":
                    {
                        dicts.Add("shogosu", hander);
                        break;
                    }

                case "ppzing":
                    {
                        dicts.Add("ppzing", hander);
                        break;
                    }
                case "shozing":
                    {
                        dicts.Add("shozing", hander);
                        break;
                    }
                case "datzing":
                    {
                        dicts.Add("datzing", hander);
                        break;
                    }
                case "mrxzing":
                    {
                        dicts.Add("mrxzing", hander);
                        break;
                    }
                case "kenzing":
                {
                    dicts.Add("kenzing", hander);
                    break;
                }

                case "ppgarena":
                    {
                        dicts.Add("ppgarena", hander);
                        break;
                    }
                case "datgarena":
                    {
                        dicts.Add("datgarena", hander);
                        break;
                    }
                case "mrxgarena":
                    {
                        dicts.Add("mrxgarena", hander);
                        break;
                    }
                case "shogarena":
                    {
                        dicts.Add("shogarena", hander);
                        break;
                    }

                case "the24vtt":
                    {
                        dicts.Add("the24vtt", hander);
                        break;
                    }
                case "the24vms":
                    {
                        dicts.Add("the24vms", hander);
                        break;
                    }
                case "the24vnp":
                    {
                        dicts.Add("the24vnp", hander);
                        break;
                    }

                case "ntnetvms":
                    {
                        dicts.Add("ntnetvms", hander);
                        break;
                    }
                case "ntnetvtt":
                    {
                        dicts.Add("ntnetvtt", hander);
                        break;
                    }
                case "ntnetvnp":
                    {
                        dicts.Add("ntnetvnp", hander);
                        break;
                    }


                case "izivms":
                    {
                        dicts.Add("izivms", hander);
                        break;
                    }
                case "izivtt":
                    {
                        dicts.Add("izivtt", hander);
                        break;
                    }
                case "izivnp":
                    {
                        dicts.Add("izivnp", hander);
                        break;
                    }

                case "pushvms":
                    {
                        dicts.Add("pushvms", hander);
                        break;
                    }
                case "pushvtt":
                    {
                        dicts.Add("pushvtt", hander);
                        break;
                    }
                case "pushvnp":
                    {
                        dicts.Add("pushvnp", hander);
                        break;
                    }
                case "cr7vms":
                    {
                        dicts.Add("cr7vms", hander);
                        break;
                    }
                case "cr7vtt":
                    {
                        dicts.Add("cr7vtt", hander);
                        break;
                    }
                case "cr7vnp":
                    {
                        dicts.Add("cr7vnp", hander);
                        break;
                    }
                case "p17vtt":
                    {
                        dicts.Add("p17vtt", hander);
                        break;
                    }
                case "p17vnp":
                    {
                        dicts.Add("p17vnp", hander);
                        break;
                    }
                case "p17vms":
                    {
                        dicts.Add("p17vms", hander);
                        break;
                    }
                case "shipvms":
                    {
                        dicts.Add("shipvms", hander);
                        break;
                    }
                case "shipvtt":
                    {
                        dicts.Add("shipvtt", hander);
                        break;
                    }
                case "shipvnp":
                    {
                        dicts.Add("shipvnp", hander);
                        break;
                    }
                case "vitevms":
                    {
                        dicts.Add("vitevms", hander);
                        break;
                    }

                case "vitevtt":
                    {
                        dicts.Add("vitevtt", hander);
                        break;
                    }
                case "vitevnp":
                    {
                        dicts.Add("vitevnp", hander);
                        break;
                    }
               

                case "monvms":
                    {
                        dicts.Add("monvms", hander);
                        break;
                    }
                case "monvtt":
                    {
                        dicts.Add("monvtt", hander);
                        break;
                    }
                case "monvnp":
                    {
                        dicts.Add("monvnp", hander);
                        break;
                    }

                case "hubvms":
                    {
                        dicts.Add("hubvms", hander);
                        break;
                    }

                case "ponvms":
                    {
                        dicts.Add("ponvms", hander);
                        break;
                    }
                case "ponvtt":
                    {
                        dicts.Add("ponvtt", hander);
                        break;
                    }
                case "ponvnp":
                    {
                        dicts.Add("ponvnp", hander);
                        break;
                    }

                case "ppvtc":
                    {
                        dicts.Add("ppvtc", hander);
                        break;
                    }
                case "shovtc":
                    {
                        dicts.Add("shovtc", hander);
                        break;
                    }

                case "ppdzo":
                {
                    dicts.Add("ppdzo", hander);
                    break;
                }

                case "ppgate":
                {
                    dicts.Add("ppgate", hander);
                    break;
                }
                case "b2bin":
                {
                    dicts.Add("b2bin", hander);
                    break;
                }

                case "shodzo":
                {
                    dicts.Add("shodzo", hander);
                    break;
                }

                case "khoaivms":
                {
                    dicts.Add("khoaivms", hander);
                    break;
                }
                case "khoaivtt":
                {
                    dicts.Add("khoaivtt", hander);
                    break;
                }
                case "khoaivnp":
                {
                    dicts.Add("khoaivnp", hander);
                    break;
                }
                case "khoaizing":
                    {
                        dicts.Add("khoaizing", hander);
                        break;
                    }
                case "bicbiczing":
                    {
                        dicts.Add("bicbiczing", hander);
                        break;
                    }
                case "bicbicvtt":
                    {
                        dicts.Add("bicbicvtt", hander);
                        break;
                    }
                case "bicbicvnp":
                    {
                        dicts.Add("bicbicvnp", hander);
                        break;
                    }
                case "bicbicvms":
                    {
                        dicts.Add("bicbicvms", hander);
                        break;
                    }
                case "bicbicvcoin":
                    {
                        dicts.Add("bicbicvcoin", hander);
                        break;
                    }
                case "tigervms":
                    {
                        dicts.Add("tigervms", hander);
                        break;
                    }
                case "tigervtt":
                    {
                        dicts.Add("tigervtt", hander);
                        break;
                    }
                case "tigervnp":
                    {
                        dicts.Add("tigervnp", hander);
                        break;
                    }
                case "huyvms":
                    {
                        dicts.Add("huyvms", hander);
                        break;
                    }
                default:
                    {
                        dicts.Add("unknown".ToLower(), hander);
                        break;
                    }
            }

        }

        public static ICardTelcoHandler GetHandler(string providerCode)
        {
            try
            {
                switch (providerCode)
                {
                    //case "abtpay5":
                    //    return dicts["abtpay5"];
                    //case "abtpay10":
                    //    return dicts["abtpay10"];
                    //case "abtpay15":
                    //    return dicts["abtpay15"];
                    //case "gate1":
                    //    return dicts["gate1"];
                    //case "gate7":
                    //    return dicts["gate7"];
                    //case "gate15":
                    //    return dicts["gate15"];
                    //case "gatesb":
                    //    return dicts["gatesb"];
                    //case "gate30":
                    //    return dicts["gate30"];
                    //case "c2c7":
                    //    return dicts["c2c7"];
                    //case "paydirect1":
                    //    return dicts["paydirect1"];
                    //case "paydirect5":
                    //    return dicts["paydirect5"];
                    //case "paydirect15":
                    //    return dicts["paydirect15"];
                    //case "paydirect30":
                    //    return dicts["paydirect30"];
                    //case "xbom5":
                    //    return dicts["xbom5"];
                    //case "xbom10":
                    //    return dicts["xbom10"];
                    //case "xbom15":
                    //    return dicts["xbom15"];
                    //case "mtop5":
                    //    return dicts["mtop5"];
                    //case "pnow15":
                    //    return dicts["pnow15"];
                    //case "1pay15":
                    //    return dicts["1pay15"];
                    //case "vmg":
                    //    return dicts["vmg"];
                    case "ppussd":
                        return dicts["ppussd"];
                    case "bigussd":
                        return dicts["bigussd"];
                    case "glbussd":
                        return dicts["glbussd"];
                    case "mrtussd":
                        return dicts["mrtussd"];

                    case "glbxhr":
                        return dicts["glbxhr"];
                    case "mrxxhr":
                        return dicts["mrxxhr"];
                    case "mrdxhr":
                        return dicts["mrdxhr"];
                    case "mraxhr":
                        return dicts["mraxhr"];
                    case "datxhr":
                        return dicts["datxhr"];
                    case "thuyxhr":
                        return dicts["thuyxhr"];
                    case "bigxhr":
                        return dicts["bigxhr"];
                    case "htoxhr":
                        return dicts["htoxhr"];
                    case "vanxhr":
                        return dicts["vanxhr"];
                    case "hocxhr":
                        return dicts["hocxhr"];
                    case "ppweb":
                        return dicts["ppweb"];
                    case "ppxhrv2":
                        return dicts["ppxhrv2"];
                    case "ppvnpgsm":
                        return dicts["ppvnpgsm"];
                    case "shovnp":
                        return dicts["shovnp"];
                    case "kenvnp":
                        return dicts["kenvnp"];

                    case "ppappvtt":
                        return dicts["ppappvtt"];
                    case "glbappvtt":
                        return dicts["glbappvtt"];
                    case "datappvtt":
                        return dicts["datappvtt"];
                    case "mrxappvtt":
                        return dicts["mrxappvtt"];
                    case "kenappvtt":
                        return dicts["kenappvtt"];
                    case "thuyappvtt":
                        return dicts["thuyappvtt"];
                    case "mraappvtt":
                        return dicts["mraappvtt"];
                    case "shoappvtt":
                        return dicts["shoappvtt"];
                    case "htoappvtt":
                        return dicts["htoappvtt"];
                    case "vanappvtt":
                        return dicts["vanappvtt"];
                    case "hocappvtt":
                        return dicts["hocappvtt"];
                    case "bigappvtt":
                        return dicts["bigappvtt"];

                    case "ppappvttv2":
                        return dicts["ppappvttv2"];

                    case "ppappmobi":
                        return dicts["ppappmobi"];
                    case "datappmobi":
                        return dicts["datappmobi"];
                    case "htoappmobi":
                        return dicts["htoappmobi"];
                    case "chuappmobi":
                        return dicts["chuappmobi"];
                    case "hocappmobi":
                        return dicts["hocappmobi"];
                    case "mrxappmobi":
                        return dicts["mrxappmobi"];
                    case "mraappmobi":
                        return dicts["mraappmobi"];
                    case "vanappmobi":
                        return dicts["vanappmobi"];
                    case "bigappmobi":
                        return dicts["bigappmobi"];
                    case "shoappmobi":
                        return dicts["shoappmobi"];
                    case "thuyappmobi":
                        return dicts["thuyappmobi"];
                    case "ppvmsgsm":
                        return dicts["ppvmsgsm"];

                    case "global":
                        return dicts["global"];

                    case "mobo":
                        return dicts["mobo"];

                    case "ezpayvnp":
                        return dicts["ezpayvnp"];
                    case "ezpayvms":
                        return dicts["ezpayvms"];
                    case "ezpayvtt":
                        return dicts["ezpayvtt"];

                    case "gosu":
                        return dicts["gosu"];
                    case "shogosu":
                        return dicts["shogosu"];

                    case "ppzing":
                        return dicts["ppzing"];
                    case "shozing":
                        return dicts["shozing"];
                    case "datzing":
                        return dicts["datzing"];
                    case "mrxzing":
                        return dicts["mrxzing"];
                    case "kenzing":
                        return dicts["kenzing"];

                    case "ppgarena":
                        return dicts["ppgarena"];
                    case "datgarena":
                        return dicts["datgarena"];
                    case "mrxgarena":
                        return dicts["mrxgarena"];
                    case "shogarena":
                        return dicts["shogarena"];

                    case "the24vtt":
                        return dicts["the24vtt"];
                    case "the24vms":
                        return dicts["the24vms"];
                    case "the24vnp":
                        return dicts["the24vnp"];

                    case "ntnetvms":
                        return dicts["ntnetvms"];
                    case "ntnetvtt":
                        return dicts["ntnetvtt"];
                    case "ntnetvnp":
                        return dicts["ntnetvnp"];

                    case "izivms":
                        return dicts["izivms"];
                    case "izivtt":
                        return dicts["izivtt"];
                    case "izivnp":
                        return dicts["izivnp"];

                    case "pushvms":
                        return dicts["pushvms"];
                    case "pushvtt":
                        return dicts["pushvtt"];
                    case "pushvnp":
                        return dicts["pushvnp"];

                    case "cr7vms":
                        return dicts["cr7vms"];
                    case "cr7vtt":
                        return dicts["cr7vtt"];
                    case "cr7vnp":
                        return dicts["cr7vnp"];

                    case "p17vms":
                        return dicts["p17vms"];
                    case "p17vtt":
                        return dicts["p17vtt"];
                    case "p17vnp":
                        return dicts["p17vnp"];

                    case "vitevms":
                        return dicts["vitevms"];
                    case "vitevtt":
                        return dicts["vitevtt"];
                    case "vitevnp":
                        return dicts["vitevnp"];

                    case "shipvms":
                        return dicts["shipvms"];
                    case "shipvtt":
                        return dicts["shipvtt"];
                    case "shipvnp":
                        return dicts["shipvnp"];

                    case "monvms":
                        return dicts["monvms"];
                    case "monvtt":
                        return dicts["monvtt"];
                    case "monvnp":
                        return dicts["monvnp"];

                    case "ponvms":
                        return dicts["ponvms"];
                    case "ponvtt":
                        return dicts["ponvtt"];
                    case "ponvnp":
                        return dicts["ponvnp"];

                    case "hubvms":
                        return dicts["hubvms"];

                    case "ppvtc":
                        return dicts["ppvtc"];
                    case "shovtc":
                        return dicts["shovtc"];

                    case "ppdzo":
                        return dicts["ppdzo"];
                    case "shodzo":
                        return dicts["shodzo"];

                    case "ppgate":
                        return dicts["ppgate"];
                    case "b2bin":
                        return dicts["b2bin"];

                    case "khoaivms":
                        return dicts["khoaivms"];
                    case "khoaivtt":
                        return dicts["khoaivtt"];
                    case "khoaivnp":
                        return dicts["khoaivnp"];
                    case "khoaizing":
                        return dicts["khoaizing"];
                    case "bicbiczing":
                        return dicts["bicbiczing"];
                    case "bicbicvtt":
                        return dicts["bicbicvtt"];
                    case "bicbicvms":
                        return dicts["bicbicvms"];
                    case "bicbicvnp":
                        return dicts["bicbicvnp"];
                    case "bicbicvcoin":
                        return dicts["bicbicvcoin"];
                    case "tigervms":
                        return dicts["tigervms"];
                    case "tigervtt":
                        return dicts["tigervtt"];
                    case "tigervnp":
                        return dicts["tigervnp"];
                    case "huyvms":
                        return dicts["huyvms"];
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
