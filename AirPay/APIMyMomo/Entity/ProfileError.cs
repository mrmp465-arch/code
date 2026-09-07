using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMomo.Entity
{

    public class PError
    {
        public int statusCode { get; set; }
        public string name { get; set; }
        public string message { get; set; }
    }

    public class ProfileError
    {
        public PError error { get; set; }
    }

}