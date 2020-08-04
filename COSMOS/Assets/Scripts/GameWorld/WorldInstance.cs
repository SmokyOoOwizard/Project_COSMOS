using COSMOS.Core;

namespace COSMOS.GameWorld
{
    public abstract class WorldInstance : EventDispatcher<WorldInstance.WorldInstanceEvents>
    {
        public enum WorldInstanceEvents
        {
            OnAddObject,
            OnRemoveObject,
            OnWorldVisible
        }
    }
}
