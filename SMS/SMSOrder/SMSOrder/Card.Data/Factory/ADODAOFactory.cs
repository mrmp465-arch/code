using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.Data.Service;


namespace SMS.Data.Factory
{
    public class ADODAOFactory : AbstractDAOFactory
    {
        public override ICampaignsService CampaignsService()
        {
            return new CampaignsService();
        }
        public override ISMSLogsService SMSLogsService()
        {
            return new SMSLogsService();
        }
        public override IUsersService UsersService()
        {
            return new UsersService();
        }
        public override ITransactionsService TransactionsService()
        {
            return new TransactionService();
        }
        public override IContentsService ContentsService()
        {
            return new ContentsService();
        }
       
    }
}
