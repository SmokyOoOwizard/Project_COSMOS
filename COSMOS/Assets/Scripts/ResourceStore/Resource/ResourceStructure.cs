using COSMOS.Core.Config;

namespace COSMOS.ResourceStore
{
    public abstract class ResourceStructure
    {
        public bool TryParse(IConfig config)
        {
            return _TryParse(config);
        }
        protected abstract bool _TryParse(IConfig config);
    }
}
