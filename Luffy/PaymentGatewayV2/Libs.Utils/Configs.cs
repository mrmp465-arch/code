using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Utils;

namespace Libs.Db
{
    public sealed class Configs
    {
        private static readonly Configs instance = new Configs();

        private string _VPGAPIConnectionStrings;
        private string _VPGLogConnectionStrings;
        private string _VPGAPIReportConnectionStrings;
        private string _VPGLogReportConnectionStrings;
        private string _CardStoreConnectionStrings;
        private string _CardStoreReportConnectionStrings;
        private string _CaptchaConnectionStrings;
        private string _CaptchaReportConnectionStrings;

        public static string VPGAPIConnectionStrings
        {
            get
            {
                return instance._VPGAPIConnectionStrings;
            }
        }

        public static string VPGLogConnectionStrings
        {
            get
            {
                return instance._VPGLogConnectionStrings;
            }
        }
        public static string VPGAPIReportConnectionStrings
        {
            get
            {
                return instance._VPGAPIReportConnectionStrings;
            }
        }

        public static string VPGLogReportConnectionStrings
        {
            get
            {
                return instance._VPGLogReportConnectionStrings;
            }
        }
        
        public static string CardStoreConnectionStrings
        {
            get
            {
                return instance._CardStoreConnectionStrings;
            }
        }

        public static string CardStoreReportConnectionStrings
        {
            get
            {
                return instance._CardStoreReportConnectionStrings;
            }
        }

        public static string CaptChaConnectionStrings
        {
            get
            {
                return instance._CaptchaConnectionStrings;
            }
        }

        public static string CaptchaReportConnectionStrings
        {
            get
            {
                return instance._CaptchaReportConnectionStrings;
            }
        }

        Configs()
        {
            _VPGAPIConnectionStrings = GetConnectionString("VPGAPIConnectionStrings");
            _VPGLogConnectionStrings = GetConnectionString("VPGLogConnectionStrings");
            _VPGAPIReportConnectionStrings = GetConnectionString("VPGAPIReportConnectionStrings");
            _VPGLogReportConnectionStrings = GetConnectionString("VPGLogReportConnectionStrings");
            _CardStoreConnectionStrings = GetConnectionString("CardStoreConnectionStrings");
            _CardStoreReportConnectionStrings = GetConnectionString("CardStoreReportConnectionStrings");
            _CaptchaConnectionStrings = GetConnectionString("CaptchaConnectionStrings");
            _CaptchaReportConnectionStrings = GetConnectionString("CaptchaReportConnectionStrings");
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
