using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Card.Data.Service;
using Card.Data.DTO;
using Card.Utility;


namespace Card.Data.Service
{
    public class UsersService : IUsersService
    {

        /// <summary>
        /// Xác thực người dùng
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="isSucess"></param>
        /// <returns></returns>
        public int Authentication(string username, string password)
        {
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_Username", username);
                pars[1] = new SqlParameter("@_Password", password);
                pars[2] = new SqlParameter("@_ClientIP", Config.GetIP());
                pars[3] = new SqlParameter("@_ResponseStatus", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("sp_User_Authenticate", pars);

                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }

        /// <summary>
        /// Get User theo UserID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Users SelectByUserID(int userId)
        {
            try
            {

                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Users>("SP_User_GetByUserID",
                                                                                                 new SqlParameter("@_UserID", userId));

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Users();
            }
        }

        /// <summary>
        /// Get User theo email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public List<Users> GetByEmail(string email)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetListSP<Users>("SP_User_GetByEmail",
                                                                                                 new SqlParameter("@_Email", email));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<Users>();
            }
        }

        /// <summary>
        /// Get User theo Username
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public Users GetByUsername(string Username)
        {
            try
            {
                var user =
              new DBHelper(Config.MainConnectionString).GetInstanceSP<Users>("SP_User_GetByUsername",
                                                                                                 new SqlParameter("@_Username", Username));
                //cấp 1 trở lên
                if (user.Type <= 2)
                    return user;
                //cấp 2
                if (user.Type == 3)
                {
                    //lấy user cấp 1
                    var parrentUser = GetByUsername(user.CreatedUser);
                    user.PercentVMS = parrentUser.PercentVMS- user.PercentVMS;
                    user.PercentVTT = parrentUser.PercentVTT- user.PercentVTT;
                    user.PercentVNP = parrentUser.PercentVNP - user.PercentVNP;
                    user.PercentGarena = parrentUser.PercentGarena- user.PercentGarena;

                    user.PercentVMSMY = parrentUser.PercentVMSMY - user.PercentVMSMY;
                    user.PercentVMSNH = parrentUser.PercentVMSNH - user.PercentVMSNH;
                    user.PercentVNMNH= parrentUser.PercentVNMNH - user.PercentVNMNH;

                    user.PercentVNPMY = parrentUser.PercentVNPMY - user.PercentVNPMY;
                    user.PercentVNPTS = parrentUser.PercentVMSNH - user.PercentVNPTS;
                    user.PercentVNPTT = parrentUser.PercentVNPTT - user.PercentVNPTT;

                    user.PercentVTTMY = parrentUser.PercentVTTMY - user.PercentVTTMY;
                    user.PercentVTTTS = parrentUser.PercentVTTTS - user.PercentVTTTS;
                    user.PercentVTTTT = parrentUser.PercentVTTTT - user.PercentVTTTT;
                }
                ////cấp 3
                //if (user.Type == 5)
                //{
                //    //lấy user cấp 2
                //    var parrentUser = GetByUsername(user.CreatedUser);
                //    user.PercentVMS += parrentUser.PercentVMS;
                //    user.PercentVTT += parrentUser.PercentVTT;
                //    user.PercentVNP += parrentUser.PercentVNP;
                //    //user.PercentZing += parrentUser.PercentZing;
                //    //user.PercentGarena += parrentUser.PercentGarena;
                //    //user.PercentVTC += parrentUser.PercentVTC;
                //}
                return user;

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Users();
            }
        }

        /// <summary>
        /// Get list<Users>theo điều kiện, có phân trang
        /// </summary>
        /// <param name="departmentID"></param>
        /// <param name="groupID"></param>
        /// <param name="isAcitve"></param>
        /// <param name="email"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Users> GetListUsers(string Keyword, string CreatedUser, int isActive, int Group, int CurrPage, int PageSize, ref int TotalRecord)
        {
            try
            {
                var pars = new SqlParameter[7];
                pars[0] = new SqlParameter("@_Status", isActive);
                pars[1] = new SqlParameter("@_Keyword", Keyword);
                pars[2] = new SqlParameter("@_CreatedUser", CreatedUser);
                pars[3] = new SqlParameter("@_CurrPage", CurrPage);
                pars[6] = new SqlParameter("@_Group", Group);
                pars[4] = new SqlParameter("@_RecordPerPage", PageSize);
                pars[5] = new SqlParameter("@_TotalRecord", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var list = new DBHelper(Config.MainConnectionString).GetListSP<Users>("SP_User_GetPage", pars);
                TotalRecord = Convert.ToInt32(pars[5].Value);
                return list;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<Users>();
            }
        }
        public List<Users> GetAll()
        {
            try
            {

                var list = new DBHelper(Config.MainConnectionString).GetListSP<Users>("SP_User_GetAll");

                return list;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new List<Users>();
            }
        }
        /// <summary>
        /// Update thông tin User
        /// </summary>
        /// <param name="users"></param>
        /// <returns>int > 0 => thành công</returns>
        public int UpdateUsers(Users users)
        {
            try
            {
                var pars = new SqlParameter[33];
                pars[0] = new SqlParameter("@_UserID", users.UserID);
                pars[1] = new SqlParameter("@_Username", users.Username);
                pars[2] = new SqlParameter("@_Email", users.Email);

                pars[3] = new SqlParameter("@_FullName", users.FullName);
                pars[4] = new SqlParameter("@_Password", users.Password);
                pars[5] = new SqlParameter("@_Password2", users.Password2);
                pars[6] = new SqlParameter("@_IsActive", users.Status);
                pars[7] = new SqlParameter("@_Type", users.Type);

                pars[8] = new SqlParameter("@_Balance", users.Balance);
                pars[9] = new SqlParameter("@_PercentVNP", users.PercentVNP);
                pars[10] = new SqlParameter("@_PercentVMS", users.PercentVMS);
                pars[11] = new SqlParameter("@_PercentVTT", users.PercentVTT);
                pars[12] = new SqlParameter("@_Config", users.Config);
                pars[13] = new SqlParameter("@_CreatedUser", users.CreatedUser);
                pars[16] = new SqlParameter("@_Piority", 0);
                pars[15] = new SqlParameter("@_Mobile", "");
                pars[17] = new SqlParameter("@_Group", 0);
                pars[18] = new SqlParameter("@_PercentVTTMY", users.PercentVTTMY);
                pars[19] = new SqlParameter("@_PercentVTTTS", users.PercentVTTTS);
                pars[20] = new SqlParameter("@_PercentVTTTT", users.PercentVTTTT);
                pars[21] = new SqlParameter("@_UserAPI", users.UserAPI);
                pars[22] = new SqlParameter("@_PasswordAPI", "");
                pars[23] = new SqlParameter("@_NumberUser", 0);
                pars[24] = new SqlParameter("@_StatusOrder", users.Status);
                pars[25] = new SqlParameter("@_MaxDay", 0);
                pars[26] = new SqlParameter("@_PercentVNPMY", users.PercentVNPMY);
                pars[27] = new SqlParameter("@_PercentVNPTS", users.PercentVNPTS);
                pars[28] = new SqlParameter("@_PercentVNPTT", users.PercentVNPTT);
                pars[29] = new SqlParameter("@_PercentVMSMY", users.PercentVMSMY);
                pars[30] = new SqlParameter("@_PercentVMSNH", users.PercentVMSNH);
                pars[31] = new SqlParameter("@_PercentVNMNH", users.PercentVNMNH);
                pars[32] = new SqlParameter("@_PercentGarena", users.PercentGarena);
                pars[14] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_InsertUpdate", pars);
                return Convert.ToInt32(pars[14].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        public int UpdateUserDynamic(string where, string updatest)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@UpdateCondition", updatest);
                pars[1] = new SqlParameter("@WhereCondition", where);
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_UpdateDynamic", pars);
                return 1;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }
        /// <summary>
        /// Xóa thông tin một user theo UserID
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int DeleteUsers(int userId)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_UserID", userId);
                pars[1] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_Delete", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return -99;
            }
        }

        public int UpdateActiveUser(int Id)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_UserID", Id);
                pars[1] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_UpdateActive", pars);
                return Convert.ToInt32(pars[1].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }

        public int ResetPassword(int UserId, string UserName, string PasswordNew)
        {
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_UserId", UserId);
                pars[2] = new SqlParameter("@_PasswordNew", PasswordNew);
                pars[1] = new SqlParameter("@_UserName", UserName);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_ResetPassword", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int ResetPassword2(int UserId, string UserName, string PasswordNew)
        {
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_UserId", UserId);
                pars[2] = new SqlParameter("@_PasswordNew", PasswordNew);
                pars[1] = new SqlParameter("@_UserName", UserName);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_ResetPassword2", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int ChangePassword(string UserName, string PasswordOld, string PasswordNew)
        {
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_PasswordOld", PasswordOld);
                pars[2] = new SqlParameter("@_PasswordNew", PasswordNew);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_ChangePassword", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }

        public int ChangePassword2(string UserName, string PasswordOld, string PasswordNew)
        {
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_UserName", UserName);
                pars[1] = new SqlParameter("@_PasswordOld", PasswordOld);
                pars[2] = new SqlParameter("@_PasswordNew", PasswordNew);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_ChangePassword2", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int Topup(int UserId, string AdminName, int Amount, string Note)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_UserId", UserId);
                pars[1] = new SqlParameter("@_AdminName", AdminName);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[3] = new SqlParameter("@_Note", Note);
                pars[5] = new SqlParameter("@_ClientIP", Config.GetIP());
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_Topup", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int Deduct(int UserId, string AdminName, int Amount, string Note)
        {
            try
            {
                var pars = new SqlParameter[6];
                pars[0] = new SqlParameter("@_UserId", UserId);
                pars[1] = new SqlParameter("@_AdminName", AdminName);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[3] = new SqlParameter("@_Note", Note);
                pars[5] = new SqlParameter("@_ClientIP", Config.GetIP());
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_Deduct", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
        public int SetDay(int UserId, string UserName, int Amount)
        {
            try
            {
                var pars = new SqlParameter[4];
                pars[0] = new SqlParameter("@_UserId", UserId);
                pars[1] = new SqlParameter("@_UserName", UserName);
                pars[2] = new SqlParameter("@_Amount", Amount);
                pars[3] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_SetDay", pars);
                return Convert.ToInt32(pars[3].Value);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
                return -99;
            }
        }
    }
}
