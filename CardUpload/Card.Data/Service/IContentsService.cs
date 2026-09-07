using System;
using System.Collections.Generic;
using Card.Data.DTO;

namespace Card.Data.Service
{
    public interface IContentsService
    {
      
    	Contents Get(int Id);
        Contents GetHot();
        List<Contents> GetFilter(int status, string keyword, string usename);
        List<Contents> GetTop(int top, string usename);
        List<Contents> GetList(string select, string where, string orde);
        int InsertUpdate(Contents group);
    	int Delete(int Id);
        int UpdateUserDynamic(string where, string updatest);
    }
}
