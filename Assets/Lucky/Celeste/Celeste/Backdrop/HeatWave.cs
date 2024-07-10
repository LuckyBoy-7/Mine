using System;
using System.Runtime.CompilerServices;
using Lucky.Celeste.Helpers;
using Lucky.Celeste.Monocle;
using Lucky.Extensions;
using Lucky.GL_;
using Lucky.Loader;
using UnityEngine;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class HeatWave : Backdrop
    {
        public enum CoreModeType
        {
            None,
            Hot,
            Cold
        }

        public CoreModeType coreModeType = CoreModeType.Hot;

        private static Color[] hotColors = new Color[]
        {
            Color.red,
            Calc.HexToColor("ffa500")
        };

        private static Color[] coldColors = new Color[]
        {
            Calc.HexToColor("87cefa"),
            Calc.HexToColor("008080")
        };

        private Color[] currentColors;
        private float colorLerp;
        private Particle[] particles = new Particle[50];
        private float fade;
        private float heat;
        private Parallax mist1;
        private Parallax mist2;
        private bool show;
        private bool wasShow;

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

            currentColors = new Color[hotColors.Length];
            colorLerp = 1f;
            mist1 = new GameObject("mist1").AddComponent<TextureDrawHelper>().gameObject.AddComponent<Parallax>();
            mist1.LoopX = mist1.LoopY = true;
            mist1.sprite = Res.Load<Sprite>("Graphics/Celeste/Misc/mist");
            mist2 = new GameObject("mist2").AddComponent<TextureDrawHelper>().gameObject.AddComponent<Parallax>();
            mist2.LoopX = mist2.LoopY = true;
            mist2.sprite = Res.Load<Sprite>("Graphics/Celeste/Misc/mist");
        }

        private void ResetParticle(int i, float p)
        {
            particles[i].Percent = p;
            particles[i].Position = new Vector2(Calc.Random.Range(0, 320), Calc.Random.Range(0, 180));
            particles[i].Speed = Calc.Random.Range(4, 14);
            particles[i].Spin = Calc.Random.Range(0.1f, 0.15f);
            particles[i].Duration = Calc.Random.Range(1f, 4f);
            particles[i].Direction = Calc.AngleToVector(Calc.Random.NextFloat(6.2831855f), 1f);
            particles[i].Color = Calc.Random.Next(hotColors.Length);
        }

        public void Update()
        {
            show = isVisible && (int)coreModeType > (int)CoreModeType.None;
            if (show)
            {
                if (!wasShow)
                    colorLerp = coreModeType == CoreModeType.Hot ? 1 : 0;


                colorLerp = Calc.Approach(colorLerp, coreModeType == CoreModeType.Hot ? 1 : 0, Time.deltaTime * 100f);
                for (int i = 0; i < currentColors.Length; i++)
                {
                    currentColors[i] = Color.Lerp(coldColors[i], hotColors[i], colorLerp);
                }
            }

            for (int j = 0; j < particles.Length; j++)
            {
                if (particles[j].Percent >= 1f)
                    ResetParticle(j, 0f);

                float speedRate = 1f;
                if (coreModeType == CoreModeType.Cold) // 冷的自然动的慢（
                    speedRate = 0.25f;

                particles[j].Percent += Time.deltaTime / particles[j].Duration;
                particles[j].Position += particles[j].Direction * (particles[j].Speed * speedRate * Time.deltaTime);
                particles[j].Direction.Rotate(particles[j].Spin * Time.deltaTime);
                if (coreModeType == CoreModeType.Hot)
                    particles[j].Position.y += 10f * Time.deltaTime;
            }

            fade = Calc.Approach(fade, show ? 1f : 0f, Time.deltaTime);
            heat = Calc.Approach(heat, (show && coreModeType == CoreModeType.Hot) ? 1f : 0f, Time.deltaTime * 100f);
            mist1.color = Color.Lerp(Calc.HexToColor("639bff"), Calc.HexToColor("f1b22b"), heat) * fade * 0.7f;
            mist2.color = Color.Lerp(Calc.HexToColor("5fcde4"), Calc.HexToColor("f12b3a"), heat) * fade * 0.7f;
            mist1.Speed = new Vector2(4f, -20f) * heat;
            mist2.Speed = new Vector2(4f, -40f) * heat;

            wasShow = show;
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

                this.DrawDot(pos, currentColors[particles[i].Color].WithA(fade * fadeControl), 1);
            }
        }
    }
}