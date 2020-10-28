namespace COSMOS.Core.Config
{
    public interface IRecordWithValue : IRecord
    {
        string Value { get; set; }
    }
}
