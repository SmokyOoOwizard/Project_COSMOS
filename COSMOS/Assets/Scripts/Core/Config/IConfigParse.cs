namespace COSMOS.Core.Config
{
    public interface IConfigParse
    {
        bool TryParse(ConfigParseData data);
    }
}
