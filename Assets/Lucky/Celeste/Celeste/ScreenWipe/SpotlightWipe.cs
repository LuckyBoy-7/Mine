using System.Runtime.CompilerServices;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using Lucky.Utilities;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class SpotlightWipe : ScreenWipe
    {
        [Range(0, 1)] public float percent;

        public float Percent
        {
            get => Mathf.Max(0, (WipeIn ? 1f - percent : percent) - 0.2f) / 0.8f;
            set => percent = value;
        }

        public bool WipeIn;
        public Vector2 FocusPoint;
        public bool Linear;
        private const float SmallCircleRadius = 288f;
        private static Vector3[] vertexBuffer = new Vector3[768]; // 3 * 4 * 64


        private void OnRenderObject()
        {
            float radius;
            if (!Linear)
            {
                if (Percent < 0.2f)
                    radius = Ease.CubicEaseInOut(Percent / 0.2f) * SmallCircleRadius;
                else if (Percent < 0.8f)
                    radius = SmallCircleRadius;
                else
                    radius = SmallCircleRadius + (Percent - 0.8f) / 0.2f * (2210 - SmallCircleRadius);
            }
            else
                // 2203是对角线长度，原本是1920的，我改成2203了
                radius = Ease.CubicEaseInOut(Percent) * 2210;

            DrawSpotlight(FocusPoint, radius);
        }

        public void DrawSpotlight(Vector2 position, float radius)
        {
            Vector2 prePos = new Vector2(1f, 0f);
            // 这里我也改了一下，原来的还向内填充了一点，应该是为了防止有缝
            for (int i = 6; i < vertexBuffer.Length; i += 6)
            {
                Vector2 curPos = Calc.AngleToVector((float)i / vertexBuffer.Length * PI(2), 1f);
                vertexBuffer[i] = position + prePos * 5000f;
                vertexBuffer[i + 1] = position + prePos * radius;
                vertexBuffer[i + 2] = position + curPos * radius;

                vertexBuffer[i + 3] = position + prePos * 5000f;
                vertexBuffer[i + 4] = position + curPos * 5000f;
                vertexBuffer[i + 5] = position + curPos * radius;

                prePos = curPos;
            }

            this.DrawTriangle3By3WithRandomColor(vertexBuffer);
        }
    }
}