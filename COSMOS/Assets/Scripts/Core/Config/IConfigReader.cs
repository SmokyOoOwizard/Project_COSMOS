namespace COSMOS.Core.Config
{
    public interface IConfigReaderErrorInfo
    {
        string GetInfoForError();
    }
    public interface IConfigReader
    {
        bool HasChildren { get; }
        int ChildCount { get; }
        IConfigReader GetChild(int i);

        string Type { get; }
        string Name { get; }
        string Value { get; }
    }
}
