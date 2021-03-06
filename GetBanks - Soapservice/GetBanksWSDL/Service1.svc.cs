﻿using System;
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
                results.Add(new Banks() { format = "XML", Input = "cphbusiness.bankXML", Output = "Group8-LoanBroker-Request", Bname = "CPHXML" });
            }
            if (CreditScore > 300)
            {
                results.Add(new Banks() { format = "JSON", Input = "cphbusiness.bankJSON", Output = "Group8-LoanBroker-Request", Bname = "CPHJSON" });
            }
            if (CreditScore > 375)
            {
                results.Add(new Banks() { format = "XML", Input = "GoBankRequest", Output = "GoBankResponse", Bname = "GoBank" });
            }
            if (CreditScore > 500)
            {
                results.Add(new Banks() { format = "LoanRequestB2", Input = "LoanRequestB2", Output = "Group8-LoanBroker-Request", Bname = "SoapC#Bank" });
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
