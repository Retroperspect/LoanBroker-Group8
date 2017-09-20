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
            RabbitMQManager MQ = new RabbitMQManager("207.154.250.196");
            while (true)
            {
                MQ.WorkerSendMessage("RequestLoan", Encoding.UTF8.GetBytes("170294-1837"));
                Console.ReadLine();
            }
           

        }



    }
}
