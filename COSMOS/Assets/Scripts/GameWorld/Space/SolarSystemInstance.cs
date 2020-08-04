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
        public SolarSystemData Data { get; protected set; }


        internal World PhysicsWorld;

        internal SolarSystemInstance(SolarSystemData systemData)
        {
            PhysicsWorld = new World(new System.Numerics.Vector2(0));

            Data = systemData;
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

        internal void PhysicsSimulate(float delta)
        {
            PhysicsWorld.Step(delta, 8, 3);
        }

        internal static SolarSystemInstance CreateInstance(SolarSystemData systemData)
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