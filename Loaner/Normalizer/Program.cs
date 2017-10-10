using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Normalizer
{
    class Program
    {
        static void Main(string[] args)
        {

            Thread t = new Thread(new ThreadStart(CPHServer));
            Thread t2 = new Thread(new ThreadStart(OwnServer));
            t.Start();
            t2.Start();


        }

        private static void OwnServer()
        {
            RabbitManager rm = new RabbitManager(new string[] { "138.197.186.82", "admin", "password" }, "GoBankResponse");
            rm.receiveMessage();
        }

        private static void CPHServer()
        {
            RabbitManager rm = new RabbitManager(new string[] { "datdb.cphbusiness.dk", "guest", "guest" }, "Group8-LoanBroker-Request");
            rm.receiveMessage();
        }
    }
}
