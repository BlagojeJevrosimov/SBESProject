using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BackupServer
{
    public class BackupService : ISyslogServerBackupData
    {
        public void BackupLog(string message, byte[] sign)
        {
            //kad je u pitanju autentifikacija putem Sertifikata
            string clienName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name); //koristimo javni kljuc od Servisa jer smo se potpisali uz pomoc privatnog
            string clientNameSign = clienName + "_sign"; //wcfservice_sign
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);

            /// Verify signature using SHA1 hash algorithm
            if (DigitalSignature.Verify(message, HashAlgorithm.SHA1, sign, certificate))
            {
                Console.WriteLine("Sign is valid");
            }
            else
            {
                Console.WriteLine("Sign is invalid");
            }

        }
    }
}
