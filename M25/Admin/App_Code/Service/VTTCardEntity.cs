using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for VTTCardEntity
/// </summary>
namespace APIProxy.VTTService
{
    public class VTTCardEntity
    {
        public string cardSerial { set; get; }
        public string cardValue { set; get; }
        public string cardExpired { set; get; }
        public string isdn { set; get; }
        public string ownerName { set; get; }
        public string dateUsed { set; get; }
    }
}