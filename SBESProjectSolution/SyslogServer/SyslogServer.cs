using Contracts;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace SyslogServer
{
    public class SyslogServer : ISyslogServer
    {
        public static Dictionary<string, Consumer> UserAccountsDB = new Dictionary<string, Consumer>();
        public void Login(string username, string password)
        {
            if (!UserAccountsDB.ContainsKey(username))
            {
                UserAccountsDB.Add(username, new Consumer(username, password));
            }
            else
            {
                Console.WriteLine($"Korisnik sa korisnickim imenom {username} vec postoji u bazi");
            }

            IIdentity identity = Thread.CurrentPrincipal.Identity;

           // Console.WriteLine("Tip autentifikacije : " + identity.AuthenticationType);

            WindowsIdentity windowsIdentity = identity as WindowsIdentity;

            Console.WriteLine("Ime klijenta koji je pozvao metodu : " + windowsIdentity.Name);
           // Console.WriteLine("Jedinstveni identifikator : " + windowsIdentity.User);

           /* Console.WriteLine("Grupe korisnika:");
            foreach (IdentityReference group in windowsIdentity.Groups)
            {
                SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                string name = (sid.Translate(typeof(NTAccount))).ToString();
                Console.WriteLine(name);
            }*/
        }
        public void Subscribe() 
        {
            WindowsIdentity windowsIdentity = Thread.CurrentPrincipal.Identity as WindowsIdentity;
            if (!Database.subscribers.ContainsKey(windowsIdentity.Name))
            {
                Database.subscribers[windowsIdentity.User.ToString()] = new Consumer(windowsIdentity.Name, windowsIdentity.User.ToString());
            }
            else {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format("User {0} is already subscribed (time : {1}).", name, time.TimeOfDay);
                throw new FaultException<SecurityException>(new SecurityException(message));
            }


        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Delete")]
        public bool Delete()
        {

            string name = Thread.CurrentPrincipal.Identity.Name;
            DateTime time = DateTime.Now;
            string message = String.Format("Access is denied. User {0} try to call Delete method (time : {1}). " +
                "For this method need to be member of group Admin.", name, time.TimeOfDay);
            throw new FaultException<SecurityException>(new SecurityException(message));

        }
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrate")]
        public void ManagePermission(bool isAdd, string rolename, params string[] permissions)
        {
            if (isAdd) // u pitanju je dodavanje
            {
                RolesConfig.AddPermissions(rolename, permissions);
            }
            else // u pitanju je brisanje
            {
                RolesConfig.RemovePermissions(rolename, permissions);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Administrate")]
        public void ManageRoles(bool isAdd, string rolename)
        {
            if (isAdd) // u pitanju je dodavanje
            {
                RolesConfig.AddRole(rolename);
            }
            else // u pitanju je brisanje
            {
                RolesConfig.RemoveRole(rolename);
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Modify")]
        public bool Update()
        {
       
            return false;
            

        }


        public string Read()
        {

            return null;
            

        }
    }
 }
