using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace COSMOS.Core.Config
{
    public class Config : IConfig
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public IConfig this[string name]
        {
            get
            {
                if (args.TryGetValue(name, out IConfig arg))
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

        private readonly Dictionary<string, IConfig> args = new Dictionary<string, IConfig>();

        bool IConfig.TryParse(IConfigReader reader)
        {
            if (string.IsNullOrEmpty(reader.Name))
            {
                return false;
            }

            Name = reader.Name;
            Value = reader.Value;

            if (reader.HasChildren)
            {
                for (int i = 0; i < reader.ChildCount; i++)
                {
                    var childReader = reader.GetChild(i);
                    IConfig c = null;
                    if(string.IsNullOrEmpty(childReader.Type))
                    {
                        c = new Config();
                    }
                    else
                    {
                        // some check function for need create not default config
                    }
                    if(c == null)
                    {
                        Log.Error("Create config failed.\n" + childReader.GetInfoForError(),
                            "Config", "Parse", childReader.Type);
                    }
                    else if (!c.TryParse(childReader))
                    {
                        Log.Error("Failed config parse.\n" + childReader.GetInfoForError(), 
                            "Config", "Parse", childReader.Type);
                    }
                    else
                    {
                        args.Add(c.Name, c);
                    }
                }
            }
            return true;
        }
    }
}
