﻿using System;
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
            while (true)
            {
                RabbitManager rm = new RabbitManager();
                rm.receiveMessage();
            }


        }
    }
}
