using System;
using System.Drawing;
using Lucky.Celeste.Helpers;
using Lucky.Celeste.Monocle;
using Lucky.Extensions;
using Lucky.GL_;
using Lucky.Loader;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class Parallax : Backdrop
    {
        public Vector2 CameraOffset = Vector2.zero;
        public Sprite sprite;
        public bool DoFadeIn;
        public float Alpha = 1f;
        private float fadeIn = 1f;
        public Vector2 Speed;
        private TextureDrawHelper draw;
        public bool LoopX;
        public bool LoopY;
        public bool FlipX;
        public bool FlipY;
        private Vector2 startPos;

        protected override void Awake()
        {
            base.Awake();
            sprite = Res.Load<Sprite>("Graphics/Celeste/Gameplay/bgs/00/bg0");
            draw = GetComponent<TextureDrawHelper>();
            startPos = transform.position;
        }

        public void Update()
        {
            startPos -= Speed * Time.deltaTime;
            if (DoFadeIn)
            {
                fadeIn = Calc.Approach(fadeIn, isVisible ? 1 : 0, Time.deltaTime);
                return;
            }

            fadeIn = isVisible ? 1 : 0;
        }

        private void OnRenderObject()
        {
            draw.Clear();
            Vector2 offset = (Vector2)camera.transform.position + CameraOffset - startPos;
            color = color.WithA(fadeIn * Alpha);
            float width = sprite.texture.width * transform.localScale.x;
            float height = sprite.texture.height * transform.localScale.x;
            if (LoopX)
                offset.x = Calc.Mod(offset.x, width);
            if (LoopY)
                offset.y = Calc.Mod(offset.y, height);

            Vector2 scale = Vector2.one;
            if (FlipX)
                scale.x *= -1;
            if (FlipY)
                scale.y *= -1;

            draw.Draw(sprite, camera.transform.position.WithZ(0) + (Vector3)offset, color, scale, useWorldPos: true);
            draw.Draw(sprite, camera.transform.position.WithZ(0) + (Vector3)offset + new Vector3(-width, -height), color, scale, useWorldPos: true);
            draw.Draw(sprite, camera.transform.position.WithZ(0) + (Vector3)offset + new Vector3(-width, 0), color, scale, useWorldPos: true);
            draw.Draw(sprite, camera.transform.position.WithZ(0) + (Vector3)offset + new Vector3(0, -height), color, scale, useWorldPos: true);
        }
    }
}