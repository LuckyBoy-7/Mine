using Lucky.Celeste.Monocle;
using Lucky.GL_;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class Godrays : Backdrop
    {
        private const int RayCount = 6;
        private Vector3[] vertices = new Vector3[RayCount * 6];
        private Color[] colors = new Color[RayCount * 6];
        private int vertexCount;
        private Color rayColor = Calc.HexToColor("f52b63") * 0.5f;
        private Ray[] rays = new Ray[RayCount];
        private float fade;

        private struct Ray
        {
            public void Reset()
            {
                Percent = 0f;
                x = Calc.Random.NextFloat(384f);
                y = Calc.Random.NextFloat(244f);
                Duration = 4f + Calc.Random.NextFloat() * 8f;
                Width = Calc.Random.Next(8, 16);
                Length = Calc.Random.Next(20, 40);
            }

            public float x;
            public float y;
            public float Percent;
            public float Duration;
            public float Width;
            public float Length;
        }

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < rays.Length; i++)
            {
                rays[i].Reset();
                rays[i].Percent = Calc.Random.NextFloat();
            }
        }

        public void Update()
        {
            fade = Calc.Approach(fade, isVisible ? 1 : 0, Time.deltaTime);
            if (fade <= 0f)
            {
                return;
            }

            // 刚好ray是一个平行四边形
            Vector2 heightVector = Calc.AngleToVector(-1.6707964f, 1f);
            Vector2 widthVector = heightVector.Perpendicular();
            int num = 0;
            for (int i = 0; i < rays.Length; i++)
            {
                if (rays[i].Percent >= 1f)
                    rays[i].Reset();

                rays[i].Percent += Time.deltaTime / rays[i].Duration;
                rays[i].y -= 8f * Time.deltaTime;
                float x = -32f + Calc.Mod(rays[i].x - camera.transform.position.x * 0.9f, 384f);
                float y = -32f + Calc.Mod(rays[i].y - camera.transform.position.y * 0.9f, 244f);
                float width = rays[i].Width;
                float length = rays[i].Length;
                Vector2 pos = new Vector2((int)x, (int)y);
                float percent = rays[i].Percent;
                Color color = rayColor * Ease.CubicEaseInOut(Calc.Clamp((percent < 0.5f ? percent : 1f - percent) * 2f, 0f, 1f)) * fade;

                Vector3 pos1 = pos + widthVector * width + heightVector * length;
                Vector3 pos2 = pos - widthVector * width;
                Vector3 pos3 = pos + widthVector * width;
                Vector3 pos4 = pos - widthVector * width - heightVector * length;
                vertices[num++] = pos1;
                colors[num] = color;
                vertices[num++] = pos2;
                colors[num] = color;
                vertices[num++] = pos3;
                colors[num] = color;
                vertices[num++] = pos2;
                colors[num] = color;
                vertices[num++] = pos3;
                colors[num] = color;
                vertices[num++] = pos4;
            }

            vertexCount = num;
        }


        private void OnRenderObject()
        {
            if (vertexCount > 0 && fade > 0f)
                // this.DrawTriangle3By3WithRandomColor(vertices);
                this.DrawTriangle3By3(vertices, colors);
        }
    }
}