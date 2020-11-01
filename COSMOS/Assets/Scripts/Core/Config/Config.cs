using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace COSMOS.Core.Config
{
    public sealed class Config : IConfig
    {
        public string Name { get; set; }


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

        public IDictionary<string, string> Args { get; private set; } = new Dictionary<string, string>();

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

        public IEnumerator<IRecord> GetEnumerator()
        {
            return records.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return records.Values.GetEnumerator();
        }
    }
}
