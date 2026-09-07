using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Data.Api
{


    public class Position
    {
        public int Port { get; set; }
        public int Slot { get; set; }
    }
    public class PortInfo
    {
        public Position Position { get; set; }
        public string Telco { get; set; }
        public string Number { get; set; }
        public int Fail { get; set; }
        public int Balance { get; set; }
        public int State { get; set; }
        public int Count { get; set; }
        public bool IsPrepaid { get; set; }
        public string Remark { get; set; }
        public string TelcoName { get; set; }
        public bool NeedSwitch { get; set; }
        public string LastChangedDateTime { get; set; }

        public DateTime LastChanged
        {
            get
            {
                if (string.IsNullOrEmpty(LastChangedDateTime))
                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                return DateTime.Parse(LastChangedDateTime);
            }
        }
    }

}