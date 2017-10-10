using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XMLTranslator
{
    class Serializer
    {
        public static string SerializeObjectToXml(object t)
        {
            XmlSerializer xml = new XmlSerializer(typeof(TranslatedRequest));
            
            using (Utf8StringWriter textwriter = new Utf8StringWriter())
            {
                
                xml.Serialize(textwriter, t);
                return textwriter.ToString();
            }
        }

        public static LoanRequest DeserializeObjectFromXml(string xml)
        {
            XmlSerializer dexml = new XmlSerializer(typeof(LoanRequest));
            using (TextReader reader = new StringReader(xml))
            {
                return (LoanRequest)dexml.Deserialize(reader);
            }

        }
    }

    public class Utf8StringWriter : StringWriter
    {
        // Use UTF8 encoding but write no BOM to the wire
        public override Encoding Encoding
        {
            get { return new UTF8Encoding(false); } // in real code I'll cache this encoding.
        }
    }


}
