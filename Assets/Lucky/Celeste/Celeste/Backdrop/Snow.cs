using System;
using Lucky.Celeste.Monocle;
using Lucky.Extensions;
using Lucky.GL_;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class Snow : Backdrop
    {
        private static Color[] ForegroundColors =
        {
            Color.white,
            Calc.HexToColor("6495ed"),
        };

        private static Color[] BackgroundColors =
        {
            new(0.2f, 0.2f, 0.2f, 1f),
            new(0.1f, 0.2f, 0.5f, 1f)
        };

        public float Alpha;
        public bool foreground;

        private float visibleFade;
        private Color[] colors;
        private Color[] blendedColors;
        private Particle[] particles;

        private struct Particle
        {
            public void Init(int maxColors, float speedMin, float speedMax)
            {
                Position = new Vector2(Calc.Random.NextFloat(ScreenWidth), Calc.Random.NextFloat(ScreenHeight));
                Color = Calc.Random.Next(maxColors);
                Speed = Calc.Random.Range(speedMin, speedMax);
                Sin = Calc.Random.NextFloat(PI(2));
            }

            public Vector2 Position;
            public int Color;
            public float Speed;
            public float Sin;
        }

        protected override void Awake()
        {
            base.Awake();
            Alpha = 1f;
            visibleFade = 1f;
            particles = new Particle[60];
            colors = (foreground ? ForegroundColors : BackgroundColors);
            blendedColors = new Color[colors.Length];
            int speedMin = foreground ? 120 : 40; // 前景的动的快点
            int speedMax = foreground ? 300 : 100;
            for (int i = 0; i < particles.Length; i++)
                particles[i].Init(colors.Length, speedMin, speedMax);
        }

        public void Update()
        {
            // tween 透明度
            visibleFade = Calc.Approach(visibleFade, isVisible ? 1 : 0, Time.deltaTime * 2f);

            for (int i = 0; i < particles.Length; i++)
            {
                // 水平移动
                particles[i].Position.x -= particles[i].Speed * Time.deltaTime;
                // 垂直移动
                particles[i].Position.y += (float)Math.Sin(particles[i].Sin) * particles[i].Speed * 0.2f * Time.deltaTime;
                // 更新sine
                particles[i].Sin += Time.deltaTime;
            }
        }

        private void OnRenderObject()
        {
            if (Alpha <= 0f || visibleFade <= 0f)
                return;

            for (int i = 0; i < blendedColors.Length; i++)
                blendedColors[i] = colors[i] * (Alpha * visibleFade);

            Camera camera = Camera.main;
            for (int j = 0; j < particles.Length; j++)
            {
                // 穿过相机会从另一侧出来
                Vector2 pos = new Vector2(
                    Calc.Mod(particles[j].Position.x - camera.transform.position.x, ScreenWidth),
                    Calc.Mod(particles[j].Position.y - camera.transform.position.y, ScreenHeight)
                );
                Color color = blendedColors[particles[j].Color];
                this.DrawDot(pos, color);
            }
        }
    }
}