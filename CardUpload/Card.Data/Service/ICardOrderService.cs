using System;
using System.Collections.Generic;
using Card.Data.Api;
using Card.Data.DTO;

namespace Card.Data.Service
{
    public interface ICardOrderService
    {
        List<CardOrder> GetByRefCode(long refcode);
        List<CardOrderReport2> GetReport();
        List<CardOrder> GetByOrderNo(string orderNo);
        int UpdateDynamic(string where, string updatest);
        List<CardOrder> Search(string orderNo, string cardseri, string telco, int amount);
        List<CardOrderReport> GetReportDaily(string parentname, string username, string orderNo, string fromdate, string todate);
         List<CardOrderGroup> GetListGroup(int top, string parentname, string username, string orderNo, string telco);

        List<CardOrderGroup> GetGroupFilter(string select, string where, string order, string group );
        List<CardOrder> GetFilter(string select, string where, string orde);
        int Add(CardOrder group);
        List<CardOrder> GetList(int top, string parentname, string username, string orderNo, string cardseri, string cardcode, int status = -1000);


    }
}
