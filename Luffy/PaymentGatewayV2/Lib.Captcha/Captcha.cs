using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;

namespace Lib.Captcha
{
    public class Captcha
    {
        public long Id { get; set; }
        public string SessionId { get; set; }
        public int TaskId { get; set; }
        public string Value { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public DateTime CreateTime { get; set; }
        public string ImgBase64 { get; set; }
        public long ReturnValue { get; set; }
        public Captcha()
        {

        }


        public void Add()
        {
            DBHelper db = new DBHelper(Configs.CaptChaConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@SessionId", SessionId);
            pars[2] = new SqlParameter("@Value", Value);
            pars[3] = new SqlParameter("@TaskId", TaskId);
            pars[4] = new SqlParameter("@Type", Type);
            pars[5] = new SqlParameter("@ImageBase64", ImgBase64);
            db.ExecuteNonQuerySP("sp_Captcha_Insert", pars);
            ReturnValue = Convert.ToInt64(pars[0].Value);
        }

        public Captcha GetCaptcha(int type)
        {
            DBHelper db = new DBHelper(Configs.CaptChaConnectionStrings);
            return db.GetInstanceSP<Captcha>("sp_Get_Captcha", new SqlParameter("@Type", type));
        }

        public Captcha GetCaptcha(int type, string prefix)
        {
            DBHelper db = new DBHelper(Configs.CaptChaConnectionStrings);
            return db.GetInstanceSP<Captcha>("sp_Get_Captcha_Prefix", new SqlParameter("@Type", type), new SqlParameter("@Prefix", prefix));
        }

        public void DeleteCaptcha(int type, string prefix)
        {
            DBHelper db = new DBHelper(Configs.CaptChaConnectionStrings);
            db.ExecuteNonQuerySP("sp_Del_Captcha_Prefix"
                , new SqlParameter("@Type", type)
                , new SqlParameter("@Prefix", prefix));
        }
    }
}
