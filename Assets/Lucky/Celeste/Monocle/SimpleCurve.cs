using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Celeste.Monocle
{
    public struct SimpleCurve
    {
        public Vector2 Begin;
        public Vector2 End;
        public Vector2 Control;
        private LineRenderer line;

        public SimpleCurve(Vector2 begin, Vector2 end, Vector2 control)
        {
            Begin = begin;
            End = end;
            Control = control;
            line = null;
        }

        public void DoubleControl()
        {
            // (Begin.x + (End.x - (double)Begin.x) / 2.0表示begin和end中点的向量
            // 感觉是把control向量翻倍然和和终点坐标blend了一下
            Control = new Vector2(
                (float)(Control.x + (double)Control.x - (Begin.x + (End.x - (double)Begin.x) / 2.0)),
                (float)(Control.y + (double)Control.y - (Begin.y + (End.y - (double)Begin.y) / 2.0))
            );
        }

        public Vector2 GetPoint(float percent)
        {
            // 这里从易读性考虑把double float的转化都删了
            // 二阶贝塞尔曲线
            // B(t) = (1-t)**2 * P0 + 2t(1-t)P1 + t**2 *P2
            float num = 1.0f - percent;
            return num * num * Begin + 2.0f * num * percent * Control + percent * percent * End;
        }

        public List<Vector2> GetPoints(int resolution = 100)
        {
            List<Vector2> res = new();
            for (int i = 0; i <= resolution; i++)
                res.Add(GetPoint((float)i / resolution));

            return res;
        }

        /// <summary>
        /// 我的理解是获取非积分版的长度，即有精度损失的长度
        /// </summary>
        /// <param name="resolution">一开始我还以为是什么分辨率呢，想想才知道是精度</param>
        /// <returns></returns>
        public float GetLengthParametric(int resolution)
        {
            Vector2 vector = Begin;
            float num = 0f;
            for (int i = 1; i <= resolution; i++)
            {
                Vector2 point = GetPoint(i / (float)resolution);
                num += (point - vector).magnitude;
                vector = point;
            }

            return num;
        }

        public void InitLine()
        {
            line = new GameObject("CurveLine").AddComponent<LineRenderer>();
            line.material = Resources.Load<Material>("Materials/LineRenderer");
            line.numCapVertices = 90;
        }

        public void Render(Vector2 offset, Color color, int resolution, float thickness = 0.2f)
        {
            Vector2 vector = offset + Begin;
            line.startColor = line.endColor = color;
            line.positionCount = resolution + 1;
            line.startWidth = line.endWidth = thickness;

            line.useWorldSpace = true;
            line.SetPosition(0, vector);
            for (int i = 1; i <= resolution; i++)
            {
                vector = offset + GetPoint(i / (float)resolution);
                line.SetPosition(i, vector);
            }
        }

        public void Render(Color color, int resolution)
        {
            Render(Vector2.zero, color, resolution);
        }

        public void Render(Color color, int resolution, float thickness)
        {
            Render(Vector2.zero, color, resolution, thickness);
        }
    }
}