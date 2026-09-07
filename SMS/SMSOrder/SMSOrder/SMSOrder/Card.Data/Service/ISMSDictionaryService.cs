using System;
using System.Collections.Generic;
using SMS.Data.DTO;

namespace SMS.Data.Service
{
    public interface ISMSDictionaryService
    {
      
    	SMSDictionary Get(int Id);
        List<SMSDictionary> GetAll(string CreatedUser);
        List<SMSDictionary> GetList(string select, string where, string orde);
        int InsertUpdate(SMSDictionary group);
        int Delete(int functionId);


    }
}
