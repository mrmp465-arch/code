using CMS.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Data.Service
{
    public interface IUsersLogService
    {
        List<UsersLog> GetListUsersLog(string fromDate, string toDate, int userId, string functioncode, string keyword, int pageNumber, int pageSize, ref int totalrecord);
        int DeleteUsersLog(string fromDate, string toDate, int userId, int functionId);
        int InsertUsersLog(UsersLog log);
        UsersLog GetLogOrder(long transId);
    }
}
