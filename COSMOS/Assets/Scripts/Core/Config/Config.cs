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

        public IEnumerator<IRecord> GetEnumerator()
        {
            return args.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return args.Values.GetEnumerator();
        }
    }
}
