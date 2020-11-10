using System.Collections.Generic;
using COSMOS.Core;
using UnityEngine;

namespace COSMOS.ResourceStore
{
    public abstract class ResourceDatabase
    {
        public string Name { get; private set; }

        public uint ResourceCount { get; }


        public abstract bool TryGetResource(string name, out IBackgroundObjectOperation<Resource> resource);
        public abstract bool TryGetResource<T>(string name, out IBackgroundObjectOperation<T> resource) where T : Resource;
        public abstract bool TryGetGameObject(string id, out IBackgroundObjectOperation<GameObject> operation);

    }
}
