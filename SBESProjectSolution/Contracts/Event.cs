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
        int key;
        CriticallityLevel criticallity;
        DateTime timestamp;
        Consumer source;
        string message;
        MessageState state;

        public Event(int key, CriticallityLevel criticallity, DateTime timestamp, Consumer source, string message, MessageState state)
        {
            this.key = key;
            this.timestamp = timestamp;
            this.source = source;
            this.message = message;
            this.state = state;
        }

        public int Key 
        {
            get 
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

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

        public void Update(Event newEv)
        {
            this.Criticallity = newEv.Criticallity;
            this.Timestamp = newEv.Timestamp;
            this.Source = newEv.Source;
            this.Message = newEv.Message;
            this.State = newEv.State;
        }
    }
}
