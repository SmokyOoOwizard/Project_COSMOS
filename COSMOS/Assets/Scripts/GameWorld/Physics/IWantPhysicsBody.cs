namespace COSMOS.GameWorld
{
    public interface IWantPhysicsBody
    {
        PhysicsBodyCreateData PhysicsBodyCreateData { get; }
        PhysicsBody PhysicsBody { get; set; }
    }
}
