using System;
using System.Web.Script.Serialization;
using System.Xml;
using System.Configuration;

namespace Libs.TopupPartner.VNPTEPAY
{
	public static class ConvertUtility
	{
		public static string FormatTimeVn(DateTime dt, string defaultText)
		{
			if (ToDateTime(dt) != new DateTime(1900, 1, 1))
				return dt.ToString("dd-mm-yy");
			else
				return defaultText;
		}
        public static double ToDouble1(string obj)
		{
			double retVal;
            try
			{
            obj = obj.Replace(",","").Replace(".", ",").Replace(" ","");
			
				retVal = Convert.ToDouble(obj);
			}
			catch
			{
				retVal = 0;
			}

			return retVal;
		}
		public static int ToInt32(object obj)
		{
			int retVal = 0;

			try
			{
				retVal = Convert.ToInt32(obj);
			}
			catch
			{
				retVal = 0;
			}

			return retVal;
		}

		public static int ToInt32(object obj, int defaultValue)
		{
			int retVal = defaultValue;

			try
			{
				retVal = Convert.ToInt32(obj);
			}
			catch
			{
				retVal = defaultValue;
			}

			return retVal;
		}

		public static string ToString(object obj)
		{
			string retVal;

			try
			{
				retVal = Convert.ToString(obj);
			}
			catch
			{
				retVal = String.Empty;
			}

			return retVal;
		}

		public static DateTime ToDateTime(object obj)
		{
			DateTime retVal;
			try
			{
				retVal = Convert.ToDateTime(obj);
			}
			catch
			{
				retVal = DateTime.Now;
			}
			if (retVal == new DateTime(1, 1, 1)) return DateTime.Now;

			return retVal;
		}

		public static DateTime ToDateTime(object obj, DateTime defaultValue)
		{
			DateTime retVal;
			try
			{
				retVal = Convert.ToDateTime(obj);
			}
			catch
			{
				retVal = DateTime.Now;
			}
			if (retVal == new DateTime(1, 1, 1)) return defaultValue;

			return retVal;
		}

		public static bool ToBoolean(object obj)
		{
			bool retVal;

			try
			{
				retVal = Convert.ToBoolean(obj);
			}
			catch
			{
				retVal = false;
			}

			return retVal;
		}

		public static double ToDouble(object obj)
		{
			double retVal;

			try
			{
				retVal = Convert.ToDouble(obj);
			}
			catch
			{
				retVal = 0;
			}

			return retVal;
		}

		public static double ToDouble(object obj, double defaultValue)
		{
			double retVal;

			try
			{
				retVal = Convert.ToDouble(obj);
			}
			catch
			{
				retVal = defaultValue;
			}

			return retVal;
		}
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RecursionLimit = recursionDepth;
            return serializer.Serialize(obj);
        }
        public static string get_setting(string setting_name)
        {
            return ConfigurationSettings.AppSettings[setting_name];
        }
        public static XmlDocument JsonToXml(string json)
        {
            XmlNode newNode = null;
            XmlNode appendToNode = null;
            XmlDocument returnXmlDoc = new XmlDocument();
            returnXmlDoc.LoadXml("<Document />");
            XmlNode rootNode = returnXmlDoc.SelectSingleNode("Document");
            appendToNode = rootNode;

            string[] arrElementData;
            string[] arrElements = json.Split('\r');
            foreach (string element in arrElements)
            {
                string processElement = element.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
                if ((processElement.IndexOf("}") > -1 || processElement.IndexOf("]") > -1) &&
                    appendToNode != rootNode)
                {
                    appendToNode = appendToNode.ParentNode;
                }
                else if (processElement.IndexOf("[") > -1)
                {
                    processElement = processElement.Replace(":", "").Replace("[", "").Replace("\"", "").Trim();
                    newNode = returnXmlDoc.CreateElement(processElement);
                    appendToNode.AppendChild(newNode);
                    appendToNode = newNode;
                }
                else if (processElement.IndexOf("{") > -1 && processElement.IndexOf(":") > -1)
                {
                    processElement = processElement.Replace(":", "").Replace("{", "").Replace("\"", "").Trim();
                    newNode = returnXmlDoc.CreateElement(processElement);
                    appendToNode.AppendChild(newNode);
                    appendToNode = newNode;
                }
                else
                {
                    if (processElement.IndexOf(":") > -1)
                    {
                        arrElementData = processElement.Replace(": \"", ":").Replace("\",", "").Replace("\"", "").Split(':');
                        newNode = returnXmlDoc.CreateElement(arrElementData[0]);
                        for (int i = 1; i < arrElementData.Length; i++)
                        { newNode.InnerText += arrElementData[i]; }
                        appendToNode.AppendChild(newNode);
                    }
                }
            }

            return returnXmlDoc;
        }

        
	}
}