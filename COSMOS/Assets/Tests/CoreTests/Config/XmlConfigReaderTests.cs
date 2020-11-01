using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using COSMOS.Core.Config;
using COSMOS.Core;

namespace ConfigTests
{
    public class XmlConfigReaderTests
    {
        [Test]
        public void XmlStringParseTest()
        {
            try
            {
                IConfigReader reader = XmlConfigReader.CreateReader(
                    "<TestName>" +
                    "   <SubConfig>" +
                    "       <SubConfigValue>TestValue</SubConfigValue>" +
                    "   </SubConfig>" +
                    "</TestName>");

                Assert.IsNotNull(reader);
                Assert.AreEqual(reader.Name, "TestName");
                Assert.IsTrue(reader.HasChildren);
                Assert.NotZero(reader.ChildCount);

                var firstChild = reader.GetChild(0);
                Assert.IsNotNull(firstChild);
                Assert.AreEqual(firstChild.Name, "SubConfig");

                var firstChildOfChild = firstChild.GetChild(0);
                Assert.IsNotNull(firstChildOfChild);
                Assert.AreEqual(firstChildOfChild.Value, "TestValue");
            }
            catch (System.Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
        [Test]
        public void XmlAttributeTest()
        {
            try
            {
                IConfigReader reader = XmlConfigReader.CreateReader(
                    "<TestName>" +
                    "   <SubConfig k=\"44\"/>" +
                    "</TestName>");

                Assert.IsNotNull(reader);
                Assert.AreEqual(reader.Name, "TestName");

                Assert.IsNotNull(reader.GetArgs());

                var record = reader.GetChild(0);

                Assert.NotZero(record.GetArgs().Count());
                Assert.IsTrue(record.GetArgs().Any(a => a.Key == "k" && a.Value == "44"));
            }
            catch (System.Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}