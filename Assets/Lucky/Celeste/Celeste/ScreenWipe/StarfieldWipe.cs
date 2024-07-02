using System;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using Lucky.Utilities;
using UnityEditor.SearchService;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class StarfieldWipe : ScreenWipe
    {
        private Star[] stars;
        private Vector3[] verts;
        private Vector3[] starShape;
        private bool hasDrawn;
        [Range(0, 1)] public float Percent;

        protected override void Awake()
        {
            base.Awake();
            stars = new Star[64];
            verts = new Vector3[1536]; // 1536 / 64 = 24

            // star的五个交叉点的位置，交叉点到中心距离为1
            starShape = new Vector3[5];
            for (int i = 0; i < 5; i++)
                starShape[i] = Calc.AngleToVector(i / 5f * PI(2), 1f);
            for (int i = 0; i < stars.Length; i++)
                stars[i] = new Star((float)Math.Pow(i / (float)stars.Length, 5.0));
        }

        public void Update()
        {
            for (int i = 0; i < stars.Length; i++)
                stars[i].Update();
        }


        public void OnRenderObject()
        {
            if (Percent > 0.8f)
            {
                // 从中间沿y轴缩放一个矩形
                float num = Calc.Map(Percent, 0.8f, 1f, 0f, 1f) * 1080f;
                this.DrawRect(Vector3.up * (1080 - num) / 2, 1920, num, Color.black);
            }

            int num2 = 0;
            for (int i = 0; i < stars.Length; i++)
            {
                // [-500, 0], [0, 1920], [1920, 2420]
                float offsetX = -500f + stars[i].X % 2920f;
                float offsetY = (float)(stars[i].Y + Math.Sin(stars[i].Sine) * stars[i].SineDistance);
                float scale = (0.1f + stars[i].Scale * 0.9f) * 1080f * 0.8f * Ease.CubicEaseIn(Percent);
                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(offsetX, offsetY), stars[i].Rotation, scale * Vector3.one);
                SetStarData(ref num2, matrix);
            }

            this.DrawTriangle3By3(verts, Color.black);
        }


        private void SetStarData(ref int index, Matrix4x4 matrix)
        {
            int start = index;
            // 先把交叉点内部画满，需要9个点
            for (int i = 1; i < starShape.Length - 1; i++)
            {
                verts[index] = starShape[0];
                index += 1;
                verts[index] = starShape[i];
                index += 1;
                verts[index] = starShape[i + 1];
                index += 1;
            }

            // 再画尖尖的部分，需要15个点
            for (int j = 0; j < 5; j++)
            {
                // 第一个交叉点向量
                Vector2 vector1 = starShape[j];
                // 第二个交叉点向量
                Vector2 vector2 = starShape[(j + 1) % 5];
                // 尖尖的角的向量
                Vector2 vector3 = (vector1 + vector2) * 0.5f + (vector2 - vector1).normalized.TurnRight();
                verts[index] = vector1;
                index += 1;
                verts[index] = vector2;
                index += 1;
                verts[index] = vector3;
                index += 1;
            }

            // 变换
            for (int k = start; k < start + 24; k++)
            {
                verts[k] = matrix.MultiplyPoint(verts[k]);
            }
        }

        private struct Star
        {
            public float X;
            public float Y;
            public float Sine;
            public float SineDistance;
            public float Speed;
            public float Scale;
            private float angle;
            public Quaternion Rotation;

            public Star(float scale)
            {
                Scale = scale;
                float num = 1f - scale;
                X = Calc.Random.Range(0, 2920);
                // 这样写导致缩放大的更靠近中间
                Y = 1080f * (0.5f + Calc.Random.Choose(-1, 1) * num * Calc.Random.Range(0.25f, 0.5f));
                // 从一个周期里抽一个
                Sine = Calc.Random.NextFloat(PI(2));
                SineDistance = scale * 1080f * 0.05f;
                // 越大的星速度越小
                Speed = (0.5f + (1f - Scale) * 0.5f) * 1920f * 0.05f;
                // 360度里抽一个
                angle = Calc.Random.NextFloat(360);
                Rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }


            public void Update()
            {
                X += Speed * Time.deltaTime;
                // 越大的星上下摆的越慢
                Sine += (1f - Scale) * 8f * Time.deltaTime;
                // 越大的星转的越慢
                angle += (1f - Scale) * Time.deltaTime;
                Rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }
}