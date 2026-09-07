using SMS.API.Auth;
using SMS.API.Models;
using SMS.Data.Factory;
using SMS.Utility;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace SMS.API.Controllers
{
    public class RequestTokenController : ApiController
    {
        [HttpPost]
        public ApiResponse<string> Post(UserInfo user)
        {
            var result = new ApiResponse<string>();
            result.Code = -99;
            result.Message = "Có lỗi trong quá trình xử lý";
            var password = Encrypt.MD5(user.password.Trim() + Config.GetAppsetting("Salt"));
            if (AbstractDAOFactory.Instance().UsersService().Authentication(user.username.Trim(), password) >0)
            {                
              
                result.Code =1;
                result.Data = JwtAuthManager.GenerateJWTToken(user.username.Trim(), 24 * 60 * 10);
                result.Message = "Success";

            }
            else
            {
                
                result.Code = -11;
                result.Message = "Invalid Request";

            }
            return result;
        }
       
    }
}
