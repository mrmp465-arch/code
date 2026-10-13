using System;
using System.Collections.Generic;
using CMS.Data.DTO;

namespace CMS.Data.Service
{
    public interface IFucntionsService
    {
      
    	Functions GetFunctionByFunctionID(int functionId);
    	List<Functions> GetListFunctionByUserID(int userId);
        List<Functions> GetListFunctions(string Keyword, int isAcitve, int pageNumber, int pageSize, ref int TotalRecord);
        int InsertUpdateFunction(Functions functions);
    	int DelleteFunction(int functionId);
        List<Functions> GetListFunctionBySystemID(int systemId);
        List<Functions> GetListFunctionsByFather(int FatherID);
        int UpdateOrder(int FunctionID, int ParentID, int Order);
        List<Functions> SelectAllFunctionID(int fatherID, string name, int isactive, int isdisplay);
    }
}
