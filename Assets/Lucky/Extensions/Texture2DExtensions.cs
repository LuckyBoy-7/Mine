using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Texture2DExtensions
{
    public static void Point(this Texture2D orig, int x, int y, Color color)
    {
        orig.SetPixel(x, y, color);
    }
}