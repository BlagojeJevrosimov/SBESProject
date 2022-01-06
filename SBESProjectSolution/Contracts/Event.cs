using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class Event
    {

        string log = null;

        public string Log 
        {
            get
            {
                return log;
            }
            set
            {
                log = value;
            }
        }
    }
}
