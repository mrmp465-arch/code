using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Games
/// </summary>
public class Games
{
    public Games()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public class ZingGame
    {
        public string serverID { set; get; }
        public string roleID { set; get; }
        public string productID { set; get; }
        public string roleName { set; get; }
        public string amount { set; get; }

    }
}