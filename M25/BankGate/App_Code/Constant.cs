using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

/// <summary>
/// Summary description for Constant
/// </summary>
public class Constant
{
    public static string HOME_ROOT = ConfigurationSettings.AppSettings["HOME_ROOT"];
    
    public Constant()
	{
		//
		// TODO: Add constructor logic here
		//
	}
}