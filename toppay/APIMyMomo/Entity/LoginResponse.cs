using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMomo.Entity
{
    public class MomoMsg
    {
        public string userId { get; set; }
        public int agentId { get; set; }
        public bool isReged { get; set; }
        public bool isActived { get; set; }
        public string identify { get; set; }
        public int capset { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public long lastLogin { get; set; }
        public string phoneOs { get; set; }
        public int appVer { get; set; }
        public string appCode { get; set; }
        public string langCode { get; set; }
        public string groupId { get; set; }
        public string createDate { get; set; }
        public string deviceName { get; set; }
        public bool isEu { get; set; }
        public bool untouchedClaimCode { get; set; }
        public bool startTrackerTrans { get; set; }
        public string verifyInfo { get; set; }
        public string bankVerifyName { get; set; }
        public string bankVerifyPersonalid { get; set; }
        public string bankVerifyDob { get; set; }
        public string walletStatus { get; set; }
        public string nationality { get; set; }
        public string lastImei { get; set; }
        public double lastSessionTime { get; set; }
        public string bankCode { get; set; }
        public string pushToken { get; set; }
        public string firmware { get; set; }
        public string hardware { get; set; }
        public string manufacture { get; set; }
        public string csp { get; set; }
        public string mcc { get; set; }
        public string mnc { get; set; }
        public string oneSignalToken { get; set; }
        public string rkey { get; set; }
        public string validateEmail { get; set; }
        public string emailKey { get; set; }
        public string idfa { get; set; }
        public bool untouchedMap { get; set; }
        public bool untouchedTrans { get; set; }
        public bool untouchedCashIn { get; set; }
        public bool isAccepted { get; set; }
        public int gender { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public List<string> beGroups { get; set; }
        public string deviceToken { get; set; }
        public bool fastLogin { get; set; }
        public string _class { get; set; }
    }

    public class Extra
    {
        public string ONESIGNAL_TOKEN { get; set; }
        public string POINT_TO_CURRENT_LEVEL { get; set; }
        public string listServiceCodeNotUpperCase { get; set; }
        public string REQUEST_ENCRYPT_KEY { get; set; }
        public string MODELID { get; set; }
        public string PRE_LOGIN { get; set; }
        public string EMAIL { get; set; }
        public string LOGIN_LAST_UPDATE_BANKLINK { get; set; }
        public string DEVICE_TOKEN { get; set; }
        public string FIRST_TIME_LOGIN { get; set; }
        public string LEVEL_PERCENT { get; set; }
        public string LOGIN_LAST_UPDATE_LOYALTY_CFG { get; set; }
        public string pHash { get; set; }
        public string VISA_NEW_FLOW { get; set; }
        public string ACCUMULATED_POINT { get; set; }
        public string FULL_NAME { get; set; }
        public string SIMULATOR { get; set; }
        public string POINT_TO_NEXT_LEVEL { get; set; }
        public string TOKEN { get; set; }
        public string AUTH_TOKEN { get; set; }
        public string LOGIN_LAST_UPDATE_PROVIDER { get; set; }
        public string SESSION_KEY { get; set; }
        public string NEXT_LEVEL { get; set; }
        public string LOGIN_LAST_UPDATE_SERVICE { get; set; }
        public string REFRESH_TOKEN { get; set; }
        public string LEVEL { get; set; }
        public string BALANCE { get; set; }
        public string LOGIN_LAST_UPDATE_CATEGORY { get; set; }
        public string IDFA { get; set; }
        public string checkSum { get; set; }
    }

    public class LoginResponse
    {
        public MomoMsg momoMsg { get; set; }
        public long time { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public string cmdId { get; set; }
        public string lang { get; set; }
        public string msgType { get; set; }
        public bool result { get; set; }
        public int errorCode { get; set; }
        public string errorDesc { get; set; }
        public string appCode { get; set; }
        public int appVer { get; set; }
        public string channel { get; set; }
        public string deviceOS { get; set; }
        public string ip { get; set; }
        public string localAddress { get; set; }
        public string session { get; set; }
        public Extra extra { get; set; }
    }


}