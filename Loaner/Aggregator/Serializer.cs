using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

namespace Aggregator
{
    class Serializer
    {
        public static string SerializeObjectToXml(object t)
        {
            XmlSerializer xml = new XmlSerializer(typeof(UniversalResponse));

            using (StringWriter textwriter = new StringWriter())
            {
                xml.Serialize(textwriter, t);
                return textwriter.ToString();
            }
        }

        public static UniversalResponse DeserializeObjectFromXml(string xml)
        {
            XmlSerializer dexml = new XmlSerializer(typeof(UniversalResponse));
            using (TextReader reader = new StringReader(xml))
            {
                return (UniversalResponse)dexml.Deserialize(reader);
            }

        }
    }
}
