 namespace COSMOS.Core.Config
{
    public interface IConfigReader
    {
        bool HasChildren { get; }
        int ChildCount { get; }
        IConfigReader GetChild(int i);

        string GetInfoForError();

        string Type { get; }
        string Name { get; }
        string Value { get; }
    }
}
