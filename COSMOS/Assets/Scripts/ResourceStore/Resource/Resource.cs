using System.Collections;
using System.Collections.Generic;

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


        public uint Id { get; internal set; }
        public string Name { get; private set; }
        public string Collection { get; private set; }
        public bool Internal { get; private set; }

        public Status CurrentStatus { get; private set; }

        private readonly List<ResourceInstance> instances = new List<ResourceInstance>();

        private readonly List<Resource> linkedResources = new List<Resource>();

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

        protected void LinkResource(Resource resourceToLink)
        {
            linkedResources.Remove(resourceToLink);
            linkedResources.Add(resourceToLink);
        }
        protected void UnLinkResource(Resource resourceToUnlink)
        {
            linkedResources.Remove(resourceToUnlink);
        }
        protected void UnLinkAllResources()
        {
            linkedResources.Clear();
        }

        protected bool LinkedResourcesLoaded()
        {
            for (int i = 0; i < linkedResources.Count; i++)
            {
                if (linkedResources[i].CurrentStatus != Status.Loaded)
                {
                    return false;
                }
            }
            return true;
        }
        protected bool LinkedResourcesUnloaded()
        {
            for (int i = 0; i < linkedResources.Count; i++)
            {
                if (linkedResources[i].CurrentStatus != Status.Unloaded)
                {
                    return false;
                }
            }
            return true;
        }
        protected bool AnyLinkedResourcesWithError()
        {
            for (int i = 0; i < linkedResources.Count; i++)
            {
                if (linkedResources[i].CurrentStatus == Status.Error)
                {
                    return true;
                }
            }
            return false;
        }

        protected void TryLoadLinkedResources()
        {
            for (int i = 0; i < linkedResources.Count; i++)
            {
                linkedResources[i].TryLoad();
            }
        }
        protected void TryUnloadLinkedResources()
        {
            for (int i = 0; i < linkedResources.Count; i++)
            {
                linkedResources[i].TryUnload();
            }
        }
    }
}
