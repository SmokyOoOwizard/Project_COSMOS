using System.Collections.Generic;
using System.Threading.Tasks;

namespace COSMOS.Core
{
    public abstract class ManagedResource : EventDispatcher
    {
        public struct ManageCompleteStatus
        {
            public enum Status
            {
                Loaded,
                Unloaded,
                Error
            }
            public Status CurrentStatus;

        }
        public enum Events
        {
            Load,
            Loaded,
            Unload,
            Unloaded,

            ZeroLinks,
            HasLinks
        }
        public enum ResourceTaskType
        {
            Loading,
            Unloading
        }
        public enum ResourceStatus
        {
            Unloaded,
            Unloading,
            Loaded,
            Loading,
            Error
        }

        private readonly List<object> locks = new List<object>();
        private object refLockObject = new object();
        private object loadUnloadLockObject = new object();

        public ResourceStatus CurrentStatus { get; private set; }

        private KeyValuePair<ResourceTaskType, Task<ManageCompleteStatus>> currentResourceTask;
        public KeyValuePair<ResourceTaskType, Task<ManageCompleteStatus>> CurrentResourceTask { get { return currentResourceTask; } }

        public void Reg(object lockO)
        {
            lock (refLockObject)
            {
                bool zero = false;
                if (locks.Count == 0)
                {
                    zero = true;
                }
                locks.Remove(lockO);
                locks.Add(lockO);

                if (zero)
                {
                    dispatchEvent(Events.HasLinks);
                    tryLoad();
                }
            }
        }

        public void Unreg(object lockObject)
        {
            lock (lockObject)
            {
                locks.Remove(lockObject);

                if (locks.Count == 1)
                {
                    dispatchEvent(Events.ZeroLinks);
                    tryUnload();
                }
            }
        }

        public bool TryLoad()
        {
            lock (loadUnloadLockObject)
            {
                return tryLoad();
            }
        }
        private bool tryLoad()
        {
            if (currentResourceTask.Value == null ||
                currentResourceTask.Key == ResourceTaskType.Unloading && currentResourceTask.Value.IsCompleted)
            {
                if (CurrentStatus == ResourceStatus.Unloaded)
                {
                    var loadTask = Load();
                    CurrentStatus = ResourceStatus.Loading;

                    currentResourceTask = new KeyValuePair<ResourceTaskType, Task<ManageCompleteStatus>>(
                        ResourceTaskType.Loading, loadTask);

                    dispatchEvent(Events.Load);

                    loadTask.ContinueWith((t) =>
                    {
                        manageCompleteStatusHandle(t.Result);
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                    if (loadTask.Status == TaskStatus.Created || loadTask.Status == TaskStatus.WaitingToRun)
                    {
                        loadTask.Start();
                    }
                    else if(loadTask.IsCompleted)
                    {
                        manageCompleteStatusHandle(loadTask.Result);
                    }

                    return true;
                }
            }
            return false;
        }
        public bool TryUnload()
        {
            lock (loadUnloadLockObject)
            {
                return tryUnload();
            }
        }
        private bool tryUnload()
        {
            if (currentResourceTask.Value == null ||
                currentResourceTask.Key == ResourceTaskType.Loading && currentResourceTask.Value.IsCompleted)
            {
                if (CurrentStatus == ResourceStatus.Loaded)
                {
                    var unloadTask = Unload();
                    CurrentStatus = ResourceStatus.Unloading;

                    currentResourceTask = new KeyValuePair<ResourceTaskType, Task<ManageCompleteStatus>>(
                        ResourceTaskType.Unloading, unloadTask);

                    dispatchEvent(Events.Unload);

                    unloadTask.ContinueWith((t) =>
                    {
                        manageCompleteStatusHandle(t.Result);
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                    if (unloadTask.Status == TaskStatus.Created || unloadTask.Status == TaskStatus.WaitingToRun)
                    {
                        unloadTask.Start();
                    }
                    else if (unloadTask.IsCompleted)
                    {
                        manageCompleteStatusHandle(unloadTask.Result);
                    }

                    return true;
                }
            }
            return false;
        }

        protected virtual bool LoadCondition()
        {
            return true;
        }
        protected virtual bool UnloadCondition()
        {
            return true;
        }

        private void manageCompleteStatusHandle(ManageCompleteStatus status)
        {
            switch (status.CurrentStatus)
            {
                case ManageCompleteStatus.Status.Loaded:
                    CurrentStatus = ResourceStatus.Loaded;
                    Log.Info("Resource loaded: " + this.ToString(), GetType().Name, "ManagedResource");
                    break;
                case ManageCompleteStatus.Status.Unloaded:
                    CurrentStatus = ResourceStatus.Unloaded;
                    Log.Info("Resource unloaded: " + this.ToString(), GetType().Name, "ManagedResource");
                    break;
                case ManageCompleteStatus.Status.Error:
                    CurrentStatus = ResourceStatus.Error;
                    Log.Error("Error while resource try unload/load process: " + this.ToString(), 
                        GetType().Name, "ManagedResource");
                    break;
            }
        }

        protected abstract Task<ManageCompleteStatus> Unload();
        protected abstract Task<ManageCompleteStatus> Load();
    }
}
