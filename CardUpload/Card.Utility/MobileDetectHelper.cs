using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Card.Utility
{
    public class MobileDetectHelper
    {
        private string _useragent = "";
        private string _httpaccept = "";

        #region Fields - Detection Argument Values

        //standardized values for detection arguments. DetectMacintoshPc
        private const string DargsMacintosh = "macintosh";
        private const string DargsWindows = "windows";
        private const string DargsIphone = "iphone";
        private const string DargsIpod = "ipod";
        private const string DargsIpad = "ipad";
        private const string DargsIphoneOrIpod = "iphoneoripod";
        private const string DargsIos = "ios";
        private const string DargsAndroid = "android";
        private const string DargsAndroid21 = "android 2.1";
        private const string DargsAndroidPhone = "androidphone";
        private const string DargsAndroidTablet = "androidtablet";
        private const string DargsGoogleTv = "googletv";
        private const string DargsWebKit = "webkit";
        private const string DargsSymbianOs = "symbianos";
        private const string DargsS60 = "series60";
        private const string DargsWindowsPhone7 = "windowsphone7";
        private const string DargsWindowsMobile = "windowsmobile";
        private const string DargsBlackBerry = "blackberry";
        private const string DargsBlackBerryWebkit = "blackberrywebkit";
        private const string DargsPalmOs = "palmos";
        private const string DargsPalmWebOs = "webos";
        private const string DargsSmartphone = "smartphone";
        private const string DargsBrewDevice = "brew";
        private const string DargsDangerHiptop = "dangerhiptop";
        private const string DargsOperaMobile = "operamobile";
        private const string DargsWapWml = "wapwml";
        private const string DargsKindle = "kindle";
        private const string DargsMobileQuick = "mobilequick";
        private const string DargsTierTablet = "tiertablet";
        private const string DargsTierIphone = "tieriphone";
        private const string DargsTierRichCss = "tierrichcss";
        private const string DargsTierOtherPhones = "tierotherphones";

        #endregion Fields - Detection Argument Values

        #region Fields - User Agent Keyword Values

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper disable InconsistentNaming
        //Initialize some initial smartphone private string private stringiables. Macintosh
        private readonly string _pcMacintosh = "macintosh".ToUpper();
        private readonly string _pcWindow = "windows".ToUpper();
        private readonly string _engineWebKit = "webkit".ToUpper();
        private readonly string _deviceIphone = "iphone".ToUpper();
        private readonly string _deviceIpod = "ipod".ToUpper();
        private readonly string _deviceIpad = "ipad".ToUpper();
        private readonly string _deviceMacPpc = "macintosh".ToUpper(); //Used for disambiguation

        private readonly string _deviceAndroid = "android".ToUpper();
        private readonly string _deviceAndroid21 = "android 2.1".ToUpper();

        private readonly string _deviceGoogleTv = "googletv".ToUpper();
        private readonly string _googleBot = "googlebot".ToUpper();
        private readonly string _deviceXoom = "xoom".ToUpper(); //Motorola Xoom
        private readonly string _deviceHtcFlyer = "htc_flyer".ToUpper(); //HTC Flyer

        private readonly string _deviceNuvifone = "nuvifone".ToUpper();  //Garmin Nuvifone

        private readonly string _deviceSymbian = "symbian".ToUpper();
        private readonly string _deviceS60 = "series60".ToUpper();
        private readonly string _deviceS70 = "series70".ToUpper();
        private readonly string _deviceS80 = "series80".ToUpper();
        private readonly string _deviceS90 = "series90".ToUpper();

        private readonly string _deviceWinPhone7 = "windows phone os 7".ToUpper();
        private readonly string _deviceWinMob = "windows ce".ToUpper();
        private readonly string _deviceWindows = "windows".ToUpper();
        private readonly string _deviceIeMob = "iemobile".ToUpper();
        private readonly string _devicePpc = "ppc".ToUpper(); //Stands for PocketPC
        private readonly string _enginePie = "wm5 pie".ToUpper(); //An old Windows Mobile

        private readonly string _deviceBb = "blackberry".ToUpper();
        private readonly string _vndRim = "vnd.rim".ToUpper(); //Detectable when BB devices emulate IE or Firefox
        private readonly string deviceBBStorm = "blackberry95".ToUpper(); //Storm 1 and 2
        private readonly string deviceBBBold = "blackberry97".ToUpper(); //Bold
        private readonly string deviceBBTour = "blackberry96".ToUpper(); //Tour
        private readonly string deviceBBCurve = "blackberry89".ToUpper(); //Curve2
        private readonly string deviceBBTorch = "blackberry 98".ToUpper(); //Torch
        private readonly string deviceBBPlaybook = "playbook".ToUpper(); //PlayBook tablet

        private readonly string devicePalm = "palm".ToUpper();
        private readonly string deviceWebOS = "webos".ToUpper(); //For Palm's new WebOS devices
        private readonly string engineBlazer = "blazer".ToUpper(); //Old Palm
        private readonly string engineXiino = "xiino".ToUpper(); //Another old Palm

        private string deviceKindle = "kindle".ToUpper();  //Amazon Kindle, eInk one.

        //Initialize private stringiables for mobile-specific content.
        private string vndwap = "vnd.wap".ToUpper();
        private string wml = "wml".ToUpper();

        //Initialize private stringiables for other random devices and mobile browsers.
        private string deviceBrew = "brew".ToUpper();
        private string deviceDanger = "danger".ToUpper();
        private string deviceHiptop = "hiptop".ToUpper();
        private string devicePlaystation = "playstation".ToUpper();
        private string deviceNintendoDs = "nitro".ToUpper();
        private string deviceNintendo = "nintendo".ToUpper();
        private string deviceWii = "wii".ToUpper();
        private string deviceXbox = "xbox".ToUpper();
        private string deviceArchos = "archos".ToUpper();

        private string engineOpera = "opera".ToUpper(); //Popular browser
        private string engineNetfront = "netfront".ToUpper(); //Common embedded OS browser
        private string engineUpBrowser = "up.browser".ToUpper(); //common on some phones
        private string engineOpenWeb = "openweb".ToUpper(); //Transcoding by OpenWave server
        private string deviceMidp = "midp".ToUpper(); //a mobile Java technology
        private string uplink = "up.link".ToUpper();
        private string engineTelecaQ = "teleca q".ToUpper(); //a modern feature phone browser

        private string devicePda = "pda".ToUpper(); //some devices report themselves as PDAs
        private string mini = "mini".ToUpper();  //Some mobile browsers put "mini" in their names.
        private string mobile = "mobile".ToUpper(); //Some mobile browsers put "mobile" in their user agent private strings.
        private string mobi = "mobi".ToUpper(); //Some mobile browsers put "mobi" in their user agent private strings.

        //Use Maemo, Tablet, and Linux to test for Nokia"s Internet Tablets.
        private string maemo = "maemo".ToUpper();
        private string maemoTablet = "tablet".ToUpper();
        private string linux = "linux".ToUpper();
        private string qtembedded = "qt embedded".ToUpper(); //for Sony Mylo
        private string mylocom2 = "com2".ToUpper(); //for Sony Mylo also

        //In some UserAgents, the only clue is the manufacturer.
        private string manuSonyEricsson = "sonyericsson".ToUpper();
        private string manuericsson = "ericsson".ToUpper();
        private string manuSamsung1 = "sec-sgh".ToUpper();
        private string manuSony = "sony".ToUpper();
        private string manuHtc = "htc".ToUpper(); //Popular Android and WinMo manufacturer

        //In some UserAgents, the only clue is the operator.
        private string svcDocomo = "docomo".ToUpper();
        private string svcKddi = "kddi".ToUpper();
        private string svcVodafone = "vodafone".ToUpper();

        //Disambiguation strings.
        private string disUpdate = "update".ToUpper(); //pda vs. update
        // ReSharper restore InconsistentNaming
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        #endregion Fields - User Agent Keyword Values

        /// <summary>
        /// To run the device detection methods andd fire 
        /// any existing OnDetectXXX events. 
        /// </summary>
        public MobileDetectHelper()
        {
            if (_useragent == "" && _httpaccept == "")
            {
                if (HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"] != null)
                {
                    _useragent = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToUpper();
                }

                if (HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT"] != null)
                {
                    _httpaccept = HttpContext.Current.Request.ServerVariables["HTTP_ACCEPT"].ToUpper();
                }
            }

        }
        public MobileDetectHelper(HttpContext context)
        {
            if (_useragent == "" && _httpaccept == "")
            {
                if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
                {
                    _useragent = context.Request.ServerVariables["HTTP_USER_AGENT"].ToUpper();
                }

                if (context.Request.ServerVariables["HTTP_ACCEPT"] != null)
                {
                    _httpaccept = context.Request.ServerVariables["HTTP_ACCEPT"].ToUpper();
                }
            }

        }
        public MobileDetectHelper(System.Web.HttpRequest context)
        {
            if (_useragent == "" && _httpaccept == "")
            {
                if (context.ServerVariables["HTTP_USER_AGENT"] != null)
                {
                    _useragent = context.ServerVariables["HTTP_USER_AGENT"].ToUpper();
                }

                if (context.ServerVariables["HTTP_ACCEPT"] != null)
                {
                    _httpaccept = context.ServerVariables["HTTP_ACCEPT"].ToUpper();
                }
            }

        }
        public class MDetectArgs : EventArgs
        {
            public MDetectArgs(string type)
            {
                Type = type;
            }

            public readonly string Type;
        }

        #region Mobile Device Detection Methods

        //**************************
        // Detects if the current device is an iPod Touch.
        public bool DetectIpod()
        {
            if (_useragent.IndexOf(_deviceIpod) != -1)
                return true;
            return false;
        }

        //Ipod delegate
        public delegate void DetectIpodHandler(object page, MDetectArgs args);
        public event DetectIpodHandler OnDetectIpod;


        //**************************
        // Detects if the current device is an iPad tablet.
        public bool DetectIpad()
        {
            if (_useragent.IndexOf(_deviceIpad) != -1 && DetectWebkit())
                return true;
            return false;
        }

        //Ipod delegate
        public delegate void DetectIpadHandler(object page, MDetectArgs args);
        public event DetectIpadHandler OnDetectIpad;


        //**************************
        // Detects if the current device is an iPhone.
        public bool DetectIphone()
        {
            if (_useragent.IndexOf(_deviceIphone) != -1)
            {
                //The iPad and iPod touch say they're an iPhone! So let's disambiguate.
                if (DetectIpad() || DetectIpod())
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        //IPhone delegate
        public delegate void DetectIphoneHandler(object page, MDetectArgs args);
        public event DetectIphoneHandler OnDetectIphone;

        //**************************
        // Detects if the current device is an iPhone or iPod Touch.
        public bool DetectIphoneOrIpod()
        {
            //We repeat the searches here because some iPods may report themselves as an iPhone, which would be okay.
            if (_useragent.IndexOf(_deviceIphone) != -1 ||
                _useragent.IndexOf(_deviceIpod) != -1)
                return true;
            return false;
        }

        //IPhoneOrIpod delegate
        public delegate void DetectIPhoneOrIpodHandler(object page, MDetectArgs args);
        public event DetectIPhoneOrIpodHandler OnDetectDetectIPhoneOrIpod;

        //**************************
        // Detects *any* iOS device: iPhone, iPod Touch, iPad.
        public bool DetectIos()
        {
            if (DetectIphoneOrIpod() || DetectIpad())
                return true;
            return false;
        }

        //Ios delegate
        public delegate void DetectIosHandler(object page, MDetectArgs args);
        public event DetectIosHandler OnDetectIos;

        public delegate void DetectWindowsPcHandler(object page, MDetectArgs args);
        public event DetectWindowsPcHandler OnDetectWindowPc;
        // Detects *any* iOS device: iPhone, iPod Touch, iPad.
        public bool DetectWindowPc()
        {
            if (_useragent.IndexOf(_pcWindow) != -1)
                return true;
            return false;
        }

        public delegate void DetectMacintoshPcHandler(object page, MDetectArgs args);
        public event DetectMacintoshPcHandler OnDetectMacintoshPc;
        // Detects *any* iOS device: iPhone, iPod Touch, iPad.
        public bool DetectMacintoshPc()
        {
            if (_useragent.IndexOf(_pcMacintosh) != -1)
                return true;
            return false;
        }


        //**************************
        // Detects *any* Android OS-based device: phone, tablet, and multi-media player.
        // Also detects Google TV.
        public bool DetectAndroid()
        {
            if ((_useragent.IndexOf(_deviceAndroid) != -1) ||
                DetectGoogleTV())
                return true;
            //Special check for the HTC Flyer 7" tablet. It should report here.
            if (_useragent.IndexOf(_deviceHtcFlyer) != -1)
                return true;
            return false;
        }
        //Android delegate
        public delegate void DetectAndroidHandler(object page, MDetectArgs args);
        public event DetectAndroidHandler OnDetectAndroid;

        //**************************
        // Detects *any* Android OS-based device: phone, tablet, and multi-media player.
        // Also detects Google TV.
        public bool DetectAndroid21()
        {
            if ((_useragent.IndexOf(_deviceAndroid21) != -1) ||
                DetectGoogleTV())
                return true;
            //Special check for the HTC Flyer 7" tablet. It should report here.
            if (_useragent.IndexOf(_deviceHtcFlyer) != -1)
                return true;
            return false;
        }
        //Android delegate
        public delegate void DetectAndroid21Handler(object page, MDetectArgs args);
        public event DetectAndroid21Handler OnDetectAndroid21;

        //**************************
        // Detects if the current device is a (small-ish) Android OS-based device
        // used for calling and/or multi-media (like a Samsung Galaxy Player).
        // Google says these devices will have 'Android' AND 'mobile' in user agent.
        // Ignores tablets (Honeycomb and later).
        public bool DetectAndroidPhone()
        {
            if (DetectAndroid() &&
                (_useragent.IndexOf(mobile) != -1))
                return true;
            //Special check for the HTC Flyer 7" tablet. It should report here.
            if (_useragent.IndexOf(_deviceHtcFlyer) != -1)
                return true;
            return false;
        }
        //Android Phone delegate
        public delegate void DetectAndroidPhoneHandler(object page, MDetectArgs args);
        public event DetectAndroidPhoneHandler OnDetectAndroidPhone;

        //**************************
        // Detects if the current device is a (self-reported) Android tablet.
        // Google says these devices will have 'Android' and NOT 'mobile' in their user agent.
        public bool DetectAndroidTablet()
        {
            //Special check for the HTC Flyer 7" tablet. It should NOT report here.
            if (_useragent.IndexOf(_deviceHtcFlyer) != -1)
                return false;

            if (DetectAndroid() && !(_useragent.IndexOf(mobile) != -1))
                return true;
            return false;
        }
        //Android Tablet delegate
        public delegate void DetectAndroidTabletHandler(object page, MDetectArgs args);
        public event DetectAndroidTabletHandler OnDetectAndroidTablet;

        //**************************
        // Detects if the current device is a GoogleTV device.
        public bool DetectGoogleTV()
        {
            if (_useragent.IndexOf(_deviceGoogleTv) != -1)
            {
                return true;
            }
            return false;
        }

        //GoogleTV delegate
        public delegate void DetectGoogleTVHandler(object page, MDetectArgs args);
        public event DetectGoogleTVHandler OnDetectGoogleTV;

        //**************************
        // Detects if the current device is a GoogleTV device.
        public bool DetectGoogleBot()
        {
            if (_useragent.IndexOf(_googleBot) != -1)
            {
                return true;
            }
            return false;
        }

        //**************************
        // Detects if the current device is an Android OS-based device and
        //   the browser is based on WebKit.
        public bool DetectAndroidWebKit()
        {
            if (DetectAndroid() && DetectWebkit())
            {
                return true;
            }
            return false;
        }

        //**************************
        // Detects if the current browser is based on WebKit.
        public bool DetectWebkit()
        {
            if (_useragent.IndexOf(_engineWebKit) != -1)
            {
                return true;
            }
            return false;
        }

        //Webkit delegate
        public delegate void DetectWebkitHandler(object page, MDetectArgs args);
        public event DetectWebkitHandler OnDetectWebkit;

        //**************************
        // Detects if the current browser is the Nokia S60 Open Source Browser.
        public bool DetectS60OssBrowser()
        {
            //First, test for WebKit, then make sure it's either Symbian or S60.
            if (DetectWebkit())
            {
                if (_useragent.IndexOf(_deviceSymbian) != -1 ||
                    _useragent.IndexOf(_deviceS60) != -1)
                {
                    return true;
                }
                return false;
            }

            return false;
        }

        //S60OssBrowser delegate
        public delegate void DetectS60OssBrowserHandler(object page, MDetectArgs args);
        public event DetectS60OssBrowserHandler OnDetectS60OssBrowser;

        //**************************
        // Detects if the current device is any Symbian OS-based device,
        //   including older S60, Series 70, Series 80, Series 90, and UIQ, 
        //   or other browsers running on these devices.
        public bool DetectSymbianOS()
        {
            if (_useragent.IndexOf(_deviceSymbian) != -1 ||
                _useragent.IndexOf(_deviceS60) != -1 ||
                _useragent.IndexOf(_deviceS70) != -1 ||
                _useragent.IndexOf(_deviceS80) != -1 ||
                _useragent.IndexOf(_deviceS90) != -1)
                return true;
            return false;
        }

        //SymbianOS delegate
        public delegate void DetectSymbianOSHandler(object page, MDetectArgs args);
        public event DetectSymbianOSHandler OnDetectSymbianOS;

        //**************************
        // Detects if the current browser is a 
        // Windows Phone 7 device.
        public bool DetectWindowsPhone7()
        {
            if (_useragent.IndexOf(_deviceWinPhone7) != -1)
                return true;
            else
                return false;
        }

        //WindowsPhone7 delegate
        public delegate void DetectWindowsPhone7Handler(object page, MDetectArgs args);
        public event DetectWindowsPhone7Handler OnDetectWindowsPhone7;

        //**************************
        // Detects if the current browser is a Windows Mobile device.
        // Excludes Windows Phone 7 devices. 
        // Focuses on Windows Mobile 6.xx and earlier.
        public bool DetectWindowsMobile()
        {
            //Exclude new Windows Phone 7.
            if (DetectWindowsPhone7())
                return false;
            //Most devices use 'Windows CE', but some report 'iemobile' 
            //  and some older ones report as 'PIE' for Pocket IE. 
            if (_useragent.IndexOf(_deviceWinMob) != -1 ||
                _useragent.IndexOf(_deviceIeMob) != -1 ||
                _useragent.IndexOf(_enginePie) != -1)
                return true;
            //Test for Windows Mobile PPC but not old Macintosh PowerPC.
            if (_useragent.IndexOf(_devicePpc) != -1 &&
                !(_useragent.IndexOf(_deviceMacPpc) != -1))
                return true;
            //Test for certain Windwos Mobile-based HTC devices.
            if (_useragent.IndexOf(manuHtc) != -1 &&
                _useragent.IndexOf(_deviceWindows) != -1)
                return true;
            if (DetectWapWml() == true &&
                _useragent.IndexOf(_deviceWindows) != -1)
                return true;
            return false;
        }

        //WindowsMobile delegate
        public delegate void DetectWindowsMobileHandler(object page, MDetectArgs args);
        public event DetectWindowsMobileHandler OnDetectWindowsMobile;

        //**************************
        // Detects if the current browser is any BlackBerry device.
        // Includes the PlayBook.
        public bool DetectBlackBerry()
        {
            if ((_useragent.IndexOf(_deviceBb) != -1) ||
                (_httpaccept.IndexOf(_vndRim) != -1))
                return true;
            return false;
        }

        //BlackBerry delegate
        public delegate void DetectBlackBerryHandler(object page, MDetectArgs args);
        public event DetectBlackBerryHandler OnDetectBlackBerry;


        //**************************
        // Detects if the current browser is on a BlackBerry tablet device.
        //    Example: PlayBook
        public bool DetectBlackBerryTablet()
        {
            if (_useragent.IndexOf(deviceBBPlaybook) != -1)
                return true;
            return false;
        }

        //**************************
        // Detects if the current browser is a BlackBerry device AND uses a
        //    WebKit-based browser. These are signatures for the new BlackBerry OS 6.
        //    Examples: Torch. Includes the Playbook.
        public bool DetectBlackBerryWebKit()
        {
            if (DetectBlackBerry() && DetectWebkit())
                return true;
            return false;
        }

        //BlackBerry Webkit delegate
        public delegate void DetectBlackBerryWebkitHandler(object page, MDetectArgs args);
        public event DetectBlackBerryWebkitHandler OnDetectBlackBerryWebkit;


        //**************************
        // Detects if the current browser is a BlackBerry Touch
        //    device, such as the Storm or Touch. Excludes the Playbook.
        public bool DetectBlackBerryTouch()
        {
            if (DetectBlackBerry() &&
                (_useragent.IndexOf(deviceBBStorm) != -1 ||
                 _useragent.IndexOf(deviceBBTorch) != -1))
                return true;
            return false;
        }

        //**************************
        // Detects if the current browser is a BlackBerry device AND
        //    has a more capable recent browser. Excludes the Playbook.
        //    Examples, Storm, Bold, Tour, Curve2
        //    Excludes the new BlackBerry OS 6 browser!!
        public bool DetectBlackBerryHigh()
        {
            //Disambiguate for BlackBerry OS 6 (WebKit) browser
            if (DetectBlackBerryWebKit())
                return false;
            if (DetectBlackBerry())
            {
                if (DetectBlackBerryTouch() ||
                    _useragent.IndexOf(deviceBBBold) != -1 ||
                    _useragent.IndexOf(deviceBBTour) != -1 ||
                    _useragent.IndexOf(deviceBBCurve) != -1)
                    return true;
                return false;
            }
            return false;
        }

        //**************************
        // Detects if the current browser is a BlackBerry device AND
        //    has an older, less capable browser. 
        //    Examples: Pearl, 8800, Curve1.
        public bool DetectBlackBerryLow()
        {
            if (DetectBlackBerry())
            {
                //Assume that if it's not in the High tier, then it's Low.
                if (DetectBlackBerryHigh() || DetectBlackBerryWebKit())
                    return false;
                return true;
            }
            return false;
        }

        //**************************
        // Detects if the current browser is on a PalmOS device.
        public bool DetectPalmOS()
        {
            //Most devices nowadays report as 'Palm', but some older ones reported as Blazer or Xiino.
            if (_useragent.IndexOf(devicePalm) != -1 ||
                _useragent.IndexOf(engineBlazer) != -1 ||
                _useragent.IndexOf(engineXiino) != -1)
            {
                //Make sure it's not WebOS first
                if (DetectPalmWebOS() == true)
                    return false;
                return true;
            }
            return false;
        }

        //PalmOS delegate
        public delegate void DetectPalmOSHandler(object page, MDetectArgs args);
        public event DetectPalmOSHandler OnDetectPalmOS;


        //**************************
        // Detects if the current browser is on a Palm device
        //    running the new WebOS.
        public bool DetectPalmWebOS()
        {
            if (_useragent.IndexOf(deviceWebOS) != -1)
                return true;
            return false;
        }

        //PalmWebOS delegate
        public delegate void DetectPalmWebOSHandler(object page, MDetectArgs args);
        public event DetectPalmWebOSHandler OnDetectPalmWebOS;


        //**************************
        // Detects if the current browser is a
        //    Garmin Nuvifone.
        public bool DetectGarminNuvifone()
        {
            if (_useragent.IndexOf(_deviceNuvifone) != -1)
                return true;
            return false;
        }


        //**************************
        // Check to see whether the device is any device
        //   in the 'smartphone' category.
        public bool DetectSmartphone()
        {
            if (DetectIphoneOrIpod() ||
                DetectAndroidPhone() ||
                DetectS60OssBrowser() ||
                DetectSymbianOS() ||
                DetectWindowsMobile() ||
                DetectWindowsPhone7() ||
                DetectBlackBerry() ||
                DetectPalmWebOS() ||
                DetectPalmOS() ||
                DetectGarminNuvifone())
                return true;
            return false;
        }

        //DetectSmartphone delegate
        public delegate void DetectSmartphoneHandler(object page, MDetectArgs args);
        public event DetectSmartphoneHandler OnDetectSmartphone;


        //**************************
        // Detects whether the device is a Brew-powered device.
        public bool DetectBrewDevice()
        {
            if (_useragent.IndexOf(deviceBrew) != -1)
                return true;
            else
                return false;
        }

        //BrewDevice delegate
        public delegate void DetectBrewDeviceHandler(object page, MDetectArgs args);
        public event DetectBrewDeviceHandler OnDetectBrewDevice;

        //**************************
        // Detects the Danger Hiptop device.
        public bool DetectDangerHiptop()
        {
            if (_useragent.IndexOf(deviceDanger) != -1 ||
                _useragent.IndexOf(deviceHiptop) != -1)
                return true;
            else
                return false;
        }
        //DangerHiptop delegate
        public delegate void DetectDangerHiptopHandler(object page, MDetectArgs args);
        public event DetectDangerHiptopHandler OnDetectDangerHiptop;

        //**************************
        // Detects if the current browser is Opera Mobile or Mini.
        public bool DetectOperaMobile()
        {
            if (_useragent.IndexOf(engineOpera) != -1)
            {
                if ((_useragent.IndexOf(mini) != -1) ||
                    (_useragent.IndexOf(mobi) != -1))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        //DangerHiptop delegate
        public delegate void DetectOperaMobileHandler(object page, MDetectArgs args);
        public event DetectOperaMobileHandler OnDetectOperaMobile;


        //**************************
        // Detects whether the device supports WAP or WML.
        public bool DetectWapWml()
        {
            if (_httpaccept.IndexOf(vndwap) != -1 ||
                _httpaccept.IndexOf(wml) != -1)
                return true;
            else
                return false;
        }
        //WapWml delegate
        public delegate void DetectWapWmlHandler(object page, MDetectArgs args);
        public event DetectWapWmlHandler OnDetectWapWml;


        //**************************
        // Detects if the current device is an Amazon Kindle.
        public bool DetectKindle()
        {
            if (_useragent.IndexOf(deviceKindle) != -1)
                return true;
            else
                return false;
        }

        //Kindle delegate
        public delegate void DetectKindleHandler(object page, MDetectArgs args);
        public event DetectKindleHandler OnDetectKindle;


        //**************************
        //   Detects if the current device is a mobile device.
        //   This method catches most of the popular modern devices.
        //   Excludes Apple iPads and other modern tablets.
        public bool DetectMobileQuick()
        {
            //Let's exclude tablets
            if (DetectTierTablet())
                return false;

            //Most mobile browsing is done on smartphones
            if (DetectSmartphone())
                return true;

            if (DetectWapWml() ||
                DetectBrewDevice() ||
                DetectOperaMobile())
                return true;

            if ((_useragent.IndexOf(engineNetfront) != -1) ||
                (_useragent.IndexOf(engineUpBrowser) != -1) ||
                (_useragent.IndexOf(engineOpenWeb) != -1))
                return true;

            if (DetectDangerHiptop() ||
                DetectMidpCapable() ||
                DetectMaemoTablet() ||
                DetectArchos())
                return true;

            if ((_useragent.IndexOf(devicePda) != -1) &&
                (_useragent.IndexOf(disUpdate) < 0)) //no index found
                return true;
            if (_useragent.IndexOf(mobile) != -1)
                return true;
            else
                return false;
        }

        //DetectMobileQuick delegate
        public delegate void DetectMobileQuickHandler(object page, MDetectArgs args);
        public event DetectMobileQuickHandler OnDetectMobileQuick;


        //**************************
        // Detects if the current device is a Sony Playstation.
        public bool DetectSonyPlaystation()
        {
            if (_useragent.IndexOf(devicePlaystation) != -1)
                return true;
            else
                return false;
        }

        //**************************
        // Detects if the current device is a Nintendo game device.
        public bool DetectNintendo()
        {
            if (_useragent.IndexOf(deviceNintendo) != -1 ||
                _useragent.IndexOf(deviceWii) != -1 ||
                _useragent.IndexOf(deviceNintendoDs) != -1)
                return true;
            else
                return false;
        }

        //**************************
        // Detects if the current device is a Microsoft Xbox.
        public bool DetectXbox()
        {
            if (_useragent.IndexOf(deviceXbox) != -1)
                return true;
            else
                return false;
        }

        //**************************
        // Detects if the current device is an Internet-capable game console.
        public bool DetectGameConsole()
        {
            if (DetectSonyPlaystation())
                return true;
            else if (DetectNintendo())
                return true;
            else if (DetectXbox())
                return true;
            else
                return false;
        }

        //**************************
        // Detects if the current device supports MIDP, a mobile Java technology.
        public bool DetectMidpCapable()
        {
            if (_useragent.IndexOf(deviceMidp) != -1 ||
                _httpaccept.IndexOf(deviceMidp) != -1)
                return true;
            else
                return false;
        }

        //**************************
        // Detects if the current device is on one of the Maemo-based Nokia Internet Tablets.
        public bool DetectMaemoTablet()
        {
            if (_useragent.IndexOf(maemo) != -1)
                return true;
            //Must be Linux + Tablet, or else it could be something else. 
            else if (_useragent.IndexOf(maemoTablet) != -1 &&
                     _useragent.IndexOf(linux) != -1)
                return true;
            else
                return false;
        }

        //**************************
        // Detects if the current device is an Archos media player/Internet tablet.
        public bool DetectArchos()
        {
            if (_useragent.IndexOf(deviceArchos) != -1)
                return true;
            else
                return false;
        }

        //**************************
        // Detects if the current browser is a Sony Mylo device.
        public bool DetectSonyMylo()
        {
            if (_useragent.IndexOf(manuSony) != -1)
            {
                if ((_useragent.IndexOf(qtembedded) != -1) ||
                    (_useragent.IndexOf(mylocom2) != -1))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        //**************************
        // The longer and more thorough way to detect for a mobile device.
        //   Will probably detect most feature phones,
        //   smartphone-class devices, Internet Tablets, 
        //   Internet-enabled game consoles, etc.
        //   This ought to catch a lot of the more obscure and older devices, also --
        //   but no promises on thoroughness!
        public bool DetectMobileLong()
        {
            if (DetectMobileQuick())
                return true;
            if (DetectGameConsole() ||
                DetectSonyMylo())
                return true;

            //Detect older phones from certain manufacturers and operators. 
            if (_useragent.IndexOf(uplink) != -1)
                return true;
            if (_useragent.IndexOf(manuSonyEricsson) != -1)
                return true;
            if (_useragent.IndexOf(manuericsson) != -1)
                return true;
            if (_useragent.IndexOf(manuSamsung1) != -1)
                return true;

            if (_useragent.IndexOf(svcDocomo) != -1)
                return true;
            if (_useragent.IndexOf(svcKddi) != -1)
                return true;
            if (_useragent.IndexOf(svcVodafone) != -1)
                return true;

            else
                return false;
        }



        //*****************************
        // For Mobile Web Site Design
        //*****************************

        //**************************
        // The quick way to detect for a tier of devices.
        //   This method detects for the new generation of
        //   HTML 5 capable, larger screen tablets.
        //   Includes iPad, Android (e.g., Xoom), BB Playbook, etc.
        public bool DetectTierTablet()
        {
            if (DetectIpad() ||
                DetectAndroidTablet() ||
                DetectBlackBerryTablet())
                return true;
            else
                return false;
        }

        //DetectTierTablet delegate
        public delegate void DetectTierTabletHandler(object page, MDetectArgs args);
        public event DetectTierTabletHandler OnDetectTierTablet;


        //**************************
        // The quick way to detect for a tier of devices.
        //   This method detects for devices which can 
        //   display iPhone-optimized web content.
        //   Includes iPhone, iPod Touch, Android, etc.
        public bool DetectTierIphone()
        {
            if (DetectIphoneOrIpod() ||
                DetectAndroidPhone() ||
                (DetectBlackBerryWebKit() &&
                 DetectBlackBerryTouch()) ||
                DetectPalmWebOS() ||
                DetectGarminNuvifone() ||
                DetectMaemoTablet())
                return true;
            else
                return false;
        }

        //DetectTierIphone delegate
        public delegate void DetectTierIphoneHandler(object page, MDetectArgs args);
        public event DetectTierIphoneHandler OnDetectTierIphone;


        //**************************
        // The quick way to detect for a tier of devices.
        //   This method detects for devices which are likely to be capable 
        //   of viewing CSS content optimized for the iPhone, 
        //   but may not necessarily support JavaScript.
        //   Excludes all iPhone Tier devices.
        public bool DetectTierRichCss()
        {
            if (DetectMobileQuick())
            {
                if (DetectTierIphone())
                    return false;

                if (DetectWebkit() ||
                    DetectS60OssBrowser())
                    return true;

                //Note: 'High' BlackBerry devices ONLY
                if (DetectBlackBerryHigh() == true)
                    return true;

                //WP7's IE-7-based browser isn't good enough for iPhone Tier.
                if (DetectWindowsPhone7() == true)
                    return true;
                if (DetectWindowsMobile() == true)
                    return true;
                if (_useragent.IndexOf(engineTelecaQ) != -1)
                    return true;

                else
                    return false;
            }
            else
                return false;
        }

        //DetectTierRichCss delegate
        public delegate void DetectTierRichCssHandler(object page, MDetectArgs args);
        public event DetectTierRichCssHandler OnDetectTierRichCss;


        //**************************
        // The quick way to detect for a tier of devices.
        //   This method detects for all other types of phones,
        //   but excludes the iPhone and Smartphone Tier devices.
        public bool DetectTierOtherPhones()
        {
            if (DetectMobileLong() == true)
            {
                //Exclude devices in the other 2 categories
                if (DetectTierIphone() ||
                    DetectTierRichCss())
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        //DetectTierOtherPhones delegate
        public delegate void DetectTierOtherPhonesHandler(object page, MDetectArgs args);
        public event DetectTierOtherPhonesHandler OnDetectTierOtherPhones;

        //***************************************************************
        #endregion

    }
}
