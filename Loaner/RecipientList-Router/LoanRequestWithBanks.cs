using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RecipientList_Router
{
    [Serializable]
    [XmlRoot("LoanRequestWithBanks")]
    public class LoanRequestWithBanks : LoanRequest
    {
        [XmlElement("Bank")]
        public List<Bank> ViableBanks { get; set; }

    }
}
