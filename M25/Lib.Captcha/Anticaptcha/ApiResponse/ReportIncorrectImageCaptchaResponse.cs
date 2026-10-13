using Lib.Captcha.Anticaptcha.Helper;

namespace Lib.Captcha.Anticaptcha.ApiResponse
{
    public class ReportIncorrectImageCaptchaResponse
    {
        public ReportIncorrectImageCaptchaResponse(dynamic json)
        {
            errorId = JsonHelper.ExtractInt(json, "errorId");

            if (errorId != null)
            {
                errorId = JsonHelper.ExtractInt(json, "errorId");
                status = JsonHelper.ExtractStr(json, "status");
            }
            else
            {
                DebugHelper.Out("Unknown error", DebugHelper.Type.Error);
            }
        }

        public int? errorId { get; private set; }
        public string status { get; private set; }
    }
}