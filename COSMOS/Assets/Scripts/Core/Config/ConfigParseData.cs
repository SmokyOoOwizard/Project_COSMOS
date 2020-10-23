namespace COSMOS.Core.Config
{
    public sealed class ConfigParseData
    {
        public string Name { get; private set; }
        public IRecord Record { get; private set; }

        internal ConfigParseData(IRecord record)
        {
            Name = record.Name;
            Record = record;
        }
    }
}
