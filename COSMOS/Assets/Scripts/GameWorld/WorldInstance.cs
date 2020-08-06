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

        public readonly WorldCreateData CreateData;

        protected WorldInstance(WorldCreateData createData)
        {
            CreateData = createData;
        }

        internal void _Update(float delta)
        {
            Update(delta);
        }
        protected virtual void Update(float delta)
        {

        }

        public virtual bool CreateDataIsCorrect()
        {
            return true;
        }
    }
}
