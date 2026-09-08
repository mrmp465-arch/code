using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Libs.Utils
{
    public class PostGetHelper
    {
        private HttpResponse Response;
        private HttpRequest Request;
        private NameValueCollection values = new NameValueCollection();

        public string FormName
        {
            get
            {
                return Get("__TransferData");
            }
            set
            {
                Add("__TransferData", value);
            }
        }

        public PostGetHelper()
        {
            
        }

        public PostGetHelper(HttpRequest request, HttpResponse response)
        {
            Request = request;
            Response = response;
            FormName = "DefaultForm";
        }

        public void Add(string key, string value)
        {
            if (key != null && value != null)
            {
                values.Set(key, value);
            }
        }

        public string Get(string key)
        {
            return values[key];
        }

        public void Clear()
        {
            values.Clear();
        }

        public void ReadPostedData()
        {
            if (Request.Form != null && this.Request.Form["__TransferData"] != null)
            {
                foreach (string key in Request.Form.Keys)
                {
                    Add(key, HttpUtility.HtmlDecode(Request.Form[key]));
                }
            }
        }


        public void RedirectWithData(string urlDestination)
        {
            Response.Clear();
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>");
            sb.Append("<body onload='document.forms[\"form\"].submit()'>");
            sb.AppendFormat("<form name='form' action='{0}' method='post'>", urlDestination);
            foreach (string key in values)
            {
                sb.AppendFormat("<input type='hidden' name='{0}' value='{1}' />", key, HttpUtility.HtmlEncode(Get(key)));
            }
            sb.Append("<noscript>Javascript is disabled, Click submit to proceed.");
            sb.Append("<br/><input type='submit' value='submit'/>");
            sb.Append("</noscript>");
            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");
            Response.Write(sb.ToString());
            Response.End();
        }

        public T GetFromQueryString<T>() where T : new()
        {
            var obj = new T();
            var properties = typeof(T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var valueAsString = HttpContext.Current.Request.QueryString[property.Name];
                var value = Parse(valueAsString, property.PropertyType);

                if (value == null)
                    continue;

                property.SetValue(obj, value, null);
            }
            return obj;
        }

        public object Parse(string valueToConvert, Type dataType)
        {
            TypeConverter obj = TypeDescriptor.GetConverter(dataType);
            object value = obj.ConvertFromString(null, CultureInfo.InvariantCulture, valueToConvert);
            return value;
        }


    }
}
