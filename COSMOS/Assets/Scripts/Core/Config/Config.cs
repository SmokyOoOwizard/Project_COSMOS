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
                    var subConfigReader = reader.GetChild(i);
                    IConfig subConfig = null;
                    if(string.IsNullOrEmpty(subConfigReader.Type))
                    {
                        subConfig = new Config();
                    }
                    else
                    {
                        subConfig = new Config();
                        // some check function for need create not default config
                    }

                    if(subConfig == null)
                    {
                        Log.Error("Create config failed.\n" + subConfigReader.GetInfoForError(),
                            "Config", "Parse", "NullException", subConfigReader.Type);
                    }
                    else if (!subConfig.TryParse(subConfigReader))
                    {
                        Log.Error("Failed config parse.\n" + subConfigReader.GetInfoForError(), 
                            "Config", "Parse", subConfigReader.Type);
                    }
                    else if (string.IsNullOrEmpty(subConfig.Name))
                    {
                        Log.Error("Sub config name is null or empty.\n" + subConfigReader.GetInfoForError(),
                            "Config", "Parse", "Name", subConfigReader.Type);
                    }
                    else
                    {
                        args.Add(subConfig.Name, subConfig);
                    }
                }
            }
            return true;
        }

        public static IConfig ParseConfig(IConfigReader reader)
        {
            if(reader == null)
            {
                return null;
            }
            try
            {
                IConfig c = new Config();
                c.TryParse(reader);

                return c;
            }
            catch (Exception ex)
            {
                Log.Error("Config parse error.\n" + ex, "Config", "Parse");
            }
            return null;
        }
    }
}
