using System;
using System.Collections.Generic;
using SMS.Data.DTO;

namespace SMS.Data.Service
{
    public interface IAccountTokenService
    {

        AccountToken Get(String Mobile);
        int InsertUpdate(AccountToken functions);
      
        
    }
}
