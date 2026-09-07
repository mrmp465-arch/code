using System.Web;

namespace SMS.Utility
{
    public class SessionsManager
    {
        public const string SESSION_USERID = "UserID";
        public const string SESSION_USERNAME = "UserName";
        public const string SESSION_USER = "User";
        public const string SESSION_TOKEN = "Token";
        public const string SESSION_USER_FULL = "UserFull";
        public const string SESSION_FUNCTIONS = "Functions";
        public const string SESSION_USERFUNCTIONS = "UserFunctions";
        public const string SESSION_LIST_FUNCTION_DETAIL = "List_Function_Detail";
       
        public const string SESSION_ID = "ID";
        public const string SESSION_PERMISSION = "Permission";
        public const string SESSION_HISTORY = "History";

       
        /// <summary>
        /// lấy ra giá trị session
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetValue(string name)
		{
			return HttpContext.Current.Session[name];
		}




		/// <summary>
		/// hủy một session
		/// </summary>
		/// <param name="name"></param>
		public void Remove(string name)
		{
			HttpContext.Current.Session.Remove(name);

		}




		/// <summary>
		/// Nạp giá trị cho session
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetValue(string name, object value)
		{
			HttpContext.Current.Session[name] = value;
		}
 
    }
}
