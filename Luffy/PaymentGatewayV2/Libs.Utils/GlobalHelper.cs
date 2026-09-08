using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Libs.Utils
{
    public class GlobalHelper
    {
        public static int? TryParseNullable(string val)
        {
            int outValue;
            return int.TryParse(val, out outValue) ? (int?)outValue : null;
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
                case "vnm":
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
                    regex = new Regex(@"^\w{13,15}$");
                    //regex = new Regex(@"^([0-9])\1{10,}$"); liên tiếp
                    return regex.IsMatch(value);
                case "vnp":
                    regex = new Regex(@"^\w{14,14}$");
                    return regex.IsMatch(value);
                case "vms":
                    regex = new Regex(@"^\w{15,15}$");
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
