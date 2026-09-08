using Lib.Captcha.Anticaptcha.ApiResponse;
using Newtonsoft.Json.Linq;

namespace Lib.Captcha.Anticaptcha
{
    public interface IAnticaptchaTaskProtocol
    {
        JObject GetPostData();
        TaskResultResponse.SolutionData GetTaskSolution();
        JObject GetReportIncorrectImagePostData();
    }
}