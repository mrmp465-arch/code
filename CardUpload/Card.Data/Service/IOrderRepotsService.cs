using System;
using System.Collections.Generic;
using Card.Data.Api;
using Card.Data.DTO;

namespace Card.Data.Service
{
    public interface IOrderReportsService
    {
        OrderReportHistory GetByCode(string Code);
        List<OrderReportHistoryItem> GetReportUserAdmin(string fromdate, string todate, string telco, int type);
        int UpdateAmount(long Id,int AmountMin);
        OrderReport Get(long Id);
        int Confirm(long Id);
        int UpdateStatus(long Id, int Status, string Description="");
        int UpdateStatusOrder(string Order, int Status);
        int UnConfirm(long Id);
        List<OrderReportHistoryItem> GetReportDaily(Users user, string fromdate, string todate, string telco, int type,string OrderNo);
        List<OrderGroup> ListGroupFinish(Users user, string fromdate, string todate, string telco);
         List<OrderReportHistoryItem> GetReportOur(Users user, string fromdate, string telco, int type);
        List<OrderReportHistory> OrderHistory(int orderId);
        List<OrderReportHistoryItem> GetReportUser(string UserAPI, string fromdate, string todate, string telco, int type);
        long Add(OrderReport group);
        List<OrderReport> OrderSearch(string mobile);
        List<OrderReport> GetTopBid(int top, string username, string telco, int BidRate);
        long AddHistory(OrderReportHistory group);
        List<UserWarning> ListUserWarning();
        int UpdateBid(long id, decimal bidRate, int bidFeeHold);

        List<OrderReport> GetMaxBid();
        List<OrderReportHistoryItem> GetGroupMaxBid(string fromdate, string todate);
        List<OrderGroupBid> GetGroupBid(int top, string username, string telco, int BidRate);
        List<OrderReportHistoryDS> GetReportDS(Users user, string fromdate, string todate);
    }
}
