using System;
using Lucky.Extensions;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace Lucky.GL_.Test
{
    public class Test : MonoBehaviour
    {
        public int lineCount = 200;
        public int radius = 200;
        public float lineWidth = 1;

        private void OnRenderObject()
        {
            // for (int i = 0; i < lineCount; ++i)
            // {
            //     float a = i / (float)lineCount;
            //     float angle = a * this.PI(2);
            //
            //     this.DrawLine(
            //         Vector3.zero,
            //         new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius),
            //         new Color(a, 1 - a, 0, 0.8F),
            //         lineWidth
            //     );
            // }

            this.DrawRect(Vector3.zero, 100, 200, Color.green);

            // this.DrawTriangle(Vector2.zero, Vector2.one * 50, Vector2.up * 50, Color.green);

            // this.DrawTriangleStrip(Color.green, Vector2.one * 50, Vector2.up * 50, Vector3.zero, new Vector2(25, -25));
            // this.DrawLines(Color.green, lineWidth, Vector3.zero, new(1, 0), new(1, -1), new Vector3(4, -1));
        }
    }
}