using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyslogServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Consumer c = new Consumer("natasa", "1");
            Event ev = new Event(1, CriticallityLevel.GREEN_ALERT, DateTime.Now, c, "poruka", MessageState.OPEN);
            Database.events.Add(ev.Key, ev);

            Thread t1 = new Thread(new ThreadStart(Program.ConsumerCommunication));
            t1.Start();


        }

        static void ConsumerCommunication()
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:9999/SyslogServer";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Console.WriteLine("Korisnik koji je pokrenuo server je : " + WindowsIdentity.GetCurrent().Name);

            ServiceHost host = new ServiceHost(typeof(SyslogServer));
            host.AddServiceEndpoint(typeof(ISyslogServer), binding, address);

            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            host.Authorization.ServiceAuthorizationManager = new CustomAuthorizationManager();
            
            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit);

            host.Open();
            Console.WriteLine("WCFService is opened. Press <enter> to finish...");
            Console.ReadLine();

            host.Close();
        }

    }
}
