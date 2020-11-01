using COSMOS.Core.Config;

namespace COSMOS.ResourceStore
{
    public class MaterialResourceStructure : ResourceStructure
    {
        protected override bool _TryParse(IConfig config)
        {
            return false;
        }
    }
}
