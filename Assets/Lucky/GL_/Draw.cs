using System;
using System.Collections.Generic;
using Lucky.Celeste.Monocle;
using Lucky.Extensions;
using Lucky.Utilities;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.GL_
{
    public static class Draw
    {
        public static Material lineMaterial;

        public static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }


        public static void DrawLine(
            this MonoBehaviour orig,
            Vector3 start,
            Vector3 end,
            Color color,
            float lineWidth = 1
        )
        {
            Vector2 vec = end - start;
            float k = Mathf.Atan2(-vec.x, vec.y);
            float dx = lineWidth / 2 * Mathf.Cos(k);
            float dy = lineWidth / 2 * Mathf.Sin(k);
            Vector3 pos1 = new Vector3(start.x - dx, start.y - dy);
            Vector3 pos2 = new Vector3(end.x - dx, end.y - dy);
            Vector3 pos3 = new Vector3(end.x + dx, end.y + dy);
            Vector3 pos4 = new Vector3(start.x + dx, start.y + dy);
            orig.DrawRect(pos1, pos2, pos3, pos4, color);
        }

        public static void DrawLines(
            this MonoBehaviour orig,
            Color color,
            float lineWidth = 1,
            params Vector2[] poses
        )
        {
            foreach (var (pos1, pos2) in Itertools.Pairwise(poses))
                orig.DrawLine(pos1, pos2, color, lineWidth);
        }

        public static void DrawTriangle(
            this MonoBehaviour orig,
            Vector3 pos1,
            Vector3 pos2,
            Vector3 pos3,
            Color color
        )
        {
            var transform = orig.transform;
            CreateLineMaterial();
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.TRIANGLES);

            GL.Color(color);
            GL.Vertex3(pos1.x, pos1.y, pos1.z);
            GL.Vertex3(pos2.x, pos2.y, pos2.z);
            GL.Vertex3(pos3.x, pos3.y, pos3.z);

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawTriangle3By3(
            this MonoBehaviour orig,
            Vector3[] poses,
            Color color
        )
        {
            var transform = orig.transform;
            CreateLineMaterial();
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.TRIANGLES);

            GL.Color(color);
            for (int i = 0; i < poses.Length; i += 3)
            {
                GL.Vertex3(poses[i].x, poses[i].y, poses[i].z);
                GL.Vertex3(poses[i + 1].x, poses[i + 1].y, poses[i + 1].z);
                GL.Vertex3(poses[i + 2].x, poses[i + 2].y, poses[i + 2].z);
            }

            GL.End();
            GL.PopMatrix();
        }

        private static List<Color> colors = new List<Color>
        {
            Color.black,
            Color.blue,
            Color.cyan,
            Color.gray,
            Color.green,
            Color.magenta,
            Color.red,
            Color.white,
            Color.yellow
        };

        public static void DrawTriangle3By3WithRandomColor(
            this MonoBehaviour orig,
            Vector3[] poses
        )
        {
            var transform = orig.transform;
            CreateLineMaterial();
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.TRIANGLES);

            for (int i = 0; i < poses.Length; i += 3)
            {
                GL.Color(colors[(i / 3) % colors.Count]);
                GL.Vertex3(poses[i].x, poses[i].y, poses[i].z);
                GL.Vertex3(poses[i + 1].x, poses[i + 1].y, poses[i + 1].z);
                GL.Vertex3(poses[i + 2].x, poses[i + 2].y, poses[i + 2].z);
            }

            GL.End();
            GL.PopMatrix();
        }

        /// 每三个顶点绘制一个三角形（所以也不是严格意义上的多边形）
        public static void DrawTriangleStrip(
            this MonoBehaviour orig,
            Color color,
            params Vector3[] poses
        )
        {
            var transform = orig.transform;
            CreateLineMaterial();
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.TRIANGLE_STRIP);

            GL.Color(color);
            foreach (var pos in poses)
                GL.Vertex3(pos.x, pos.y, pos.z);

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawRect(
            this MonoBehaviour orig,
            Vector3 pos,
            float width,
            float height,
            Color color
        )
        {
            var transform = orig.transform;
            CreateLineMaterial();
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.QUADS);

            GL.Color(color);
            GL.Vertex3(pos.x, pos.y, pos.z);
            GL.Vertex3(pos.x + width, pos.y, pos.z);
            GL.Vertex3(pos.x + width, pos.y + height, pos.z);
            GL.Vertex3(pos.x, pos.y + height, pos.z);

            GL.End();
            GL.PopMatrix();
        }


        public static void DrawDot(this MonoBehaviour orig, Vector3 pos, Color color, float size = 1)
        {
            orig.DrawRect(pos - (Vector3)Vector2.one * (0.5f * size), size, size, color);
        }

        public static void DrawDot(this MonoBehaviour orig, Vector3 pos, Color color, Vector2 size)
        {
            orig.DrawRect(pos - (Vector3)size * 0.5f, size.x, size.y, color);
        }

        public static void DrawDot(this MonoBehaviour orig, Vector3 pos, Color color, Vector2 size, float rotation)
        {
            Vector3 bottomLeft = new Vector3(-size.x, -size.y) * 0.5f;
            Vector3 bottomRight = new Vector3(size.x, -size.y) * 0.5f;
            Vector3 topRight = new Vector3(size.x, size.y) * 0.5f;
            Vector3 topLeft = new Vector3(-size.x, size.y) * 0.5f;

            rotation = Mathf.Rad2Deg * rotation;
            bottomLeft = Quaternion.Euler(0, 0, rotation) * bottomLeft + pos;
            bottomRight = Quaternion.Euler(0, 0, rotation) * bottomRight + pos;
            topRight = Quaternion.Euler(0, 0, rotation) * topRight + pos;
            topLeft = Quaternion.Euler(0, 0, rotation) * topLeft + pos;

            orig.DrawRect(bottomLeft, bottomRight, topRight, topLeft, color);
        }

        public static void DrawDots(this MonoBehaviour orig, IEnumerable<Vector3> poses, Color color)
        {
            var transform = orig.transform;
            CreateLineMaterial();
            lineMaterial.SetPass(0);
            GL.LoadPixelMatrix();

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.QUADS);

            GL.Color(color);
            float halfWidth = 0.5f;
            float halfHeight = 0.5f;
            foreach (var pos in poses)
            {
                GL.Vertex3(pos.x - halfWidth, pos.y - halfHeight, pos.z);
                GL.Vertex3(pos.x + halfWidth, pos.y - halfHeight, pos.z);
                GL.Vertex3(pos.x + halfWidth, pos.y + halfHeight, pos.z);
                GL.Vertex3(pos.x - halfWidth, pos.y + halfHeight, pos.z);
            }

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawRect(
            this MonoBehaviour orig,
            Vector3 pos1,
            Vector3 pos2,
            Vector3 pos3,
            Vector3 pos4,
            Color color
        )
        {
            var transform = orig.transform;
            CreateLineMaterial();
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.QUADS);

            GL.Color(color);
            GL.Vertex3(pos1.x, pos1.y, pos1.z);
            GL.Vertex3(pos2.x, pos2.y, pos2.z);
            GL.Vertex3(pos3.x, pos3.y, pos3.z);
            GL.Vertex3(pos4.x, pos4.y, pos4.z);

            GL.End();
            GL.PopMatrix();
        }

        public static void DrawWireCircle(
            this MonoBehaviour orig,
            Vector2 center,
            float radius,
            Color color,
            float lineWidth = 1,
            int resolution = 30
        )
        {
            Vector2 prePoint = center + Vector2.right * radius;
            for (int i = 1; i <= resolution; i++)
            {
                Vector2 curPoint = center + Calc.AngleToVector((float)i / resolution * PI(2), radius);
                orig.DrawLine(prePoint, curPoint, color, lineWidth);
                prePoint = curPoint;
            }
        }
    }
}