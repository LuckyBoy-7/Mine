using System;
using System.Collections.Generic;
using Lucky.Celeste.Helpers;
using Lucky.Celeste.Monocle;
using Lucky.Extensions;
using Lucky.GL_;
using Lucky.Loader;
using UnityEngine;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class Planets : Backdrop
    {
        private Planet[] planets;
        public const int MapWidth = 640;
        public const int MapHeight = 360;
        public string size = "small";
        public int count = 10;
        private TextureDrawHelper draw;

        private struct Planet
        {
            public Sprite Texture;
            public Vector2 Position;
        }

        protected override void Awake()
        {
            base.Awake();
            draw = GetComponent<TextureDrawHelper>();
            List<Sprite> atlasSubtextures = Res.LoadSubtextures("Graphics/Celeste/Gameplay/bgs/10/" + size);
            planets = new Planet[count];
            for (int i = 0; i < planets.Length; i++)
            {
                planets[i].Texture = atlasSubtextures.Choice();
                planets[i].Position = new Vector2
                {
                    x = Calc.Random.NextFloat(MapWidth),
                    y = Calc.Random.NextFloat(MapHeight)
                };
            }
        }

        private void OnRenderObject()
        {
            draw.Clear();
            for (int i = 0; i < planets.Length; i++)
            {
                Vector2 vector = new Vector2
                {
                    x = -32f + Calc.Mod(planets[i].Position.x - camera.transform.position.x, MapWidth),
                    y = -32f + Calc.Mod(planets[i].Position.y - camera.transform.position.y, MapHeight)
                };
                draw.Draw(planets[i].Texture, vector, color, Vector2.one, useWorldPos: false);
            }
        }
    }
}