using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Libs.CardTelco.Intecom
{
    [XmlRoot("CardResponse")]
    public class VcoinResponse
    {
        [XmlElement("ResponseStatus")]
        public int ResponseStatus { get; set; }

        [XmlElement("Descripton")]
        public string Descripton { get; set; }

        public VcoinResponse()
        {

        }

        public VcoinResponse(string xml)
        {
            XmlSerializer s = new XmlSerializer(typeof(VcoinResponse));
            TextReader t = new StringReader(xml);
            VcoinResponse r = (VcoinResponse)s.Deserialize(t);

            ResponseStatus = r.ResponseStatus;
            Descripton = r.Descripton;
        }

        public string ToXMLString()
        {
            XmlSerializer s = new XmlSerializer(typeof(VcoinResponse));
            TextWriter w = new StringWriter();
            s.Serialize(w, this);
            return w.ToString();
        }
    }
}
