using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COSMOS.Core
{
    public delegate void EventDispatcherDelegate(CosmosEvent evt);
    public class CosmosEvent
    {
        public IEventDispatcher Caller { get; private set; }
        public object EventObject { get; private set; }

        public CosmosEvent(IEventDispatcher caller, object eventObject)
        {
            Caller = caller;
            EventObject = eventObject;
        }
    }
    public class EventDispatcher : IEventDispatcher
    {
        private Dictionary<string, List<EventDispatcherDelegate>> listeners = new Dictionary<string, List<EventDispatcherDelegate>>();

        public void AddListener(string evtName, EventDispatcherDelegate callback)
        {
            List<EventDispatcherDelegate> evtListeners = null;
            if (listeners.TryGetValue(evtName, out evtListeners))
            {
                evtListeners.Remove(callback);
                evtListeners.Add(callback);
            }
            else
            {
                evtListeners = new List<EventDispatcherDelegate>();
                evtListeners.Add(callback);

                listeners.Add(evtName, evtListeners);
            }
        }
        public void DropListener(string evtName, EventDispatcherDelegate callback)
        {
            List<EventDispatcherDelegate> evtListeners = null;
            if (listeners.TryGetValue(evtName, out evtListeners))
            {
                for (int i = 0; i < evtListeners.Count; i++)
                {
                    evtListeners.Remove(callback);
                }
            }
        }
        protected void Dispatch(string evtName, object obj)
        {
            List<EventDispatcherDelegate> evtListeners = null;
            if (listeners.TryGetValue(evtName, out evtListeners))
            {
                var evt = new CosmosEvent(this, obj);
                for (int i = 0; i < evtListeners.Count; i++)
                {
                    evtListeners[i](evt);
                }
            }
        }
    }
    public class EventDispatcher<E> : EventDispatcher, IEventDispatcher<E> where E : Enum
    {
        private Dictionary<E, List<EventDispatcherDelegate>> listeners = new Dictionary<E, List<EventDispatcherDelegate>>();

        public void AddListener(E eventType, EventDispatcherDelegate callback)
        {
            List<EventDispatcherDelegate> evtListeners = null;
            if (listeners.TryGetValue(eventType, out evtListeners))
            {
                evtListeners.Remove(callback);
                evtListeners.Add(callback);
            }
            else
            {
                evtListeners = new List<EventDispatcherDelegate>();
                evtListeners.Add(callback);

                listeners.Add(eventType, evtListeners);
            }
        }
        public void DropListener(E eventType, EventDispatcherDelegate callback)
        {
            List<EventDispatcherDelegate> evtListeners = null;
            if (listeners.TryGetValue(eventType, out evtListeners))
            {
                for (int i = 0; i < evtListeners.Count; i++)
                {
                    evtListeners.Remove(callback);
                }
            }
        }
        protected void Dispatch(E eventType, object obj)
        {
            List<EventDispatcherDelegate> evtListeners = null;
            if (listeners.TryGetValue(eventType, out evtListeners))
            {
                var evt = new CosmosEvent(this, obj);
                for (int i = 0; i < evtListeners.Count; i++)
                {
                    evtListeners[i](evt);
                }
            }
        }

    }
    public interface IEventDispatcher
    {
        void AddListener(string evtName, EventDispatcherDelegate callback);
        void DropListener(string evtName, EventDispatcherDelegate callback);
    }
    public interface IEventDispatcher<E> : IEventDispatcher where E : Enum
    {
        void AddListener(E eventType, EventDispatcherDelegate callback);
        void DropListener(E eventType, EventDispatcherDelegate callback);
    }
}
