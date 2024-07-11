using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Lucky.Celeste.Helpers;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using Lucky.Loader;
using Lucky.Utilities;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.Backdrop
{
    /// <summary>
    /// 大概思路是把屏幕按列分成n块区域，每个区域采样（随机）一个y值，总计YNodes
    /// 然后定义星星在YNode之间移动，然后算出sine的法线，在原来路线的基础上做sine运动，整体就是一条厚带子了
    /// 至于粒子突然的抖动，应该是nodeId变化时，法线变化过于剧烈导致的
    /// </summary>
    public class Starfield : Backdrop
    {
        public const int StepSize = 32;
        public const int Steps = 15;
        public const float MinDist = 4f;
        public const float MaxDist = 24f;
        public float FlowSpeed = 1;
        public List<float> YNodes = new();
        public Star[] Stars = new Star[128];
        public Vector2 Scroll = new(1, 1);
        private TextureDrawHelper draw;

        public struct Star
        {
            public Sprite Texture;
            public Vector2 Position;
            public Color Color;
            public int NodeIndex; // 对应y node的下标
            public float NodePercent; // 计时器，记到一后NodeIndex++，同时也表示两个节点之间的lerp k
            public float Distance;
            public float Sine;
        }


        protected override void Awake()
        {
            base.Awake();
            draw = GetComponent<TextureDrawHelper>();

            float curHeight = Calc.Random.NextFloat(ScreenHeight);
            for (int i = 0; i < Steps; i++)
            {
                YNodes.Add(curHeight);
                curHeight += Calc.Random.Choose(-1, 1) * (16f + Calc.Random.NextFloat(MaxDist));
            }

            // 把倒数MinDist个node朝第一个nodelerp，防止出现极端情况
            for (int j = 0; j < MinDist; j++)
                YNodes[YNodes.Count - 1 - j] = Calc.LerpClamp(YNodes[YNodes.Count - 1 - j], YNodes[0], 1f - j / MinDist);

            List<Sprite> subtextures = Res.LoadSubtextures("Graphics/Celeste/Gameplay/particles/starfield/");
            for (int k = 0; k < Stars.Length; k++)
            {
                // 0-1随机数
                float rate = Calc.Random.NextFloat(1f);
                Stars[k].NodeIndex = Calc.Random.Next(YNodes.Count - 1);
                Stars[k].NodePercent = Calc.Random.NextFloat(1f);
                Stars[k].Distance = 4f + rate * 20f;
                Stars[k].Sine = Calc.Random.NextFloat(PI(2));
                Stars[k].Position = GetTargetOfStar(ref Stars[k]);
                Stars[k].Color = Color.Lerp(color, new(0, 0, 0, 0), rate * 0.5f);
                int r = (int)Calc.Clamp(Ease.CubicEaseIn(1f - rate) * subtextures.Count, 0f, subtextures.Count - 1);
                Stars[k].Texture = subtextures[r];
            }
        }

        public void Update()
        {
            for (int i = 0; i < Stars.Length; i++)
                UpdateStar(ref Stars[i]);
        }

        private void UpdateStar(ref Star star)
        {
            star.Sine += Time.deltaTime * FlowSpeed;
            star.NodePercent += Time.deltaTime * 0.25f * FlowSpeed;
            if (star.NodePercent >= 1f)
            {
                star.NodePercent -= 1f;
                star.NodeIndex++;
                if (star.NodeIndex >= YNodes.Count - 1)
                {
                    star.NodeIndex = 0;
                    star.Position.x -= ScreenWidth + 128;
                }
            }

            star.Position += (GetTargetOfStar(ref star) - star.Position) / 50f;
        }

        private Vector2 GetTargetOfStar(ref Star star)
        {
            Vector2 curNodePos = new Vector2(star.NodeIndex * StepSize, YNodes[star.NodeIndex]);
            Vector2 nextNodePos = new Vector2((star.NodeIndex + 1) * StepSize, YNodes[star.NodeIndex + 1]);
            Vector2 pos = curNodePos + (nextNodePos - curNodePos) * star.NodePercent;
            Vector2 dir = (nextNodePos - curNodePos).normalized;
            // 在轴上做sine运动
            Vector2 normal = dir.TurnRight();
            return pos + normal * (star.Distance * (float)Math.Sin(star.Sine));
        }

        private void OnRenderObject()
        {
            draw.Clear();
            Vector2 position = camera.transform.position;
            for (int i = 0; i < Stars.Length; i++)
            {
                Vector2 vector = new Vector2
                {
                    x = -64f + Calc.Mod(Stars[i].Position.x - position.x * Scroll.x, ScreenWidth + 128),
                    y = -16f + Calc.Mod(Stars[i].Position.y - position.y * Scroll.y, ScreenHeight + 32)
                };
                draw.Draw(Stars[i].Texture, vector, Stars[i].Color, Vector2.one, useWorldPos: false);
            }
        }
    }
}