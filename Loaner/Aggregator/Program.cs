using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            RabbitManager rm = new RabbitManager();
            
            rm.receiveMessage();
            
        }
    }
}
