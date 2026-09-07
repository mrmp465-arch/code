using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IZISoft.Entity
{

    public class Group
    {
        public int groupId { get; set; }
        public string groupname { get; set; }
        public string code { get; set; }
    }

    public class Account
    {
        public long accId { get; set; }
        public string username { get; set; }
        public List<Group> groups { get; set; }
    }

    public class Data
    {
        public Account account { get; set; }
        public string sid { get; set; }
    }

    public class ProfileResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }
}