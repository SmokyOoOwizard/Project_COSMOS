using UnityEngine;
using UnityEngine.SceneManagement;
using Box2DSharp;
using Box2DSharp.Dynamics;
using System;
using Box2DSharp.Collision.Shapes;

namespace COSMOS.Core.Space
{
    public class SolarSystemInstance
    {
        public SolarSystemData Data { get; protected set; }

        internal World PhysicsWorld;

        internal SolarSystemInstance(SolarSystemData systemData)
        {
            PhysicsWorld = new World(new System.Numerics.Vector2(0));

            //var groundBodyDef = new BodyDef { BodyType = BodyType.StaticBody };
            //groundBodyDef.Position = new System.Numerics.Vector2(0.0f, -10.0f);
            //
            //var groundBody = PhysicsWorld.CreateBody(groundBodyDef);
            //
            //var groundBox = new PolygonShape();
            //groundBox.SetAsBox(1000.0f, 10.0f);
            //
            //groundBody.CreateFixture(groundBox, 0.0f);

            Data = systemData;
        }

        public void AttachSpaceObject(SpaceObject spaceObject)
        {
            if (spaceObject != null)
            {
                spaceObject._ChangeSolarSystem(this);
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