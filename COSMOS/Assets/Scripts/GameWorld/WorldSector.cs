using COSMOS.Core;

namespace COSMOS.GameWorld
{
    public class WorldSector
    {
        public readonly QuadTree<WorldObject> WorldObjects;
        public Vector3I SectorPosition;
    }
}
