using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBank
{
    [Serializable]
    public class LoanRequest
    {
        public string ssn { get; set; }
        public string LoanDuration { get; set; }
        public double LoanAmmount { get; set; }
        public int CreditScore { get; set; }
    }
}