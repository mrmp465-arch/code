using System;
using System.Collections.Generic;
using SMS.Data.DTO;

namespace SMS.Data.Service
{
    public interface IContactsService
    {
      
    	Contacts Get(int Id);

        List<Contacts> GetFilter(int groupId, string keyword, string createUser, int page, int pageSize, ref int total);
        int DeleteDynamic(string where);
        int InsertUpdate(Contacts group);
    	int Delete(int Id);
        int UpdateDynamic(string where, string updatest);
    }
}
