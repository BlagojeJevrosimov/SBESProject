﻿using Contracts;
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
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);      // "wcfserviceb"

            #region ServerAsAClient

            string expectedSrvCertCN = "wcfbackup";
            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, expectedSrvCertCN);

            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9988/BackupService"),
                                      new X509CertificateEndpointIdentity(srvCert));

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                    StoreLocation.LocalMachine, signCertCN);

            var t = new Thread(() => BackupLogOperation(binding, address, certificateSign));
            t.Start();


            #endregion



            #region AFCC

                NetTcpBinding bindingAFCC = new NetTcpBinding();
            bindingAFCC.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string addressAFCC = "net.tcp://localhost:7777/SyslogServerSecurityEvent";
            ServiceHost hostAFCC = new ServiceHost(typeof(SyslogServerSecurityEvent));
            hostAFCC.AddServiceEndpoint(typeof(ISyslogServerSecurityEvent), bindingAFCC, addressAFCC);

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            hostAFCC.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            hostAFCC.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            hostAFCC.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            hostAFCC.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            // izvlacimo iz serverske app (My)

            hostAFCC.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            hostAFCC.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

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

            #region AF

            NetTcpBinding bindingAF = new NetTcpBinding();
            bindingAF.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string addressAF = "net.tcp://localhost:8888/SyslogServerSecurityEvent";
            ServiceHost hostAF = new ServiceHost(typeof(SyslogServerSecurityEvent));
            hostAF.AddServiceEndpoint(typeof(ISyslogServerSecurityEvent), bindingAF, addressAF);

            hostAF.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            hostAF.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            // host.Credentials.ServiceCertificate.Certificate
            hostAF.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            // izvlacimo iz serverske app (My)

            hostAF.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            hostAF.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

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

            #region Client

            NetTcpBinding bindingCC = new NetTcpBinding();
            string addressCC = "net.tcp://localhost:9999/SyslogServer";

            bindingCC.Security.Mode = SecurityMode.Transport;
            bindingCC.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            bindingCC.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(SyslogServer));
            host.AddServiceEndpoint(typeof(ISyslogServer), bindingCC, addressCC);

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

        public static void BackupLogOperation(NetTcpBinding binding, EndpointAddress address, X509Certificate2 certificateSign)
        {
            using (ServiceClient proxy = new ServiceClient(binding, address))
            {
                //proveravace da li ima nesto u listi pristiglih eventova, ako ima kontaktira backup, salje i tamo se vrsi auditing
                while (true)
                {
                    
                    Thread.Sleep(5000);
                    if (Database.formatedEvents.Count > 0)
                    {
                        SyslogServerSecurityEvent.mutex.WaitOne();
                        foreach (string message in Database.formatedEvents)
                        {
                            byte[] signature = DigitalSignature.Create(message, HashAlgorithm.SHA1, certificateSign);

                            proxy.BackupLog(message, signature);
                            Console.WriteLine("Backup log executed");
                        }

                        Database.formatedEvents.Clear();
                        SyslogServerSecurityEvent.mutex.ReleaseMutex();

                    }
                }
            }
        }
    }
}
