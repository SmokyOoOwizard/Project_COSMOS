using COSMOS.Core;
using System.Threading.Tasks;
using BestBundle;
using UnityEngine;
using BestBundle.UnityResources;

namespace COSMOS.ResourceStore
{
    public sealed class BundleMeshResource : MeshResource
    {
        private Bundle bundle;
        private string meshNameId;

        private Task resourceTask;

        internal BundleMeshResource(Bundle bundle, string meshNameId)
        {
            this.bundle = bundle;
            this.meshNameId = meshNameId;
        }

        protected override void tryLoad()
        {
            if (bundle.EntityDatabase.ContainsEntity(meshNameId))
            {
                if (bundle.EntityDatabase.GetEntityType(meshNameId) == typeof(MeshEntity))
                {
                    SetCurrentStatus(Status.Loading);
                    resourceTask = Task.Run(() =>
                    {
                        try
                        {
                            var mesh = bundle.GetEntity<MeshEntity>(meshNameId);
                            if (mesh != null)
                            {
                                loadedMesh = mesh.Mesh;
                                SetCurrentStatus(Status.Loaded);
                            }
                            else
                            {
                                Log.Error($"Bundle mesh resource:\"{meshNameId}\" was loaded like null", "BestBundle", "Mesh", "Resource");
                                SetCurrentStatus(Status.Error);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error($"While bundle mesh resource:\"{meshNameId}\" try load was error: " + ex, "BestBundle", "Mesh", "Resource");
                            SetCurrentStatus(Status.Error);
                        }
                    });
                }
            }
        }

        protected override void tryUnload()
        {
            if (loadedMesh != null)
            {
                SetCurrentStatus(Status.Unloading);
                UnityThreading.Execute(() =>
                {
                    try
                    {
                        Object.Destroy(loadedMesh);
                        SetCurrentStatus(Status.Unloaded);
                    }
                    catch (System.Exception ex)
                    {
                        SetCurrentStatus(Status.Error);
                        Log.Error($"While bundle mesh resource:\"{meshNameId}\" try unload was error: " + ex, "BestBundle", "Mesh", "Resource");
                    }
                });
            }
        }
    }

}
