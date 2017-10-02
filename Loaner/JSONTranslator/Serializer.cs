﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

namespace JSONTranslator
{
    class Serializer
    {
        //public static string SerializeObjectToXml(object t)
        //{
        //    XmlSerializer xml = new XmlSerializer(typeof(TranslatedRequest));

        //    using (StringWriter textwriter = new StringWriter())
        //    {
        //        xml.Serialize(textwriter, t);
        //        return textwriter.ToString();
        //    }
        //}

        public static LoanRequest DeserializeObjectFromXml(string xml)
        {
            XmlSerializer dexml = new XmlSerializer(typeof(LoanRequest));
            using (TextReader reader = new StringReader(xml))
            {
                return (LoanRequest)dexml.Deserialize(reader);
            }

        }


        public static string JSONSerializeObject(object t)
        {
            return new JavaScriptSerializer().Serialize(t);

        }
    }
}
