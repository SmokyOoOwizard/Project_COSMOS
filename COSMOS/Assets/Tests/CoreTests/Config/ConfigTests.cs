using NUnit.Framework;
using COSMOS.Core.Config;

namespace ConfigTests
{
    public class ConfigTests
    {
        [Test]
        public void ConfigParseTest()
        {
            try
            {
                IConfigReader reader = XmlConfigReader.CreateReader(
                        "<TestName>" +
                        "   <SubConfig Type=\"ConfigType\">" +
                        "       <SubConfigValue>TestValue</SubConfigValue>" +
                        "   </SubConfig>" +
                        "</TestName>");

                var config = Config.ParseConfig(reader);

                Assert.IsNotNull(config);
                Assert.AreEqual(config.Name, "TestName");

                var subConfig = config["SubConfig"];
                Assert.IsNotNull(subConfig);

                var subSubConfig = subConfig["SubConfigValue"];
                Assert.IsNotNull(subSubConfig);
                Assert.AreEqual(subSubConfig.Value, "TestValue");

                Assert.IsNull(subSubConfig["NullObject"]);

            }
            catch (System.Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}