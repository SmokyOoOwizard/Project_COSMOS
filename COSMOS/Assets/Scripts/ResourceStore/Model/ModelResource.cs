using UnityEngine;
using COSMOS.Core;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace COSMOS.ResourceStore
{
    public class ModelResource : Resource
    {
        private List<(ModelLODResourceStructure, GameObject)> loadedModelLODs = new List<(ModelLODResourceStructure, GameObject)>();

        private ModelResourceStructure structure;
        protected override ResourceInstance OnGetInstance()
        {
            ModelInstance instance = null;
            UnityThreading.WaitExecute(() =>
            {
                GameObject instanceGO = createModelGameObjectInstance();
                instance = new ModelInstance(this, instanceGO);
            });
            return instance;
        }

        private GameObject createModelGameObjectInstance()
        {
            GameObject go = new GameObject();

            var lodsGroup = go.AddComponent<LODGroup>();
            LOD[] lods = new LOD[loadedModelLODs.Count];

            for (int i = 0; i < loadedModelLODs.Count; i++)
            {
                var lodStructure = loadedModelLODs[i].Item1;
                var lodModel = loadedModelLODs[i].Item2;

                var lodModelInstance = GameObject.Instantiate(lodModel);

                lodModelInstance.transform.SetParent(go.transform);
                lodModelInstance.transform.rotation = Quaternion.identity;
                lodModelInstance.transform.localScale = Vector3.one;
                lodModelInstance.transform.localPosition = Vector3.zero;

                var lodModelRenders = lodModelInstance.GetComponentsInChildren<Renderer>();

                //lods[i] = new LOD(1 / (i + 1), lodModelRenders);
                lods[i] = new LOD((lods.Length - i - 1) / (float)lods.Length, lodModelRenders);
                //lods[i] = new LOD((lods.Length - 1 - i) / lods.Length, lodModelRenders);
            }
            lodsGroup.SetLODs(lods);

            return go;
        }

        protected override void OnReturnInstance(ResourceInstance instance)
        {
            instance.InternalFree();
        }

        protected override void tryLoad()
        {
            UnityThreading.StartCoroutine(loading());
        }

        private IEnumerator loading()
        {
            var lods = structure.detailLevels[0].LODS;

            SetCurrentStatus(Status.Loading);

            List<(ModelLODResourceStructure, IBackgroundObjectOperation)> requests = new List<(ModelLODResourceStructure, IBackgroundObjectOperation)>();

            for (int i = 0; i < lods.Count; i++)
            {
                var lod = lods[i];
                string lodId = lod.meshPath;
                Log.Info($"Try load model:\"{Name}\" LOD {i}:\"{lodId}\"", "Resource", "Load");

                if (ResourceStore.Instance.TryGetGameObject(lodId, out IBackgroundObjectOperation<GameObject> operation))
                {
                    requests.Add((lod, operation));
                }
                else
                {
                    Log.Error($"LOD of model:\"{Name}\" not found:\"{lodId}\"", "Resource", "Load");
                }
            }

            if (requests.Count > 0)
            {
                List<(ModelLODResourceStructure, GameObject)> loadedLODs = new List<(ModelLODResourceStructure, GameObject)>();

                while (true)
                {
                    for (int i = 0; i < requests.Count; i++)
                    {
                        var r = requests[i];
                        if (r.Item2.IsDone)
                        {
                            if (r.Item2.Object != null)
                            {
                                if (r.Item2.Object is GameObject)
                                {
                                    Log.Info($"Load model LOD success:\"{r.Item1.meshPath}\"", "Resource", "Load");
                                    loadedLODs.Add((r.Item1, r.Item2.Object as GameObject));
                                    requests.RemoveAt(i);
                                    break;
                                }
                                else
                                {
                                    Log.Error($"LOD of model {Name} not GameObject. " + r.Item2.Object.GetType(), "Resource", "Load");
                                }
                            }
                            else
                            {
                                Log.Error($"LOD of model {Name} null.", "Resource", "Load");
                            }
                        }
                    }
                    if (requests.Count == 0)
                    {
                        break;
                    }
                    yield return new WaitForEndOfFrame();
                }

                loadedModelLODs = loadedLODs;
                SetCurrentStatus(Status.Loaded);
            }
            else
            {
                SetCurrentStatus(Status.Error);
                Log.Error($"Model with 0 lods!!! Model:\"{Name}\"");
            }
            yield return null;
        }

        protected override bool TryParseResourceStucture(ResourceStructure structure)
        {
            if (!(structure is ModelResourceStructure))
            {
                return false;
            }

            this.structure = structure as ModelResourceStructure;
            return true;
        }

        protected override void tryUnload()
        {
            SetCurrentStatus(Status.Unloading);
            loadedModelLODs.Clear();
            SetCurrentStatus(Status.Unloaded);
        }
    }
}
