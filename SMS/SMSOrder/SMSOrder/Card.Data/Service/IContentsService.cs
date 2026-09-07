using System;
using System.Collections.Generic;
using SMS.Data.DTO;

namespace SMS.Data.Service
{
    public interface IContentsService
    {
      
    	Contents Get(int Id);
        Contents GetHot();
        List<Contents> GetFilter(int status, string keyword, string user);
        List<Contents> GetTop(int top);
        List<Contents> GetList(string select, string where, string orde);
        int InsertUpdate(Contents group);
    	int Delete(int Id);
        int UpdateUserDynamic(string where, string updatest);
    }
}
