using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using COSMOS.Core;
using UnityEngine.TestTools;

namespace InitSystemTests
{
    public class InitSystemTests
    {
        private class InitTest
        {
            public static int i = 0;

            [InitMethod("Test", 0)]
            public static void Init() 
            {
                i = 1;
            }
            
            [InitMethod("Test", 1)]
            private static void Init2() 
            {
                if(i == 1)
                {
                    i = 10;
                }
            }
        }

        [OneTimeSetUp]
        public void InitTests() 
        {
            ReflectionsKeeper.instance.CollectReflections();
        }


        [Test]
        public void SimpleInitOrderTest()
        {
            InitSystem.Init("Test");
            Assert.AreEqual(InitTest.i, 10);
        }
    }
}
