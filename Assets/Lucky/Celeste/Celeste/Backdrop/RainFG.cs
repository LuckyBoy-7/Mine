using Lucky.Celeste.Monocle;
using Lucky.GL_;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class RainFG : Backdrop
    {
        public float Alpha = 1f;
        private float visibleFade = 1f;
        private Particle[] particles = new Particle[240];

        private struct Particle
        {
            public void Init()
            {
                Position = new Vector2(-32f + Calc.Random.NextFloat(ScreenWidth + 64), -32f + Calc.Random.NextFloat(ScreenHeight + 64));
                Rotation = PI(0.5f) + Calc.Random.Range(-0.05f, 0.05f);
                Speed = -Calc.AngleToVector(Rotation, Calc.Random.Range(200f, 600f));
                Scale = new Vector2(4f + (Speed.magnitude - 200f) / 400f * 12f, 1f);
            }

            public Vector2 Position;
            public Vector2 Speed;
            public float Rotation;
            public Vector2 Scale;
        }

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < particles.Length; i++)
                particles[i].Init();
        }

        public void Update()
        {
            visibleFade = Calc.Approach(visibleFade, isVisible ? 1 : 0, Time.deltaTime * (isVisible ? 10f : 0.25f));

            for (int i = 0; i < particles.Length; i++)
                particles[i].Position += particles[i].Speed * Time.deltaTime;
        }

        private void OnRenderObject()
        {
            if (Alpha <= 0f || visibleFade <= 0f)
                return;
            color = Calc.HexToColor("161933") * 0.5f * Alpha * visibleFade;
            for (int i = 0; i < particles.Length; i++)
            {
                Vector2 vector = new Vector2(
                    Calc.Mod(particles[i].Position.x - camera.transform.position.x - 32f, ScreenWidth + 64f),
                    Calc.Mod(particles[i].Position.y - camera.transform.position.y - 32f, ScreenHeight + 64f)
                );
                this.DrawDot(vector, color, particles[i].Scale, particles[i].Rotation);
            }
        }
    }
}