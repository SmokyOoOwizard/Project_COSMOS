using UnityEngine;
using UnityEngine.SceneManagement;
using Box2DSharp;
using Box2DSharp.Dynamics;
using System;

using Box2DSharp.Collision.Shapes;

namespace COSMOS.GameWorld.Space
{
    [WorldArtist(typeof(SolarSystemArtist))]
    public class SolarSystemInstance : WorldInstance
    {
        internal World PhysicsWorld;

        public SolarSystemInstance(WorldCreateData createData) : base(createData)
        {
            PhysicsWorld = new World(new System.Numerics.Vector2(0));
        }

        public void AttachSpaceObject(SpaceObject spaceObject)
        {
            if (spaceObject != null)
            {
                if (spaceObject._AttachToSolarSystem(this))
                {
                    Dispatch(WorldInstanceEvents.OnAddObject, spaceObject);
                }
            }
        }
        public void DettachSpaceObject(SpaceObject spaceObject)
        {
            if (spaceObject != null)
            {
                if (spaceObject._DettachFromSolarSystem(this))
                {
                    Dispatch(WorldInstanceEvents.OnRemoveObject, spaceObject);
                }
            }
        }

        protected override void Update(float delta)
        {
            base.Update(delta);

            PhysicsSimulate(delta);
        }

        internal void PhysicsSimulate(float delta)
        {
            PhysicsWorld.Step(delta, 8, 3);
        }

        internal static SolarSystemInstance CreateInstance(WorldCreateData systemData)
        {
            if (systemData == null)
            {
                return null;
            }
            SolarSystemInstance instance = new SolarSystemInstance(systemData);
            return instance;
        }
    }
}