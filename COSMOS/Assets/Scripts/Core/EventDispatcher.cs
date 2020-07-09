using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COSMOS.Core
{
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
    public delegate void EventDispatcherDelegate(object evtData);
    public interface IEventDispatcher
    {
        void AddListener(string evtName, EventDispatcherDelegate callback);
        void DropListener(string evtName, EventDispatcherDelegate callback);
        void Dispatch(string evtName, object evt);

    }
}
