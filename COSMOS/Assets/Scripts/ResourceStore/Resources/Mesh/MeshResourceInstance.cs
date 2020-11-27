using COSMOS.Core;
using UnityEngine;


namespace COSMOS.ResourceStore
{
    public sealed class MeshResourceInstance : ResourceInstance
    {
        public Mesh MeshInstance { get; private set; }

        internal MeshResourceInstance(MeshResource resource, Mesh meshInstance) : base(resource)
        {
            this.MeshInstance = meshInstance;
        }
        protected override void OnFree()
        {
            UnityThreading.Execute(() =>
            {
                Object.Destroy(MeshInstance);
                MeshInstance = null;
            });
        }
    }

}
