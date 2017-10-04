using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Normalizer
{
    [XmlRoot("LoanResponse")]
    public class XMLCPHBankClass
    {
        [XmlElement("interestRate")]
        public decimal interestRate { get; set; }
        [XmlElement("ssn")]
        public string ssn { get; set; }

    }
}
