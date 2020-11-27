using COSMOS.Core;
using UnityEngine;


namespace COSMOS.ResourceStore
{
    public abstract class MeshResource : Resource
    {
        protected Mesh loadedMesh;

        protected sealed override ResourceInstance OnGetInstance()
        {
            MeshResourceInstance mri = null;
            UnityThreading.WaitExecute(() =>
            {
                var meshClone = Object.Instantiate(loadedMesh);
                meshClone.name = loadedMesh.name;
                mri = new MeshResourceInstance(this, meshClone);
            });
            return mri;
        }

        protected sealed override void OnReturnInstance(ResourceInstance instance)
        {

        }
    }

}
