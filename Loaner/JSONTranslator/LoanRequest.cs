using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONTranslator
{
    [Serializable]
    public class LoanRequest
    {
        public string ssn { get; set; }
        public int creditScore { get; set; }
        public double LoanAmmount { get; set; }
        public string LoanDuration { get; set; }



    }
}
