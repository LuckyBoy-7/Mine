using Lucky.GL_;
using UnityEngine;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class MountainWipe : ScreenWipe
    {
        private Vector3[] vertexBuffer = new Vector3[9];
        [Range(0, 1)] public float Percent;
        public bool WipeIn;

        private void OnRenderObject()
        {
            int height = 1080;
            // [0, 2*height]
            Vector2 center = new Vector2(960f, height * 2 * Percent);
            // [-height, height]
            Vector2 left = new Vector2(0, height * 2 * (Percent - 0.5f));
            Vector2 right = new Vector2(1920, height * 2 * (Percent - 0.5f));
            if (WipeIn)
            {
                vertexBuffer[0] = center;
                vertexBuffer[1] = left;
                vertexBuffer[2] = right;

                vertexBuffer[3] = left;
                vertexBuffer[4] = right;
                vertexBuffer[5] = new Vector3(left.x, left.y - height);

                vertexBuffer[6] = right;
                vertexBuffer[8] = new Vector3(right.x, right.y - height);
                vertexBuffer[7] = new Vector3(left.x, left.y - height);
            }
            else
            {
                vertexBuffer[0] = new Vector3(left.x, center.y + height);
                vertexBuffer[1] = new Vector3(right.x, center.y + height);
                vertexBuffer[2] = center;

                vertexBuffer[3] = new Vector3(left.x, center.y + height);
                vertexBuffer[4] = center;
                vertexBuffer[5] = left;

                vertexBuffer[6] = new Vector3(right.x, center.y + height);
                vertexBuffer[7] = right;
                vertexBuffer[8] = center;
            }

            this.DrawTriangle3By3WithRandomColor(vertexBuffer);
        }
    }
}