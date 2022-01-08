using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [DataContract]
    public enum CriticallityLevel { [EnumMember] GREEN_ALERT, [EnumMember] YELLOW_ALERT, [EnumMember] RED_ALERT }
    [DataContract]
    public enum MessageState { [EnumMember] OPEN, [EnumMember] CLOSE }

    [DataContract]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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
