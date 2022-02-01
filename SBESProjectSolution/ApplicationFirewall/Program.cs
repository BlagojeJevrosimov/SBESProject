using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace ApplicationFirewall
{
    class Program
    {
        static void Main(string[] args)
        {
            WhitelistConfig wc = new WhitelistConfig();
            Event ev;

            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("\t1. Add port");
                Console.WriteLine("\t2. Remove port");
                Console.WriteLine("\t3. Add protocol");
                Console.WriteLine("\t4. Remove protocol");

                string x = Console.ReadLine();
                string input;
                bool val;

                switch (x)
                {
                    case "1":
                        Console.Write("Port: ");
                        input = Console.ReadLine();
                        val = wc.AddPort(Int32.Parse(input), out ev);
                        if (!val)
                            Console.WriteLine("Port already exists.");
                        break;
                    case "2":
                        Console.Write("Port: ");
                        input = Console.ReadLine();
                        val = wc.RemovePort(Int32.Parse(input), out ev);
                        if (!val)
                            Console.WriteLine("Port doesn't exist.");
                        break;
                    case "3":
                        Console.Write("Protocol: ");
                        input = Console.ReadLine();
                        val = wc.AddProtocol(input, out ev);
                        if (!val)
                            Console.WriteLine("Protocol already exists.");
                        break;
                    case "4":
                        Console.Write("Protocol: ");
                        input = Console.ReadLine();
                        val = wc.RemoveProtocol(input, out ev);
                        if (!val)
                            Console.WriteLine("Protocol doesn't exist.");
                        break;
                    default:
                        Console.WriteLine("Please enter 1, 2, 3 or 4.");
                        break;
                }
            }
        }
    }
}
