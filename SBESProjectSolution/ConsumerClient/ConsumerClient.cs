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
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Subscribe : {0}", e.Message);
            }
        }
        #region Read()

        public string Read()
        {
            try
            {
                Console.WriteLine(factory.Read());
                Console.WriteLine("Read allowed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Read : {0}", e.Message);
            }
            return "";

        }

        #endregion

        #region Modify()

        public bool Update()
        {
            bool retValue = false;
            try
            {
                retValue = factory.Update();
                Console.WriteLine("Update allowed");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error while trying to Update : {0}", e.Message);
            }
            return retValue;
        }


        #endregion

        #region Delete()
        public bool Delete()
        {
            bool retValue = false;
            try
            {
                retValue = factory.Delete();
                Console.WriteLine("Delete allowed");
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
