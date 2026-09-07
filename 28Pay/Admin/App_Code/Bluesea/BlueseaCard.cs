using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BlueseaCard
/// </summary>
public class BlueseaCard
{
    protected string merchant_code = "VGG20103013";
    protected string merchant_key = "VGG20!0#@13";
    protected string webserviceUrl = "http://sms.8x77.vn:8077/Card/Card";
    
    public BlueseaCard()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public string CheckCard(string cardCode, string cardSerial, string fromDate, string toDate)
    {
        Bluesea.Card _Card = new Bluesea.Card(webserviceUrl);
        return _Card.CardLog(merchant_code, merchant_key, cardCode, cardSerial, fromDate, toDate);
    }
}