using System;
using System.Collections.Generic;
using SMS.Data.DTO;

namespace SMS.Data.Service
{
    public interface ICommentsService
    {
      
    	Comments Get(int Id);
        
        List<Comments> GetFilter(int status, string keyword);
        List<Comments> GetTop(int top,int newsId);
        List<Comments> GetList(string select, string where, string orde);
        int InsertUpdate(Comments group);
    	int Delete(int Id);
        int UpdateUserDynamic(string where, string updatest);
    }
}
