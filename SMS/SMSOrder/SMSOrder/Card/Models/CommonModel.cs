using SMS.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.CMS.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string value { get; set; }
    }
    public class ModelFunctionDetail
    {
        public List<Functions> ListFunction { get; set; }
        public Functions FunctionDetail { get; set; }
    }

    public class ReturnData
    {
        public long ResponseCode { get; set; }
        public string Description { get; set; }
        public string Extended { get; set; }
    }

    public class UserFunctionModel
    {
        public List<Functions> ListFunction { get; set; }
        public List<UserFunction> UserFunction { get; set; }
    }

    public class FunctionOrder
    {
        public int Id { get; set; }
        public int FatherID { get; set; }
        public int Order { get; set; }
    }
    public class MetaDataUser
    {
        public int Balance { get; set; }
        public int NumberContact { get; set; }
        public int NumberSMSSend { get; set; }
        public int NumberSMSFinish { get; set; }
    }
}