using System;
using System.Collections.Generic;
using CMS.Data.DTO;

namespace CMS.Data.Service
{
    public interface IUserPartnersService
    {

       
        List<UserPartner> GetList(int UserId);
        int Add(UserPartner group);
    	int Delete(int Id);
        
    }
}
