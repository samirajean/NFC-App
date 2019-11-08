using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace test1
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialReader x = new SerialReader();
            if (!x.initialize(true))
            {
                Console.WriteLine("Error");
                Console.Read();
            }

            while (x.myport.IsOpen)
            {
                x.Read();
                //x.Write();

            }
        }

       

        
    }
}
