using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMomo.Entity
{
    
    public class AppInfo
    {
        public string id { get; set; }
        public string version { get; set; }
        public string firebase_app_id { get; set; }
    }

    public class Device
    {
        public string category { get; set; }
        public string mobile_brand_name { get; set; }
        public string mobile_model_name { get; set; }
        public string operating_system { get; set; }
        public string operating_system_version { get; set; }
    }

    public class CustomData
    {
        public string avatar { get; set; }
        public string _id { get; set; }
        public string name { get; set; }
        public string userName { get; set; }
    }

    public class Payload
    {
        public string type { get; set; }
        public string content { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public CustomData customData { get; set; }
    }

    public class Parts
    {
        public string partType { get; set; }
        public Payload payload { get; set; }
    }

    public class MessageStatus
    {
        public string _01698103845 { get; set; }
        public string _0912440644 { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public string customId { get; set; }
        public string senderId { get; set; }
        public string roomId { get; set; }
        public string requestId { get; set; }
        public Parts parts { get; set; }
        public bool isNotReceived { get; set; }
        public List<object> hideFrom { get; set; }
        public long createdAt { get; set; }
        public MessageStatus messageStatus { get; set; }
    }

    public class EventParams
    {
        public string action { get; set; }
        public string stage { get; set; }
        public string ip_address { get; set; }
        public string location { get; set; }
        public string momo_session_id { get; set; }
        public string phone_number { get; set; }
        public string net_infor1 { get; set; }
        public string net_infor2 { get; set; }
        public string mac_address { get; set; }
        public string roomID { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public string requestID { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string appVersion { get; set; }
        public string phoneOs { get; set; }
        public string message_type { get; set; }
        public Message message { get; set; }
    }

    public class MessageRequest
    {
        public string ip_address { get; set; }
        public string location { get; set; }
        public string momo_session_id { get; set; }
        public string phone_number { get; set; }
        public string net_infor1 { get; set; }
        public string net_infor2 { get; set; }
        public AppInfo app_info { get; set; }
        public Device device { get; set; }
        public string event_name { get; set; }
        public EventParams event_params { get; set; }
        public object timestamp { get; set; }
    }


}