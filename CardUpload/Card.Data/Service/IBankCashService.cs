using System;
using System.Collections.Generic;
using Card.Data.DTO;

namespace Card.Data.Service
{
    public interface IBankCashService
    {

        BankCashAPI Get(int Id);
        List<BankCashAPI> GetList(string select, string where, string orde);
        int Add(BankCashAPI group);
        int Update(BankCashAPI functions);
        List<BankCashAPI> GetFilter(int UserId, int Top, string OrderNo, int Amount, int Status, string FromDate = "", string ToDate = "");
        // Lấy báo cáo
        List<BankGateReport> Report(int UserId, int year, int month, int day, ref int totalTransaction, ref long totalAmount);
        List<BankCashAPI> ReportDoiSoat(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect = 1);
        List<BankCashAPI> ListReportDoiSoat(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect);
        List<BankCashAPI> ReportDoiSoatDaily(int UserId, DateTime beginTime, DateTime endTime, int TypeSelect);


    }
}
