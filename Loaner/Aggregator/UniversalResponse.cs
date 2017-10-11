using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Aggregator
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
