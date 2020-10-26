using System;

namespace COSMOS.Core
{
    public class Event
    {
        public EventDispatcher Owner;
        public int EventType;
        public IEventData Data;
    }
    public class Event<T> : Event where T : Enum
    {
        public new T EventType
        {
            get { return (T)(object)base.EventType; }
            set { base.EventType = (int)(object)value; }
        }
    }
}
