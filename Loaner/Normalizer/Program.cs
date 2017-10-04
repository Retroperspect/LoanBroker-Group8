using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normalizer
{
    class Program
    {
        static void Main(string[] args)
        {
            RabbitManager rm = new RabbitManager();
            while (true)
            {
                rm.receiveMessage();
            }
        }
    }
}
