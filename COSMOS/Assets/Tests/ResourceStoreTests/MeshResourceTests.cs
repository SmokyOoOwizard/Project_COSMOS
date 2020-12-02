using System.Collections;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using COSMOS.ResourceStore;
using COSMOS.Core;
using UnityEngine.TestTools;
using BestBundle;
using BestBundle.UnityResources;

namespace Tests
{
    public class MeshResourceTests
    {
        private Mesh originalMesh;
        private byte[] savedMeshBundle;

        [UnitySetUp]
        public IEnumerator SetupTests()
        {
            yield return new EnterPlayMode();
            UnityThreading.Init();
            BundleFactory.Instance.SetUnitySynchronizationContext(SynchronizationContext.Current);
        }

        [UnityTest, Order(0)]
        public IEnumerator RuntimeCreateBundleWithMesh()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                var mesh = obj.GetComponent<MeshFilter>().sharedMesh;

                originalMesh = mesh;

                BundleFactory.Instance.CreateBundle(ms, "TestBundle", new Dictionary<string, IBundleEntity>() { { "TestMesh", new MeshEntity(mesh) } });

                savedMeshBundle = ms.ToArray();
            }
            yield return null;
        }

        [UnityTest, Order(1)]
        public IEnumerator MeshLoadAndGetInstnaceTest()
        {
            Assert.IsNotNull(savedMeshBundle);
            Assert.NotZero(savedMeshBundle.Length);
            using (MemoryStream ms = new MemoryStream(savedMeshBundle))
            {
                Assert.IsTrue(Bundle.TryOpenBundle(ms, out Bundle bundle));

                ResourceStore.Instance.AddDB(new BundleResourceDatabase(bundle));

                Assert.IsTrue(ResourceStore.Instance.TryGetResource("TestMesh", out Resource resource));

                Assert.IsNotNull(resource);

                Assert.AreEqual(resource.CurrentStatus, Resource.Status.Unloaded);

                Assert.AreEqual(resource.GetType(), typeof(BundleMeshResource));

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

                var resInstance = resource.GetInstance();

                Assert.IsNotNull(resInstance);

                Assert.AreEqual(resInstance.GetType(), typeof(MeshResourceInstance));

                Assert.AreEqual((resInstance as MeshResourceInstance).MeshInstance.name, originalMesh.name);
            }


            yield return null;
        }
    }
}
