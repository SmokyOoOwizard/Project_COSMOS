using System.Collections.Generic;

namespace COSMOS.Core.Config
{
    public interface IRecord
    {
        string Name { get; set; }

        IDictionary<string, string> Args { get; }
    }
}
