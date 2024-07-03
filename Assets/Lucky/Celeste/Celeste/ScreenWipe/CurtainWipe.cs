using System;
using System.Linq;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using Lucky.Utilities;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class CurtainWipe : ScreenWipe
    {
        public bool WipeIn;
        private Vector3[] vertexBufferLeft;
        private Vector3[] vertexBufferRight;
        [Range(0, 1)] public float percent;

        private float Percent
        {
            get => Ease.CubicEaseInOut(WipeIn ? percent : 1 - percent);
            set => percent = value;
        }

        protected override void Awake()
        {
            base.Awake();
            vertexBufferLeft = new Vector3[192];
            vertexBufferRight = new Vector3[192];
        }

        private void OnRenderObject()
        {
            // 意味着当Percent > 0.3时，sectorPoint的y值就不变了
            float sectorPointRatioY = Math.Min(1f, Percent / 0.3f);
            // 扇形的一个端点，用来控制帘子的上端
            Vector2 sectorPoint = new Vector2(896f + 200f * Percent, 1080 - (-350f + 256f * sectorPointRatioY));
            // 上端帘子和下端帘子的交点（即打结的地方）
            Vector2 curveStart = new Vector2(0f, 1080 - 540f * sectorPointRatioY);
            Vector2 curveEnd = new Vector2(1920f, 512f) / 2f; // 即屏幕中间下面点的地方
            Vector2 curveControl = (curveStart + curveEnd) / 2f - Vector2.up * 1080f * 0.25f;
            float crossPointPercent = Math.Clamp((Percent - 0.1f) / 0.9f / 0.9f, 0, 1);
            Vector2 crossPoint = new SimpleCurve(curveStart, curveEnd, curveControl).GetPoint(crossPointPercent);
            // 帘子最下端的部分
            int i = 0;
            // 控制扇形的三角（虽然不画也行，但是方便debug）
            vertexBufferLeft[i++] = new Vector3(0, 1080);
            vertexBufferLeft[i++] = new Vector3(sectorPoint.x, 1080);
            vertexBufferLeft[i++] = sectorPoint;

            // 帘子下半部分的3个三角形
            vertexBufferLeft[i++] = new Vector3(0, 1080);
            vertexBufferLeft[i++] = new Vector3(0, crossPoint.y);
            vertexBufferLeft[i++] = crossPoint;

            vertexBufferLeft[i++] = crossPoint;
            vertexBufferLeft[i++] = new Vector3(0, crossPoint.y, 0f);
            vertexBufferLeft[i++] = Vector3.zero;

            vertexBufferLeft[i++] = crossPoint;
            vertexBufferLeft[i++] = Vector3.zero;
            vertexBufferLeft[i++] = new Vector3(crossPoint.x + 64f * Percent, 0, 0f);

            int i0 = i;
            Vector2 prePoint = sectorPoint;
            // 画扇形
            while (i < vertexBufferLeft.Length)
            {
                SimpleCurve curve = new SimpleCurve(sectorPoint, crossPoint, (sectorPoint + crossPoint) / 2f - new Vector2(0f, 384f * crossPointPercent));
                // 因为植树问题，取length段三角形要有length + 1个端点，对应3 * (length + 1)的顶点，
                Vector2 curPoint = curve.GetPoint((i - i0) / (float)(vertexBufferLeft.Length - i0 - 3));
                vertexBufferLeft[i++] = new Vector3(0, 1080);
                vertexBufferLeft[i++] = prePoint;
                vertexBufferLeft[i++] = curPoint;
                prePoint = curPoint;
            }

            for (i = 0; i < vertexBufferLeft.Length; i++)
            {
                vertexBufferRight[i] = vertexBufferLeft[i];
                vertexBufferRight[i].x = 1920f - vertexBufferRight[i].x;
            }

            this.DrawTriangle3By3WithRandomColor(vertexBufferLeft);
            this.DrawTriangle3By3WithRandomColor(vertexBufferRight);

            // debug
            this.DrawDot(curveStart, Color.red, 60);
            this.DrawDot(curveEnd, Color.red, 60);
            this.DrawDot(curveControl, Color.red, 60);
            this.DrawLines(Color.red, 1, new SimpleCurve(curveStart, curveEnd, curveControl).GetPoints().ToArray());

            this.DrawDot(sectorPoint, Color.green, 60);
            this.DrawDot(crossPoint, Color.green, 60);
            this.DrawDot((sectorPoint + crossPoint) / 2f - new Vector2(0f, 384f * crossPointPercent), Color.green, 60);
            SimpleCurve curveTest = new SimpleCurve(sectorPoint, crossPoint, (sectorPoint + crossPoint) / 2f + new Vector2(0f, 384f * crossPointPercent));
            this.DrawLines(Color.green, 1, curveTest.GetPoints().ToArray());
        }
    }
}