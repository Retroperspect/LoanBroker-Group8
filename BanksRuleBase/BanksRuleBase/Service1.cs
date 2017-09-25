using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BanksRuleBase
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public List<Banks> GetBanks(int CreditScore)
        {
            List<Banks> results = new List<Banks>();

            if (CreditScore> 200)
            {
                results.Add(new Banks() { format = "XML", Input = "cphbusiness.bankXML", Output = "cphbusiness.LoanBroker.Group8" });
            }
            if (CreditScore > 300)
            {
                results.Add(new Banks() { format = "JSON", Input = "cphbusiness.bankJSON", Output = "cphbusiness.LoanBroker.Group8" });
            }
            if (CreditScore > 400)
            {
                results.Add(new Banks() { format = "XML", Input = "LoanRequestB1", Output = "cphbusiness.LoanBroker.Group8" });
            }
            if (CreditScore > 600)
            {
                results.Add(new Banks() { format = "XML", Input = "LoanRequestB2", Output = "cphbusiness.LoanBroker.Group8" });
            }
            return results;
        }

    }
}
