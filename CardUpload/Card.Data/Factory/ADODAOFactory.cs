using Card.Data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SMS.Data.Factory
{
    public class ADODAOFactory : AbstractDAOFactory
    {
        public override IOrderReportsService OrderReportsService()
        {
            return new OrderReportsService();
        }
        public override IBidHistoryService BidHistoryService()
        {
            return new BidHistoryService();
        }
        public override ITransactionsService TransactionService()
        {
            return new TransactionService();
        }
        public override ITopupOrderService TopupOrderService()
        {
            return new TopupOrderService();
        }
        public override IUsersService UsersService()
        {
            return new UsersService();
        }
        public override ICardOrderService CardOrderService()
        {
            return new CardOrderService();
        }
        public override IBuyCardService BuyCardService()
        {
            return new BuyCardService();
        }
    }
}
