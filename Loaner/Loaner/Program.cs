﻿using System;
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
            RabbitManager MQ = new RabbitManager();
            while (true)
            {
                MQ.receiveMessage();
            }
           

        }



    }
}
