using UnityEngine;

namespace Lucky.Extensions
{
    public static class Vector2IntExtensions
    {
        public static Vector2Int WithX(this Vector2Int orig, int x)
        {
            orig.x = x;
            return orig;
        }
        
        public static Vector2Int WithY(this Vector2Int orig, int y)
        {
            orig.y = y;
            return orig;
        }
        
        public static Vector3 WithZ(this Vector2Int orig, int z)
        {
            return new Vector3(orig.x, orig.y, z);
        }
        
        public static Vector2Int Sign(this Vector2Int orig)
        {
            return new Vector2Int((int)Mathf.Sign(orig.x), (int)Mathf.Sign(orig.y));
        }
        
        public static void Deconstruct(this Vector2Int vector, out int x, out int y)
        {
            x = vector.x;
            y = vector.y;
        }
    }
}