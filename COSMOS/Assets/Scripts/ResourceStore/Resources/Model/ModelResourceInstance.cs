using UnityEngine;
using COSMOS.Core;

namespace COSMOS.ResourceStore
{
    public class ModelResourceInstance : ResourceInstance
    {
        public GameObject Model { get; private set; }
        private MeshResourceInstance meshInstance;
        private MaterialResourceInstance[] materialInstance;
        private ModelResourceInstance[] subModels;

        internal ModelResourceInstance(ModelResource resource, GameObject instance, MeshResourceInstance meshInstance, MaterialResourceInstance[] materialInstance, ModelResourceInstance[] subModels) : base(resource)
        {
            this.Model = instance;

            this.meshInstance = meshInstance;
            this.materialInstance = materialInstance;
            this.subModels = subModels;
        }

        protected override void OnFree()
        {
            if (meshInstance != null)
            {
                meshInstance.Free();
            }

            if (materialInstance != null && materialInstance.Length > 0)
            {
                for (int i = 0; i < materialInstance.Length; i++)
                {
                    materialInstance[i].Free();
                }
            }

            if (subModels != null && subModels.Length > 0)
            {
                for (int i = 0; i < subModels.Length; i++)
                {
                    subModels[i].Free();
                }
            }

            if (Model != null)
            {
                UnityThreading.Execute(() => Object.Destroy(Model));
                Model = null;
            }
        }
    }
}
