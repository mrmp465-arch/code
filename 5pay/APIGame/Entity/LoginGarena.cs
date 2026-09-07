using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIGame.Entity
{
    public class LoginGarena
    {
    }

    public class PreLogin
    {
        public string v1 { get; set; }
        public string account { get; set; }
        public string v2 { get; set; }
        public string id { get; set; }
    }

    public class GrantToken
    {
        public string access_token { get; set; }
        public List<string> scope { get; set; }
        public string redirect_uri { get; set; }
        public string open_id { get; set; }
        public int platform { get; set; }
    }

    public class InspectToken
    {
        public string token { get; set; }
    }

    public class Exec
    {
        public string display_id { get; set; }
    }

    public class PayResponse
    {
        public string display_id { get; set; }
        public string result { get; set; }
        public Exec exec { get; set; }
    }

    public class ChannelData
    {
        public string card_password { get; set; }
        public string friend_username { get; set; }

    }

    public class ChannelDataCaptcha
    {
        public string card_password { get; set; }
        public string friend_username { get; set; }
        public string captchaKey { get; set; }
        public string captcha { get; set; }

    }

    public class PayRequest
    {
        public string service { get; set; }
        public int app_id { get; set; }
        public int packed_role_id { get; set; }
        public int channel_id { get; set; }
        public ChannelData channel_data { get; set; }
    }

    public class PayRequestOpenId
    {
        public string service { get; set; }
        public int app_id { get; set; }
        public int packed_role_id { get; set; }
        public int channel_id { get; set; }
        public ChannelData channel_data { get; set; }
        public string open_id { get; set; }
    }

    public class PayRequestCaptchaOpenId
    {
        public string service { get; set; }
        public int app_id { get; set; }
        public int packed_role_id { get; set; }
        public int channel_id { get; set; }

        public string captcha_key { get; set; }
        public string captcha { get; set; }
        public ChannelDataCaptcha channel_data { get; set; }
        public string open_id { get; set; }
    }

    public class PayRequestCaptcha
    {
        public string service { get; set; }
        public int app_id { get; set; }
        public int packed_role_id { get; set; }
        public int channel_id { get; set; }

        public string captcha_key { get; set; }
        public string captcha { get; set; }
        public ChannelDataCaptcha channel_data { get; set; }
    }

    public class PayCheckRequest
    {
        public string display_id { get; set; }
    }

    public class Promo
    {
        public int promo_amount { get; set; }
        public List<object> promo_item { get; set; }
    }

    public class App
    {
        public string point_icon { get; set; }
        public bool client_share { get; set; }
        public int app_id { get; set; }
        public List<int> platforms { get; set; }
        public string point_name { get; set; }
        public string app_name { get; set; }
        public string icon { get; set; }
    }

    public class PayCheckResponse
    {
        public string display_id { get; set; }
        public string currency { get; set; }
        public int update_time { get; set; }
        public string currency_symbol { get; set; }
        public Promo promo { get; set; }
        public int point_amount { get; set; }
        public App app { get; set; }
        public string channel { get; set; }
        public int currency_amount { get; set; }
    }

    public class LoginOpenIdRequest
    {
        public int app_id { get; set; }
        public string login_id { get; set; }
    }
    public class LoginOpenId
    {
        public string region { get; set; }
        public string nickname { get; set; }
        public string open_id { get; set; }
        public string error { get; set; }
    }
}