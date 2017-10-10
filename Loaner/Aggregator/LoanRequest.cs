using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator
{
    [Serializable]
    public class LoanRequest
    {
        public string ssn { get; set; }
        public string interestRate { get; set; }
        public string orderNumber { get; set; }
    }
}
