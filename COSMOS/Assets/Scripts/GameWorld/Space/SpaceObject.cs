using Box2DSharp.Dynamics;

namespace COSMOS.GameWorld.Space
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

        internal bool _AttachToSolarSystem(SolarSystemInstance solarSystem)
        {
            if (solarSystem != SolarSystem)
            {
                if (SolarSystem != null)
                {
                    SolarSystem.DettachSpaceObject(this);
                }
                SolarSystem = solarSystem;
                if (solarSystem != null)
                {
                    PhysicsBody = BodyConstructor.CreateBody(solarSystem.PhysicsWorld);
                    Transform.PhysicsBody = PhysicsBody;
                }
                return true;
            }
            return false;
        }
        internal bool _DettachFromSolarSystem(SolarSystemInstance solarSystem)
        {
            if(solarSystem == SolarSystem)
            {
                if (PhysicsBody != null)
                {
                    if (!solarSystem.PhysicsWorld.DestroyBody(PhysicsBody))
                    {
                        Log.Error("Physics body dont destroy", "Box2D", "Physics", "SpaceObject");
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}