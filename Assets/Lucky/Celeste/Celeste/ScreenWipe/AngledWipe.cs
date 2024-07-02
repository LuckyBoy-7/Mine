using System;
using Lucky.GL_;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class AngledWipe : ScreenWipe
    {
        // private const int rows = 6;
        // private const float angleSize = 64f;
        public int rows = 6;
        public float angleSize = 64f;
        public bool WipeIn;
        [Range(0, 1)] public float Percent;
        [Range(0, 1)] public float gap;

        private void OnRenderObject()
        {
            float unitHeight = 1080f / rows; // height
            float totalWidth = 1920 + angleSize;
            // 1080 / 6 = 180
            for (int i = 0; i < 6; i++)
            {
                float bottomY = (WipeIn ? i : 5 - i) * unitHeight;
                float percentY = i / 6f;
                // 与windWipe类似
                float percentX = Math.Clamp(Percent - percentY * gap, 0, 1 - gap) / (1 - gap);
                if (!WipeIn)
                    percentX = 1f - percentX;

                float width = totalWidth * percentX; // width
                float leftX = WipeIn ? -angleSize : 1920f - width; // leftX

                Vector3 bottomLeft = new Vector3(leftX, bottomY);
                Vector3 bottomRight = new Vector3(leftX + width, bottomY);
                Vector3 topRight = new Vector3(leftX + width + angleSize, bottomY + unitHeight);
                Vector3 topLeft = new Vector3(leftX + angleSize, bottomY + unitHeight);
                this.DrawRect(bottomLeft, bottomRight, topRight, topLeft, Color.black);
            }
        }
    }
}