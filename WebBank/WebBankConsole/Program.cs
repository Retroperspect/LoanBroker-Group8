using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBankConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            WebBankService.Service1Client client = new WebBankService.Service1Client();
            /// This is the translator, no?. This Console Application should listen to a RabbitMQ channel somewhere more specificly "LoanRequestB2".
            /// As from the GetBanks Service: format = "XML", Input = "LoanRequestB2", Output = "Group8-LoanBroker-Request" }) you can change it if you want.
            /// It should translate the message to the banks prefered format and then send to the bank by one of the 4 ways of system integration.
            /// in this case just contact a SOAP service to either get the UniversalResponse within the code or have the webservice send to a rabbitMQ channel.
            while (true)
            {
                client.HandleRequest();
            }
        }
    }
}
