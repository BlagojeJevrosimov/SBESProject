using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SecurityManager;

namespace SyslogServer
{
    public class SyslogServerSecurityEvent : ISyslogServerSecurityEvent
    {
        public Mutex m = new Mutex();
        public void sendEvent(Event ev)
        {
            m.WaitOne();
            Database.events[Database.eventKey] = ev;
            Console.WriteLine("Event successfully added to database.");
            Database.eventKey++;
            m.ReleaseMutex();

            try
            {
                Audit.EventSuccess(ev);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
