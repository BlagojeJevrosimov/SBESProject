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
            if (!Database.subscribers.ContainsKey(windowsIdentity.Name))
            {
                Database.subscribers[windowsIdentity.User.ToString()] = new Consumer(windowsIdentity.Name, windowsIdentity.User.ToString());
            }
           
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Delete")]
        public bool Delete()
        {
            return false;
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

        [PrincipalPermission(SecurityAction.Demand, Role = "Update")]
        public bool Update()
        {

            return false;


        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Read")]
        public string Read()
        {

            return "";


        }
    }
}
 
