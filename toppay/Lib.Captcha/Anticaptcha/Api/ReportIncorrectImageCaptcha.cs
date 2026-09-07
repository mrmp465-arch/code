using Lib.Captcha.Anticaptcha.ApiResponse;
using Lib.Captcha.Anticaptcha.Helper;
using Newtonsoft.Json.Linq;

namespace Lib.Captcha.Anticaptcha.Api
{
    public class ReportIncorrectImageCaptcha : AnticaptchaBase, IAnticaptchaTaskProtocol
    {
        
        public override JObject GetPostData()
        {
            throw new System.NotImplementedException();
        }

        public TaskResultResponse.SolutionData GetTaskSolution()
        {
            throw new System.NotImplementedException();
        }

        public override JObject GetReportIncorrectImagePostData()
        {
            return new JObject
            {
               
            };
        }
    }
}