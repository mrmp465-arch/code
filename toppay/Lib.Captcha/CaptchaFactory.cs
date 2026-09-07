using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lib.Captcha.Anticaptcha;
using Lib.Captcha.CaptchaComVn;
using Lib.Captcha.InhouseCaptcha;
using Libs.Utils;

namespace Lib.Captcha
{
    public static class CaptchaFactory
    {
        static Dictionary<string, ICaptchaFactory> dicts = new Dictionary<string, ICaptchaFactory>();

        static CaptchaFactory()
        {
            Register("captcha.com.vn", new CaptchaComVnService());
            Register("anti-captcha.com", new AnticaptchaService());
            Register("inhouse-captcha", new InhouseCaptchaService());

        }


        public static void Register(string captchaCode, ICaptchaFactory hander)
        {

            switch (captchaCode)
            {

                case "captcha.com.vn":
                    {
                        dicts.Add("captcha.com.vn", hander);
                        break;
                    }
                case "anti-captcha.com":
                    {
                        dicts.Add("anti-captcha.com", hander);
                        break;
                    }
                case "inhouse-captcha":
                {
                    dicts.Add("inhouse-captcha", hander);
                    break;
                }

                default:
                    {
                        dicts.Add("unknown".ToLower(), hander);
                        break;
                    }
            }

        }

        public static ICaptchaFactory GetHandler(string captchaCode)
        {
            try
            {
                switch (captchaCode)
                {
                    case "captcha.com.vn":
                        return dicts["captcha.com.vn"];
                    case "anti-captcha.com":
                        return dicts["anti-captcha.com"];
                    case "inhouse-captcha":
                        return dicts["inhouse-captcha"];
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
