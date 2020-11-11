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

        private readonly List<AbstractResourceDatabase> connectedDB = new List<AbstractResourceDatabase>();


        private readonly List<IGameObjectDatabase> gameObjectDB = new List<IGameObjectDatabase>();
        /// <summary>
        /// texture
        /// sprite
        /// audio clip
        /// material
        /// text
        /// 
        /// </summary>

        private object dbLock = new object();

        public bool AddDB(AbstractResourceDatabase database)
        {
            lock (dbLock)
            {
                if (connectedDB.Contains(database))
                {
                    return false;
                }

                connectedDB.Add(database);

                if (database is IGameObjectDatabase)
                {
                    gameObjectDB.Add(database as IGameObjectDatabase);
                }
                return true;
            }
        }
        public AbstractResourceDatabase GetDB(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            for (int i = 0; i < connectedDB.Count; i++)
            {
                if (connectedDB[i].Name == name)
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
                if (gameObjectDB[i].TryGetGameObject(name, out IBackgroundObjectOperation<GameObject> op))
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
