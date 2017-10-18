using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WebBank
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
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

        public UniversalResponse HandleRequest(LoanRequest request) //// This one should take Argument. public void HandleRequest(LoanRequest)
        {
            decimal interestrate = 0;
            //figure out interest rate
            if (request.CreditScore < 400)
            {
                interestrate = 6.5m;
            }
            else if (request.CreditScore > 400 && request.CreditScore <= 750)
            {
                interestrate = 3.5m;
            }
            else if (request.CreditScore > 750)
            {
                interestrate = 0.5m;
            }

            //convert
            return URConversion(request, interestrate);
        }

        
        public UniversalResponse URConversion(LoanRequest request, decimal interestRate)
        {
            UniversalResponse response = new UniversalResponse();
            response.ssn = request.ssn;
            response.interestrate = interestRate;
            return response;
        }
    }
}
