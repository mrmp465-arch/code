using System;
using System.Collections.Generic;
using Card.Data.Api;
using Card.Data.DTO;

namespace Card.Data.Service
{
    public interface IOrderTempsService
    {

        OrderInput Get(int Id);
       
        List<OrderInput> GetFilter(string username);
       
        List<OrderInput> GetList(string select, string where, string orde);
        int InsertUpdate(OrderInput group);
    	int Delete(int Id);
       
    }
}
