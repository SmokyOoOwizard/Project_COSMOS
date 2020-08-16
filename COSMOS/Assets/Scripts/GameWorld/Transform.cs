using UnityEngine;
using Box2DSharp.Dynamics;
using COSMOS.Core;

namespace COSMOS.GameWorld
{
    public class Transform
    {
        public virtual Vector3 Position { get; set; }
        public virtual Vector3 Rotation { get; set; }
    }
}
