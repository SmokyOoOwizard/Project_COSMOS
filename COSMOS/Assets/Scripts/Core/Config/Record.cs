using System.Collections.Generic;

namespace COSMOS.Core.Config
{
    public sealed class Record : IRecord, IRecordWithValue
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public IDictionary<string, string> Args { get; private set; } = new Dictionary<string, string>();
    }
}
