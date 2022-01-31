using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/SyslogServer";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            using (ConsumerClient proxy = new ConsumerClient(binding, new EndpointAddress(new Uri(address))))
            {
                 int input = 0;
                 while (input != 4)
                {
                     Console.WriteLine("Choose and action: \n\t1.Subscribe \n\t2.Update \n\t3.Delete \n\t4.Exit");
                     input = Int32.Parse(Console.ReadLine());
                     switch (input)
                     {
                         case 1: { proxy.Subscribe(); break; }
                         case 2: { proxy.Update(); break; }
                         case 3: { proxy.Delete(); break; }
                         default: break;

                     }
                 }

            }        
        }
        
    }
}
