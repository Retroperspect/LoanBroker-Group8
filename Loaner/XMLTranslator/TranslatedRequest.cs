using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XMLTranslator
{
    [Serializable]
    [XmlRoot("LoanRequest")]
    public class TranslatedRequest
    {
        [XmlElement("ssn")]
        public string ssn { get; set; }
        [XmlElement("creditScore")]
        public int creditScore { get; set; }
        [XmlElement("loanAmount")]
        public float LoanAmmount { get; set; }
        [XmlElement("loanDuration")]
        public string LoanDuration { get; set; }
    }
}
