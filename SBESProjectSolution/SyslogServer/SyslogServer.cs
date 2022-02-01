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
        string address = "net.tcp://localhost:9998/Client";
        Dictionary<string, List<string>> logs = new Dictionary<string, List<string>>();
        [PrincipalPermission(SecurityAction.Demand, Role = "Read")]
        public void Subscribe() 
        {
            WindowsIdentity windowsIdentity = Thread.CurrentPrincipal.Identity as WindowsIdentity;

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (!Database.subscribers.ContainsKey(windowsIdentity.Name))
            {
                Database.subscribers[windowsIdentity.User.ToString()] = 
                    new Consumer(windowsIdentity.Name, windowsIdentity.User.ToString());
                logs[windowsIdentity.User.ToString()] = new List<string>();
                                                        // security identifier
                string message = Audit.AuthorizationSuccess(userName,
                    OperationContext.Current.IncomingMessageHeaders.Action);

                foreach (var c in logs)
                {
                    c.Value.Add(message);
                }

            }
            else {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format("User {0} is already subscribed (time : {1}).", name, time.TimeOfDay);
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Delete")]
        public bool Delete(int key)
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
                    return Database.events.Remove(key);
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return false;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "Delete method need Delete permission.");
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
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
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
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
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
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
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
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
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
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
        public bool Update(Event ev)
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
                    if (Database.events.ContainsKey(ev.Key))
                    {
                        Event oldEv = Database.events[ev.Key];
                        oldEv.Update(ev);
                    }
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return false;
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "User doesn't have Update permission.");
                }
                catch (FaultException<SecurityException> e)
                {
                    Console.WriteLine(e.Detail.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " tried to call Update method without permission");
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Read")]
        public List<string> Read()
        {
            // Posto se read proverava u CheckAccessCore metodi, autorizacija je uspesna, ne moramo proveravati
            Console.WriteLine("Read successfully executed.");
            WindowsIdentity windowsIdentity = Thread.CurrentPrincipal.Identity as WindowsIdentity;

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            try
            {
                // logujemo uspesnu autorizaciju
                string log = Audit.AuthorizationSuccess(userName,
                    OperationContext.Current.IncomingMessageHeaders.Action);    // naziv servisa
                logs[windowsIdentity.User.ToString()].Add(log);
                List<string> logovi = new List<string>();
                logs[windowsIdentity.User.ToString()].ForEach(delegate(string s){
                    logovi.Add(s);
                }) ;
                logs[windowsIdentity.User.ToString()].Clear();
                return logovi;

            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine(e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

    }
 }
