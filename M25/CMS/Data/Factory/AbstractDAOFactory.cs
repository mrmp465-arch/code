using CMS.Data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace CMS.Data.Factory
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
      

        public abstract IUsersLogService UsersLogService();

        public abstract IUsersService UsersService();

        public abstract IFucntionsService FunctionsService();

        public abstract IGroupsService GroupsService();

        public abstract IUserRoleService UserRoleService();

        public abstract IUserPartnersService UserPartnersService();
    }
}

