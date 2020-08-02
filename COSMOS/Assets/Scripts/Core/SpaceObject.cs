using Box2DSharp.Dynamics;

namespace COSMOS.Core.Space
{
    public class SpaceObject : IHasTransform
    {
        public SolarSystemInstance SolarSystem { get; private set; }
        public PhysicsTransform Transform { get; private set; }
        public PhysicsBodyConstructor BodyConstructor { get; private set; }

        public Body PhysicsBody { get; private set; }

        public SpaceObject()
        {
            Transform = new PhysicsTransform();
            BodyConstructor = new PhysicsBodyConstructor();
        }
        public Transform GetTransform()
        {
            return Transform;
        }

        internal void _ChangeSolarSystem(SolarSystemInstance solarSystem)
        {
            if (solarSystem != SolarSystem)
            {
                if (SolarSystem != null)
                {
                    if (PhysicsBody != null)
                    {
                        if (!SolarSystem.PhysicsWorld.DestroyBody(PhysicsBody))
                        {
                            Log.Error("Physics body dont destroy", "Box2D", "Physics", "SpaceObject");
                        }
                    }
                }
                SolarSystem = solarSystem;
                PhysicsBody = BodyConstructor.CreateBody(solarSystem.PhysicsWorld);
                Transform.PhysicsBody = PhysicsBody;
            }
        }
    }
}