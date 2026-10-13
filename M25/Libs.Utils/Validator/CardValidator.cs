using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Libs.Utils.Validator
{
    public class CardValidator
    {
        // Độ dài 12, bắt đầy VG sau đó là 10 chữ số
        public static bool VGGSerial(string cardSerial)
        {
            cardSerial = cardSerial.ToUpper();

            if (!cardSerial.StartsWith("VGG") || cardSerial.Length != 12) return false;

            cardSerial = cardSerial.Substring(3);

            return new Regex(@"^[0-9]{9}$").Match(cardSerial).Success;
        }

        // Độ dài 12, là các chữ số
        public static bool VGGCode(string cardCode)
        {
            return new Regex(@"^[0-9]{12}$").Match(cardCode).Success;
        }

        // Độ dài 15, là các chữ số
        public static bool MobiFoneSerial(string cardSerial)
        {
            return new Regex(@"^[0-9]{9,15}$").Match(cardSerial).Success;
        }

        // Độ dài 12, là các chữ số
        public static bool MobiFoneCode(string cardCode)
        {
            return new Regex(@"^[0-9]{12,14}$").Match(cardCode).Success;
        }

        // Độ dài 9, là các chữ cái, chữ số
        public static bool VinaphoneSerial(string cardSerial)
        {
            return new Regex(@"^[A-Z0-9]{9,12}$").Match(cardSerial.ToUpper()).Success;
        }

        // Độ dài 12-14, là các chữ số
        public static bool VinaphoneCode(string cardCode)
        {
            return new Regex(@"^[0-9]{12,14}$").Match(cardCode).Success;
        }

        // Độ dài 11, là các chữ số
        public static bool ViettelSerial(string cardSerial)
        {
            return new Regex(@"^[0-9]{11}$").Match(cardSerial).Success;
        }

        // Độ dài 13, là các chữ số
        public static bool ViettelCode(string cardCode)
        {
            return new Regex(@"^[0-9]{13}$").Match(cardCode).Success;
        }

        // Độ dài 12, bắt đầy PM sau đó là 10 chữ số
        public static bool VcoinSerial(string cardSerial)
        {
            cardSerial = cardSerial.ToUpper();

            if (!cardSerial.StartsWith("PM") && !cardSerial.StartsWith("ID")) return false;

            if (cardSerial.Length != 12) return false;

            cardSerial = cardSerial.Substring(2);

            return new Regex(@"^[0-9]{10}$").Match(cardSerial).Success;
        }

        // Độ dài 12, là các chữ số
        public static bool VcoinCode(string cardCode)
        {
            return new Regex(@"^[0-9]{12}$").Match(cardCode).Success;
        }

    }
}
