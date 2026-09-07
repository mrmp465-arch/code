using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SMS.Data.Service;
using SMS.Data.DTO;
using SMS.Utility;


namespace SMS.Data.Service
{
    public class UsersService : IUsersService
    {

        public void GetMetaData(string username, ref  int balance,ref int numberContact, ref int numberSMSSend, ref int numberSMSFinish)
        {
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_Username", username);
                pars[1] = new SqlParameter("@_Balance", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[2] = new SqlParameter("@_NumberContact", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[3] = new SqlParameter("@_NumberSMSSend", SqlDbType.Int) { Direction = ParameterDirection.Output };
                pars[4] = new SqlParameter("@_NumberSMSFinish", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_User_GetMetaData", pars);
                balance=Convert.ToInt32(pars[1].Value);
                numberContact = Convert.ToInt32(pars[2].Value);
                numberSMSSend = Convert.ToInt32(pars[3].Value);
                numberSMSFinish = Convert.ToInt32(pars[4].Value);
                return;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return;
            }
        }
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
        public Users GetByEmail(string email)
        {
            try
            {
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Users>("SP_User_GetByEmail",
                                                                                                 new SqlParameter("@_Email", email));
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return new Users();
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
                return new DBHelper(Config.MainConnectionString).GetInstanceSP<Users>("SP_User_GetByUsername",
                                                                                                 new SqlParameter("@_Username", Username));
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
                var pars = new SqlParameter[23];
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
                pars[16] = new SqlParameter("@_Piority", users.Piority);
                pars[15] = new SqlParameter("@_Mobile", users.Mobile);
                pars[17] = new SqlParameter("@_Group", users.Group);
                pars[18] = new SqlParameter("@_StatusVMS", users.StatusVMS);
                pars[19] = new SqlParameter("@_StatusVTT", users.StatusVTT);
                pars[20] = new SqlParameter("@_StatusVNP", users.StatusVNP);
                pars[21] = new SqlParameter("@_UserAPI", users.UserAPI);
                pars[22] = new SqlParameter("@_PasswordAPI", users.PasswordAPI);
               // pars[23] = new SqlParameter("@_NumberUser", users.NumberUser);
                //pars[24] = new SqlParameter("@_StatusOrder", users.StatusOrder);
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
       
        public int ResetPassword(int UserId,string UserName,  string PasswordNew)
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
        public int ResetPassword2(int UserId, string UserName,string PasswordNew)
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
        public int Deduct(int UserId, string AdminName,int Amount, string Note)
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
    }
}
