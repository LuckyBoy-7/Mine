using Lucky.Celeste.Monocle;
using Lucky.GL_;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class DreamWipe : ScreenWipe
    {
        public bool WipeIn;
        [Range(0, 1)] public float Duration;

        private readonly int circleColumns = 15;
        private readonly int circleRows = 8;
        private const int circleSegments = 32;
        private const float circleFillSpeed = 400f;
        private static Circle[] circles;
        private static Vector3[] vertexBuffer;

        protected override void Awake()
        {
            base.Awake();
            
            vertexBuffer ??= new Vector3[(circleColumns + 2) * (circleRows + 2) * circleSegments * 3];
            circles ??= new Circle[(circleColumns + 2) * (circleRows + 2)];

            int unitX = 1920 / circleColumns;
            int unitY = 1080 / circleRows;
            for (int x = 0; x < circleColumns + 2; x++)
            {
                for (int y = 0; y < circleRows + 2; y++)
                {
                    int id = x * (circleRows + 2) + y;
                    // 因为col和row加了2，所以整体向左下移一个单位，同时在自身的单元格内做一定的抖动
                    circles[id].Position = new Vector2(
                        (x - 1 + 0.2f + Calc.Random.NextFloat(0.6f)) * unitX,
                        (y - 1 + 0.2f + Calc.Random.NextFloat(0.6f)) * unitY
                    );
                    circles[id].Delay = Calc.Random.NextFloat(0.05f) + (WipeIn ? x : circleColumns - x) * 0.018f;
                    circles[id].Radius = WipeIn ? 0 : circleFillSpeed * (Duration - circles[id].Delay);
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < circles.Length; i++)
            {
                if (WipeIn)
                {
                    circles[i].Delay -= Time.deltaTime;
                    if (circles[i].Delay <= 0f)
                        circles[i].Radius += Time.deltaTime * circleFillSpeed;
                }
                else if (circles[i].Radius > 0f)
                    circles[i].Radius -= Time.deltaTime * circleFillSpeed;
                else
                    circles[i].Radius = 0f;
            }
        }

        private void OnRenderObject()
        {
            int num = 0;
            // 绘制每个circle
            for (int i = 0; i < circles.Length; i++)
            {
                Circle circle = circles[i];
                Vector2 prePoint = new Vector2(1f, 0f);
                for (int j = 0; j < circleSegments; j++)
                {
                    Vector2 curPoint = Calc.AngleToVector((j + 1f) / circleSegments * PI(2), 1f);
                    vertexBuffer[num++] = circle.Position;
                    vertexBuffer[num++] = circle.Position + prePoint * circle.Radius;
                    vertexBuffer[num++] = circle.Position + curPoint * circle.Radius;
                    prePoint = curPoint;
                }
            }

            this.DrawTriangle3By3WithRandomColor(vertexBuffer);
        }


        private struct Circle
        {
            public Vector2 Position;
            public float Radius;
            public float Delay;
        }
    }
}