using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loaner
{
    class Program
    {
        static void Main(string[] args)
        {
            RabbitMQManager MQ = new RabbitMQManager("138.197.186.82");
            while (true)
            {
                LoanRequest lr = new LoanRequest() {ssn= "170294-1837", LoanDuration = TimeSpan.FromDays(350).ToString(), LoanAmmount = 10000 };
                MQ.WorkerSendMessage("RequestLoan", Encoding.UTF8.GetBytes(Serializer.SerializeObjectToXml(lr)));
                Console.ReadLine();
            }
           

        }



    }
}
