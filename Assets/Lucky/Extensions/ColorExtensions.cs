using UnityEngine;

namespace Lucky.Extensions
{
    public static class ColorExtensions
    {
        public static Color WithR(this Color orig, float x)
        {
            orig.r = x;
            return orig;
        }

        public static Color WithG(this Color orig, float x)
        {
            orig.g = x;
            return orig;
        }

        public static Color WithB(this Color orig, float x)
        {
            orig.b = x;
            return orig;
        }

        public static Color WithA(this Color orig, float x)
        {
            orig.a = x;
            return orig;
        }
    }
}