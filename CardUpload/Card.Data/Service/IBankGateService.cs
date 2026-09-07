using System;
using System.Collections.Generic;
using Card.Data.DTO;

namespace Card.Data.Service
{
    public interface IBankGateService
    {

        BankGateAPI Get(int Id);
        List<BankGateAPI> GetList(string select, string where, string orde);
        List<BankGateAPI> GetFilter(int UserId, int Top, string OrderNo, int Amount, int Status, string FromDate = "", string ToDate = "");
        int Add(BankGateAPI group);
        int Update(BankGateAPI functions);
        int UpdateBank(BankGateAPI functions);
        // Lấy báo cáo
        List<BankGateReport> Report(int UserId, int year, int month, int day, ref int totalTransaction, ref long totalAmount);
        List<BankGateAPI> ReportDoiSoat(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect = 1);
        List<BankGateAPI> ListReportDoiSoat(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect);
        List<BankGateAPI> ReportDoiSoatDaily(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect);
       
    }
}
