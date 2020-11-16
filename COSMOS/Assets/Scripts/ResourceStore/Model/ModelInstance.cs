using UnityEngine;
using COSMOS.Core;

namespace COSMOS.ResourceStore
{
    public class ModelInstance : ResourceInstance
    {
        private GameObject instance;
        private ModelResource resource;

        internal ModelInstance(ModelResource resource, GameObject instance)
        {
            this.resource = resource;
            this.instance = instance;
        }

        protected override void OnFree()
        {
            if (instance != null)
            {
                UnityThreading.Execute(() => UnityEngine.Object.Destroy(instance));
                instance = null;
            }
        }
    }
}
