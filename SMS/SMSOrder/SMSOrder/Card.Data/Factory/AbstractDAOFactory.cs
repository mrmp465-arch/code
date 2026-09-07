using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.Data.Service;


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
        public abstract IContentsService ContentsService();
        public abstract ICampaignsService CampaignsService();
        public abstract ISMSLogsService SMSLogsService();
        public abstract IUsersService UsersService();

        public abstract ITransactionsService TransactionsService();

       

    }
}

