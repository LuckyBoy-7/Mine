using System;
using Lucky.Celeste.Helpers;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using Lucky.Loader;
using Lucky.Utilities;
using UnityEngine;
using Color = UnityEngine.Color;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.Backdrop
{
    /// <summary>
    /// 大概就是中间黑洞图片分层次，然后旋转，缩放一点在变回去（比如1 -> 0.75f，然后直接变到1，只要周期形状对上就看不出来，然后背景可以用矩形填充，这里暂时改相机颜色，因为sr和GL逻辑不通），
    /// stream就是一个个流向黑洞中心的矩形，只不过前后变长缩放不同，营造拉伸感
    /// particle就是一个个流向黑洞中心的粒子
    /// </summary>
    public class BlackholeBG : Backdrop
    {
        private struct StreamParticle
        {
            public Sprite Texture;
            public float Percent;
            public float Speed;
            public Vector2 Normal;
        }

        private struct Particle
        {
            public int Color;
            public Vector2 Normal;
            public float Percent;
        }


        private const string STRENGTH_FLAG = "blackhole_strength";
        private const int BG_STEPS = 20;
        private const int STREAM_MIN_COUNT = 30;
        private const int STREAM_MAX_COUNT = 50;
        private const int PARTICLE_MIN_COUNT = 150;
        private const int PARTICLE_MAX_COUNT = 220;
        private const int SPIRAL_MIN_COUNT = 0;
        private const int SPIRAL_MAX_COUNT = 10;
        private const int SPIRAL_SEGMENTS = 12;
        private Color[] colorsMild;
        private Color[] colorsWild;
        private Color[] colorsLerp;
        private Color[,] colorsLerpTransparent;
        private const int colorSteps = 20;

        public float Direction;
        public float StrengthMultiplier;
        public Vector2 CenterOffset;
        public Vector2 OffsetOffset;
        public Vector2 Wind;
        private TextureDrawHelper draw;

        private Strengths strength;
        private Color bgColorInner;
        private Color bgColorOuterMild;
        private Color bgColorOuterWild;
        private Sprite bgTexture;
        private StreamParticle[] streams;
        private Particle[] particles;
        private Vector2 center;
        private Vector2 offset;
        private Vector2 shake;
        private float spinTime;
        private bool checkedFlag;
        private Vector3[] streamVerts = new Vector3[300];
        public int StreamCount => (int)Mathf.Lerp(30f, 50f, (StrengthMultiplier - 1f) / 3f);
        public int ParticleCount => (int)Mathf.Lerp(150f, 220f, (StrengthMultiplier - 1f) / 3f);

        public enum Strengths
        {
            Mild,
            Medium,
            High,
            Wild
        }


        protected override void Awake()
        {
            base.Awake();
            draw = GetComponent<TextureDrawHelper>();
            // 强度较小时黑洞的颜色
            colorsMild = new[]
            {
                Calc.HexToColor("6e3199") * 0.8f,
                Calc.HexToColor("851f91") * 0.8f,
                Calc.HexToColor("3026b0") * 0.8f
            };
            // 强度较大时黑洞的颜色
            colorsWild = new[]
            {
                Calc.HexToColor("ca4ca7"),
                Calc.HexToColor("b14cca"),
                Calc.HexToColor("ca4ca7")
            };
            Direction = 1f;
            StrengthMultiplier = 1f;
            bgColorInner = Calc.HexToColor("000000");
            bgColorOuterMild = Calc.HexToColor("512a8b");
            bgColorOuterWild = Calc.HexToColor("bd2192");
            streams = new StreamParticle[STREAM_MAX_COUNT];
            particles = new Particle[PARTICLE_MAX_COUNT];
            bgTexture = Res.Load<Sprite>("Graphics/Celeste/Gameplay/objects/temple/portal/portal");
            int num = 0;
            for (int i = 0; i < STREAM_MAX_COUNT; i++)
            {
                streams[i].Percent = Calc.Random.NextFloat();
                streams[i].Speed = Calc.Random.Range(0.2f, 0.4f);
                streams[i].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * PI(2), 1f);
                num += 6;
            }


            for (int l = 0; l < PARTICLE_MAX_COUNT; l++)
            {
                particles[l].Percent = Calc.Random.NextFloat();
                particles[l].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * PI(2), 1f);
                particles[l].Color = Calc.Random.Next(colorsMild.Length);
            }

            center = new Vector2(ScreenWidth, ScreenHeight) / 2f;
            offset = Vector2.zero;
            colorsLerp = new Color[colorsMild.Length];
            colorsLerpTransparent = new Color[colorsMild.Length, colorSteps];
        }


        public void Update()
        {
            if (!isVisible)
                return;

            // 使强度逐渐靠近enum对应int的位置
            StrengthMultiplier = Calc.Approach(StrengthMultiplier, 1f + (float)strength, Time.deltaTime * 0.1f);
            // 根据强度lerp黑洞颜色
            if (Timer.OnInterval(0.05f))
            {
                for (int i = 0; i < colorsMild.Length; i++)
                {
                    colorsLerp[i] = Color.Lerp(colorsMild[i], colorsWild[i], (StrengthMultiplier - 1f) / 3f);
                    for (int j = 0; j < 20; j++)
                        colorsLerpTransparent[i, j] = Color.Lerp(colorsLerp[i], new Color(), j / 19f);
                }
            }

            float rate = 1f + (StrengthMultiplier - 1f) * 0.7f;
            int streamCount = StreamCount;
            int vId = 0;
            // stream部分
            for (int k = 0; k < streamCount; k++)
            {
                streams[k].Percent += streams[k].Speed * Time.deltaTime * rate * Direction;
                // percent到点了就再随个方向
                if (streams[k].Percent >= 1f && Direction > 0f)
                {
                    streams[k].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * PI(2), 1f);
                    streams[k].Percent -= 1f;
                }
                else if (streams[k].Percent < 0f && Direction < 0f)
                {
                    streams[k].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * PI(2), 1f);
                    streams[k].Percent += 1f;
                }

                // 根据stream的方向和percent来绘制一个离黑洞中心近的边短，远的边长的梯形(营造拉伸感)
                float percent = streams[k].Percent;
                float easeShorEdge = Ease.CubicEaseIn(Calc.ClampedMap(percent, 0f, 0.8f));
                float easeLongEdge = Ease.CubicEaseIn(Calc.ClampedMap(percent, 0.2f, 1f));
                Vector2 normal = streams[k].Normal;
                Vector2 perpendicular = normal.Perpendicular();
                // 这个是被黑洞吸进去的短的边的偏移
                Vector2 shortEdgeCenter = normal * 16f + normal * ((1f - easeShorEdge) * 200f);
                float shortEdgeHalfVector = (1f - easeShorEdge) * 8f;
                Vector2 longtEdgeCenter = normal * 16f + normal * ((1f - easeLongEdge) * 280f);
                float longEdgeHalfVector = (1f - easeLongEdge) * 8f;
                Vector2 vector4 = shortEdgeCenter - perpendicular * shortEdgeHalfVector + center;
                Vector2 vector5 = shortEdgeCenter + perpendicular * shortEdgeHalfVector + center;
                Vector2 vector6 = longtEdgeCenter + perpendicular * longEdgeHalfVector + center;
                Vector2 vector7 = longtEdgeCenter - perpendicular * longEdgeHalfVector + center;
                AssignVertPosition(streamVerts, vId, ref vector4, ref vector5, ref vector6, ref vector7);
                vId += 6;
            }

            // particle部分，就是随着percent沿着黑洞中心方向运动
            rate = StrengthMultiplier * 0.25f;
            int particleCount = ParticleCount;
            for (int l = 0; l < particleCount; l++)
            {
                particles[l].Percent += Time.deltaTime * rate * Direction;
                if (particles[l].Percent >= 1f && Direction > 0f)
                {
                    particles[l].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * PI(2), 1f);
                    particles[l].Percent -= 1f;
                }
                else if (particles[l].Percent < 0f && Direction < 0f)
                {
                    particles[l].Normal = Calc.AngleToVector(Calc.Random.NextFloat() * PI(2), 1f);
                    particles[l].Percent += 1f;
                }
            }

            // spiral部分，就是随着时间修改percent，由于没有对应api，所以percent不是很用得到，单纯用spin和在数组中的占比就足够营造层次感了


            Vector2 newCenter = new Vector2(ScreenWidth, ScreenHeight) / 2f + Wind * 0.15f + CenterOffset;
            center += (newCenter - center) * (1f - (float)Math.Pow(0.01f, Time.deltaTime));
            Vector2 newOffset = -Wind * 0.25f + OffsetOffset;
            offset += (newOffset - offset) * (1f - (float)Math.Pow(0.01f, Time.deltaTime));
            if (Timer.OnInterval(0.025f)) // 大约两帧随即一次振动
                shake = Calc.AngleToVector(Calc.Random.NextFloat(PI(2)), 2f * (StrengthMultiplier - 1f));

            spinTime += (2f + StrengthMultiplier) * Time.deltaTime;
        }

        private void AssignVertPosition(Vector3[] verts, int v, ref Vector2 a, ref Vector2 b, ref Vector2 c, ref Vector2 d)
        {
            verts[v] = a;
            verts[v + 1] = b;
            verts[v + 2] = c;
            verts[v + 3] = a;
            verts[v + 4] = c;
            verts[v + 5] = d;
        }

        private void OnRenderObject()
        {
            draw.Clear();
            // 根据strength lerp一个base color，然后黑洞的层次再在这个base上lerp
            color = Color.Lerp(bgColorOuterMild, bgColorOuterWild, (StrengthMultiplier - 1f) / 3f);
            for (int i = 0; i < BG_STEPS; i++)
            {
                float k = (1f - spinTime % 1f) * 0.05f + i / (float)BG_STEPS;
                Color color2 = Color.Lerp(bgColorInner, color, Ease.SineEaseOut(k));
                float scale = Calc.ClampedMap(k, 0f, 1f, 0.1f, 4f);
                float rotation = PI(2) * k;
                // 由于GL.draw 和spriterenderer的不一致性，这里还要再转化一下
                Vector2 c = center * transform.localScale.x;
                draw.Draw(bgTexture, c + offset * k + shake * (1f - k), color2, Vector2.one * scale, rotation);
            }

            this.DrawTriangle3By3WithRandomColor(streamVerts);

            int particleCount = ParticleCount;
            for (int j = 0; j < particleCount; j++)
            {
                float k = Ease.CubicEaseIn(Calc.Clamp(particles[j].Percent, 0f, 1f));
                Vector2 pos = center + particles[j].Normal * Calc.ClampedMap(k, 1f, 0f, 8f, 220f);
                Color color3 = colorsLerpTransparent[particles[j].Color, (int)(k * 19f)];
                float size = 1f + (1f - k) * 1.5f;
                this.DrawDot(pos, color3, new Vector2(size, size));
            }
        }
    }
}