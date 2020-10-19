namespace COSMOS.Core.Config
{
    public interface IConfig
    {
        string Name { get; set; }
        string Value { get; set; }

        bool TryParse(IConfigReader reader);

        IConfig this[string name]
        {
            get;
            set;
        }
    }
}
