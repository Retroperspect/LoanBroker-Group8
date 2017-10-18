using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Loaner_Library
{
    class Serializer
    {

        /// <summary>
        /// Serializes any object into the XML string format of the specified type
        /// </summary>
        /// <param name="o">object to serialize</param>
        /// <param name="t">the objects data structure type</param>
        /// <returns>a XML serialized string of the object</returns>
        public string SerializeObjectToXmlType(object o,Type t)
        {
            using (StringWriter textwriter = new StringWriter())
            {
                new XmlSerializer(t).Serialize(textwriter, t);
                return textwriter.ToString();
            }
        }

        /// <summary>
        /// Deserializes any XML string to the specified object data format
        /// </summary>
        /// <param name="xml">content to deserialize</param>
        /// <param name="t">the contents data structure</param>
        /// <returns>object in the format of the defined data structure type</returns>
        public object DeserializeObjectFromXmlType(string xml,Type t)
        {
            XmlSerializer dexml = new XmlSerializer(t);
            using (TextReader reader = new StringReader(xml))
            {
                return dexml.Deserialize(reader);
            }
        }

        /// <summary>
        /// Serializes any object in JSON format to a string
        /// </summary>
        /// <param name="o">object to serialize</param>
        /// <returns>a JSON serialized string of the object</returns>
        public string SerializeObjectToJsonType(object o)
        {
            return new JavaScriptSerializer().Serialize(o);
        }

        /// <summary>
        /// Deserializes any JSON string to the specified object data format
        /// </summary>
        /// <param name="json">JSON string to Deserialize</param>
        /// <param name="t">object type that it converts to</param>
        /// <returns>object in the format of the defined data structure type</returns>
        public object DeserializeObjectFromJsonType(string json, Type t)
        {
            return new JavaScriptSerializer().Deserialize(json, t);
        }

    }
}
