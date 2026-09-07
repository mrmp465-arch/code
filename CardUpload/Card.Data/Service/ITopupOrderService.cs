using System;
using System.Collections.Generic;
using Card.Data.Api;
using Card.Data.DTO;

namespace Card.Data.Service
{
    public interface ITopupOrderService
    {
        List<TopupOrder> GetByOrderNo(string orderNo);
        int UpdateDynamic(string where, string updatest);
        List<TopupOrderReport> GetReportDaily(string parentname, string username, string orderNo, string fromdate, string todate);
         List<TopupOrderGroup> GetListGroup(int top, string parentname, string username, string orderNo);

        List<TopupOrderGroup> GetGroupFilter(string select, string where, string order, string group );
        List<TopupOrder> GetFilter(string select, string where, string orde);
        int Add(TopupOrder group);
        List<TopupOrder> GetList(int top, string parentname, string username, string orderNo, string cardseri, string cardcode, int status = -1000);


    }
}
