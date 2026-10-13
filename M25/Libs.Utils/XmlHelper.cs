using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Libs.Utils
{
    public class XmlHelper
    {
        public XmlHelper()
        {

        }

        public static string XmlSerialize(object serializingObject)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineHandling = NewLineHandling.Replace;
                settings.NewLineChars = "\n";

                StringWriter strWriter = new StringWriter();
                XmlWriter writer = XmlWriter.Create(strWriter, settings);
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                XmlSerializer serializer = new XmlSerializer(serializingObject.GetType());

                serializer.Serialize(writer, serializingObject, namespaces);
                return strWriter.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object Deserialize(string xml, Type type)
        {
            try
            {
                StringReader rd = new StringReader(xml);
                XmlSerializer srz = new XmlSerializer(type);
                return srz.Deserialize(rd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
