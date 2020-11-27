using System.Collections.Generic;
using COSMOS.Core;
using UnityEngine;

namespace COSMOS.ResourceStore
{
    public abstract class AbstractResourceDatabase
    {
        public string Name { get; private set; }

        public uint ResourceCount { get; }


        public abstract bool TryGetResource(string name, out Resource resource);
        public abstract bool TryGetResource<T>(string name, out T resource) where T : Resource;
    }

}
