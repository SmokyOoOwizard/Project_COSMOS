using UnityEngine;
using Box2DSharp.Dynamics;
using COSMOS.Core;
using System;

namespace COSMOS.GameWorld
{
    public struct TransformEventData : IEventData
    {
        public Transform Transform;
    }
    public class Transform : EventDispatcher
    {
        public enum Events
        {
            OnMove,
            OnTranslate,
            OnRotate,

        }
        public WorldObject Object { get; private set; }
        public virtual Vector3I Position { get; set; }
        public virtual Vector3 Rotation { get; set; }

        public Transform(WorldObject worldObject)
        {
            Object = worldObject;
        }

        public void SetDirty()
        {
            TransformEventData ted = new TransformEventData();
            ted.Transform = this;
            dispatchEvent(Events.OnMove, ted);
        }
    }
    [Serializable]
    public struct Vector3I
    {
        public Vector3 LocalPosition;
        public Vector3Int Sector;

        public double GetFullX()
        {
            return LocalPosition.x + Sector.x * 1000;
        }
        public double GetFullY()
        {
            return LocalPosition.y + Sector.y * 1000;
        }
        public double GetFullZ()
        {
            return LocalPosition.z + Sector.z * 1000;
        }


        public override string ToString()
        {
            return $"[{Sector.x}:{Sector.y}:{Sector.z}][{LocalPosition.x}:{LocalPosition.y}:{LocalPosition.z}]";
        }
    }
}
