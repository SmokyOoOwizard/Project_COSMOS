using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using COSMOS.Core.Config;
using COSMOS.Core;

public class XmlConfigReaderTests
{
    [Test]
    public void XmlStringParseTest()
    {
        try
        {
            IConfigReader reader = XmlConfigReader.CreateReader(
                "<TestName>" +
                "   <SubConfig Type=\"ConfigType\">" +
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
            Assert.AreEqual(firstChild.Type, "ConfigType");

            var firstChildOfChild = firstChild.GetChild(0);
            Assert.IsNotNull(firstChildOfChild);
            Assert.AreEqual(firstChildOfChild.Value, "TestValue");
        }
        catch (System.Exception ex)
        {
            Assert.Fail(ex.ToString());
        }
    }
}
