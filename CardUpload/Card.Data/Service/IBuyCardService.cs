using System;
using System.Collections.Generic;
using Card.Data.Api;
using Card.Data.DTO;

namespace Card.Data.Service
{
    public interface IBuyCardService
    {
        BuyCard GetDetail(long Id);
        int UpdateDynamic(string where, string updatest);
      
        List<BuyCardReport> GetReportDaily(string parentname, string username, string telco, string fromdate, string todate);
        
        List<BuyCard> GetFilter(string select, string where, string orde);
        long Add(BuyCard group);
        List<BuyCard> GetList(int top, string parentname, string username, string telco, string cardseri, int status, long? id, string fromdate, string todate);


    }
}
