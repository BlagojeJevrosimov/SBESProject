using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyslogServer
{
    class Program
    {
        static void Main(string[] args)
        {
            #region AF

            /// srvCertCN.SubjectName should be set to the service's username. .NET WindowsIdentity class provides information about Windows user running the given process
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);      // "wcfserver"

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:8888/SyslogServerSecurityEvent";
            ServiceHost hostAF = new ServiceHost(typeof(SyslogServerSecurityEvent));
            hostAF.AddServiceEndpoint(typeof(ISyslogServerSecurityEvent), binding, address);

            hostAF.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            hostAF.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            // host.Credentials.ServiceCertificate.Certificate
            hostAF.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            // izvlacimo iz serverske app (My)

            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            hostAF.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            hostAF.Description.Behaviors.Add(newAudit);

            try
            {
                hostAF.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }

            Console.WriteLine("AF process is working.");

            #endregion

            #region AFCC

            binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            address = "net.tcp://localhost:7777/SyslogServerSecuritzEvent";
            ServiceHost hostAFCC = new ServiceHost(typeof(SyslogServerSecurityEvent));
            hostAFCC.AddServiceEndpoint(typeof(ISyslogServerSecurityEvent), binding, address);

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator

            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            hostAFCC.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            hostAFCC.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            hostAFCC.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            hostAFCC.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            // izvlacimo iz serverske app (My)

            ServiceSecurityAuditBehavior newAudit2 = new ServiceSecurityAuditBehavior();
            newAudit2.AuditLogLocation = AuditLogLocation.Application;
            newAudit2.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            hostAFCC.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            hostAFCC.Description.Behaviors.Add(newAudit2);

            try
            {
                hostAFCC.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }

            Console.WriteLine("AFCC process is working.");

            #endregion

            #region Client

            binding = new NetTcpBinding();
            address = "net.tcp://localhost:9999/SyslogServer";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(SyslogServer));
            host.AddServiceEndpoint(typeof(ISyslogServer), binding, address);

            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            //host.Authorization.ServiceAuthorizationManager = new CustomAuthorizationManager();

            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            ServiceSecurityAuditBehavior newAudit3 = new ServiceSecurityAuditBehavior();
            newAudit3.AuditLogLocation = AuditLogLocation.Application;
            newAudit3.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            host.Description.Behaviors.Add(newAudit3);

            host.Open();
            Console.WriteLine("Consumer Client process is working.\n");

            #endregion

            Console.ReadKey();
            host.Close();
            hostAF.Close();
            hostAFCC.Close();
        }
    }
}
