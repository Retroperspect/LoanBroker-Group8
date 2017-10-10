using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Normalizer
{
    [XmlRoot("loanResponse")]
    public class GoBankResponse
    {
        [XmlElement("ssn")]
        public string ssn { get; set; }
        [XmlElement("interestRate")]
        public decimal interestRate { get; set; }
    }
}
