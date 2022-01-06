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

            Console.WriteLine("Ime klijenta koji je pozvao metodu login : " + windowsIdentity.Name);
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

        //[PrincipalPermission(SecurityAction.Demand, Role = "Delete")]
        public bool Delete()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Delete"))
            {
                Console.WriteLine("Delete successfully executed.");

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return true;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "Delete method need Delete permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call Delete method. Delete method need Delete permission.");
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Administrate")]
        public void ManagePermission(bool isAdd, string rolename, params string[] permissions)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Administrate"))     // provera da li korisnik ima odgovarajucu permisiju
            {
                Console.WriteLine("ManagePermission successfully executed.");

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);

                    if (isAdd) // u pitanju je dodavanje
                    {
                        RolesConfig.AddPermissions(rolename, permissions);
                    }
                    else // u pitanju je brisanje
                    {
                        RolesConfig.RemovePermissions(rolename, permissions);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "ManagePermission method needs Administrate permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " tried to call ManagePermission method. ManagePermission method needs Administrate permission.");
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Administrate")]
        public void ManageRoles(bool isAdd, string rolename)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Administrate"))     // provera da li korisnik ima odgovarajucu permisiju
            {
                Console.WriteLine("ManageRoles successfully executed.");

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);

                    if (isAdd) // u pitanju je dodavanje
                    {
                        RolesConfig.AddRole(rolename);
                    }
                    else // u pitanju je brisanje
                    {
                        RolesConfig.RemoveRole(rolename);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "ManageRoles method needs Administrate permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " tried to call ManageRoles method. ManageRoles method needs Administrate permission.");
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Update")]
        public bool Update()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Update"))     // provera da li korisnik ima odgovarajucu permisiju
            {
                Console.WriteLine("Update successfully executed.");

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return true;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "Update method needs Update permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " tried to call Update method. Update method needs Update permission.");
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Read")]
        public string Read()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("Update"))
            {
                try
                {
                    // logujemo uspesnu autorizaciju
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);    // naziv servisa
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "Read method needs Read permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " tried to call Read method. Read method needs Read permission.");
            }
            
            return null;
        }
    }
 }
