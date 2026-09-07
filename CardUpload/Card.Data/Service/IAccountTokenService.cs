using System;
using System.Collections.Generic;
using Card.Data.DTO;

namespace Card.Data.Service
{
    public interface IAccountTokenService
    {

        AccountToken Get(String Mobile);
        int InsertUpdate(AccountToken functions);
      
        
    }
}
