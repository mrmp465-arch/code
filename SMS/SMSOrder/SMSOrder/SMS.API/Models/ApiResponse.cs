using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.API.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }

        //public long Total { get; set; }
    }
}