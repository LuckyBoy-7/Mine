using System;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.GL_
{
    public static class Draw
    {
        static Material lineMaterial;

        static void CreateLineMaterial()
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
            params Vector3[] poses
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
            Vector3 pos1,
            float width,
            float height,
            Color color
        )
        {
            var transform = orig.transform;
            CreateLineMaterial();
            lineMaterial.SetPass(0);
            GL.LoadPixelMatrix();

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.QUADS);

            GL.Color(color);
            GL.Vertex3(pos1.x, pos1.y, pos1.z);
            GL.Vertex3(pos1.x + width, pos1.y, pos1.z);
            GL.Vertex3(pos1.x + width, pos1.y + height, pos1.z);
            GL.Vertex3(pos1.x, pos1.y + height, pos1.z);

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
    }
}