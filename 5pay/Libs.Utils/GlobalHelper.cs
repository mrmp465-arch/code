using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Libs.Utils
{
    public class GlobalHelper
    {
        public static string GetLanguage()
        {
            var request = System.Web.HttpContext.Current.Request;
            var lang = "vi-vn";
            if (request.Cookies["lang"] != null)
            {
                lang = request.Cookies["lang"].Value.ToString().ToLowerInvariant();
            }
            if (lang != "vi-vn" && lang != "en-us")
            {
                lang = "vi-vn";
            }
            return lang;
        }
        public static void SetLanguage(string value)
        {
            HttpCookie ck = new HttpCookie("lang", value) { HttpOnly = true, Path = " / " };
            ck.Expires = DateTime.Now.AddYears(1);
            HttpContext.Current.Request.Cookies.Set(ck);
            HttpContext.Current.Response.Cookies.Set(ck);
        }
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                    }
                }
                return objT;
            }).ToList();
        }
        public static int? TryParseNullable(string val)
        {
            int outValue;
            return int.TryParse(val, out outValue) ? (int?)outValue : null;
        }
        public static string ReplaceVietnameseChar(string s)
        {
            if (string.IsNullOrEmpty(s))
                return String.Empty;
            // replace specification character
            s = s.Trim().ToLower();
            s = s.Replace('á', 'a');
            s = s.Replace('à', 'a');
            s = s.Replace('ả', 'a');
            s = s.Replace('ã', 'a');
            s = s.Replace('ạ', 'a');
            s = s.Replace('ă', 'a');
            s = s.Replace('ắ', 'a');
            s = s.Replace('ằ', 'a');
            s = s.Replace('ẳ', 'a');
            s = s.Replace('ẵ', 'a');
            s = s.Replace('ặ', 'a');
            s = s.Replace('â', 'a');
            s = s.Replace('ấ', 'a');
            s = s.Replace('ầ', 'a');
            s = s.Replace('ẩ', 'a');
            s = s.Replace('ẫ', 'a');
            s = s.Replace('ậ', 'a');
            s = s.Replace('é', 'e');
            s = s.Replace('è', 'e');
            s = s.Replace('ẻ', 'e');
            s = s.Replace('ẽ', 'e');
            s = s.Replace('ẹ', 'e');
            s = s.Replace('ê', 'e');
            s = s.Replace('ế', 'e');
            s = s.Replace('ề', 'e');
            s = s.Replace('ể', 'e');
            s = s.Replace('ễ', 'e');
            s = s.Replace('ệ', 'e');
            s = s.Replace('í', 'i');
            s = s.Replace('ì', 'i');
            s = s.Replace('ỉ', 'i');
            s = s.Replace('ĩ', 'i');
            s = s.Replace('ị', 'i');
            s = s.Replace('ó', 'o');
            s = s.Replace('ò', 'o');
            s = s.Replace('ỏ', 'o');
            s = s.Replace('õ', 'o');
            s = s.Replace('ọ', 'o');
            s = s.Replace('ô', 'o');
            s = s.Replace('ố', 'o');
            s = s.Replace('ồ', 'o');
            s = s.Replace('ổ', 'o');
            s = s.Replace('ỗ', 'o');
            s = s.Replace('ộ', 'o');
            s = s.Replace('ơ', 'o');
            s = s.Replace('ớ', 'o');
            s = s.Replace('ờ', 'o');
            s = s.Replace('ở', 'o');
            s = s.Replace('ỡ', 'o');
            s = s.Replace('ợ', 'o');
            s = s.Replace('ú', 'u');
            s = s.Replace('ù', 'u');
            s = s.Replace('ủ', 'u');
            s = s.Replace('ũ', 'u');
            s = s.Replace('ụ', 'u');
            s = s.Replace('ư', 'u');
            s = s.Replace('ứ', 'u');
            s = s.Replace('ừ', 'u');
            s = s.Replace('ử', 'u');
            s = s.Replace('ữ', 'u');
            s = s.Replace('ự', 'u');
            s = s.Replace('ý', 'y');
            s = s.Replace('ỳ', 'y');
            s = s.Replace('ỷ', 'y');
            s = s.Replace('ỹ', 'y');
            s = s.Replace('ỵ', 'y');
            s = s.Replace('đ', 'd');
            return s.ToUpper();
        }
        public static bool CheckUnicode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;
            foreach (char c in input)
            {
                if (c > 127)
                {
                    //string doesn't contains UniCode characters
                    return false;
                }
               
            }
            return true;
        }
        public static bool CheckCardCode(string cardType, string value)
        {
            Regex regex;
            switch (cardType)
            {
                case "viettel":
                    //regex = new Regex(@"^(?=[0-9]*$)(?:.{13}|.{14}|.{15})$");
                    regex = new Regex(@"^(?=[0-9]*$)(?:.{13}|.{15})$");
                    return regex.IsMatch(value);
                case "vnp":
                    regex = new Regex(@"^(?=[0-9]*$)(?:.{12}|.{14})$");
                    return regex.IsMatch(value);
                case "vms":
                    regex = new Regex(@"^(?=[0-9]*$)(?:.{12}|.{14})$");
                    return regex.IsMatch(value);
                case "vcoin":
                    regex = new Regex(@"^\d{12}$");
                    return regex.IsMatch(value);
                case "zing":
                    regex = new Regex(@"^\w{9}$");
                    return regex.IsMatch(value);
                case "gate":
                    regex = new Regex(@"^\d{10}$");
                    return regex.IsMatch(value);
                case "bit":
                case "gosu":
                    regex = new Regex(@"^\d{9}$");
                    return regex.IsMatch(value);
                case "garena":
                    regex = new Regex(@"^\d{16}$");
                    return regex.IsMatch(value);
                default:
                    return true;
            }

        }

        public static bool CheckCardSerial(string cardType, string value)
        {
            Regex regex;
            switch (cardType)
            {
                case "viettel":
                    regex = new Regex(@"^\w{11,15}$");
                    //regex = new Regex(@"^([0-9])\1{10,}$"); liên tiếp
                    return regex.IsMatch(value);
                case "vnp":
                    regex = new Regex(@"^\w{8,15}$");
                    return regex.IsMatch(value);
                case "vms":
                    regex = new Regex(@"^\w{9,15}$");
                    return regex.IsMatch(value);
                case "vcoin":
                    regex = new Regex(@"^(id)\d{10}$", RegexOptions.IgnoreCase);
                    return regex.IsMatch(value);
                case "zing":
                    regex = new Regex(@"^(\w)(\w)\d{10}$", RegexOptions.IgnoreCase);
                    return regex.IsMatch(value);
                case "gate":
                case "bit":
                    regex = new Regex(@"^([a-z])([a-z])\d{8}$", RegexOptions.IgnoreCase);
                    return regex.IsMatch(value);
                case "gosu":
                    regex = new Regex(@"^([a-z])([a-z])\d{9}$", RegexOptions.IgnoreCase);
                    return regex.IsMatch(value);
                case "garena":
                    regex = new Regex(@"^\d{9}$");
                    return regex.IsMatch(value);
                default:
                    return true;
            }

        }

        public static string GenProxy()
        {
            string[] pp = ("us.smartproxy.io,jp.smartproxy.io,my.smartproxy.io,th.smartproxy.io,kr.smartproxy.io,ph.smartproxy.io,vn.smartproxy.io").Split(',');
            //string[] pp = ("vn.smartproxy.io").Split(',');
            Random rd = new Random();
            var proxySr = pp[rd.Next(0, pp.Length)];
            var proxy = proxySr + ":";

            switch (proxySr)
            {
                case "vn.smartproxy.io":
                    proxy = proxy + rd.Next(46001, 46999 + 1);
                    break;
                case "us.smartproxy.io":
                    proxy = proxy + rd.Next(20000, 29999 + 1);
                    break;
                case "kr.smartproxy.io":
                    proxy = proxy + rd.Next(10001, 19999 + 1);
                    break;
                case "jp.smartproxy.io":
                case "my.smartproxy.io":
                case "th.smartproxy.io":
                case "ph.smartproxy.io":
                    proxy = proxy + rd.Next(30001, 39999 + 1);
                    break;
            }
            return proxy;

        }

        public static bool IsAnyNullOrEmpty(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public static class AbbrevationUtility
    {
        private static readonly SortedDictionary<long, string> abbrevations = new SortedDictionary<long, string>
        {
            {1000,"K"},
            {1000000, "M" },
            {1000000000, "B" },
            {1000000000000,"T"}
        };

        public static string AbbreviateNumber(float number)
        {
            for (int i = abbrevations.Count - 1; i >= 0; i--)
            {
                KeyValuePair<long, string> pair = abbrevations.ElementAt(i);
                if (Math.Abs(number) >= pair.Key)
                {
                    float roundedNumber = (number / pair.Key);
                    return roundedNumber + pair.Value;
                }
            }
            return number.ToString();
        }
    }
    



}
