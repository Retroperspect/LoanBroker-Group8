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

namespace Normalizer
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

        public static XMLCPHBankClass DeserializeObjectFromXmlCPH(string xml)
        {
            XmlSerializer dexml = new XmlSerializer(typeof(XMLCPHBankClass));
            using (TextReader reader = new StringReader(xml)) { return (XMLCPHBankClass)dexml.Deserialize(reader); }
        }
        public static GoBankResponse DeserializeObjectFromXmlGo(string xml)
        {
            XmlSerializer dexml = new XmlSerializer(typeof(GoBankResponse));
            using (TextReader reader = new StringReader(xml)) { return (GoBankResponse)dexml.Deserialize(reader); }
        }

        public static JSONResponse DeserializeObjectFromJSONCPH(string xml)
        {
            return new JavaScriptSerializer().Deserialize<JSONResponse>(xml);
        }

    }




}
