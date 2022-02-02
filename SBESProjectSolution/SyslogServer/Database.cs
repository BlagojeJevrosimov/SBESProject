using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyslogServer
{
    public class Database
    {
        internal static Dictionary<string, Consumer> subscribers = new Dictionary<string, Consumer>();
        internal static int eventKey = 0;
        internal static Dictionary<int, Event> events = new Dictionary<int, Event>();   // logovi

        static Database()
        {
            events[1] = new Event(CriticallityLevel.GREEN_ALERT,DateTime.Now,new Consumer("pera","sid"),"Poruka",MessageState.OPEN);
            events[2] = new Event(CriticallityLevel.YELLOW_ALERT, DateTime.Now, new Consumer("pera", "sid"), "Poruka2", MessageState.CLOSE);
        }
    }
}
