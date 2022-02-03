using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SyslogServer
{
    public class SyslogServerSecurityEvent : ISyslogServerSecurityEvent
    {
        public Mutex m = new Mutex();
        public void sendEvent(Event ev)
        {
            m.WaitOne();
            Console.WriteLine(ev);
            Database.events[Database.eventKey] = ev;
            Database.eventKey++;
            m.ReleaseMutex();
        }
    }
}
