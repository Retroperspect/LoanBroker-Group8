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
            while (true)
            {
                client.HandleRequest();
            }
        }
    }
}
