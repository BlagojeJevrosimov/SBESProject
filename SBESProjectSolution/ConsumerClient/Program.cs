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

            Console.WriteLine("Korisnik koji je pokrenuo klijenta je : " + WindowsIdentity.GetCurrent().Name);

            //EndpointAddress endpointAddress = new EndpointAddress(new Uri(address),
            //    EndpointIdentity.CreateUpnIdentity("wcfServer"));

            using (ConsumerClient proxy = new ConsumerClient(binding, new EndpointAddress(new Uri(address))))
            {
                proxy.Subscribe();

                while (true)
                {
                    PrintMenu();
                    string x = Console.ReadLine();

                    switch (x)
                    {
                        case "1":
                            proxy.Read(1);
                            break;
                        case "2":
                            //proxy.Update();
                            break;
                        case "3":
                            //proxy.Delete();
                            break;
                    }
                }
            }

        }

        static void PrintMenu()
        {
            Console.WriteLine("Odaberite opciju:");
            Console.WriteLine("1. Read");
            Console.WriteLine("2. Update");
            Console.WriteLine("3. Delete");
        }
    }
}
