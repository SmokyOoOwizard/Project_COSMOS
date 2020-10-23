namespace COSMOS.Core.Config
{
    public interface IConfig : IRecord
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
