using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public enum CriticallityLevel { GREEN_ALERT, YELLOW_ALERT, RED_ALERT }
    public enum MessageState { OPEN, CLOSE }

    public class Event
    {
        CriticallityLevel criticallity;
        DateTime timestamp;
        Consumer source;
        string message;
        MessageState state;

        public CriticallityLevel Criticallity 
        {
            get
            {
                return criticallity;
            }
            set
            {
                criticallity = value;
            } 
        }

        public DateTime Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                timestamp = value;
            }
        }

        public Consumer Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        public MessageState State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }
    }
}
