using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using COSMOS.Core;

namespace COSMOS.ResourceStore
{
    public abstract class Resource : EventDispatcher
    {
        // ресурс
        public enum Status
        {
            Unloaded,
            Loaded,
            Unloading,
            Loadign
        }

        public string Name { get; private set; }
        public string Collection { get; private set; }
        public bool Internal { get; private set; }

        public Status CurrentStatus { get; private set; }

        private readonly List<ResourceInstance> instances = new List<ResourceInstance>();

        protected void SetCurrentStatus(Status status)
        {
            CurrentStatus = status;
            dispatchEvent(status);
        }

        public ResourceInstance GetInstance()
        {
            var instance = OnGetInstance();

            if(instance != null)
            {
                instances.Add(instance);
            }

            return instance;
        }

        protected abstract ResourceInstance OnGetInstance();

        internal void ReturnInstance(ResourceInstance instance)
        {

        }
    }
}
