using System;
using System.Collections.Generic;
namespace Libs.BankDirect.GPay
{
    public static class VietQrEmvBuilder
    {
        // Map bank code -> BIN (NAPAS)
        private static readonly Dictionary<string, string> BankBin = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "TCB", "970407" }, // Techcombank
        { "MSB", "970426" } ,// Maritime Bank
        { "VCB", "970436" } , // Maritime Bank
          { "ICB", "970415" } , // Maritime Bank
          { "VTB", "970415" } , // Maritime Bank
           { "TIMO", "963388" } , // Maritime Bank
            { "OCB", "970448" } , // Maritime 
             { "BIDV", "970418" } , //
             { "MB", "970422" } , //
             { "VIKKI", "970406" } , //
              { "DAB", "970406" } , //
              { "ACB", "970416" } , //
              { "VPB", "970432" } , //
              { "HDB", "970437" } , //
              { "TPB", "970423" } , //
               { "SEAB", "970440" } , //

    };

        public static string BuildEmv(string bank, string account, decimal? amount = null, string addInfo = null)
        {
            if (!BankBin.ContainsKey(bank))
                throw new Exception("Bank không hỗ trợ: " + bank);

            string acqId = BankBin[bank];

            return BuildVietQrEmv(acqId, account, amount, addInfo);
        }

        private static string BuildVietQrEmv(string acqId, string account, decimal? amount, string addInfo)
        {
            string payload = "";

            // 00
            payload += "000201";

            // 01
            payload += amount.HasValue ? "010212" : "010211";

            // 38 - Merchant Info
            string consumerAccount =
                Tlv("00", acqId) +
                Tlv("01", account);

            string merchant =
                Tlv("00", "A000000727") +
                Tlv("01", consumerAccount) +
                Tlv("02", "QRIBFTTA");

            payload += Tlv("38", merchant);

            // 52
            payload += "52040000";

            // 53
            payload += "5303704";

            // 54
            if (amount.HasValue)
                payload += Tlv("54", ((long)amount.Value).ToString());

            // 58
            payload += "5802VN";

            // 62
            if (!string.IsNullOrEmpty(addInfo))
                payload += Tlv("62", Tlv("08", addInfo));

            // CRC
            string crcInput = payload + "6304";
            string crc = CRC16(crcInput);

            return crcInput + crc;
        }

        private static string Tlv(string tag, string value)
        {
            return tag + value.Length.ToString("00") + value;
        }

        private static string CRC16(string input)
        {
            ushort crc = 0xFFFF;

            foreach (byte b in System.Text.Encoding.ASCII.GetBytes(input))
            {
                crc ^= (ushort)(b << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }

            return crc.ToString("X4");
        }
    }
}