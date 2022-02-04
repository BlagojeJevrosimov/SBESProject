using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AFManager;
using Contracts;

namespace ApplicationFirewall
{
    class Program
    {
        public static AFCCProxy afccProxy;

        static void Main(string[] args)
        {

            /// Define the expected service certificate. It is required to establish cmmunication using certificates.
            string srvCertCN = "wcfservice";


            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            // Moramo da znamo gde se koji sertifikat instalira !

            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            // iscitava se server iz trusted people
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:8888/SyslogServerSecurityEvent"),
                                      new X509CertificateEndpointIdentity(srvCert));

            Console.WriteLine("Korisnik koji je pokrenuo ApplicationFirewall: " + WindowsIdentity.GetCurrent().Name);

            Thread thr = new Thread(ConsumerProcess);
            thr.Start();

            WhitelistConfig wc = new WhitelistConfig();
            Event ev;

            while (true)
            {
                
                using (AFProxy proxy = new AFProxy(binding, address))
                {
                    Thread.Sleep(100);
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
                            else
                                proxy.sendEvent(ev);
                            break;
                        case "2":
                            Console.Write("Port: ");
                            input = Console.ReadLine();
                            val = wc.RemovePort(Int32.Parse(input), out ev);
                            if (!val)
                                Console.WriteLine("Port doesn't exist.");
                            else
                                proxy.sendEvent(ev);
                            break;
                        case "3":
                            Console.Write("Protocol: ");
                            input = Console.ReadLine();
                            val = wc.AddProtocol(input, out ev);
                            if (!val)
                                Console.WriteLine("Protocol already exists.");
                            else
                                proxy.sendEvent(ev);
                            break;
                        case "4":
                            Console.Write("Protocol: ");
                            input = Console.ReadLine();
                            val = wc.RemoveProtocol(input, out ev);
                            if (!val)
                                Console.WriteLine("Protocol doesn't exist.");
                            else
                                proxy.sendEvent(ev);
                            break;
                        default:
                            Console.WriteLine("Please enter 1, 2, 3 or 4.");
                            break;
                    }
                }
            }
        }

        static void ConsumerProcess()
        {
            Thread.Sleep(100);

            /// Define the expected service certificate. It is required to establish cmmunication using certificates.
			string srvCertCN = "wcfservice";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            // Moramo da znamo gde se koji sertifikat instalira !

            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            // iscitava se server iz trusted people
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:7777/SyslogServerSecurityEvent"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (afccProxy = new AFCCProxy(binding, address))
            {

                NetTcpBinding binding2 = new NetTcpBinding();
                string address2 = "net.tcp://localhost:5555/ClientService";

                binding2.Security.Mode = SecurityMode.Transport;
                binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                binding2.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

                ServiceHost serviceHost = new ServiceHost(typeof(ClientService));
                serviceHost.AddServiceEndpoint(typeof(IClientService), binding2, address2);
                serviceHost.Open();

                Console.WriteLine("ClientProcess is working.");
            }

            Console.ReadLine();
        }
    }
}
