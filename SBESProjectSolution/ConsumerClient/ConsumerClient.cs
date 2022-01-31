using Contracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace ConsumerClient
{
    public class ConsumerClient: ChannelFactory<ISyslogServer>, ISyslogServer, IDisposable
    {
        ISyslogServer factory;
        public ConsumerClient(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            factory = this.CreateChannel();

        }
        public void Subscribe()
        {
            try
            {
                factory.Subscribe();
                Console.WriteLine("Successfuly subscribed");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Subscribe : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Subscribe : {0}", e.Message);
            }
        }

        #region Read()

        public Event Read(int key)
        {
            Event ret = null;
            try
            {
                Event ev = factory.Read(key);
                Console.WriteLine(ev);
                Console.WriteLine("Read allowed");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Read : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Read : {0}", e.Message);
            }
            return ret;

        }

        #endregion

        #region Modify()

        public bool Update(Event ev)
        {
            bool retValue = false;
            try
            {
                retValue = factory.Update(ev);
                Console.WriteLine("Update allowed");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Update : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Update : {0}", e.Message);
            }
            return retValue;
        }


        #endregion

        #region Delete()
        public bool Delete(int key)
        {
            bool retValue = false;
            try
            {
                retValue = factory.Delete(key);
                Console.WriteLine("Delete allowed");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Delete : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Delete : {0}", e.Message);
            }
            return retValue;
        }

        #endregion

        #region ManagePrms

        public void ManagePermission(bool isAdd, string rolename, params string[] permissions)
        {
            try
            {
                factory.ManagePermission(isAdd, rolename, permissions);
                Console.WriteLine("Manage allowed");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to MenagePermission : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ManagePermission : {0}", e.Message);
            }
        }

        public void ManageRoles(bool isAdd, string rolename)
        {
            try
            {
                factory.ManageRoles(isAdd, rolename);
                Console.WriteLine("Manage allowed");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to ManageRoles : {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to ManageRoles : {0}", e.Message);
            }
        }

        #endregion
        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }
    }
}
