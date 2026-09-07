using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Card.Data.DTO;
namespace Card.Data.Service
{
    public interface IUserRoleService
    {
        #region Quyền truy cập chức năng
        UserFunction CheckPermission(int UserID, int FunctionID);
        int UserFunctionInsert(UserFunction RoleFunction);
        
        int UserFunctionInsertList(int UserID, string ListRole); //Thêm ds quyền -> clear toàn bộ chức năng đang tồn tại
        int GroupFunctionInsertList(int UserID, string ListRole);

        int UserFunctionInsertListV2(int UserID, string ListRole);//Thêm ds quyền nếu ko tồn tại quyền có trong ds
        int UserFunctionDelete(int UserID, int FunctionID);
        int UserFunctionDeleteAll(int UserID);
        List<UserFunction> UserFunction_GetByUserID(int UserID);
         List<UserFunction> GroupFunction_GetByID(int id);
        List<Functions> GetListFunctionByID(int id);
        #endregion



    }
}
