using UnityEngine;
using COSMOS.Core;

namespace COSMOS.ResourceStore
{
    public class MaterialResourceInstance : ResourceInstance
    {
        public Material Material { get; protected set; }

        internal MaterialResourceInstance(MaterialResource resource, Material instance) : base(resource)
        {
            Material = instance;
        }

        protected override void OnFree()
        {
            UnityThreading.Execute(() =>
            {
                if (Material != null)
                {
                    Object.Destroy(Material);
                }
            });
        }
    }
}
