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
        public static Mutex mutex = new Mutex();
        public void sendEvent(Event ev)
        {
            mutex.WaitOne();
            
            Database.events[Database.eventKey] = ev;
            Console.WriteLine("Event successfully added to database.");
            Database.eventKey++;

            string source = "";
            if (ev.Source != null)
                source = ev.Source.ToString();
            string message = String.Format(ev.Criticallity.ToString(), ev.Timestamp.ToString(), source, ev.Message, ev.State.ToString());
            Database.formatedEvents.Add(message);

            mutex.ReleaseMutex();

            try
            {
                Audit.EventSuccess(ev);
                byte[] signature = DigitalSignature.Create(message, HashAlgorithm.SHA1, Program.certificateSign);
                Program.proxyBS.BackupLog(message, signature);
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
