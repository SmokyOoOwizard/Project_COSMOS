using System;

namespace COSMOS.Core
{
    public class PublicEventDispatcher : EventDispatcher
    {
        public void DispatchEvent<T>(T evt, IEventData data) where T : Enum
        {
            dispatchEvent(evt, data);
        }
    }

}
