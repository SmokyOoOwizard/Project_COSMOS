namespace COSMOS.GameWorld
{
    public abstract class PhysicsWorld
    {
        public enum UpdateType
        {
            LigthWeight,
            Full
        }
        public abstract PhysicsBody CreatePhysicsBody(PhysicsBodyCreateData createData);
        public abstract void Update(float delta, UpdateType type);
        public abstract void DestroyPhysicsBody(PhysicsBody physicsBody);
        public abstract bool CorrectPhysicsBodyCreateData(PhysicsBodyCreateData createData);
    }
}
