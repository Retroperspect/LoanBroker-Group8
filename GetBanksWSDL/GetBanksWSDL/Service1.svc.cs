using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace GetBanksWSDL
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public List<Banks> GetBanks(int CreditScore)
        {
            List<Banks> results = new List<Banks>();

            if (CreditScore > 200)
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
    }
}
