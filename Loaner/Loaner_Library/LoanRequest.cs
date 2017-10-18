using System;

namespace Loaner_Library
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
