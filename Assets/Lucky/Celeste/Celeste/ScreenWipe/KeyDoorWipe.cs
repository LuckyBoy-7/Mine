using System;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using Lucky.Utilities;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class KeyDoorWipe : ScreenWipe
    {
        private Vector3[] vertex = new Vector3[57];
        public bool WipeIn;
        [Range(0, 1)] public float percent;

        public float Percent
        {
            get => ((WipeIn ? percent : 1f - percent) - 0.2f) / 0.8f;
            set => percent = value;
        }

        private void OnRenderObject()
        {
            int screenHeight = 1080;
            int halfScreenHeight = 540;
            int halfScreenWidth = 960;
            // 到一定程度x就不动了，就只缩放了[-0.3, 1]
            float kX = Ease.SineEaseInOut(Math.Min(1f, Percent / 0.5f));
            float scale = Ease.SineEaseInOut(1f - Calc.Clamp((Percent - 0.5f) / 0.3f, 0f, 1f));
            // [1, 1.65]
            float bControl = 1f + (1f - kX) * 0.5f;
            float leftCircleCenterX = halfScreenWidth * kX;
            // 表示椭圆的长短半轴
            float a = 128f * scale * kX;
            float b = 128f * scale * bControl;
            float leftCircleCenterY = halfScreenHeight + halfScreenHeight * 0.3f * scale * bControl;
            float keyTopY = halfScreenHeight - halfScreenHeight * 0.5f * scale * bControl;
            float radians = 0f;

            int i = 0;
            vertex[i++] = new Vector3(0, screenHeight);
            vertex[i++] = new Vector3(leftCircleCenterX, screenHeight);
            vertex[i++] = new Vector3(leftCircleCenterX, leftCircleCenterY + b);
            for (int j = 1; j <= 8; j++)
            {
                float radians0 = PI(0.5f) + (j - 1) / 8f * PI(0.5f);
                radians = PI(0.5f) + j / 8f * PI(0.5f);
                vertex[i++] = new Vector3(0, screenHeight);
                vertex[i++] = new Vector2(leftCircleCenterX, leftCircleCenterY) + Calc.AngleToVector(radians0, 1f) * new Vector2(a, b);
                vertex[i++] = new Vector2(leftCircleCenterX, leftCircleCenterY) + Calc.AngleToVector(radians, 1f) * new Vector2(a, b);
            }

            vertex[i++] = new Vector3(0, screenHeight);
            vertex[i++] = new Vector3(leftCircleCenterX - a, leftCircleCenterY);
            vertex[i++] = Vector3.zero;
            for (int k = 1; k <= 6; k++)
            {
                // 这里选择/8而不是/7是为了让结束点稍微靠左一点
                float radians0 = PI() + (k - 1) / 8f * PI(0.5f);
                radians = PI() + k / 8f * PI(0.5f);
                vertex[i++] = Vector3.zero;
                vertex[i++] = new Vector2(leftCircleCenterX, leftCircleCenterY) + Calc.AngleToVector(radians0, 1f) * new Vector2(a, b);
                vertex[i++] = new Vector2(leftCircleCenterX, leftCircleCenterY) + Calc.AngleToVector(radians, 1f) * new Vector2(a, b);
            }

            vertex[i++] = Vector3.zero;
            vertex[i++] = new Vector2(leftCircleCenterX, leftCircleCenterY) + Calc.AngleToVector(radians, 1f) * new Vector2(a, b);
            vertex[i++] = new Vector3(leftCircleCenterX - a * 0.8f, keyTopY);

            vertex[i++] = Vector3.zero;
            vertex[i++] = new Vector3(leftCircleCenterX - a * 0.8f, keyTopY);
            vertex[i++] = new Vector3(leftCircleCenterX, keyTopY);

            vertex[i++] = Vector3.zero;
            vertex[i++] = new Vector3(leftCircleCenterX, keyTopY);
            vertex[i++] = new Vector3(leftCircleCenterX, 0);
            this.DrawTriangle3By3WithRandomColor(vertex);

            for (i = 0; i < vertex.Length; i++)
                vertex[i].x = 1920f - vertex[i].x;
            this.DrawTriangle3By3WithRandomColor(vertex);
        }
    }
}