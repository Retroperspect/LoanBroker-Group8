using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Loaner_Library
{
    [Serializable]
    [XmlRoot("LoanRequestWithBanks")]
    public class LoanRequestWithBanks : LoanRequest
    {
        [XmlElement("Bank")]
        public List<Bank> ViableBanks { get; set; }

    }
}
