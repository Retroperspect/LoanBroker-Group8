using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loaner_Library;

namespace Loaner
{
    class Program
    {
        static void Main(string[] args)
        {
            RabbitMQManager MQ = new RabbitMQManager("138.197.186.82");
            while (true)
            {
                
                LoanRequest lr = new LoanRequest() {ssn= "170494-1837", LoanDuration = TimeSpan.FromDays(650).ToString(), LoanAmmount = 20000 };
                MQ.WorkerSendMessage("RequestLoan", Encoding.UTF8.GetBytes(Serializer.SerializeObjectToXmlType(lr,lr.GetType())));
                Console.ReadLine();
            }
           

        }



    }
}
