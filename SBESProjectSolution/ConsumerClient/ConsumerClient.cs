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
                Console.Clear();
                factory.Subscribe();
                Console.WriteLine("Successfuly subscribed to security event logs.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Subscribe : {0}", e.Detail.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Subscribe : {0}", e.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        #region Modify()

        public void Update(int key, MessageState ms)
        {
            
            try
            {
                Read();
                Console.WriteLine("Choose key of event log: ");
                key = Int32.Parse(Console.ReadLine());
                Console.WriteLine("Chose new State of log(OPEN/CLOSE):");
                string state = Console.ReadLine();
                if (state.ToLower() == "open")
                {
                    ms = MessageState.OPEN;
                }
                else if (state.ToLower() == "close")
                {
                    ms = MessageState.CLOSE;
                }
                else ms = MessageState.OPEN;
                
                factory.Update(key,ms);
                Console.WriteLine("Updated event with key:{0} to state {1} successfully",key,ms);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Update : {0}", e.Detail.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Update : {0}", e.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
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


        public Dictionary<int, Event> Read()
        {
            try
            {
                Console.Clear();
                Dictionary<int, Event> events = factory.Read();
                Console.WriteLine("Recieved logs:\n");
                foreach (var e in events)
                {
                    Console.WriteLine(e.Key.ToString() + ":" + e.Value);
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return events;
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Read : {0}", e.Detail.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Read : {0}", e.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            return null;
        }
    }
    
}
