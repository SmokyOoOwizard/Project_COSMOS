using System.Collections.Generic;
using BestBundle;
using BestBundle.UnityResources;
using System;
using System.Reflection;

namespace COSMOS.ResourceStore
{
    public sealed class BundleResourceFactory
    {
        public static BundleResourceFactory Instance { get; private set; }

        private readonly Dictionary<Type, Type> resourceTypesByBundleEntitiesTypes = new Dictionary<Type, Type>();

        static BundleResourceFactory()
        {
            Instance = new BundleResourceFactory();
        }

        private BundleResourceFactory()
        {
            AddResourceType<MeshEntity, BundleMeshResource>();
            AddResourceType<ModelStructureBundleEntity, BundleModelResource>();

            //

            BundleFactory.Instance.AddEntityType<ModelStructureBundleEntity>();
        }

        public void AddResourceType<EntityType, ResourceType>() where EntityType : IBundleEntity, new() where ResourceType : Resource
        {
            resourceTypesByBundleEntitiesTypes.Add(typeof(EntityType), typeof(ResourceType));
        }

        public bool TryCreateResource(Bundle bundle, string nameId, out Resource resource)
        {
            try
            {
                if (bundle.EntityDatabase.ContainsEntity(nameId))
                {
                    var bundleEntityType = bundle.EntityDatabase.GetEntityType(nameId);
                    if (bundleEntityType != null)
                    {
                        if (resourceTypesByBundleEntitiesTypes.TryGetValue(bundleEntityType, out Type resourceType))
                        {
                            var ctor = resourceType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

                            var res = ctor[0].Invoke(new object[] { bundle, nameId });
                            resource = res as Resource;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Create resource error:\"{ex}\"", "BestBundle", "ResourceFactory");
            }
            resource = null;
            return false;
        }
        public bool TryCreateResource<T>(Bundle bundle, string nameId, out T resource) where T : Resource
        {
            try
            {
                if (bundle.EntityDatabase.ContainsEntity(nameId))
                {
                    var bundleResourceType = bundle.EntityDatabase.GetEntityType(nameId);
                    if (bundleResourceType != null)
                    {
                        if (resourceTypesByBundleEntitiesTypes.TryGetValue(bundleResourceType, out Type resourceType))
                        {
                            if (resourceType == typeof(T) || resourceType.IsSubclassOf(typeof(T)))
                            {
                                var ctor = resourceType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

                                var res = ctor[0].Invoke(new object[] { bundle, nameId }) as T;
                                resource = res;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Create resource error:\"{ex}\"", "BestBundle", "ResourceFactory");
            }
            resource = null;
            return false;
        }
    }

}
