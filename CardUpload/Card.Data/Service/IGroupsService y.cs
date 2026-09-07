using System;
using System.Collections.Generic;
using Card.Data.DTO;

namespace Card.Data.Service
{
    public interface IGroupsService
    {
      
    	Groups Get(int Id);
        List<Groups> GetList();
        int InsertUpdate(Groups group);
    	int Delete(int Id);
        
    }
}
