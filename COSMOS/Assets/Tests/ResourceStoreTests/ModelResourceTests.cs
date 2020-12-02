using BestBundle;
using BestBundle.UnityResources;
using COSMOS.Core;
using COSMOS.ResourceStore;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ModelResourceTests
    {
        private byte[] savedMeshBundle;

        [UnitySetUp]
        public IEnumerator SetupTests()
        {
            yield return new EnterPlayMode();
            UnityThreading.Init();
            BundleFactory.Instance.SetUnitySynchronizationContext(SynchronizationContext.Current);
        }

        [UnityTest, Order(0)]
        public IEnumerator RuntimeCreateBundleWithModel()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

                var mesh = obj.GetComponent<MeshFilter>().sharedMesh;

                mesh.name = "CubeMesh";

                ModelStructureBundleEntity msbe = new ModelStructureBundleEntity();

                msbe.meshId = "TestMesh";
                msbe.name = "Cube";

                BundleFactory.Instance.CreateBundle(ms, "TestBundle", new Dictionary<string, IBundleEntity>() { { "TestMesh", new MeshEntity(mesh) }, {"TestModel", msbe } });

                savedMeshBundle = ms.ToArray();
            }
            yield return null;
        }

        [UnityTest, Order(1)]
        public IEnumerator ModelLoadAndGetInstnaceTest()
        {
            Assert.IsNotNull(savedMeshBundle);
            Assert.NotZero(savedMeshBundle.Length);
            using (MemoryStream ms = new MemoryStream(savedMeshBundle))
            {
                Assert.IsTrue(Bundle.TryOpenBundle(ms, out Bundle bundle));

                ResourceStore.Instance.AddDB(new BundleResourceDatabase(bundle));

                Assert.IsTrue(ResourceStore.Instance.TryGetResource("TestModel", out Resource resource));

                Assert.IsNotNull(resource);

                Assert.AreEqual(resource.CurrentStatus, Resource.Status.Unloaded);

                Assert.IsTrue(resource is ModelResource);

                resource.TryLoad();

                if (resource.CurrentStatus != Resource.Status.Loaded)
                {
                    var time = 10f;
                    while (time > 0)
                    {
                        time -= Time.deltaTime;
                        Assert.IsFalse(resource.CurrentStatus == Resource.Status.Error);
                        if (resource.CurrentStatus == Resource.Status.Loaded)
                        {
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }

                    Assert.AreEqual(resource.CurrentStatus, Resource.Status.Loaded);
                }

                var resourceInstance = resource.GetInstance();

                Assert.IsNotNull(resourceInstance);

                Assert.AreEqual(typeof(ModelResourceInstance), resourceInstance.GetType());

                var modelInstance = resourceInstance as ModelResourceInstance;

                Assert.IsNotNull(modelInstance.Model);

                Assert.AreEqual("Cube", modelInstance.Model.name);

                var meshFilter = modelInstance.Model.GetComponent<MeshFilter>();

                Assert.IsNotNull(meshFilter);

                Assert.IsNotNull(meshFilter.sharedMesh);

                Assert.AreEqual("CubeMesh", meshFilter.sharedMesh.name);

            }

            yield return null;
        }
    }
}
