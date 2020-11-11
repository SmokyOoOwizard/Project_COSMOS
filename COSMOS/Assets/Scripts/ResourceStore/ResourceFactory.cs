using COSMOS.Core.Config;

namespace COSMOS.ResourceStore
{
    public sealed class ResourceFactory
    {
        public static ResourceFactory Instance { get; private set; }

        static ResourceFactory()
        {
            Instance = new ResourceFactory();
        }

        public bool TryCreateResource(IRecord record, out Resource resource)
        {
            resource = null;
            return false;
        }
    }
}
