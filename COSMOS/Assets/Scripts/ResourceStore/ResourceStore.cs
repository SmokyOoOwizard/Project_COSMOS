using System.Collections.Generic;
using UnityEngine;

using COSMOS.Core;

namespace COSMOS.ResourceStore
{
    public sealed class ResourceStore
    {
        public static ResourceStore Instance { get; private set; }

        static ResourceStore()
        {
            Instance = new ResourceStore();
        }

        private readonly List<ResourceDatabase> connectedDB = new List<ResourceDatabase>();

        private object dbLock = new object();

        public bool AddDB(ResourceDatabase database)
        {
            lock (dbLock)
            {
                if (connectedDB.Contains(database))
                {
                    return false;
                }

                connectedDB.Add(database);

                return true;
            }
        }
        public ResourceDatabase GetDB(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            for (int i = 0; i < connectedDB.Count; i++)
            {
                if(connectedDB[i].Name == name)
                {
                    return connectedDB[i];
                }
            }
            return null;
        }

        public bool TryGetResource(string name, out IBackgroundObjectOperation<Resource> operation)
        {
            for (int i = 0; i < connectedDB.Count; i++)
            {
                if (connectedDB[i].TryGetResource(name, out IBackgroundObjectOperation<Resource> op))
                {
                    operation = op;
                    return true;
                }
            }
            operation = null;
            return false;
        }
        public bool TryGetResource<T>(string name, out IBackgroundObjectOperation<T> operation) where T : Resource
        {
            for (int i = 0; i < connectedDB.Count; i++)
            {
                if (connectedDB[i].TryGetResource<T>(name, out IBackgroundObjectOperation<T> op))
                {
                    operation = op;
                    return true;
                }
            }
            operation = null;
            return false;
        }

        public bool TryGetGameObject(string name, out IBackgroundObjectOperation<GameObject> operation)
        {
            for (int i = 0; i < connectedDB.Count; i++)
            {
                if(connectedDB[i].TryGetGameObject(name, out IBackgroundObjectOperation<GameObject> op))
                {
                    operation = op;
                    return true;
                }
            }
            operation = null;
            return false;
        }
    }
}
