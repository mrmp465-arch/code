using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Libs.TopupPartner.VNPTEPAY
{
    public class TopupConstant
    {
        //Key Test
        //public const string webserviceUrl = "http://itopup-test.megapay.net.vn:8086/CDV_Partner_Services_V1.0/services/Interfaces";
        //public const string TopupPartner = "partnerTest_NET";
        //public const string TopupPass = "123456abc";//
        //public const string Topupkey_3DES = "123456abc";
        //public const string topupvmgprivatekey = "<RSAKeyValue><Modulus>tFDCIgaEh+YKkHXQZEvX3EA9BgN3ZwxDdYQDBLM2xizy9JFqFaBFBwdmrIbnhpZc50OZN+rkx9r9LfpQNyR2Jwq+Tc5P0myOgacq0izByl+TEA/lFmY+d1oDFiITH6EN1ysUOcZYpNisZz1Ig2LVtWWzkeVAUGQfiubgffSlJAE=</Modulus><Exponent>AQAB</Exponent><P>9A5rEfbWapBuXZDkjgsb9NNRSJc6uQ68WBHCkwEpBQ9R/PfFnYYkVHXxfS3uTcjoVtzzAB7wmI+09pSbMu+Dqw==</P><Q>vSPId26jF6H0o49b+KTw0fL8ABUfc7GTKHZd82l9+hI/W33NYa6ffg+QWa8c1euLXpD6yGepu7m5k0BmHUrLAw==</Q><DP>5AdIsPsxcVXLWK7VbYYhs8lxi+KI4nlLLvpBEzslSW38C+CxCjJYZXXhkTVGD/VkaCx++yDtaY3z5eQJcjjpiw==</DP><DQ>Oxrzil8ImFoEGRizpP/mQbWtClmPepgDZKGYung85ejN48lsZRosvTgf4+bVHAR/iQ3FUNsUL5XaaCm3v2+gAQ==</DQ><InverseQ>YFm7hhqEWaXEiYaYKSaQk5GSdHvXsSrEjkw8hWITXaGy8Hu57nC6GxfTOZBWETP+XvyWLg4gSYEhqRnmwE0soQ==</InverseQ><D>sJ1piWpY5cOtFVJQi+y9wj4ph3pdudGJQdHMIw5m2DWuw16rSgNxmTDmoJaCsJEW7m6H5P1umVMmKaLEbz6Heo37bL+TywPCt0Jrl73SIqNH7IC2Uab+qomapneQzJfNlZZ3T2Q/AGnGL2viWCV9rZm6nMSGIo3game4mJ1zBwE=</D></RSAKeyValue>";
        //public const string topupvmgpublickey = "<RSAKeyValue><Modulus>tFDCIgaEh+YKkHXQZEvX3EA9BgN3ZwxDdYQDBLM2xizy9JFqFaBFBwdmrIbnhpZc50OZN+rkx9r9LfpQNyR2Jwq+Tc5P0myOgacq0izByl+TEA/lFmY+d1oDFiITH6EN1ysUOcZYpNisZz1Ig2LVtWWzkeVAUGQfiubgffSlJAE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        //End Key Test

        //Key Live
        public const string webserviceUrl = "http://naptien.thanhtoan247.vn:8082/CDV_Partner_Services_V1.0/services/Interfaces";
        public const string TopupPartner = "HNVGGJSC";
        public const string TopupPass = "iLangHa23@#";//
        public const string Topupkey_3DES = "0011F303153F7148DE661C15257204C2";
        public const string topupvmgprivatekey = "<RSAKeyValue><Modulus>oKOUAvGvLdra0XbR8cv3g/338FPKOfnX3ROd9DFI9Vm6dldpuHz11JAkJTHxNsPHnVJiyol+7MyTtfPJl5AsiOrgkEmCpP4QgU37xCSk6ezN3+h0GA8fli4fI4287UaJa0neoaVv3apQml+cOH/n0PlUQquQ8vM5fB3zrr9BmoE=</Modulus><Exponent>AQAB</Exponent><P>1rDl44zujs9kfnWktJ3rN0mZfnFxkXhNJ6zn9+drM8AuVxfMs1b6No0pWvL3bipQ/ufYsn55Z6o2fjLuNlv+jQ==</P><Q>v4w48JKftOXdufpQxvX3T0f5ql+Y6xDWDkrzAuixEJtyCtr/dTeJ/Yv37cyH2P55/t9KurOELjvE1hwBaV+YxQ==</Q><DP>Kr5dhshwVo6D+4mAmVU3l3JX1QzRB51j/xo1tO2mMk946m4amzod5u6D5U+qY4yZQ6b1RdRYZ4NEBiwtsTTvoQ==</DP><DQ>a63+TG+AzIb0cJphUpfvYWhK2BZJtsuOKhbEKfx5naZlI+kc9t4z+o723329WKUmG7uXHQHR6tO6wcqYmI4fKQ==</DQ><InverseQ>rC7OQnZAagkzMk7Z65vUUozFQi+ptUkmIHhAor8kkSsQ6mDBrSAf8nWE4F8IZ6hk+xJtQMrQvjdKOnQ6yBihPw==</InverseQ><D>n1yoYaNSnrMXkeXGsZIfoxo49nqpjIqH1BK6CJ88OesiSHS/tP6DArVcpmm9Ww8u4KvyxetRt6ncgA2yWGH3Sup0mwJRoyKNky/U2thuxGBwQdN79uMNzlDQ5/aaaV83/qvGEq0pAo98yOwdXMMm/tBnWC+QFOrZBFMG1XR4hoE=</D></RSAKeyValue>";
        public const string topupvmgpublickey = "<RSAKeyValue><Modulus>oKOUAvGvLdra0XbR8cv3g/338FPKOfnX3ROd9DFI9Vm6dldpuHz11JAkJTHxNsPHnVJiyol+7MyTtfPJl5AsiOrgkEmCpP4QgU37xCSk6ezN3+h0GA8fli4fI4287UaJa0neoaVv3apQml+cOH/n0PlUQquQ8vM5fB3zrr9BmoE=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        //End Key Live

        public enum TopupErrorCode { 

            //Success
            Success = 0,
            //Tai khoan dang bi khoa
            AccountBlock = 10,
            //Ten partner khong dung
            PartnerNameInvalid = 11,
            //Dia chi IP khong cho phep
            IPInvalid = 12,
            //Ma don hang bi loi
            OrderCodeError = 13,
            //Ma don hang da ton tai
            OrderExist = 14,
            //Ma don hang khong ton tai
            OrderNotExist = 15,
            //Sai so luong tai khoan
            AccountWrongCount = 16,
            //sai tong tien
            TotalAmountWrong = 17,
            //Sai thoi gian tao don hang
            TimeCreatedOrderWrong = 18,
            //Sai thoi gian ket thuc don hang
            TimeFinishedWrong = 19,
            //Lay danh sach tai khoan can nap bi loi
            ListAccountError = 20,
            //Sai chu ky
            SignError = 21,
            //Du lieu gui len rong hoac co ky tu dac biet
            DataSendError = 22,
            //Tai khoan dang duoc nap tien
            AccountIsCharging = 23,
            //So du kha dung khong du
            AmountNotEnough = 30,
            //Chiet khau chua duoc cap nhat cho partner
            DiscountNotUpdate = 31,
            //Partner chua duoc cap nhat public key
            PublicKeyNotUpdate = 32,
            // Partner chua duoc set IP
            IPNotSet = 33,
            //thoi gian ket thuc dat qua lau
            TimeWaitingLong = 34

        }

        public enum transType { 
            TopupAirtime = 1,
            DownloadSoftPin = 2
        } 


    }
}