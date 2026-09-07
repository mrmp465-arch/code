using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.CardTelco.PayPlusApp
{
    public class UserType
    {
        public string type { get; set; }
        public string type_name { get; set; }
        public string user_type { get; set; }
        public string user_type_name { get; set; }
    }

    public class Data2
    {
        public string token { get; set; }
        public string keyRefresh { get; set; }
        public int isChargePasswordSet { get; set; }
        public object telType { get; set; }
        public string display_name { get; set; }
        public string phone_number { get; set; }
        public int is_security { get; set; }
        public int isLockApp { get; set; }
        public int survey { get; set; }
        public UserType user_type { get; set; }
        public string serviceType { get; set; }
        public string contract_id { get; set; }
        public string sub_id { get; set; }
        public string cusId { get; set; }
        public string contractPhone { get; set; }
        public string productCode { get; set; }
        public string user_type_name { get; set; }
        public string fullName { get; set; }
        public string birthday { get; set; }
        public string cmnd_date { get; set; }
        public string cmnd_place { get; set; }
        public string subscriber_classId { get; set; }
        public string subscriber_className { get; set; }
        public string subscriber_isdn { get; set; }
        public object pointRate_pri { get; set; }
        public object pointExchange_pri { get; set; }
        public object subName_pri { get; set; }
        public object birthDay_pri { get; set; }
        public object startDate_pri { get; set; }
        public object endDate_pri { get; set; }
        public int is_privilege { get; set; }
        public int is_member { get; set; }
        public string adminPrivilege { get; set; }
        public string avatar { get; set; }
        public string email { get; set; }
        public string job { get; set; }
        public string hobby { get; set; }
        public int is_viettel_user { get; set; }
        public List<string> jobs { get; set; }
        public List<string> hobbies { get; set; }
        public int time_syn { get; set; }
        public string theme { get; set; }
        public string contactNo { get; set; }
        public string lastSynContact { get; set; }
        public bool need_confirm_device { get; set; }
    }

    public class Data
    {
        public int debug_mode { get; set; }
        public int errorCode { get; set; }
        public string message { get; set; }
        public Data2 data { get; set; }
    }

    public class RootObject
    {
        public string errorCode { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public int flagChucTet { get; set; }
    }
}
