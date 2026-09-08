using Lib.Captcha.Anticaptcha.Api;
using Lib.Captcha.Anticaptcha.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Captcha.Anticaptcha
{
    public class AnticaptchaService : ICaptchaFactory
    {

        private string ClientKey = "d03a2e5bf11a326ff257b63d38493e84"; //BB2D
                                                                       //private string ClientKey = "5221e26cd8c370ee7ce6e4c3d0a761a6"; //LongLX

        public int TaskId { get; set; }
        public string Value { get; set; }

        public string ImageToText(string imageBase64, int type)
        {

            DebugHelper.VerboseMode = true;

            var api = new ImageToText
            {
                ClientKey = ClientKey,
                ImageBase64 = imageBase64,
                Case = true

            };

            if (!api.CreateTask())
                DebugHelper.Out("API v2 send failed. " + api.ErrorMessage, DebugHelper.Type.Error);
            else if (!api.WaitForResult())
                DebugHelper.Out("Could not solve the captcha.", DebugHelper.Type.Error);
            else
            {
                DebugHelper.Out("Result: " + api.GetTaskSolution().Text, DebugHelper.Type.Success);
                return string.Format("{0}|{1}", api.GetTaskSolution().Text, api.TaskId);
            }
            return null;
        }

        public string Incorrect(int taskId)
        {
            return null;
        }

        public string NoCaptchaTaskProxyless(string websiteURL, string websiteKey)
        {
            DebugHelper.VerboseMode = true;

            var api = new NoCaptchaProxyless
            {
                ClientKey = ClientKey,
                WebsiteUrl = new Uri(websiteURL),
                WebsiteKey = websiteKey

            };

            if (!api.CreateTask())
                DebugHelper.Out("API v2 send failed. " + api.ErrorMessage, DebugHelper.Type.Error);
            else if (!api.WaitForResult())
                DebugHelper.Out("Could not solve the captcha.", DebugHelper.Type.Error);
            else
            {
                DebugHelper.Out("Result: " + api.GetTaskSolution().GRecaptchaResponse, DebugHelper.Type.Success);
                return string.Format("{0}|{1}", api.GetTaskSolution().GRecaptchaResponse, api.TaskId);
            }
            return null;
        }

        public bool ReportIncorrectImageCaptcha(int taskId)
        {

            DebugHelper.VerboseMode = true;

            var api = new ReportIncorrectImageCaptcha
            {
                ClientKey = ClientKey,
                TaskId = taskId
            };
            if (!api.ReportIncorrectImage())
            {
                DebugHelper.Out("ReportIncorrect Result " + api.TaskId + " : " + api.ErrorMessage, DebugHelper.Type.Error);
                return false;
            }

            return true;
        }


    }
}
