using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Lib.Captcha
{
    public interface ICaptchaFactory
    {
        string ImageToText(string imageBase64, int type);
        //string ReportIncorrectImageCaptcha(int taskId, string imageBase64, int type, string resultCap);
        string NoCaptchaTaskProxyless(string websiteURL, string websiteKey);

    }
}
