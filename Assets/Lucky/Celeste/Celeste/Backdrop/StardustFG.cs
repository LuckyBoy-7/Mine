using System;
using System.Runtime.CompilerServices;
using Lucky.Celeste.Monocle;
using Lucky.Extensions;
using Lucky.GL_;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class StardustFG : Backdrop
    {
        private static Color[] colors =
        {
            Calc.HexToColor("4cccef"),
            Calc.HexToColor("f243bd"),
            Calc.HexToColor("42f1dd")
        };

        private Particle[] particles = new Particle[50];
        private float fade;
        private Vector2 scale = Vector2.one;

        // 先这么写（
        [Range(-1200, 1200)] public float windX;
        [Range(-1200, 1200)] public float windY;
        private Vector2 Wind => new(windX, windY);

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


        private void ResetParticle(int i, float p)
        {
            // 进度(用来控制fade)
            particles[i].Percent = p;
            // 位置
            particles[i].Position = new Vector2(Calc.Random.Range(0, 320), Calc.Random.Range(0, 180));
            // 速度
            particles[i].Speed = Calc.Random.Range(4, 14);
            // 旋转速度
            particles[i].Spin = Calc.Random.Range(0.1f, 0.15f);
            // 周期
            particles[i].Duration = Calc.Random.Range(1f, 4f);
            // 移动方向
            particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat(PI(2)), 1f);
            // 颜色
            particles[i].Color = Calc.Random.Next(colors.Length);
        }

        public void Update()
        {
            bool isDirX = Wind.y == 0f;
            // 处理后的windSpeed，同时设置粒子缩放
            Vector2 windSpeed = Vector2.zero;
            if (isDirX)
            {
                scale.x = Math.Max(1f, Math.Abs(Wind.x) / 100f);
                scale.y = 1f;
                windSpeed = new Vector2(Wind.x, 0f);
            }
            else
            {
                scale.x = 1f;
                scale.y = Math.Max(1f, Math.Abs(Wind.y) / 40f);
                windSpeed = new Vector2(0f, Wind.y * 2f);
            }

            // 移动粒子
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].Percent >= 1f)
                {
                    ResetParticle(i, 0f);
                }

                particles[i].Percent += Time.deltaTime / particles[i].Duration;
                particles[i].Position += (particles[i].Direction * particles[i].Speed + windSpeed) * Time.deltaTime;
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

                this.DrawDot(pos, colors[particles[i].Color].WithA(fade * fadeControl), new Vector2(scale.x, scale.y));
            }
        }
    }
}