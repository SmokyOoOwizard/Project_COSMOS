using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using COSMOS.Core;
using System;
using System.Linq;

namespace ReflectionsBagTests
{
    internal class AttributeForTest : Attribute 
    {
        public string Text;

        public AttributeForTest()
        {

        }
        public AttributeForTest(string text)
        {
            Text = text;
        }
    }
    [AttributeForTest]
    internal class ReflectionTestClass
    {
        [AttributeForTest("Test")]
        internal class InternalReflectionTestClass
        {

        }

        [AttributeForTest]
        public int TestIntField;
        [AttributeForTest("TestText")]
        public int TestIntFieldWithText;

        [AttributeForTest]
        private int testIntField;
        [AttributeForTest("TestText")]
        private int testIntFieldWithText;

        [AttributeForTest]
        public int TestIntProperty { get; private set; }
        [AttributeForTest("TestText")]
        public int TestIntPropertyWithText { get; set; }

        [AttributeForTest]
        private int testIntProperty { get; set; }
        [AttributeForTest("TestText")]
        private int testIntPropertyWithText { get; set; }

        [AttributeForTest]
        public void TestMethod()
        {

        }
        [AttributeForTest("TextTest")]
        public void TestMethodTextAttribute()
        {

        }

        [AttributeForTest]
        public static void StaticTestMethod()
        {

        }
        [AttributeForTest("TextTest")]
        public static void StaticTestMethodTextAttribute()
        {

        }
        
        [AttributeForTest]
        private void privateTestMethod()
        {

        }
        [AttributeForTest("TextTest")]
        private void privateTestMethodTextAttribute()
        {

        }
        
        [AttributeForTest]
        private static void privateStaticTestMethod()
        {

        }
        [AttributeForTest("TextTest")]
        private static void privateStaticTestMethodTextAttribute()
        {

        }
    }
    internal class ReflectionInheritanceTestClass
    {

    }
    [AttributeForTest("Struct")]
    internal struct ReflectionTestStruct
    {

    }

    public class ReflectionsTests
    {
        [SetUp]
        public void PrepareKeeperForTest()
        {
            ReflectionsKeeper.instance.CollectReflections();
        }
        [Test]
        public void CollectTypeReflectionTest()
        {
            var all = ReflectionsKeeper.instance.GetAllWithAttribute<AttributeForTest>();
            Assert.IsTrue(all != null && all.Count > 0);
        }

        [Test]
        public void ClassAttributeTest()
        {
            var all = ReflectionsKeeper.instance.GetAllWithAttribute<AttributeForTest>();

            Assert.IsTrue(all.Any((x) => (x is TypeReflectionInfo) && (x as TypeReflectionInfo).Type == typeof(ReflectionTestClass)));
            Assert.IsTrue(all.Any((x) => (x is TypeReflectionInfo) && (x as TypeReflectionInfo).Type == typeof(ReflectionTestClass.InternalReflectionTestClass)));


        }
        [Test]
        public void StructAttributeTest()
        {
            var all = ReflectionsKeeper.instance.GetAllWithAttributeByCondition<AttributeForTest>((a, mri) => a.Text == "Struct");

            Assert.IsNotNull(all);
            Assert.NotZero(all.Count);

            var s = all[0];

            Assert.IsTrue(s is TypeReflectionInfo);
            Assert.IsTrue(s.ContaintsAttribute<AttributeForTest>());
        }
    }
}
