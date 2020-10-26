using System;

namespace COSMOS.Core
{
    public class RemoteEventDispatcher : EventDispatcher
    {
        private interface IInternal
        {
            RemoteEventDispatcher dispatcher { set; }
        }
        public class RemoteDispatch : IInternal
        {
            RemoteEventDispatcher IInternal.dispatcher { set { dispatcher = value; } }
            RemoteEventDispatcher dispatcher;
            public void DispatchEvent<T>(T evt, IEventData data) where T : Enum
            {
                dispatcher.dispatchEvent(evt, data);
            }
        }

        public RemoteEventDispatcher(RemoteDispatch remouteDispatch)
        {
            (remouteDispatch as IInternal).dispatcher = this;
        }
    }

}
