using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using COSMOS.Core;

namespace COSMOS.ResourceStore
{
    public abstract class Resource : EventDispatcher
    {
        public enum Status
        {
            Unloaded,
            Loaded,
            Unloading,
            Loading,
            Error
        }

        public string Name { get; private set; }
        public string Collection { get; private set; }
        public bool Internal { get; private set; }

        public Status CurrentStatus { get; private set; }

        private readonly List<ResourceInstance> instances = new List<ResourceInstance>();

        private readonly object instancesLock = new object();
        private readonly object tryLoadUnloadLock = new object();

        protected void SetCurrentStatus(Status status)
        {
            CurrentStatus = status;
            dispatchEvent(status);
        }

        public void TryLoad()
        {
            if (CurrentStatus == Status.Unloaded || CurrentStatus == Status.Error)
            {
                lock (tryLoadUnloadLock)
                {
                    if (CurrentStatus == Status.Unloaded || CurrentStatus == Status.Error)
                    {
                        tryLoad();
                    }
                }
            }
        }
        public void TryUnload()
        {
            if (CurrentStatus == Status.Loaded || CurrentStatus == Status.Error)
            {
                lock (tryLoadUnloadLock)
                {
                    if (CurrentStatus == Status.Loaded || CurrentStatus == Status.Error)
                    {
                        lock (instancesLock)
                        {
                            if (instances.Count == 0)
                            {
                                tryUnload();
                            }
                        }
                    }
                }
            }
        }

        protected abstract void tryLoad();
        protected abstract void tryUnload();

        public ResourceInstance GetInstance()
        {
            lock (instancesLock)
            {
                var instance = OnGetInstance();

                if (instance != null)
                {
                    instances.Add(instance);
                }

                return instance;
            }
        }
        protected abstract ResourceInstance OnGetInstance();

        internal void ReturnInstance(ResourceInstance instance)
        {
            if (instance != null)
            {
                lock (instancesLock)
                {
                    if (instances.Remove(instance))
                    {
                        OnReturnInstance(instance);
                    }
                }
            }
        }
        protected abstract void OnReturnInstance(ResourceInstance instance);


        internal void ForceReturnInstances()
        {
            lock (instancesLock)
            {
                foreach (var instance in instances)
                {
                    instance.InternalFree();
                    OnReturnInstance(instance);
                }
                instances.Clear();
            }
        }
    }
}
