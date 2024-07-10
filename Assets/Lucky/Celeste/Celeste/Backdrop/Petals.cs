using System;
using System.Runtime.CompilerServices;
using Lucky.Celeste.Helpers;
using Lucky.Celeste.Monocle;
using Lucky.Loader;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class Petals : Backdrop
    {
        private static Color[] colors = { Calc.HexToColor("ff3aa3") };
        private Particle[] particles = new Particle[40];
        private float fade;
        private TextureDrawHelper draw;

        private struct Particle
        {
            public Vector2 Position;
            public float Speed;
            public float Spin;
            public float MaxRotate;
            public int Color;
            public float RotationCounter;
        }

        protected override void Awake()
        {
            base.Awake();
            draw = GetComponent<TextureDrawHelper>();
            for (int i = 0; i < particles.Length; i++)
                ResetParticle(i);
        }

        private void ResetParticle(int i)
        {
            particles[i].Position = new Vector2(Calc.Random.Range(0, ScreenWidth + 32), Calc.Random.Range(0, ScreenHeight + 32));
            particles[i].Speed = -Calc.Random.Range(6f, 16f);
            particles[i].Spin = Calc.Random.Range(8f, 12f) * 0.2f; // 旋转速度
            particles[i].Color = Calc.Random.Next(colors.Length);
            particles[i].RotationCounter = Calc.Random.NextAngle(); // sine的自变量
            particles[i].MaxRotate = Calc.Random.Range(0.3f, 0.6f) * PI(-0.5f); // rotate的系数（感觉和spin重了？）
        }

        public void Update()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Position.y += particles[i].Speed * Time.deltaTime;
                particles[i].RotationCounter += particles[i].Spin * Time.deltaTime;
            }

            fade = Calc.Approach(fade, isVisible ? 1f : 0f, Time.deltaTime);
        }

        private void OnRenderObject()
        {
            draw.Clear();
            if (fade <= 0f)
                return;
            Sprite mtexture = Res.Load<Sprite>("Graphics/Celeste/Gameplay/particles/petal");
            for (int i = 0; i < particles.Length; i++)
            {
                Vector2 vector = default(Vector2);
                vector.x = -16f + Calc.Mod(particles[i].Position.x - camera.transform.position.x, ScreenWidth + 32);
                vector.y = -16f + Calc.Mod(particles[i].Position.y - camera.transform.position.y, ScreenHeight + 32);
                // 同时表示了一定的偏移和旋转
                float k = (float)(PI(-0.5f) + Math.Sin(particles[i].RotationCounter * particles[i].MaxRotate));
                vector += Calc.AngleToVector(k, 4f);
                draw.Draw(mtexture, vector, colors[particles[i].Color] * fade, Vector2.one, k - 0.8f, false);
            }
        }
    }
}