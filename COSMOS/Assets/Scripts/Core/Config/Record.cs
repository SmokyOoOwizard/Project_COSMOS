namespace COSMOS.Core.Config
{
    public sealed class Record : IRecord, IRecordWithValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
