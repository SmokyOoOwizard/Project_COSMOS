using System.Collections.Generic;
using UnityEngine;

using COSMOS.Core;
using System;

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

        private readonly List<AbstractResourceDatabase> databasesWithSomeResource = new List<AbstractResourceDatabase>();

        private readonly Dictionary<Type, List<AbstractResourceDatabase>> databasesByContaintsType = new Dictionary<Type, List<AbstractResourceDatabase>>();

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
                if (database is IHasContainsTypes)
                {
                    var types = (database as IHasContainsTypes).GetContainsTypes();
                    for (int i = 0; i < types.Length; i++)
                    {
                        var type = types[i];
                        if (databasesByContaintsType.TryGetValue(type, out List<AbstractResourceDatabase> dbList))
                        {
                            dbList.Add(database);
                        }
                        else
                        {
                            databasesByContaintsType[type] = new List<AbstractResourceDatabase>() { database };
                        }
                    }
                }
                else if (database is IUnknowContainsTypes)
                {
                    databasesWithSomeResource.Add(database);
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

        public bool TryGetResource(string name, out Resource operation)
        {
            for (int i = 0; i < connectedDB.Count; i++)
            {
                if (connectedDB[i].TryGetResource(name, out Resource op))
                {
                    operation = op;
                    return true;
                }
            }
            operation = null;
            return false;
        }
        public bool TryGetResource<T>(string name, out T operation) where T : Resource
        {
            List<AbstractResourceDatabase> dbList = null;

            var resourceType = typeof(T);
            foreach (var item in databasesByContaintsType)
            {
                if (resourceType.IsSubclassOf(item.Key) || resourceType == item.Key)
                {
                    dbList = item.Value;
                    break;
                }
            }

            if (dbList != null)
            {
                for (int i = 0; i < dbList.Count; i++)
                {
                    if (dbList[i].TryGetResource<T>(name, out T op))
                    {
                        operation = op;
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < databasesWithSomeResource.Count; i++)
                {
                    if (databasesWithSomeResource[i].TryGetResource<T>(name, out T op))
                    {
                        operation = op;
                        return true;
                    }
                }
            }
            operation = null;
            return false;
        }
    }
}
