using System;
using System.Runtime.CompilerServices;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using UnityEngine;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class DreamStars : Backdrop
    {
        private Stars[] stars = new Stars[50];
        private Vector2 angle = new Vector2(-2f, 7f).normalized;
        private Vector2 lastCamera = Vector2.zero;

        private struct Stars
        {
            public Vector2 Position;
            public float Speed;
            public float Size;
        }

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].Position = new Vector2(Calc.Random.NextFloat(ScreenWidth), Calc.Random.NextFloat(ScreenHeight));
                stars[i].Speed = 24f + Calc.Random.NextFloat(24f);
                stars[i].Size = 2f + Calc.Random.NextFloat(6f);
            }
        }

        public void Update()
        {
            Vector2 position = camera.transform.position;
            // 随着摄像机移动相对速度减少一定量
            Vector2 vector = position - lastCamera;
            for (int i = 0; i < stars.Length; i++)
                stars[i].Position += (angle * (stars[i].Speed * Time.deltaTime) - vector * 0.5f);

            lastCamera = position;
        }

        private void OnRenderObject()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                this.DrawRect(
                    new Vector2(
                        Calc.Mod(stars[i].Position.x, ScreenWidth),
                        Calc.Mod(stars[i].Position.y, ScreenHeight)
                    ),
                    stars[i].Size,
                    stars[i].Size,
                    Calc.HexToColor("008080"), // teal（青色）色
                    isWire: true
                );
            }
        }
    }
}