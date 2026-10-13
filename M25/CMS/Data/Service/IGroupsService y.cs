using System;
using System.Collections.Generic;
using CMS.Data.DTO;

namespace CMS.Data.Service
{
    public interface IGroupsService
    {
      
    	Groups Get(int Id);
        List<Groups> GetList();
        int InsertUpdate(Groups group);
    	int Delete(int Id);
        
    }
}
