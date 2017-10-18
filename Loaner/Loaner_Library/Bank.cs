using System.Xml.Serialization;

namespace Loaner_Library
{
    public class Bank
    {
        [XmlElement("format")]
        public string format { get; set; }
        [XmlElement("Input")]
        public string Input { get; set; }
        [XmlElement("Output")]
        public string Output { get; set; }
        [XmlElement("BankName")]
        public string Bname { get; set; }
    }
}
