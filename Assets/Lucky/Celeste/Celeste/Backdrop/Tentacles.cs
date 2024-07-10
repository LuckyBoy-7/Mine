using System;
using System.Runtime.CompilerServices;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using UnityEngine;

namespace Lucky.Celeste.Celeste.Backdrop
{
    /// <summary>
    /// 整体思路就是定位置，定方向，定触手宽度，然后对每个触手从根部往外延伸n次，每次垂直于当前平面（适当做sine偏移，适当缩减宽度），就形成了摇曳的感觉
    /// </summary>
    public class Tentacles : Backdrop
    {
        // 一个触手几个节点
        private const int NodesPerTentacle = 10;

        // 触手从哪边伸出
        public Side side;

        // 触手根部宽度
        private float width;

        // 触手根部中心点
        private Vector2 origin;

        // 触手延伸方向
        private Vector2 outwards;

        // 调整触手延伸长度的控制量之一
        public float outwardsOffset;
        private Vector3[] vertices;
        private int vertexCount;
        private Tentacle[] tentacles;
        private int tentacleCount;
        private float hideTimer = 5f;

        public Vector2 entity;

        public enum Side
        {
            Right,
            Left,
            Top,
            Bottom
        }

        private struct Tentacle
        {
            public float Length; // 触手长度
            public float Offset;
            public float Step;
            public float Position; // 当前位置
            public float Approach; // 接近的速度
            public float Width;
        }

        protected override void Awake()
        {
            base.Awake();
            switch (side)
            {
                case Side.Right:
                    outwards = new Vector2(-1f, 0f);
                    width = ScreenHeight;
                    origin = new Vector2(ScreenWidth, ScreenHeight / 2);
                    break;
                case Side.Left:
                    outwards = new Vector2(1f, 0f);
                    width = ScreenHeight;
                    origin = new Vector2(0f, ScreenHeight / 2);
                    break;
                case Side.Top:
                    outwards = new Vector2(0f, -1f);
                    width = ScreenWidth;
                    origin = new Vector2(ScreenWidth / 2, ScreenHeight);
                    break;
                case Side.Bottom:
                    outwards = new Vector2(0f, 1f);
                    width = ScreenWidth;
                    origin = new Vector2(ScreenWidth / 2, 0f);
                    break;
            }

            float curWidth = 0f;
            tentacles = new Tentacle[100];
            int i = 0;
            while (i < tentacles.Length && curWidth < width + 40f)
            {
                tentacles[i].Length = Calc.Random.NextFloat();
                tentacles[i].Offset = Calc.Random.NextFloat();
                tentacles[i].Step = Calc.Random.NextFloat();
                tentacles[i].Position = -200f;
                tentacles[i].Approach = Calc.Random.NextFloat();
                curWidth += tentacles[i].Width = 6f + Calc.Random.NextFloat(20f);
                tentacleCount++;
                i++;
            }

            vertices = new Vector3[tentacleCount * 11 * 6];
        }

        public void Update()
        {
            float targetPos = 0f;
            if (isVisible)
            {
                targetPos = side switch
                {
                    Side.Right => ScreenWidth - (entity.x - camera.transform.position.x) - ScreenWidth / 2,
                    Side.Bottom => ScreenHeight - (entity.y - camera.transform.position.y) - ScreenHeight,
                    _ => targetPos
                };

                hideTimer = 0f;
            }
            else
            {
                targetPos = -200f;
                hideTimer += Time.deltaTime;
            }

            // 期望的位置
            targetPos += outwardsOffset;
            bool visible = hideTimer < 5f;
            if (visible)
            {
                // 触手排布方向的单位向量
                Vector2 perpendicular = -outwards.Perpendicular();
                int vId = 0;
                Vector2 start = origin - perpendicular * (width / 2f + 2f);
                for (int i = 0; i < tentacleCount; i++)
                {
                    start += perpendicular * (tentacles[i].Width * 0.5f);
                    // approach到targetPos，然后加了缓动(EaseOut)
                    tentacles[i].Position += (targetPos - tentacles[i].Position) *
                                             (1f - (float)Math.Pow(0.5f * (0.5f + tentacles[i].Approach * 0.5f), Time.deltaTime));
                    Vector2 rootHeightVector = (tentacles[i].Position + (float)Math.Sin(Time.time + tentacles[i].Offset * 4f) * 8f +
                                                (origin - start).magnitude * 0.7f) * outwards;
                    Vector2 rootTopCenter = start + rootHeightVector;
                    // 两个center之间的距离，当然newcenter还会做一点点偏移
                    float rootHeight = 2f + tentacles[i].Length * 8f;
                    Vector2 rootHalfWidthVector = perpendicular * (tentacles[i].Width * 0.5f);
                    vertices[vId++] = start + rootHalfWidthVector;
                    vertices[vId++] = start - rootHalfWidthVector;
                    vertices[vId++] = rootTopCenter - rootHalfWidthVector;

                    vertices[vId++] = rootTopCenter - rootHalfWidthVector;
                    vertices[vId++] = start + rootHalfWidthVector;
                    vertices[vId++] = rootTopCenter + rootHalfWidthVector;
                    // 一根触手有20个triangle，根部是两个triangle组成的矩形，剩下18个是会动的触手
                    for (int j = 1; j < NodesPerTentacle; j++)
                    {
                        double num5 = Time.time * tentacles[i].Offset * (float)Math.Pow(1.1f, j) * 2f;
                        float num6 = tentacles[i].Offset * 3f + j * (0.1f + tentacles[i].Step * 0.2f) + rootHeight * j * 0.1f;
                        float num7 = 2f + 4f * (j / (float)NodesPerTentacle);
                        // 对新的rootTopCenter做了微小的偏移，然后重新计算perpendicular，就有了一种摇曳的感觉
                        // 然后这个偏移受时间，tentacle.Offset，tentacle.Step 和node在触手中的位置共同决定，(然后上面一连串公式有点看不懂了，大概思路差不多了)
                        Vector2 offset = perpendicular * ((float)Math.Sin(num5 + num6) * num7);
                        // 越往尖端走越细
                        float newRootHalfWidth = (1f - j / (float)NodesPerTentacle) * tentacles[i].Width * 0.5f;

                        Vector2 newRootTopCenter = rootTopCenter + outwards * rootHeight + offset;
                        Vector2 newRootHalfWidthCector = (rootTopCenter - newRootTopCenter).normalized.Perpendicular() * newRootHalfWidth;
                        vertices[vId++] = rootTopCenter + rootHalfWidthVector;
                        vertices[vId++] = rootTopCenter - rootHalfWidthVector;
                        vertices[vId++] = newRootTopCenter - newRootHalfWidthCector;

                        vertices[vId++] = newRootTopCenter - newRootHalfWidthCector;
                        vertices[vId++] = rootTopCenter + rootHalfWidthVector;
                        vertices[vId++] = newRootTopCenter + newRootHalfWidthCector;
                        rootTopCenter = newRootTopCenter;
                        rootHalfWidthVector = newRootHalfWidthCector;
                    }

                    start += perpendicular * (tentacles[i].Width * 0.5f);
                }

                vertexCount = vId;
            }
        }

        private void OnRenderObject()
        {
            this.DrawTriangle3By3WithRandomColor(vertices);
        }
    }
}