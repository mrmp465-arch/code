using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Utils;

namespace Libs.BankCash
{
    public sealed class Configs
    {
        private static readonly Configs instance = new Configs();

        private string _VPGAPIConnectionStrings;

        public static string VPGAPIConnectionStrings
        {
            get
            {
                return instance._VPGAPIConnectionStrings;
            }
        }

        Configs()
        {
            _VPGAPIConnectionStrings = GetConnectionString("VPGAPIConnectionStrings");
        }

        public string GetConnectionString(string Name)
        {
            if (ConfigurationManager.ConnectionStrings[Name] == null) return "";
            RijndaelEnhanced rijndaelKey = new RijndaelEnhanced("pay", "@1B2c3D4e5F6g7H8");
            return rijndaelKey.Decrypt(ConfigurationManager.ConnectionStrings[Name].ConnectionString);
        }

        public static Configs Instance
        {
            get { return instance; }
        }



    }
}
