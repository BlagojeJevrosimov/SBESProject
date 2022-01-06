using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationFirewall
{
    public class WhitelistConfig
    {
        List<int> validPorts = new List<int>();
        List<string> validProtocols = new List<string>();

        public bool AddPort(int port)
        {
            if (!validPorts.Contains(port))
            {
                validPorts.Add(port);
                return true;
            }
            return false;
        }

        public bool RemovePort(int port)
        {
            if (validPorts.Contains(port))
            {
                validPorts.Remove(port);
                return true;
            }
            return false;
        }

        public bool AddProtocol(string protocol)
        {
            if (!validProtocols.Contains(protocol))
            {
                validProtocols.Add(protocol);
                return true;
            }
            return false;
        }

        public bool RemoveProtocol(string protocol)
        {
            if (validProtocols.Contains(protocol))
            {
                validProtocols.Remove(protocol);
                return true;
            }
            return false;
        }
    }
}
