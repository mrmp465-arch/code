using System;
using System.Collections.Generic;
using CMS.Data.DTO;

namespace CMS.Data.Service
{
    public interface ITransactionsService
    {
      
    	
        List<Transactions> GetList(string Username,int Type,  int pageNumber, int pageSize, ref int TotalRecord, ref long TotalDeduct, ref long TotalTopup, int ActionType,string ReferenceId,string keyword, string FromDate = "", string ToDate = "");
        List<Transactions> GetListHold(string Username, int Type, int pageNumber, int pageSize, ref int TotalRecord);
        int Deduct(string UserName, int Amount, string Description, string ReferenceId);
        int Topup(string UserName, int Amount, string Description, string ReferenceId,int ActionType);
        int DeductHold(string UserName, int Amount, string Description, string ReferenceId="");
        int TopupHold(string UserName, int Amount, string Description, string ReferenceId="");
        int DeductBid(string UserName, int AmountSuccess, long OrderId, string Description, string Note);
        int Confrim(string UserName, string ParrentName, int AmountParrent, int AmountSuccess, int AmountTopupHold, string ReferenceId);
        int DeductCard(string UserName, int AmountSuccess, long OrderId, string Description, string Note);
    }
}
