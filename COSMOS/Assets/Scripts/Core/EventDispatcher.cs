using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COSMOS.Core
{
	public delegate void EventDispatcherDelegate(object evtData);
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
		public void Dispatch(string evtName, object evt)
		{
			List<EventDispatcherDelegate> evtListeners = null;
			if (listeners.TryGetValue(evtName, out evtListeners))
			{
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

		public void AddListener(E evt, EventDispatcherDelegate callback)
		{
			List<EventDispatcherDelegate> evtListeners = null;
			if (listeners.TryGetValue(evt, out evtListeners))
			{
				evtListeners.Remove(callback);
				evtListeners.Add(callback);
			}
			else
			{
				evtListeners = new List<EventDispatcherDelegate>();
				evtListeners.Add(callback);

				listeners.Add(evt, evtListeners);
			}
		}
		public void DropListener(E evt, EventDispatcherDelegate callback)
		{
			List<EventDispatcherDelegate> evtListeners = null;
			if (listeners.TryGetValue(evt, out evtListeners))
			{
				for (int i = 0; i < evtListeners.Count; i++)
				{
					evtListeners.Remove(callback);
				}
			}
		}
		public void Dispatch(E evt, object obj)
		{
			List<EventDispatcherDelegate> evtListeners = null;
			if (listeners.TryGetValue(evt, out evtListeners))
			{
				for (int i = 0; i < evtListeners.Count; i++)
				{
					evtListeners[i](obj);
				}
			}
		}

	}
	public interface IEventDispatcher
	{
        void AddListener(string evtName, EventDispatcherDelegate callback);
        void DropListener(string evtName, EventDispatcherDelegate callback);
        void Dispatch(string evtName, object evt);
	}
    public interface IEventDispatcher<E> : IEventDispatcher where E : Enum
    {
		void AddListener(E evt, EventDispatcherDelegate callback);
		void DropListener(E evt, EventDispatcherDelegate callback);
		void Dispatch(E evt, object obj);
    }
}
