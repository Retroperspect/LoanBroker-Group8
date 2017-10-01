using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading;

namespace GetBanks
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                RabbitManager rm = new RabbitManager();
                rm.receiveMessage();
            }


        }



        
    }
}
