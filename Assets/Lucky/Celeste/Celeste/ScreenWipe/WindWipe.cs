using System;
using System.Collections.Generic;
using Lucky.GL_;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lucky.Celeste.Celeste
{
    // 两个方块为一组，行与行之间错开来，改ratioY
    public class WindWipe : MonoBehaviour
    {
        public bool WipeIn;
        [Range(0, 1)] public float Percent;
        [Range(0, 1)] public float amplitude = 300;

        private int size = 40; // 每个块的大小
        private int columns;
        private int rows;

        private void Awake()
        {
            float cameraHeight = Camera.main.orthographicSize * 2;
            transform.localScale = Vector3.one / (1080 / cameraHeight);
        }

        private void OnRenderObject()
        {
            columns = 1920 / size;
            rows = 1080 / size;

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    // float percentX = ((y + x) % 2 + x) / (float)columns;
                    // 这个块的的x位置在width中所占的分量
                    // 可以从x / columns开始理解，这是最简单的占比方式
                    // 然后加上 (x % 2 + x) / columns，就可以把奇偶位置的x分到一组
                    // 然后加上 ((y + x) % 2 + x) / columns，就可以把奇偶位置的x分到一组的同时在y方向又是交错的

                    // float percentX = ((y + x) % 2 + x) / (float)columns * (1 - amplitude);
                    // // Percent [0, 1]
                    // // percentX [0, 1 - amplitude]
                    // // Percent - percentX [amplitude - 1, 1]
                    // // 分成三个部分[amplitude - 1, 0], [0, amplitude], [amplitude, 1]
                    // // ratioY             0               lerp            1
                    // float ratioY = Math.Clamp(Percent - percentX, 0, amplitude) / amplitude;

                    float percentX = ((y + x) % 2 + x) / (float)columns * (1 - amplitude);
                    float ratioY = Math.Clamp(Percent - percentX, 0, amplitude) / amplitude;
                    if (WipeIn)
                        ratioY = 1 - ratioY;

                    float leftX = x * size;
                    float bottomY = y * size + (1 - ratioY) * 0.5f * size;
                    float rightX = leftX + size;
                    float topY = bottomY + size * ratioY;

                    this.DrawRect(
                        new Vector3(leftX, bottomY, 0f), // 左下
                        new Vector3(rightX, bottomY, 0f), // 右下
                        new Vector3(rightX, topY, 0f), // 右上
                        new Vector3(leftX, topY, 0f), // 左上
                        new Color(x * 0.05f, y * 0.05f, 100) // 这样棱角看的清楚点
                    );
                }
            }
        }
    }
}