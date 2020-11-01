using NUnit.Framework;
using COSMOS.Core.Config;
using System.Collections.Generic;
using COSMOS.Core;
using System.Linq;
using System.Collections;

namespace ConfigTests
{
    public class ConfigTests
    {
        [Config("CustomConfigForTestCustomParse")]
        private class ConfigCustomParseTestClass : IConfig, IConfigParse
        {
            public string Name { get; set; }

            public IDictionary<string, string> Args { get; private set; } = new Dictionary<string, string>();

            public IRecord this[string name]
            {
                get
                {
                    if (records.TryGetValue(name, out IRecord arg))
                    {
                        return arg;
                    }
                    return default;
                }
                set
                {
                    records[name] = value;
                }
            }

            private readonly Dictionary<string, IRecord> records = new Dictionary<string, IRecord>();

            public IConfig GetConfig(string name)
            {
                if (records.TryGetValue(name, out IRecord record))
                {
                    return record as IConfig;
                }
                return null;
            }

            public bool TryGetConfig(string name, out IConfig config)
            {
                config = null;
                if (records.TryGetValue(name, out IRecord record))
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
                records.Add(data.Name, data.Record);
                return true;
            }

            public IEnumerator<IRecord> GetEnumerator()
            {
                return records.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return records.Values.GetEnumerator();
            }
        }

        [OneTimeSetUp]
        public void CollectCustomConfigsInfo()
        {
            ReflectionsKeeper.instance.CollectReflections();
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

        [Test]
        public void ConfigParseRecordsWithIdenticalNameTest()
        {
            try
            {
                IConfigReader reader = XmlConfigReader.CreateReader(
                        "<TestName>" +
                        "   <SubConfig>test1</SubConfig>" +
                        "   <SubConfig>test2</SubConfig>" +
                        "</TestName>");

                var config = ConfigFactory.Factory.ReadConfig(reader);

                Assert.IsNotNull(config);
                Assert.AreEqual(config.Name, "TestName");

                var container = config["SubConfig"];
                Assert.IsNotNull(container);

                Assert.IsTrue(container is IRecordsWithIdenticalName);

                var recordsContainer = container as IRecordsWithIdenticalName;

                Assert.IsTrue(recordsContainer.Any(r => r is IRecordWithValue && (r as IRecordWithValue).Value == "test1"));
                Assert.IsTrue(recordsContainer.Any(r => r is IRecordWithValue && (r as IRecordWithValue).Value == "test2"));
            }
            catch (System.Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [Test]
        public void ConfigForeachTest()
        {
            try
            {
                IConfigReader reader = XmlConfigReader.CreateReader(
                        "<TestName>" +
                        "   <SubConfig>231</SubConfig>" +
                        "   <SubConfig>241</SubConfig>" +
                        "   <SubConfigValue>TestValue</SubConfigValue>" +
                        "</TestName>");

                var config = ConfigFactory.Factory.ReadConfig(reader);

                Assert.IsNotNull(config);
                Assert.AreEqual(config.Name, "TestName");

                Assert.IsTrue(config.Any(r => r.Name == "SubConfig" && r is IRecordsWithIdenticalName && (r as IRecordsWithIdenticalName).Any(v => v is IRecordWithValue && (v as IRecordWithValue).Value == "231")));
                Assert.IsTrue(config.Any(r => r.Name == "SubConfig" && r is IRecordsWithIdenticalName && (r as IRecordsWithIdenticalName).Any(v => v is IRecordWithValue && (v as IRecordWithValue).Value == "241")));
                Assert.IsTrue(config.Any(r => r.Name == "SubConfigValue" && r is IRecordWithValue && (r as IRecordWithValue).Value == "TestValue"));
            }
            catch (System.Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
        [Test]
        public void ConfigAttributeTest()
        {
            try
            {
                IConfigReader reader = XmlConfigReader.CreateReader(
                        "<TestName>" +
                        "   <SubConfig k=\"44\">231</SubConfig>" +
                        "</TestName>");

                var config = ConfigFactory.Factory.ReadConfig(reader);

                Assert.IsNotNull(config);
                Assert.AreEqual(config.Name, "TestName");

                var record = config["SubConfig"];
                Assert.NotZero(record.Args.Count);

                Assert.IsNotNull(record.Args["k"]);

                Assert.AreEqual(record.Args["k"], "44");

            }
            catch (System.Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}