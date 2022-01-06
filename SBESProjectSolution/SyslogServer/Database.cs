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

        internal static Dictionary<int, Event> events = new Dictionary<int, Event>();
    }
}
