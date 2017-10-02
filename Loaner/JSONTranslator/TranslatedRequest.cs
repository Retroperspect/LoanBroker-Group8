using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace JSONTranslator
{
    
    public class TranslatedRequest
    {
        public string ssn { get; set; }

        public int creditScore { get; set; }

        public decimal loanAmount { get; set; }

        public string loanDuration { get; set; }
    }
}
