using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Security.Policy;
using System.Security.Cryptography.X509Certificates;
using System.Net;

/// <summary>
/// Summary description for MyPolicy
/// </summary>
namespace Libs.CardTelco
{
    public class MyPolicy : ICertificatePolicy
    {
        public MyPolicy()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #region ICertificatePolicy Members
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }
        #endregion
    }

}