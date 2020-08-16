using COSMOS.Core;
using System;
using System.Collections.Generic;

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

        private List<Action> updateWorldObjects = new List<Action>();
        private HashSet<WorldObject> worldObjects = new HashSet<WorldObject>();

        public bool Visible { get; internal set; }
        public bool Loaded { get; internal set; }

        protected PhysicsWorld PhysicsWorld;

        protected WorldInstance(WorldCreateData createData)
        {
            CreateData = createData;
        }

        public void AttachWorldObject(WorldObject worldObject)
        {
            if (worldObject != null && worldObject.World == null)
            {
                if (worldObjects.Remove(worldObject))
                {
                    worldObjects.Add(worldObject);
                    return;
                }
                worldObjects.Add(worldObject);
                updateWorldObjects.Add(worldObject._Update);

                worldObject.World = this;

                if (worldObject is IWantPhysicsBody)
                {
                    var iWantBody = worldObject as IWantPhysicsBody;
                    if (PhysicsWorld != null)
                    {
                        if (PhysicsWorld.CorrectPhysicsBodyCreateData(iWantBody.PhysicsBodyCreateData))
                        {
                            iWantBody.PhysicsBody = PhysicsWorld.CreatePhysicsBody(iWantBody.PhysicsBodyCreateData);
                        }
                    }
                }

                OnAttachWorldObject(worldObject);
            }
        }

        protected abstract void OnAttachWorldObject(WorldObject worldObject);

        public void DettachWorldObject(WorldObject worldObject)
        {
            if (worldObject != null && worldObject.World == this)
            {
                worldObjects.Remove(worldObject);
                updateWorldObjects.Remove(worldObject._Update);

                worldObject.World = null;

                if (worldObject is IWantPhysicsBody)
                {
                    var iWantBody = worldObject as IWantPhysicsBody;
                    if (PhysicsWorld != null)
                    {
                        PhysicsWorld.DestroyPhysicsBody(iWantBody.PhysicsBody);
                        iWantBody.PhysicsBody = null;
                    }
                }

                OnDettachObject(worldObject);
            }
        }

        protected abstract void OnDettachObject(WorldObject worldObject);

        internal void _PhysicsUpdate()
        {
            if(PhysicsWorld != null)
            {
                PhysicsWorld.Update(WorldManager.PhysicsDelta, Visible ? PhysicsWorld.UpdateType.Full : PhysicsWorld.UpdateType.LigthWeight);
            }
            PhysicsUpdate(WorldManager.PhysicsDelta);
        }

        protected virtual void PhysicsUpdate(float delta)
        {

        }

        internal void _UpdateWorld()
        {
            var toUpdates = updateWorldObjects.ToArray();
            for (int i = 0; i < toUpdates.Length; i++)
            {
                toUpdates[i].Invoke();
            }
            UpdateWorld(WorldManager.Delta);
        }

        protected virtual void UpdateWorld(float delta)
        {

        }



        public virtual bool CreateDataIsCorrect()
        {
            return true;
        }
    }
}
