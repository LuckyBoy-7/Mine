using System;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class HeartWipe : ScreenWipe
    {
        private Vector3[] vertex = new Vector3[111];
        public bool WipeIn;
        [Range(0, 1)] public float percent;

        public float Percent
        {
            get => ((WipeIn ? 1f - percent : percent) - 0.2f) / 0.8f;
            set => percent = value;
        }

        /// <summary>
        /// 怎么说呢
        /// 就是刚好心的下面90度，与圆相切刚好是1/4圆弧位置，然后让两个圆稍微重合一部分，看起来就像心了
        /// </summary>
        private void OnRenderObject()
        {
            if (Percent <= 0f)
            {
                this.DrawRect(Vector3.zero, 1920, 1080, Color.black);
                return;
            }

            Vector2 screenCenter = new Vector2(1920, 1080) / 2f;
            float circleRadius = 1920 * 0.75f * Percent;
            // 底部中心到heart顶部交叉中心的距离
            float topCenterToCrossCenterDist = 1920 * Percent;
            float startRadians = 0.25f;
            // 刚好是1/4个圆
            float endRadians = PI(0.5f);
            Vector2 leftCircleCenter = screenCenter + new Vector2(-(float)Math.Cos(startRadians) * circleRadius, circleRadius / 2f);

            this.DrawWireCircle(leftCircleCenter, circleRadius, Color.red, 2);
            this.DrawDot(leftCircleCenter, Color.red, 30);

            int i = 0;
            for (int j = 1; j <= 16; j++)
            {
                float radians1 = startRadians + (endRadians - startRadians) * ((j - 1) / 16f);
                float radians2 = startRadians + (endRadians - startRadians) * (j / 16f);
                vertex[i++] = new Vector3(screenCenter.x, 1080 + topCenterToCrossCenterDist);
                vertex[i++] = leftCircleCenter + Calc.AngleToVector(radians1, circleRadius);
                vertex[i++] = leftCircleCenter + Calc.AngleToVector(radians2, circleRadius);
            }

            vertex[i++] = new Vector3(screenCenter.x, 1080 + topCenterToCrossCenterDist);
            vertex[i++] = leftCircleCenter + new Vector2(0f, circleRadius);
            vertex[i++] = new Vector3(-topCenterToCrossCenterDist, 1080 + topCenterToCrossCenterDist);

            vertex[i++] = new Vector3(-topCenterToCrossCenterDist, 1080 + topCenterToCrossCenterDist);
            vertex[i++] = leftCircleCenter + new Vector2(0f, circleRadius);
            vertex[i++] = new Vector3(-topCenterToCrossCenterDist, leftCircleCenter.y);


            float gapRadians = PI(3f / 4); // 也就是要转0.75个半圆
            startRadians = PI(0.5f);
            for (int k = 1; k <= 16; k++)
            {
                float radians1 = startRadians + (k - 1) / 16f * gapRadians;
                float radians2 = startRadians + k / 16f * gapRadians;
                vertex[i++] = new Vector3(-topCenterToCrossCenterDist, leftCircleCenter.y, 0f);
                vertex[i++] = leftCircleCenter + Calc.AngleToVector(radians1, circleRadius);
                vertex[i++] = leftCircleCenter + Calc.AngleToVector(radians2, circleRadius);
            }

            Vector2 start = leftCircleCenter + Calc.AngleToVector(startRadians + gapRadians, circleRadius);
            Vector2 bottom = screenCenter - new Vector2(0f, circleRadius * 1.8f);
            vertex[i++] = new Vector3(-topCenterToCrossCenterDist, leftCircleCenter.y, 0f);
            vertex[i++] = start;
            vertex[i++] = new Vector3(-topCenterToCrossCenterDist, -topCenterToCrossCenterDist);

            vertex[i++] = new Vector3(-topCenterToCrossCenterDist, -topCenterToCrossCenterDist);
            vertex[i++] = start;
            vertex[i++] = bottom;

            vertex[i++] = new Vector3(-topCenterToCrossCenterDist, -topCenterToCrossCenterDist);
            vertex[i++] = bottom;
            vertex[i++] = new Vector3(screenCenter.x, -topCenterToCrossCenterDist);

            this.DrawTriangle3By3WithRandomColor(vertex);

            for (i = 0; i < vertex.Length; i++)
                vertex[i].x = 1920f - vertex[i].x;
            this.DrawTriangle3By3WithRandomColor(vertex);
        }
    }
}