using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace COSMOS.GameWorld.Space
{
    public class PhysicsBodyConstructor : ICloneable
    {
        public BodyDef BodyDef;
        public List<FixtureDef> FixturesDef = new List<FixtureDef>();

        public PhysicsBodyConstructor()
        {
            BodyDef.Enabled = true;
            BodyDef.Awake = true;
            BodyDef.BodyType = BodyType.DynamicBody;
            BodyDef.AllowSleep = false;
            BodyDef.Position = new Vector2(0, 0f);

            var dynamicBox = new PolygonShape();
            dynamicBox.SetAsBox(1f, 1f, Vector2.Zero, 0);

            // Define the dynamic body fixture.
            var fixtureDef = new FixtureDef
            {
                Shape = dynamicBox,
                Density = 1.0f,
                Friction = 0.3f
            };

            FixturesDef.Add(fixtureDef);
        }
        public Body CreateBody(World world)
        {
            var body = world.CreateBody(BodyDef);
            if (FixturesDef != null)
            {
                for (int i = 0; i < FixturesDef.Count; i++)
                {
                    body.CreateFixture(FixturesDef[i]);
                }
            }
            return body;
        }
        public object Clone()
        {
            var rbc = (PhysicsBodyConstructor)this.MemberwiseClone();

            return rbc;
        }
    }
}