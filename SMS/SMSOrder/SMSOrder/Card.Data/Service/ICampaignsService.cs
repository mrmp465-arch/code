using System;
using System.Collections.Generic;
using SMS.Data.DTO;

namespace SMS.Data.Service
{
    public interface ICampaignsService
    {
      
    	Campaigns Get(int Id);
        Campaigns Get(string name);
        List<Campaigns> GetFilter(string keyword, string createUser, int page, int pageSize, ref int total);
        int DeleteDynamic(string where);
        int InsertUpdate(Campaigns group);
    	int Delete(int Id, string Username);
        int Send(int functionId, string Username);
        int Lock(int functionId, string Username);
        int UnLock(int functionId, string Username);
        int Confirm(int functionId, string Username);
        int UpdateDynamic(string where, string updatest);
        int SetData(int id, int delete, string username, int priority);
        string Preview(string group, string content);
    }
}
