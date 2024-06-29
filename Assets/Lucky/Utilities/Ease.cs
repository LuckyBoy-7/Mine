using System;
using Lucky.Extensions;
using UnityEngine;

namespace Lucky.Utilities
{
    /// <summary>
    /// 把ease封装了一下，后续转化更方便
    /// </summary>
    public class Ease
    {
        private float duration;
        private float timestamp;
        private Func<float, float> easeFunction;
        private bool isLoop;

        public Ease(float duration, Func<float, float> easeFunction, bool isLoop = true)
        {
            timestamp = Time.time;
            this.duration = duration;
            this.easeFunction = easeFunction;
            this.isLoop = isLoop;
        }

        private float elapse => Time.time - timestamp;
        public float curVal => easeFunction((elapse / duration).GetDecimal());
        public bool isOver => elapse > duration && !isLoop;

        public static float Linear(float t)
        {
            return t;
        }

        public static float SmoothInOut(float t)
        {
            return 3f * t * t - 2f * t * t * t;
        }

        public static float ExpoEaseOut(float t)
        {
            return t == 1f ? 1f : (-Mathf.Pow(2f, -10f * t) + 1f);
        }

        public static float ExpoEaseIn(float t)
        {
            return (t == 0f) ? 0f : Mathf.Pow(2f, 10f * (t - 1f));
        }

        public static float ExpoEaseInOut(float t)
        {
            bool flag = t == 0f;
            float num;
            if (flag)
            {
                num = 0f;
            }
            else
            {
                bool flag2 = t == 1f;
                if (flag2)
                {
                    num = 1f;
                }
                else
                {
                    t *= 2f;
                    bool flag3 = t < 1f;
                    if (flag3)
                    {
                        num = 0.5f * Mathf.Pow(2f, 10f * (t - 1f));
                    }
                    else
                    {
                        num = 0.5f * (-Mathf.Pow(2f, -10f * (t - 1f)) + 2f);
                    }
                }
            }

            return num;
        }

        public static float ExpoEaseOutIn(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = ExpoEaseOut(2f * t) / 2f;
            }
            else
            {
                num = ExpoEaseIn(2f * t - 1f) / 2f + 0.5f;
            }

            return num;
        }

        public static float CircEaseOut(float t)
        {
            t -= 1f;
            return Mathf.Sqrt(1f - t * t);
        }

        public static float CircEaseIn(float t)
        {
            return -(Mathf.Sqrt(1f - t * t) - 1f);
        }

        public static float CircEaseInOut(float t)
        {
            t *= 2f;
            bool flag = t < 1f;
            float num;
            if (flag)
            {
                num = -0.5f * (Mathf.Sqrt(1f - t * t) - 1f);
            }
            else
            {
                t -= 2f;
                num = 0.5f * (Mathf.Sqrt(1f - t * t) + 1f);
            }

            return num;
        }

        public static float CircEaseOutIn(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = CircEaseOut(2f * t) / 2f;
            }
            else
            {
                num = CircEaseIn(2f * t - 1f) / 2f + 0.5f;
            }

            return num;
        }

        public static float QuadEaseOut(float t)
        {
            return -t * (t - 2f);
        }

        public static float QuadEaseIn(float t)
        {
            return t * t;
        }

        public static float QuadEaseInOut(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = QuadEaseOut(t * 2f) * 0.5f;
            }
            else
            {
                num = QuadEaseIn(2f * t - 1f) * 0.5f + 0.5f;
            }

            return num;
        }

        public static float QuadEaseOutIn(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = QuadEaseOut(t * 2f) * 0.5f;
            }
            else
            {
                num = QuadEaseIn(2f * t - 1f) * 0.5f + 0.5f;
            }

            return num;
        }

        public static float SineEaseOut(float t)
        {
            return Mathf.Sin(t * 3.1415927f / 2f);
        }

        public static float SineEaseIn(float t)
        {
            return -Mathf.Cos(t * 3.1415927f / 2f) + 1f;
        }

        public static float SineEaseInOut(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = SineEaseOut(2f * t) / 2f;
            }
            else
            {
                num = SineEaseIn(2f * t - 1f) / 2f + 0.5f;
            }

            return num;
        }

        public static float SineEaseOutIn(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = SineEaseOut(2f * t) / 2f;
            }
            else
            {
                num = SineEaseIn(2f * t - 1f) / 2f + 0.5f;
            }

            return num;
        }

        public static float CubicEaseOut(float t)
        {
            t -= 1f;
            return t * t * t + 1f;
        }

        public static float CubicEaseIn(float t)
        {
            return t * t * t;
        }

        public static float CubicEaseInOut(float t)
        {
            t *= 2f;
            bool flag = t < 1f;
            float num;
            if (flag)
            {
                num = 0.5f * t * t * t;
            }
            else
            {
                t -= 2f;
                num = 0.5f * (t * t * t + 2f);
            }

            return num;
        }

        public static float CubicEaseOutIn(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = CubicEaseOut(2f * t) / 2f;
            }
            else
            {
                num = CubicEaseIn(2f * t - 1f) / 2f + 0.5f;
            }

            return num;
        }

        public static float QuartEaseOut(float t)
        {
            t -= 1f;
            return -(t * t * t * t - 1f);
        }

        public static float QuartEaseIn(float t)
        {
            return t * t * t * t;
        }

        public static float QuartEaseInOut(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = CubicEaseOut(2f * t) / 2f;
            }
            else
            {
                num = CubicEaseIn(2f * t - 1f) / 2f + 0.5f;
            }

            return num;
        }

        public static float QuartEaseOutIn(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = CubicEaseOut(2f * t) / 2f;
            }
            else
            {
                num = CubicEaseIn(2f * t - 1f) / 2f + 0.5f;
            }

            return num;
        }

        public static float QuintEaseOut(float t)
        {
            t -= 1f;
            return t * t * t * t * t + 1f;
        }

        public static float QuintEaseIn(float t)
        {
            return t * t * t * t * t;
        }

        public static float QuintEaseInOut(float t)
        {
            t *= 2f;
            bool flag = t < 1f;
            float num;
            if (flag)
            {
                num = 0.5f * t * t * t * t * t;
            }
            else
            {
                t -= 2f;
                num = 0.5f * (t * t * t * t * t + 2f);
            }

            return num;
        }

        public static float QuintEaseOutIn(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = QuintEaseOut(2f * t) / 2f;
            }
            else
            {
                num = QuintEaseIn(2f * t - 1f) / 2f + 0.5f;
            }

            return num;
        }

        public static float ElasticEaseOut(float t)
        {
            bool flag = t == 1f;
            float num;
            if (flag)
            {
                num = 1f;
            }
            else
            {
                float p = 0.3f;
                float s = p / 4f;
                num = Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - s) * 6.2831855f / p) + 1f;
            }

            return num;
        }

        public static float ElasticEaseIn(float t)
        {
            bool flag = t == 1f;
            float num;
            if (flag)
            {
                num = 1f;
            }
            else
            {
                num = -(Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t - 0.075f) * 6.2831855f / 0.3f));
            }

            return num;
        }

        public static float BounceEaseOut(float t)
        {
            bool flag = t < 0.36363637f;
            float num;
            if (flag)
            {
                num = 7.5625f * t * t;
            }
            else
            {
                bool flag2 = t < 0.72727275f;
                if (flag2)
                {
                    num = 7.5625f * (t -= 0.54545456f) * t + 0.75f;
                }
                else
                {
                    bool flag3 = t < 0.9090909090909091;
                    if (flag3)
                    {
                        num = 7.5625f * (t -= 0.8181818f) * t + 0.9375f;
                    }
                    else
                    {
                        num = 7.5625f * (t -= 0.95454544f) * t + 0.984375f;
                    }
                }
            }

            return num;
        }

        public static float BounceEaseIn(float t)
        {
            return 1f - BounceEaseOut(1f - t);
        }

        public static float BackEaseOut(float t)
        {
            t -= 1f;
            return t * t * (2.70158f * t + 1.70158f) + 1f;
        }

        public static float BackEaseIn(float t)
        {
            return t * t * (2.70158f * t - 1.70158f);
        }

        public static float BackEaseInOut(float t)
        {
            float s = 1.70158f;
            t *= 2f;
            bool flag = t < 1f;
            float num;
            if (flag)
            {
                s *= 1.525f;
                num = 0.5f * (t * t * ((s + 1f) * t - s));
            }
            else
            {
                t -= 2f;
                s *= 1.525f;
                num = 0.5f * (t * t * ((s + 1f) * t + s) + 2f);
            }

            return num;
        }

        public static float BackEaseOutIn(float t)
        {
            bool flag = t < 0.5f;
            float num;
            if (flag)
            {
                num = BackEaseOut(2f * t) / 2f;
            }
            else
            {
                num = BackEaseIn(2f * t - 1f) / 2f + 0.5f;
            }

            return num;
        }

        public static float CosPulse(float t)
        {
            float normalizedT = 3.1415927f * t * 2f;
            return (-Mathf.Cos(normalizedT) + 1f) * 0.5f;
        }

        public static float Impulse(float t, float k = 12f)
        {
            float h = k * t;
            return h * Mathf.Exp(1f - h);
        }

        public static float pcurve(float x, float a = 5f, float b = 5f)
        {
            float i = Mathf.Pow(a + b, a + b) / (Mathf.Pow(a, a) * Mathf.Pow(b, b));
            return i * Mathf.Pow(x, a) * Mathf.Pow(1f - x, b);
        }

        public enum FunctionType
        {
            Linear,
            SmoothInOut,
            Impulse,
            ExpoEaseOut,
            ExpoEaseIn,
            ExpoEaseInOut,
            ExpoEaseOutIn,
            CircEaseOut,
            CircEaseIn,
            CircEaseInOut,
            CircEaseOutIn,
            QuadEaseOut,
            QuadEaseIn,
            QuadEaseInOut,
            QuadEaseOutIn,
            SineEaseOut,
            SineEaseIn,
            SineEaseInOut,
            SineEaseOutIn,
            CubicEaseOut,
            CubicEaseIn,
            CubicEaseInOut,
            CubicEaseOutIn,
            QuartEaseOut,
            QuartEaseIn,
            QuartEaseInOut,
            QuartEaseOutIn,
            QuintEaseOut,
            QuintEaseIn,
            QuintEaseInOut,
            QuintEaseOutIn,
            ElasticEaseOut,
            ElasticEaseIn,
            BounceEaseOut,
            BounceEaseIn,
            BackEaseOut,
            BackEaseIn,
            BackEaseInOut,
            BackEaseOutIn
        }
    }
}