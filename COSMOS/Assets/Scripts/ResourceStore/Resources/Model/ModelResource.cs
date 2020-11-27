using UnityEngine;
using COSMOS.Core;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using BestBundle;

namespace COSMOS.ResourceStore
{
    public abstract class ModelResource : Resource
    {
        protected ModelResourceStructure mrs;

        protected string modelStructureNameId;

        protected MeshResource mesh;

        protected MaterialResource[] materials;

        protected ModelResource[] subModels;

        protected override void tryLoad()
        {
            SetCurrentStatus(Status.Loading);
            Task.Run(loadingMethod);
        }

        private void loadingMethod()
        {
            LoadStructure();


            if (mrs.subModels != null)
            {
                var subModels = new List<ModelResource>();

                for (int i = 0; i < mrs.subModels.Length; i++)
                {
                    string subModelNameId = mrs.subModels[i];

                    if (ResourceStore.Instance.TryGetResource(subModelNameId, out ModelResource subModel))
                    {
                        subModels.Add(subModel);
                    }
                    else
                    {
                        subModels.Clear();
                        SetCurrentStatus(Status.Error);
                        Log.Error($"Sub model failed to load. Model: \"{subModelNameId}\"", "ModelResource", "Resource", "Load");
                    }
                }

                for (int i = 0; i < subModels.Count; i++)
                {
                    LinkResource(subModels[i]);
                }

                this.subModels = subModels.ToArray();
            }

            if (!string.IsNullOrEmpty(mrs.meshId))
            {
                if (ResourceStore.Instance.TryGetResource(mrs.meshId, out MeshResource mesh))
                {
                    this.mesh = mesh;
                    LinkResource(mesh);
                }
                else
                {
                    Log.Error($"Mesh failed to load. Mesh: \"{mrs.meshId}\"", "ModelResource", "Resource", "Load");
                    UnLinkAllResources();
                    SetCurrentStatus(Status.Error);
                    return;
                }
            }

            if (mrs.materialsId != null)
            {
                var materials = new List<MaterialResource>();

                for (int i = 0; i < mrs.materialsId.Length; i++)
                {
                    string matName = mrs.materialsId[i];

                    if (ResourceStore.Instance.TryGetResource(matName, out MaterialResource material))
                    {
                        materials.Add(material);
                        LinkResource(material);
                    }
                    else
                    {
                        Log.Error($"Material failed to load. Material: \"{matName}\"", "ModelResource", "Resource", "Load");
                    }
                }

                this.materials = materials.ToArray();
            }
            TryLoadLinkedResources();
            while (!LinkedResourcesLoaded())
            {
                if (AnyLinkedResourcesWithError())
                {
                    Log.Error("Model resource load failed.", "ModelResource", "Resource", "Load", "WaitLinkedResource", "LinkedResource");
                    SetCurrentStatus(Status.Error);
                    UnLinkAllResources();
                    return;
                }

                Task.Delay(25);
            }

            SetCurrentStatus(Status.Loaded);
        }

        protected abstract void LoadStructure();


        protected override ResourceInstance OnGetInstance()
        {
            ModelResourceInstance[] subModelsInstance = new ModelResourceInstance[subModels.Length];

            for (int i = 0; i < subModels.Length; i++)
            {
                var modelInstance = subModels[i].GetInstance() as ModelResourceInstance;

                if (modelInstance != null)
                {
                    subModelsInstance[i] = modelInstance;
                }
                else
                {
                    Log.Error("Sub model resource return null instance or wrong type", "ModelResource", "SubModel", "GetInstance");
                }
            }

            MaterialResourceInstance[] materialInstance = new MaterialResourceInstance[materials.Length];

            for (int i = 0; i < materialInstance.Length; i++)
            {
                var material = materials[i].GetInstance() as MaterialResourceInstance;

                if (material != null)
                {
                    materialInstance[i] = material;
                }
                else
                {
                    Log.Error("Material resource return null instance or wrong type", "ModelResource", "MaterialResource", "GetInstance");
                }
            }

            GameObject obj = null;


            MeshResourceInstance meshInstance = mesh.GetInstance() as MeshResourceInstance;

            UnityThreading.WaitExecute(() =>
            {
                if (mrs.components != null && mrs.components.Length > 0)
                {
                    obj = new GameObject(mrs.name, mrs.components);
                }
                else
                {
                    obj = new GameObject(mrs.name);
                }



                if (meshInstance != null)
                {
                    var filter = obj.AddComponent<MeshFilter>();
                    filter.sharedMesh = meshInstance.MeshInstance;

                    var renderer = obj.AddComponent<MeshRenderer>();

                    if (materialInstance.Length > 0)
                    {
                        Material[] m = new Material[materialInstance.Length];
                        for (int i = 0; i < materialInstance.Length; i++)
                        {
                            m[i] = materialInstance[i].Material;
                        }

                        renderer.materials = m;
                    }
                }
            });

            return new ModelResourceInstance(this, obj, meshInstance, materialInstance, subModelsInstance);

        }


        protected override void OnReturnInstance(ResourceInstance instance)
        {

        }

        protected override void tryUnload()
        {
            TryUnloadLinkedResources();
        }
    }
    public class BundleModelResource : ModelResource
    {
        private Bundle bundle;

        internal BundleModelResource(Bundle bundle, string modelNameId)
        {
            this.bundle = bundle;
            this.modelStructureNameId = modelNameId;
        }

        protected override void LoadStructure()
        {
            var modelStructure = bundle.GetEntity<ModelStructureBundleEntity>(modelStructureNameId);
            if (modelStructure != null)
            {
                mrs = modelStructure;
            }
        }
    }
}
