using System;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using Lucky.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class DropWipe : ScreenWipe
    {
        private const int columns = 10;
        private float[] meetings;
        public bool WipeIn;
        [Range(0, 1)] public float Percent;

        protected override void Awake()
        {
            base.Awake();
            meetings = new float[columns];
            // 从高度中抽一个当作这两个柱子相碰的高度
            for (int i = 0; i < columns; i++)
                meetings[i] = 0.05f + Calc.Random.NextFloat() * 0.9f;
        }

        private void OnRenderObject()
        {
            float percent = WipeIn ? 1f - Percent : Percent;
            float unitX = 1920f / columns;
            if (percent >= 0.995f)
                this.DrawRect(Vector3.zero, 1920, 1080, Color.black);
            else
            {
                // 绘制columns列，每一列画两个矩阵
                for (int i = 0; i < columns; i++)
                {
                    float percentX = (float)i / columns;
                    // 这里 *0.3做blend（类似WindWipe）是为了让移动稍微有点错位感，（有的先出来，有的晚贴合）
                    float num4 = (WipeIn ? 1f - percentX : percentX) * 0.3f;
                    if (percent > num4)
                    {
                        float p = Ease.CubicEaseIn(Math.Min(1f, (percent - num4) / 0.7f));
                        float heightDown = 1080f * meetings[i] * p; // 下面柱子的高
                        float heightUp = 1080f * (1f - meetings[i]) * p; // 上面柱子的高
                        this.DrawRect(new Vector3(i * unitX, 0), unitX, heightDown, Color.black);
                        this.DrawRect(new Vector3(i * unitX, 1080f - heightUp), unitX, heightUp, Color.black);
                    }
                }
            }
        }
    }
}