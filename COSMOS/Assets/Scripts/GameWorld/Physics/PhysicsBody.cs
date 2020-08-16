using UnityEngine;

namespace COSMOS.GameWorld
{
    public abstract class PhysicsBody
    {
        public abstract Vector3 Velocity { get; set; }
        public abstract Vector3 AngularVelocity { get; set; }

        public virtual float Mass { get; set; }

        public abstract void ApplyForce(Vector3 localPosition, Vector3 force);
        public abstract void ApplyTorque(Vector3 torque);
    }
}
