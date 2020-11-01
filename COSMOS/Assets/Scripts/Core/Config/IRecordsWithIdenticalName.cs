using System.Collections.Generic;

namespace COSMOS.Core.Config
{
    public interface IRecordsWithIdenticalName : IRecord, IEnumerable<IRecord>
    {
        void AddRecord(IRecord record);
        bool RemoveRecord(IRecord record);
    }
}
