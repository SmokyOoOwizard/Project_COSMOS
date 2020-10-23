using NUnit.Framework;
using COSMOS.Core.Config;
using System.Collections.Generic;

namespace ConfigTests
{
    public class ConfigTests
    {
        private class ConfigCustomParseTestClass : IConfig, IConfigParse
        {
            public string Name { get; set; }

            public IRecord this[string name]
            {
                get
                {
                    if (args.TryGetValue(name, out IRecord arg))
                    {
                        return arg;
                    }
                    return default;
                }
                set
                {
                    args[name] = value;
                }
            }

            private readonly Dictionary<string, IRecord> args = new Dictionary<string, IRecord>();

            public IConfig GetConfig(string name)
            {
                if (args.TryGetValue(name, out IRecord record))
                {
                    return record as IConfig;
                }
                return null;
            }

            public bool TryGetConfig(string name, out IConfig config)
            {
                config = null;
                if (args.TryGetValue(name, out IRecord record))
                {
                    if (record is IConfig)
                    {
                        config = record as IConfig;
                        return true;
                    }
                }
                return false;
            }

            public bool TryParse(ConfigParseData data)
            {
                args.Add(data.Name, data.Record);
                return true;
            }
        }

        [Test]
        public void CustomConfigParseTypeTest()
        {
            try
            {
                IConfigReader reader = XmlConfigReader.CreateReader(
                        "<TestName Type=\"CustomConfigForTestCustomParse\">" +
                        "   <Record>TestValue</Record>" +
                        "</TestName>");

                var config = ConfigFactory.Factory.ReadConfig(reader);

                Assert.IsNotNull(config);
                Assert.AreEqual(config.Name, "TestName");

                Assert.IsTrue(config is ConfigCustomParseTestClass);

                Assert.IsNotNull(config["Record"]);
                Assert.IsNotNull(config["Record"] as IRecordWithValue);
                Assert.AreEqual((config["Record"] as IRecordWithValue).Value, "TestValue");
            }
            catch (System.Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [Test]
        public void ConfigParseTest()
        {
            try
            {
                IConfigReader reader = XmlConfigReader.CreateReader(
                        "<TestName>" +
                        "   <SubConfig>" +
                        "       <SubConfigValue>TestValue</SubConfigValue>" +
                        "   </SubConfig>" +
                        "</TestName>");

                var config = ConfigFactory.Factory.ReadConfig(reader);

                Assert.IsNotNull(config);
                Assert.AreEqual(config.Name, "TestName");

                var subConfig = config.GetConfig("SubConfig");
                Assert.IsNotNull(subConfig);

                var subSubConfig = subConfig["SubConfigValue"];
                Assert.IsNotNull(subSubConfig);
                Assert.IsNotNull(subSubConfig as IRecordWithValue);
                Assert.AreEqual((subSubConfig as IRecordWithValue).Value, "TestValue");

                Assert.IsNull(subConfig["NullObject"]);

            }
            catch (System.Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}