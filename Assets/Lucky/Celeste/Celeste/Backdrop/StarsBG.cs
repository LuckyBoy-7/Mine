using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lucky.Celeste.Helpers;
using Lucky.Celeste.Monocle;
using Lucky.Extensions;
using Lucky.GL_;
using Lucky.Loader;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class StarsBG : Backdrop
    {
        private const int StarCount = 100;
        private Star[] stars;
        private Color[] colors;
        private List<List<Sprite>> textures;
        private float falling;
        private Color tealColor = Calc.HexToColor("008080");
        public bool isDreaming;
        private TextureDrawHelper draw;
        protected float fade;

        private struct Star
        {
            public Vector2 Position; // 位置
            public int TextureSet; // 纹理id
            public float Timer; // 对应sine函数的x
            public float Rate; // 对应sine函数的w
        }

        protected override void Awake()
        {
            base.Awake();
            textures = new List<List<Sprite>>
            {
                Res.LoadSubtextures("Graphics/Celeste/Gameplay/bgs/02/stars/a"),
                Res.LoadSubtextures("Graphics/Celeste/Gameplay/bgs/02/stars/b"),
                Res.LoadSubtextures("Graphics/Celeste/Gameplay/bgs/02/stars/c")
            };
            draw = GetComponent<TextureDrawHelper>();

            stars = new Star[StarCount];
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new Star
                {
                    Position = new Vector2(Calc.Random.NextFloat(ScreenWidth), Calc.Random.NextFloat(ScreenHeight)), // 抽一个位置
                    Timer = Calc.Random.NextFloat(PI(2)), // 抽一个初相
                    Rate = 2f + Calc.Random.NextFloat(2f), // 抽一个下落速度和动画帧速度
                    TextureSet = Calc.Random.Next(textures.Count) // 抽一个星星类型
                };
            }

            // 一个star对应的8个层次， 越往上越暗
            colors = new Color[8];
            for (int j = 0; j < colors.Length; j++)
                colors[j] = tealColor * 0.7f * (1f - j / (float)colors.Length); // 越往右越暗，顺便调了透明度
        }

        public void Update()
        {
            fade = Calc.Approach(fade, isVisible ? 1 : 0, Time.deltaTime);
            if (isVisible)
            {
                for (int i = 0; i < stars.Length; i++)
                    stars[i].Timer += Time.deltaTime * stars[i].Rate;

                if (isDreaming)
                    falling += Time.deltaTime * 12f;
            }
        }

        private void OnRenderObject()
        {
            draw.Clear();

            int starCount = StarCount;
            if (isDreaming)
                color = tealColor * 0.7f;
            else
                starCount /= 2;

            for (int i = 0; i < starCount; i++)
            {
                List<Sprite> subtextures = textures[stars[i].TextureSet];
                // 范围是[0, subtextures.Count - 1]，变化的速度是正弦式的
                int id = (int)((Math.Sin(stars[i].Timer) + 1.0) / 2.0 * subtextures.Count) % subtextures.Count;
                Vector2 position = stars[i].Position;
                position.x = Calc.Mod(position.x - camera.transform.position.x, ScreenWidth);
                Sprite sprite = subtextures[id];
                if (isDreaming)
                {
                    position.y -= camera.transform.position.y;
                    position.y -= falling * stars[i].Rate;
                    position.y = Calc.Mod(position.y, ScreenHeight);

                    for (int j = 0; j < colors.Length; j++)
                        draw.Draw(sprite, position + Vector2.up * j, colors[j].WithA(colors[j].a * fade), Vector2.one, useWorldPos: false);
                }

                draw.Draw(sprite, position, color.WithA(color.a * fade), Vector2.one, useWorldPos: false); // 这样就能在不影响原来透明度的情况下渐变
            }
        }
    }
}