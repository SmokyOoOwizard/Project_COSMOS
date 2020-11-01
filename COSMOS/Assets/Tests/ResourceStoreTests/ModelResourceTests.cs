using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using COSMOS.ResourceStore;
using COSMOS.Core;
using UnityEngine.TestTools;
using COSMOS.Core.Config;

namespace Tests
{
    public class ModelResourceTests
    {
        [UnitySetUp]
        private IEnumerator SetupTests()
        {
            yield return new EnterPlayMode();
            UnityThreading.Init();
        }

        [UnityTest]
        public IEnumerator ModelLoadAndGetInstnaceTest()
        {
            IConfigReader reader = XmlConfigReader.CreateReader(
                        "<Model>" +
                        "   <Detail>" +
                        "       <LOD>Models/Wraith Raider Starship</LOD>" +
                        "   </Detail>" +
                        "</Model>");

            var config = ConfigFactory.Factory.ReadConfig(reader);

            ModelResourceStructure mrs = new ModelResourceStructure();

            Assert.IsTrue(mrs.TryParse(config));
            ModelResource mr = new ModelResource();

            Assert.IsTrue(mr._TryParseResourceStructure(mrs));

            Assert.AreEqual(mr.CurrentStatus, Resource.Status.Unloaded);

            mr.TryLoad();

            yield return new WaitForSecondsRealtime(5);

            if (mr.CurrentStatus == Resource.Status.Loading)
            {
                yield return new WaitForSecondsRealtime(10);
            }
            Assert.AreEqual(mr.CurrentStatus, Resource.Status.Loaded);

            var modelInstance = mr.GetInstance();

            Assert.IsNotNull(modelInstance);


            yield return new WaitForSecondsRealtime(1);
            modelInstance.Free();


            yield return null;
        }
    }
}
