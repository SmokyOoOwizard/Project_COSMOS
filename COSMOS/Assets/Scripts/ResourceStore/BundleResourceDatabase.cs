using COSMOS.Core.Config;
using System.Collections.Generic;
using BestBundle;
using System;

namespace COSMOS.ResourceStore
{
    public sealed class BundleResourceDatabase : AbstractResourceDatabase, IHasContainsTypes
    {
        private Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

        private Bundle bundle;

        public BundleResourceDatabase(Bundle bundle)
        {
            this.bundle = bundle;
        }

        public Type[] GetContainsTypes()
        {
            var bundleTypes = bundle.EntityDatabase.GetEntityTypes();

            List<Type> types = new List<Type>();

            foreach (var type in bundleTypes)
            {
                switch (type)
                {
                    case "Mesh":
                        types.Add(typeof(MeshResource));
                        break;
                    case "ModelStructure":
                        types.Add(typeof(ModelResource));
                        break;
                }
            }

            return types.ToArray();
        }

        public override bool TryGetResource(string name, out Resource resource)
        {
            if (resources.TryGetValue(name, out Resource res))
            {
                resource = res;
                return true;
            }
            else
            {
                if (BundleResourceFactory.Instance.TryCreateResource(bundle, name, out Resource createdResource))
                {
                    resources.Add(name, createdResource);
                    resource = createdResource;
                    return true;
                }
            }
            resource = null;
            return false;
        }
        public override bool TryGetResource<T>(string name, out T resource)
        {
            if (resources.TryGetValue(name, out Resource res))
            {
                if (res is T)
                {
                    resource = res as T;
                    return true;
                }
                else
                {
                    Log.Warning($"Bundle resource database cache contains resource with need name, but different type. contains type\"{res.GetType().FullName}\" need type\"{typeof(T).FullName}\"", "BestBundle", "ResourceDatabase");
                }
            }
            else
            {
                if (BundleResourceFactory.Instance.TryCreateResource<T>(bundle, name, out T createdResource))
                {
                    resources.Add(name, createdResource);
                    resource = createdResource;
                    return true;
                }
            }
            resource = default(T);
            return false;
        }
    }

}
