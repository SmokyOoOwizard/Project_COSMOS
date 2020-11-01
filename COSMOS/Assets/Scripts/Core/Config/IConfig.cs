using System.Collections.Generic;

namespace COSMOS.Core.Config
{
    public interface IConfig : IRecord, IEnumerable<IRecord>
    {
        IRecord this[string name]
        {
            get;
            set;
        }

        IConfig GetConfig(string name);
        bool TryGetConfig(string name, out IConfig config);
    }
}
