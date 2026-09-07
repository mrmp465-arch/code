using Card.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.Service
{
    public interface IBidHistoryService
    {
        List<BidHistory> GetListBidHistory(int userId, long OrderId);
     
        int InsertBidHistory(BidHistory log);
       
    }
}
