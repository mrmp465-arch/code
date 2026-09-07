using System;
using System.Collections.Generic;
using SMS.Data.DTO;

namespace SMS.Data.Service
{
    public interface ISMSLogsService
    {
        SMSLogs GetSMS(int id);

        List<SMSReport> GetReporSim(string fromdate, string telco);
        List<SMSReport> GetReportDaily(Users user, string fromdate, string todate);
        List<SMSReport> GetReporHour(Users user, string fromdate, string todate);

        List<SMSReport> GetReporRevenue(string fromdate, string todate, Users user);
        int UpdateTime(int Id, DateTime StartTime);
        int Add(string Number, string Content, string Username, int Telco, int CampaignId,int price);
        List<SMSLogs>  GetFilter(Users user, int cId, string keyword, string port, string telco, int status, int page, int pageSize, ref int total);
        List<SMSLogs> GetTopSMS(int top, int Telco);
        List<SMSReport> GetReporUser(string fromdate, string telco);
        List<SMSReport> GetReporLockSim(DateTime fromdate, DateTime todate);
        List<SMSReport>  GetReporTopup(DateTime fromdate, DateTime todate);
        int UpdateDynamic(string where, string updatest);
        void UpdateRespone(long Id, int CampaignId, int Status, string Message);
        void UpdateMultiRespone(List<long> lstId, int CampaignId, int Status, string Message);
    }
}
