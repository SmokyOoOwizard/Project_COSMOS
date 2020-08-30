using UnityEngine;
using Box2DSharp.Dynamics;
using COSMOS.Core;

namespace COSMOS.GameWorld
{
    public class Transform
    {
        public virtual Vector3I Position { get; set; }
        public virtual Vector3 Rotation { get; set; }
    }
    public struct Vector3I
    {
        public Vector3 LocalPosition;
        public Vector3Int Sector;


    }
}
