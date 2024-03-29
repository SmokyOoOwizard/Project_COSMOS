﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace COSMOS.Core
{
    public delegate bool EventCallBack(Event evt);
    public abstract class EventDispatcher
    {
        private Dictionary<Type, Dictionary<int, List<EventCallBack>>> events = new Dictionary<Type, Dictionary<int, List<EventCallBack>>>();
        private Mutex mutex = new Mutex();

        public bool AddListener<T>(EventCallBack action, T evt) where T : Enum
        {
            if (action != null)
            {
                var eventType = typeof(T);
                int eventNumber = (int)(object)evt;
                mutex.WaitOne();
                if (events.TryGetValue(eventType, out Dictionary<int, List<EventCallBack>> eventTypeCollection))
                {
                    if (eventTypeCollection.TryGetValue(eventNumber, out List<EventCallBack> RightTypeEvents))
                    {
                        RightTypeEvents.Add(action);
                    }
                    else
                    {
                        eventTypeCollection.Add(eventNumber, new List<EventCallBack>() { action });
                    }
                }
                else
                {
                    events[eventType] = new Dictionary<int, List<EventCallBack>>() { { eventNumber, new List<EventCallBack>() { action } } };
                }
                mutex.ReleaseMutex();
                return true;
            }
            return false;
        }
        public bool RemoveListener<T>(EventCallBack action, T evt) where T : Enum
        {
            if (action != null)
            {
                var eventType = typeof(T);
                int eventNumber = (int)(object)evt;
                mutex.WaitOne();
                if (events.TryGetValue(eventType, out Dictionary<int, List<EventCallBack>> eventTypeCollection))
                {
                    if (eventTypeCollection.TryGetValue(eventNumber, out List<EventCallBack> RightTypeEvents))
                    {
                        bool result = RightTypeEvents.Remove(action);
                        mutex.ReleaseMutex();
                        return result;
                    }
                }
            }
            return false;
        }
        protected void dispatchEvent<T>(T evt, IEventData data = null) where T : Enum
        {
            var eventType = typeof(T);
            int eventNumber = (int)(object)evt;
            mutex.WaitOne();
            if (events.TryGetValue(eventType, out Dictionary<int, List<EventCallBack>> eventTypeCollection))
            {
                if (eventTypeCollection.TryGetValue(eventNumber, out List<EventCallBack> RightTypeEvents))
                {

                    var e = new Event<T>();
                    e.EventType = evt;
                    e.Data = data;
                    e.Owner = this;
                    Event evtObject = e as Event;

                    var events = new List<EventCallBack>(RightTypeEvents);
                    mutex.ReleaseMutex();

                    try
                    {
                        for (int i = 0; i < events.Count; i++)
                        {
                            if (!events[i].Invoke(evtObject))
                            {
                                events.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return;
                }
            }
            mutex.ReleaseMutex();
        }

        ~EventDispatcher()
        {
            mutex.Dispose();
        }
    }
}
