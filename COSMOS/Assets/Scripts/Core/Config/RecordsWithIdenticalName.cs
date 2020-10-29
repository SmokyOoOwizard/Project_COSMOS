using System.Collections.Generic;

namespace COSMOS.Core.Config
{
    public sealed class RecordsWithIdenticalName : IRecordsWithIdenticalName
    {
        public string Name { get; set; }

        private readonly List<IRecord> records = new List<IRecord>();

        public void AddRecord(IRecord record)
        {
            records.Add(record);
        }

        public IEnumerable<IRecord> GetRecords()
        {
            return records;
        }

        public bool RemoveRecord(IRecord record)
        {
            return records.Remove(record);
        }
    }
}
