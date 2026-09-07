using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Card.Data.Service;
using Card.Data.DTO;
using Card.Utility;


namespace Card.Data.Service
{
    public class BidHistoryService : IBidHistoryService
    {
      
        public int InsertBidHistory(BidHistory log)
        {
            try
            {
                var pars = new SqlParameter[5];
                pars[0] = new SqlParameter("@_UserID", log.UserID);
                pars[1] = new SqlParameter("@_OrderId", log.OrderId);
                pars[2] = new SqlParameter("@_Description", log.Description);
                pars[3] = new SqlParameter("@_OrderNo", log.OrderNo);
                pars[4] = new SqlParameter("@_ResponseCode", SqlDbType.Int) { Direction = ParameterDirection.Output };
                new DBHelper(Config.MainConnectionString).ExecuteNonQuerySP("SP_BidHistory_Insert", pars);
                return Convert.ToInt32(pars[4].Value);
            }
            catch (Exception ex)
            {
                 NLogLogger.PublishException(ex);
                return -99;
            }
        }


        public List<BidHistory> GetListBidHistory(int userId, long OrderId)
        {
            try
            {
                var pars = new SqlParameter[2];
                pars[0] = new SqlParameter("@_UserID", userId);
                pars[1] = new SqlParameter("@_OrderId", OrderId);
                var list_result = new DBHelper(Config.MainConnectionString).GetListSP<BidHistory>("SP_BidHistory_GetList", pars);
               
                if (list_result == null || list_result.Count <= 0)
                    return new List<BidHistory>();
                return list_result;
            }
            catch (Exception ex)
            {
                 NLogLogger.PublishException(ex);
                return new List<BidHistory>();
            }
        }



       
    }
}
