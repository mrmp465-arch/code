using System;
using System.Collections.Generic;
using SMS.Data.DTO;

namespace SMS.Data.Service
{
    public interface ITransactionsService
    {
      
    	
        List<Transactions> GetList(string Username,int Type,  int pageNumber, int pageSize, ref int TotalRecord);
        int Deduct(string UserName, int Amount, string Description);
        int Topup(string UserName, int Amount, string Description);
        int DeductHold(string UserName, int Amount, string Description);
        int TopupHold(string UserName, int Amount, string Description);
        int Confrim(string UserName, string ParrentName, int AmountParrent, int AmountSuccess, int AmountTopupHold, string ReferenceId);

    }
}
