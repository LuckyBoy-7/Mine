using System;
using System.Collections.Generic;
using Lucky.Celeste.Monocle;
using UnityEngine;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class Backdrop : MonoBehaviour
    {
        public static float ScreenWidth = 320;
        public static float ScreenHeight = 180;
        public bool isVisible = true;

        private struct Segment
        {
            public float PositionFrom;
            public float PositionTo;
            public float From;
            public float To;
        }

        protected virtual void Awake()
        {
            float cameraHeight = Camera.main.orthographicSize * 2;
            transform.localScale = Vector3.one * cameraHeight / ScreenHeight;
        }

        /// <summary>
        /// 大概原理就是给一个fade很多段segment，然后根据对象位置在每个segment的占比来混合出一个alpha
        /// </summary>
        public class Fader
        {
            private List<Segment> Segments = new();

            public Fader Add(float posFrom, float posTo, float fadeFrom, float fadeTo)
            {
                Segments.Add(new Segment
                {
                    PositionFrom = posFrom,
                    PositionTo = posTo,
                    From = fadeFrom,
                    To = fadeTo
                });
                return this;
            }

            public float Value(float position)
            {
                float num = 1f;
                foreach (Segment segment in Segments)
                    num *= Calc.ClampedMap(position, segment.PositionFrom, segment.PositionTo, segment.From, segment.To);
                return num;
            }
        }
    }
}