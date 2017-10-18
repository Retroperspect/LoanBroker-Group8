using System;
using System.Xml.Serialization;

namespace Loaner_Library
{
    [Serializable]
    public class UniversalResponse
    {
        [XmlElement("ssn")]
        public string ssn { get; set; }
        [XmlElement("interestrate")]
        public decimal interestrate { get; set; }
    }
}
