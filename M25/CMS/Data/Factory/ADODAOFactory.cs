using CMS.Data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace CMS.Data.Factory
{
    public class ADODAOFactory : AbstractDAOFactory
    {
      
        public override IUsersLogService UsersLogService()
        {
            return new UsersLogService();
        }
        public override IUsersService UsersService()
        {
            return new UsersService();
        }
        public override IFucntionsService FunctionsService()
        {
            return new FunctionsService();
        }
        public override IGroupsService GroupsService()
        {
            return new GroupsService();
        }
        public override IUserRoleService UserRoleService()
        {
            return new UserRoleService();
        }
        public override IUserPartnersService UserPartnersService()
        {
            return new UserPartnersService();
        }
    }
}
