using UnityEngine;

namespace Lucky.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 WithX(this Vector2 orig, float x)
        {
            orig.x = x;
            return orig;
        }
        
        public static Vector2 WithY(this Vector2 orig, float y)
        {
            orig.y = y;
            return orig;
        }
        
        public static Vector3 WithZ(this Vector2 orig, float z)
        {
            return new Vector3(orig.x, orig.y, z);
        }
        
        public static Vector2 Sign(this Vector2 orig)
        {
            return new Vector2(Mathf.Sign(orig.x), Mathf.Sign(orig.y));
        }
        
        public static void Deconstruct(this Vector2 vector, out float x, out float y)
        {
            x = vector.x;
            y = vector.y;
        }
    }
}