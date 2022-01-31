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
                                                        // security identifier
                Audit.AuthorizationSuccess(userName,
                    OperationContext.Current.IncomingMessageHeaders.Action);
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
                        OperationContext.Current.IncomingMessageHeaders.Action, "Update method needs Update permission.");
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
                    " tried to call Update method. Update method needs Update permission.");
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Read")]
        public Event Read(int key)
        {
            // Posto se read proverava u CheckAccessCore metodi, autorizacija je uspesna, ne moramo proveravati
            Console.WriteLine("Read successfully executed.");

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            try
            {
                // logujemo uspesnu autorizaciju
                Audit.AuthorizationSuccess(userName,
                    OperationContext.Current.IncomingMessageHeaders.Action);    // naziv servisa
                if (Database.events.ContainsKey(key))
                {
                    return Database.events[key];
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

            return null;
        }
    }
 }
