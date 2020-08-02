using UnityEngine;
using Box2DSharp.Dynamics;

namespace COSMOS.Core
{
    public class Transform
    {
        public virtual Vector3 Position { get; set; }
        public virtual Vector3 Rotation { get; set; }
    }
    public class PhysicsTransform : Transform
    {
        public override Vector3 Position
        {
            get
            {
                if (PhysicsBody != null)
                {
                    base.Position = PhysicsBody.GetPosition().ToUnity().ToV3();
                }
                return base.Position;
            }
            set
            {
                if (base.Position != value)
                {
                    if (PhysicsBody != null)
                    {
                        PhysicsBody.SetTransform(value.ToV2().ToNumerics(), PhysicsBody.GetAngle());
                    }
                    base.Position = value;
                }
            }
        }
        public override Vector3 Rotation
        {
            get
            {
                if (PhysicsBody != null)
                {
                    var r = base.Rotation;
                    r.y = PhysicsBody.GetAngle();
                    base.Rotation = r;
                }
                return base.Rotation;
            }
            set
            {
                if (base.Rotation != value)
                {
                    if (PhysicsBody != null)
                    {
                        PhysicsBody.SetTransform(PhysicsBody.GetPosition(), value.y);
                    }
                    base.Rotation = value;
                }
            }
        }
        public Body PhysicsBody;
    }
}
