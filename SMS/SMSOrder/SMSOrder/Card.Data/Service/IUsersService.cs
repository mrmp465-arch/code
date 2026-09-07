using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.Data.DTO;
namespace SMS.Data.Service
{
    public interface IUsersService
    {
        int Authentication(string username, string password);
        Users SelectByUserID(int userId);
        Users GetByEmail(string email);
        Users GetByUsername(string Username);

        List<Users> GetListUsers(string Keyword, string createdUser, int isActive, int Group, int CurrPage, int PageSize, ref int TotalRecord);
        int UpdateUsers(Users users);
        int DeleteUsers(int userId);
        int UpdateActiveUser(int Id);
        int UpdateUserDynamic(string where, string updatest);
        List<Users> GetAll();
        int ChangePassword(string UserName, string PasswordOld, string PasswordNew);
        int ChangePassword2(string UserName, string PasswordOld, string PasswordNew);
        int ResetPassword(int UserId, string UserName, string PasswordNew);
        int ResetPassword2(int UserId, string UserName, string PasswordNew);
        int Topup(int UserId, string AdminName, int Amount, string Note);
        int Deduct(int UserId, string AdminName, int Amount, string Note);
        void GetMetaData(string username, ref int balance, ref int numberContact, ref int numberSMSSend, ref int numberSMSFinish);


    }
}
