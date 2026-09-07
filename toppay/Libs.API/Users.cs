using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Libs.Db;
using System.ComponentModel;
using System.Linq;
namespace Libs.API
{
    public class Users
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedTime { get; set; }
        public int Status { get; set; }
        public DateTime LastestTime { get; set; }
        public string AccessKey { get; set; }
        public int IsAdmin { get; set; }
        public int IsPartner { get; set; }
        public int IsTopup { get; set; }
        public int IsProvider { get; set; }
        public int ParentId { get; set; }
        public int ReturnValue { get; set; }

        public long Balance { get; set; }
        public long Deposit { get; set; }
        public string F2a { get; set; }
        public string Ip { get; set; }
        public int Withdraw { get; set; }
        public Users()
        {

        }

        public Users Get()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Users>("sp_Users_Select"
                , new SqlParameter("@UserID", UserID));
        }
        public void Delete()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            var oCommand = new SqlCommand("sp_Users_Delete");
            oCommand.CommandType = CommandType.StoredProcedure;
            oCommand.Parameters.Add(new SqlParameter("@UserID", this.UserID));
            db.ExecuteNonQuery(oCommand);
        }
        public Users Get(int userID)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Users>("sp_Users_Select"
                , new SqlParameter("@UserID", userID));
        }
        public Users GetByUserName(string userName)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            return db.GetInstanceSP<Users>("sp_Users_SelectByUserName"
                , new SqlParameter("@UserName", userName));
        }
        public List<Users> GetList()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            var lst =db.GetListSP<Users>("sp_Users_SelectList");  
            return lst;
        }
        public bool IsTopupCap1(int UserID)
        {
            var item=new Users() { UserID = UserID }.Get(); 
            if (item != null && item.IsTopup == 1){
                  item = new Users() { UserID = item.ParentId }.Get();
                  return (item != null && item.IsAdmin == 1);
            }
            return false;
        }
        public List<Users> GetListTopupBySort(bool IsAdmin, bool IsTopup,int UserID)
        {

            var lstUsers = GetList();
            var Lstusers2 = new List<Users>();
            if (lstUsers != null)
            {
                if (IsAdmin)
                    lstUsers = lstUsers.Where(e => e.IsTopup == 1 || e.IsAdmin == 1).ToList();
                else if (IsTopup)
                    lstUsers = lstUsers.Where(e =>  e.IsTopup == 1 && e.ParentId == UserID || e.UserID == UserID).ToList();

                //var lst = new Users().GetList().Where(e => e.IsAdmin == 1).ToList();
                //foreach (var item in lst)
                //{
                //    Lstusers2.Add(item);
                //    var lst1 = lstUsers.Where(e => e.ParentId == item.UserID);
                //    if (lst1 != null)
                //    {
                //        foreach (var item1 in lst1)
                //        {
                //            Lstusers2.Add(item1);
                //            var lst2 = lstUsers.Where(e => e.ParentId == item1.UserID).ToList();
                //            foreach (var item2 in lst2)
                //            {
                //                item2.UserName = " --- " + item2.UserName;
                //                Lstusers2.Add(item2);
                //            }
                //        }
                //    }
                //} 
            }
            return lstUsers;
        }

        public void Add()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[10];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserName", UserName);
            pars[2] = new SqlParameter("@Password", Password);
            pars[3] = new SqlParameter("@FullName", FullName);
            pars[4] = new SqlParameter("@Status", Status);
            pars[5] = new SqlParameter("@IsAdmin", IsAdmin); 
            pars[6] = new SqlParameter("@IsPartner", IsPartner);
            pars[7] = new SqlParameter("@IsTopup", IsTopup);
            pars[8] = new SqlParameter("@ParentId", ParentId);
            pars[9] = new SqlParameter("@IsProvider", IsProvider);
            db.ExecuteNonQuerySP("sp_Users_Insert", pars);
            UserID = Convert.ToInt32(pars[0].Value);
        }

        public void Update()
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[14];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@UserID", UserID);
            pars[2] = new SqlParameter("@UserName", UserName);
            pars[3] = new SqlParameter("@Password", Password);
            pars[4] = new SqlParameter("@FullName", FullName);
            pars[5] = new SqlParameter("@Status", Status);
            pars[6] = new SqlParameter("@IsAdmin", IsAdmin);
            pars[7] = new SqlParameter("@IsPartner",IsPartner);
            pars[8] = new SqlParameter("@IsTopup", IsTopup);
            pars[9] = new SqlParameter("@ParentId", ParentId);
            pars[10] = new SqlParameter("@IsProvider", IsProvider);
            pars[11] = new SqlParameter("@F2a", F2a);
            pars[12] = new SqlParameter("@Ip", Ip);
            pars[13] = new SqlParameter("@Withdraw", Withdraw);
            db.ExecuteNonQuerySP("sp_Users_Update", pars);
            ReturnValue = Convert.ToInt32(pars[0].Value);
        } 

        public bool Authentication(string userName, string password)
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[5];
            pars[0] = new SqlParameter("@UserName", userName);
            pars[1] = new SqlParameter("@Password", password);
            pars[2] = new SqlParameter("@UserID", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[3] = new SqlParameter("@AccessKey", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output };
            pars[4] = new SqlParameter("@LastestTime", SqlDbType.DateTime) { Direction = ParameterDirection.Output };

            db.ExecuteNonQuerySP("sp_Users_Authentication", pars);
            UserID = Convert.ToInt32(pars[2].Value);
            if (UserID > 0)
            {
                UserName = userName;
                Password = password;
                AccessKey = pars[3].Value.ToString();
                LastestTime = Convert.ToDateTime(pars[4].Value);
                return true;
            }
            else
            {
                return false;
            }
        }
        public int Topup(long Amount,string UserName, string PartnerCode,string Note, string RefCode = "")
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Amount", Amount);
            pars[2] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[3] = new SqlParameter("@UserName", UserName);
            pars[4] = new SqlParameter("@Note", Note);
            pars[5] = new SqlParameter("@RefCode", RefCode);
            db.ExecuteNonQuerySP("sp_UsersTopup", pars);
            return Convert.ToInt32(pars[0].Value);
        }
        public int Deduct(long Amount, string UserName, string PartnerCode, string Note,string RefCode="")
        {
            DBHelper db = new DBHelper(Configs.VPGAPIConnectionStrings);
            SqlParameter[] pars = new SqlParameter[6];
            pars[0] = new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.Output };
            pars[1] = new SqlParameter("@Amount", Amount);
            pars[2] = new SqlParameter("@PartnerCode", PartnerCode);
            pars[3] = new SqlParameter("@UserName", UserName);
            pars[4] = new SqlParameter("@Note", Note);
            pars[5] = new SqlParameter("@RefCode", RefCode);
            db.ExecuteNonQuerySP("sp_UsersDeduct", pars);
            return Convert.ToInt32(pars[0].Value);
        }

    }
}
