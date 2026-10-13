using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Libs.API;
using Libs.Utils;

public partial class VPGHttp : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string partnerCode, serviceCode, commandCode, requestContent, urlReturn, signature;

        PostGetHelper helper = new PostGetHelper(Request, Response);
        helper.ReadPostedData();

        partnerCode = helper.Get("PartnerCode");
        serviceCode = helper.Get("ServiceCode");
        commandCode = helper.Get("CommandCode");
        requestContent = helper.Get("RequestContent");
        urlReturn = helper.Get("UrlReturn");
        signature = helper.Get("Signature");

        if (partnerCode == null || serviceCode == null || commandCode == null || signature == null || urlReturn == null)
        {
            return;
        }

        string result = VPGUtils.Request(partnerCode, serviceCode, commandCode, requestContent, signature);

        helper.Clear();
        helper.FormName = "VPGResponse";
        helper.Add("Content", result);
        helper.RedirectWithData(urlReturn);
    }
}