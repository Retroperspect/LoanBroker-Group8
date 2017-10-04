using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

//work on datatype
namespace Normalizer
{
    class Serializer
    {
        public static string SerializeObjectToXml(object t)
        {
            XmlSerializer xml = new XmlSerializer(typeof(LoanRequest));

            using (StringWriter textwriter = new StringWriter())
            {
                xml.Serialize(textwriter, t);
                return textwriter.ToString();
            }
        }

        /// <summary>
        /// Standard XML Deserializer
        /// </summary>
        /// <param name="Message">LoanRequestFromBank XML Message</param>
        /// <returns>LoanRequestFromBank converted to LoanRequest</returns>
        public static LoanRequest DeserializeObjectFromXml(string Message)
        {
            XmlSerializer dexml = new XmlSerializer(typeof(LoanRequest));
            using (TextReader reader = new StringReader(Message))
            {
                return (LoanRequest)dexml.Deserialize(reader);
            }

        }

        public static UniversalResponse DeserializeObjectFromXmlSchoolBank(string Message)
        {
            XmlSerializer dexml = new XmlSerializer(typeof(XMLCPHBankClass));
            using (TextReader reader = new StringReader(Message))
            {
                var medium = (XMLCPHBankClass)dexml.Deserialize(reader);
                return new UniversalResponse() { interestrate = medium.interestRate, ssn = medium.ssn };
            }

        }

        public static LoanRequest DeserializeObjectFromXmlStudentBank(string Message)
        {
            XmlSerializer dexml = new XmlSerializer(typeof(LoanRequest));
            using (TextReader reader = new StringReader(Message))
            {
                return (LoanRequest)dexml.Deserialize(reader);
            }

        }

        /// <summary>
        /// not implemented yet
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static UniversalResponse DeserializeObjectFromJsonSchoolBank(string Message)
        {
            var medium = new JavaScriptSerializer().Deserialize<JSONResponse>(Message);
            return new UniversalResponse() { interestrate = medium.interestRate, ssn = medium.ssn };

        }

        internal static object SerializeObjectToXml(string v, LoanRequest loanRequest)
        {
            throw new NotImplementedException();
        }
    }

        /// <summary>
        /// not implemented yet
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static LoanRequest DeserializeObjectFromJsonStudentBank(string Message)
        {
            return null;
        }
    }
}
