using System;
using System.Runtime.CompilerServices;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using UnityEngine;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class FinalBossStarfield : Backdrop
    {
        public float Alpha = 1;
        private const int particleCount = 200;
        private Particle[] particles = new Particle[particleCount];
        private Vector3[] verts = new Vector3[particleCount * 6 + 6];
        private Color[] colors = new Color[particleCount * 6 + 6];
        public Vector2 targetDir = new Vector2(0, -1);

        private static Color[] particleColors =
        {
            Calc.HexToColor("030c1b"),
            Calc.HexToColor("0b031b"),
            Calc.HexToColor("1b0319"),
            Calc.HexToColor("0f0301")
        };

        private struct Particle
        {
            public Vector2 Position;
            public Vector2 Direction;
            public float Speed;
            public Color Color;
            public float DirectionApproach;
        }

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < 200; i++)
            {
                particles[i].Speed = Calc.Random.Range(500f, 1200f);
                particles[i].Direction = new Vector2(-1f, 0f);
                particles[i].DirectionApproach = Calc.Random.Range(0.25f, 4f);
                particles[i].Position.x = Calc.Random.Range(0, 384);
                particles[i].Position.y = Calc.Random.Range(0, 244);
                particles[i].Color = Calc.Random.Choose(particleColors);
            }
        }

        public void Update()
        {
            if (isVisible && Alpha > 0f)
            {
                float targetAngle = targetDir.Angle();
                for (int i = 0; i < particleCount; i++)
                {
                    particles[i].Position += particles[i].Direction * (particles[i].Speed * Time.deltaTime);
                    float curAngle = particles[i].Direction.Angle();
                    // 如果不调角度，那么效果就是群魔乱舞，给人一种很混乱的感觉
                    curAngle = Calc.AngleApproach(curAngle, targetAngle, particles[i].DirectionApproach * Time.deltaTime);
                    particles[i].Direction = Calc.AngleToVector(curAngle, 1f);
                }
            }
        }

        private void OnRenderObject()
        {
            Vector2 cameraPos = camera.transform.position;
            color = Color.black * Alpha;
            // 背景rect
            colors[0] = color;
            verts[0] = new Vector3(-10f, -10f, 0f);
            colors[1] = color;
            verts[1] = new Vector3(330f, -10f, 0f);
            colors[2] = color;
            verts[2] = new Vector3(330f, 190f, 0f);
            colors[3] = color;
            verts[3] = new Vector3(-10f, -10f, 0f);
            colors[4] = color;
            verts[4] = new Vector3(330f, 190f, 0f);
            colors[5] = color;
            verts[5] = new Vector3(-10f, 190f, 0f);
            for (int i = 0; i < particleCount; i++)
            {
                int vId = (i + 1) * 6;
                // 速度越大，粒子越长
                float speedHeightMap = Calc.ClampedMap(particles[i].Speed, 0f, 1200f, 1f, 64f);
                float speedWidthMap = Calc.ClampedMap(particles[i].Speed, 0f, 1200f, 3f, 0.6f);
                Vector2 direction = particles[i].Direction;
                Vector2 perpendicular = direction.Perpendicular();
                Vector2 position = particles[i].Position;
                position.x = -32f + Calc.Mod(position.x - cameraPos.x * 0.9f, 384f);
                position.y = -32f + Calc.Mod(position.y - cameraPos.y * 0.9f, 244f);
                // 平行四边形
                Vector2 vector2 = position - direction * speedHeightMap * 0.5f - perpendicular * speedWidthMap;
                Vector2 vector3 = position + direction * speedHeightMap * 1f - perpendicular * speedWidthMap;
                Vector2 vector4 = position + direction * speedHeightMap * 0.5f + perpendicular * speedWidthMap;
                Vector2 vector5 = position - direction * speedHeightMap * 1f + perpendicular * speedWidthMap;
                Color color2 = particles[i].Color * Alpha;

                colors[vId] = color2;
                verts[vId] = vector2;
                colors[vId + 1] = color2;
                verts[vId + 1] = vector3;
                colors[vId + 2] = color2;
                verts[vId + 2] = vector4;

                colors[vId + 3] = color2;
                verts[vId + 3] = vector2;
                colors[vId + 4] = color2;
                verts[vId + 4] = vector4;
                colors[vId + 5] = color2;
                verts[vId + 5] = vector5;
            }

            this.DrawTriangle3By3(verts, colors);
        }
    }
}