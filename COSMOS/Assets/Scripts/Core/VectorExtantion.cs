using UnityEngine;

namespace COSMOS.Core
{
    public static class VectorExtantion
    {
        public static Vector2 ToUnity(this System.Numerics.Vector2 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }
        public static System.Numerics.Vector2 ToNumerics(this Vector2 vector)
        {
            return new System.Numerics.Vector2(vector.x, vector.y);
        }
        public static Vector3 ToV3(this Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }
        public static Vector2 ToV2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }
    }
}
