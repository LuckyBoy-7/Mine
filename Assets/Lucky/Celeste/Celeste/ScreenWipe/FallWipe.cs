using Lucky.GL_;
using UnityEngine;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class FallWipe : ScreenWipe
    {
        private Vector3[] vertexBuffer = new Vector3[9];
        [Range(0, 1)] public float Percent;
        public bool WipeIn;

        private void OnRenderObject()
        {
            Vector2 center = new Vector2(960f, 1080f - 2160f * Percent);
            Vector2 left = new Vector2(0, 2160f * (1f - Percent));
            Vector2 right = new Vector2(1920, 2160f * (1f - Percent));
            if (WipeIn)
            {
                vertexBuffer[0] = center;
                vertexBuffer[1] = left;
                vertexBuffer[2] = right;

                vertexBuffer[3] = left;
                vertexBuffer[4] = right;
                vertexBuffer[5] = new Vector3(left.x, left.y + 1080f);

                vertexBuffer[6] = right;
                vertexBuffer[8] = new Vector3(right.x, right.y + 1080f);
                vertexBuffer[7] = new Vector3(left.x, left.y + 1080f);
            }
            else
            {
                vertexBuffer[0] = new Vector3(left.x, center.y - 1080f);
                vertexBuffer[1] = new Vector3(right.x, center.y - 1080f);
                vertexBuffer[2] = center;

                vertexBuffer[3] = new Vector3(left.x, center.y - 1080f);
                vertexBuffer[4] = center;
                vertexBuffer[5] = left;

                vertexBuffer[6] = new Vector3(right.x, center.y - 1080f);
                vertexBuffer[7] = right;
                vertexBuffer[8] = center;
            }

            this.DrawTriangle3By3WithRandomColor(vertexBuffer);
        }
    }
}