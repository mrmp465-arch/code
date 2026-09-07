using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Card.Data.Service;



namespace SMS.Data.Factory
{
    public abstract class AbstractDAOFactory
    {
        public static AbstractDAOFactory Instance()
        {
            try
            {
                return (AbstractDAOFactory)new ADODAOFactory();
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't create AbstractDAOFactory: ");
            }
        }
        public abstract IOrderReportsService OrderReportsService();

        public abstract IBidHistoryService BidHistoryService();

        public abstract ITransactionsService TransactionService();

        public abstract IUsersService UsersService();

        public abstract ICardOrderService CardOrderService();
        public abstract IBuyCardService BuyCardService();

        public abstract ITopupOrderService TopupOrderService();
    }
}

