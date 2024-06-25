using System;
using Lucky.Celeste.Monocle;
using UnityEngine;
using Random = System.Random;

namespace Lucky.Celeste.Celeste
{
    public class Wire : MonoBehaviour
    {
        public Transform from;
        public Transform to;
        public Color Color;
        public SimpleCurve Curve;
        private float sineX;
        private float sineY;
        public float scale = 0.08f;

        private void Start()
        {
            Curve = new SimpleCurve(from.position, to.position, Vector2.zero);
            Random random = new Random((int)Mathf.Min(from.position.x, to.position.y));
            sineX = (float)random.NextDouble() * 4;
            sineY = (float)random.NextDouble() * 4;
        }

        private void Update()
        {
            // 这里感觉原来是加了风的扰动（对Control点），算上了看不见的和看得见的，应该是这样（
            Vector2 vector = new Vector2((float)Math.Sin(sineX + Time.time * 2f), (float)Math.Sin(sineY + Time.time * 2.8f)) * 8f;
            // 因为蔚蓝是像素级的，所以这里加个scale
            Curve.Control = (Curve.Begin + Curve.End) / 2f - (new Vector2(0f, 24f) - vector) * scale;
            Curve.Render(Color, 100);
        }
    }
}