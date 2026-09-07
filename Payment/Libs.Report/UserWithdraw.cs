using Libs.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;

namespace Libs.Report
{
    public class UserWithdraw
    {
        public DateTime CreatedTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Note { get; set; }

        public string BankInfo { get; set; }
        public string UserName { get; set; }
        public string Admin { get; set; }
        public long Id { get; set; }
        public long Amount { get; set; }
        public long Balance { get; set; }
        public long BalanceBefore { get; set; }

        public long Fee { get; set; }

        public long Reward { get; set; }

        public int Type { get; set; }

        public int Usdt { get; set; }
        public int RateIn { get; set; }

        public int RateOut { get; set; }
        public int Status { get; set; }
        public List<UserWithdraw> GetList(int Top, string username,int status,DateTime FromDate,DateTime ToDate,int type=-0, bool isDay = false)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[7];
            pars[0] = new SqlParameter("@UserName", username);
            pars[1] = new SqlParameter("@Status", status);
            pars[2] = new SqlParameter("@FromDate", FromDate);
            pars[3] = new SqlParameter("@ToDate", ToDate);
            pars[4] = new SqlParameter("@Top", Top);
            pars[5] = new SqlParameter("@Type", type);
            pars[6] = new SqlParameter("@IsDay", isDay);
            return db.GetListSP<UserWithdraw>("sp_UserWithdraw_Select", pars);
        }
        public long Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserName", UserName);
            pars[2] = new SqlParameter("@Amount", Amount);
            pars[3] = new SqlParameter("@Note", Note);
            pars[4] = new SqlParameter("@BankInfo", BankInfo);
            pars[5] = new SqlParameter("@Fee", Fee);
           
            pars[6] = new SqlParameter("@Reward", Reward);
            pars[7] = new SqlParameter("@Type", Type);
            pars[8] = new SqlParameter("@Usdt", Usdt);
            pars[9] = new SqlParameter("@RateIn", RateIn);
            pars[10] = new SqlParameter("@RateOut", RateOut);
            db.ExecuteNonQuerySP("sp_UserWithdraw_Insert", pars);
            return Convert.ToInt64(pars[0].Value);
        }
        public long Add2()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[11];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserName", UserName);
            pars[2] = new SqlParameter("@Amount", Amount);
            pars[3] = new SqlParameter("@Note", Note);
            pars[4] = new SqlParameter("@Admin", Admin);
            pars[5] = new SqlParameter("@Fee", Fee);
            pars[6] = new SqlParameter("@Reward", Reward);
            pars[7] = new SqlParameter("@Type", Type);
            pars[8] = new SqlParameter("@Usdt", Usdt);
            pars[9] = new SqlParameter("@RateIn", RateIn);
            pars[10] = new SqlParameter("@RateOut", RateOut);
            db.ExecuteNonQuerySP("sp_UserWithdraw_InsertV2", pars);
            return Convert.ToInt64(pars[0].Value);
        }
        public UserWithdraw Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<UserWithdraw>("sp_UserWithdraw_Get"
                , new SqlParameter("@Id", Id));
        }
        public int Cancel()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = new SqlParameter("@Admin", Admin);
            db.ExecuteNonQuerySP("sp_UserWithdraw_Cancel", pars);
            return Convert.ToInt32(pars[0].Value);
        }

        public int Confirm()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[3];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = new SqlParameter("@Admin", Admin);
            db.ExecuteNonQuerySP("sp_UserWithdraw_Confirm", pars);
            return Convert.ToInt32(pars[0].Value);
        }
        public int ConfirmV2()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[10];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Id", Id);
            pars[2] = new SqlParameter("@Admin", Admin);
            pars[3] = new SqlParameter("@Note", Note);
            pars[4] = new SqlParameter("@Fee", Fee);
            pars[5] = new SqlParameter("@Reward", Reward);
            pars[7] = new SqlParameter("@Type", Type);
            pars[6] = new SqlParameter("@Usdt", Usdt);
            pars[9] = new SqlParameter("@RateIn", RateIn);
            pars[8] = new SqlParameter("@RateOut", RateOut);
            db.ExecuteNonQuerySP("sp_UserWithdraw_ConfirmV2", pars);
            return Convert.ToInt32(pars[0].Value);
        }
    }
}
