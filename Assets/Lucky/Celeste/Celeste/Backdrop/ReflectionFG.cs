using System;
using System.Runtime.CompilerServices;
using Lucky.Celeste.Monocle;
using Lucky.Extensions;
using Lucky.GL_;
using UnityEditor.SearchService;
using UnityEngine;

namespace Lucky.Celeste.Celeste.Backdrop
{
    /// <summary>
    /// 跟stardust差不多
    /// </summary>
    public class ReflectionFG : Backdrop
    {
        private static Color[] colors = { Calc.HexToColor("f52b63") };
        private Particle[] particles = new Particle[50];
        private float fade;

        private struct Particle
        {
            public Vector2 Position;
            public float Percent;
            public float Duration;
            public Vector2 Direction;
            public float Speed;
            public float Spin;
            public int Color;
        }

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < particles.Length; i++)
                ResetParticle(i, Calc.Random.NextFloat());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ResetParticle(int i, float p)
        {
            particles[i].Percent = p;
            particles[i].Position = new Vector2(Calc.Random.Range(0, 320), Calc.Random.Range(0, 180));
            particles[i].Speed = Calc.Random.Range(4, 14);
            particles[i].Spin = Calc.Random.Range(0.1f, 0.15f);
            particles[i].Duration = Calc.Random.Range(1f, 4f);
            particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
            particles[i].Color = Calc.Random.Next(colors.Length);
        }

        public void Update()
        {
            // 移动粒子
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].Percent >= 1f)
                    ResetParticle(i, 0f);

                particles[i].Percent += Time.deltaTime / particles[i].Duration;
                particles[i].Position += particles[i].Direction * (particles[i].Speed * Time.deltaTime);
                particles[i].Direction = particles[i].Direction.Rotate(particles[i].Spin * Time.deltaTime);
            }

            fade = Calc.Approach(fade, isVisible ? 1f : 0f, Time.deltaTime);
        }

        private void OnRenderObject()
        {
            if (fade <= 0f)
                return;

            for (int i = 0; i < particles.Length; i++)
            {
                Vector3 pos = Vector3.zero;
                pos.x = Calc.Mod(particles[i].Position.x - camera.transform.position.x, ScreenWidth);
                pos.y = Calc.Mod(particles[i].Position.y - camera.transform.position.y, ScreenHeight);
                // 根据percent改透明度，形成先实后虚的循环
                float percent = particles[i].Percent;
                float fadeControl = percent < 0.7f ? Calc.ClampedMap(percent, 0f, 0.3f) : Calc.ClampedMap(percent, 0.7f, 1f, 1f, 0f);

                this.DrawDot(pos, colors[particles[i].Color].WithA(fade * fadeControl), 1);
            }
        }
    }
}