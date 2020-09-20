using COSMOS.Core;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace COSMOS.GameWorld
{
    public class WorldInstance
    {
        private readonly HashSet<WorldObject> objects = new HashSet<WorldObject>();
        private readonly QuadTree<WorldObject> objectsMap = new QuadTree<WorldObject>();

        public bool AddWorldObject(WorldObject worldObject)
        {
            if (!objects.Contains(worldObject))
            {
                var objectSector = worldObject.Transform.Position;
                objectsMap.Insert(new Core.Rect(objectSector.GetFullX(), objectSector.GetFullZ(), 0, 0), worldObject);
                objects.Add(worldObject);
            }
            return false;
        }

        public IReadOnlyQuadTree<WorldObject> GetWorldQuadTree()
        {
            return objectsMap as IReadOnlyQuadTree<WorldObject>;
        }
    }
}
