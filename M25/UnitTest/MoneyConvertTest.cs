using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class MoneyConvertTest
    {
        [TestMethod]
        public void MoneyShowTest()
        {
            //Console.WriteLine(Math.Floor(259.000));
            Console.WriteLine(AbbrevationUtility.AbbreviateNumber(12320540000));
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
