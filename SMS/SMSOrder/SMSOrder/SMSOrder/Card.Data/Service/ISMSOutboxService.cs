using System;
using System.Collections.Generic;
using SMS.Data.DTO;

namespace SMS.Data.Service
{
    public interface ISMSOutboxService
    {
      
    	
      
        List<SMSOutbox> GetList(string select, string where, string order);
        int InsertUpdate(SMSOutbox group);
        List<SMSOutbox> GetFilter(int port, string sender);




    }
}
