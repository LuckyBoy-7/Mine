using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Lucky.Celeste.Monocle;
using Lucky.GL_;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.Backdrop
{
    /// <summary>
    /// 大概就是定义n条带状物（这里3条），每条带状物有n个节点上下sine浮动，颜色随一个，然后lerp到另一个，然后加上图片（但这里没有（））
    /// </summary>
    public class NorthernLights : Backdrop
    {
        private static Color[] particleColors =
        {
            Calc.HexToColor("2de079"),
            Calc.HexToColor("62f4f6"),
            Calc.HexToColor("45bc2e"),
            Calc.HexToColor("3856f0")
        };

        private List<Strand> strands;
        private Particle[] particles;
        private Vector3[] verts;
        private Color[] colors;
        private Vector3[] gradient;
        public float Offsety;
        public float NorthernLightsAlpha;


        protected override void Awake()
        {
            base.Awake();
            strands = new List<Strand>();
            particles = new Particle[50];
            verts = new Vector3[1024];
            colors = new Color[1024];
            gradient = new Vector3[6];
            NorthernLightsAlpha = 1f;
            for (int i = 0; i < 3; i++)
                strands.Add(new Strand());

            for (int j = 0; j < particles.Length; j++)
            {
                particles[j].Position = new Vector2(Calc.Random.Range(0, 320), Calc.Random.Range(0, 180));
                particles[j].Speed = Calc.Random.Range(4, 14);
                particles[j].Color = Calc.Random.Choose(particleColors);
            }

            Color color = Calc.HexToColor("020825");
            Color color2 = Calc.HexToColor("170c2f");
            gradient[0] = new Vector3(0f, 0f);
            gradient[1] = new Vector3(320f, 0f);
            gradient[2] = new Vector3(320f, 180f);

            gradient[3] = new Vector3(0f, 0f);
            gradient[4] = new Vector3(320f, 180f);
            gradient[5] = new Vector3(0f, 180f);
        }

        public void Update()
        {
            if (isVisible)
            {
                foreach (Strand strand in strands)
                {
                    strand.Percent += Time.deltaTime / strand.Duration;
                    strand.Alpha = Calc.Approach(strand.Alpha, strand.Percent < 1f ? 1 : 0, Time.deltaTime);
                    if (strand.Alpha <= 0f && strand.Percent >= 1f)
                        strand.Reset(0f);

                    foreach (Node node in strand.Nodes)
                        node.SineOffset += Time.deltaTime;
                }

                for (int i = 0; i < particles.Length; i++)
                    particles[i].Position.y -= particles[i].Speed * Time.deltaTime;
            }
        }

        private void OnRenderObject()
        {
            int vId = 0;
            foreach (Strand strand in strands)
            {
                Node preNode = strand.Nodes[0];
                for (int i = 1; i < strand.Nodes.Count; i++)
                {
                    Node curNode = strand.Nodes[i];
                    // 上下浮动的效果
                    float preOffsetY = Offsety + (float)Math.Sin(preNode.SineOffset) * 3f;
                    float curOffsetY = Offsety + (float)Math.Sin(curNode.SineOffset) * 3f;
                    float num2 = Math.Min(1f, i / 4f) * NorthernLightsAlpha;
                    float num3 = Math.Min(1f, (strand.Nodes.Count - i) / 4f) * NorthernLightsAlpha;
                    colors[vId] = preNode.Color * (preNode.BottomAlpha * strand.Alpha * num2);
                    verts[vId++] = new Vector3(preNode.Position.x, preNode.Position.y + preOffsetY);
                    colors[vId] = preNode.Color * (preNode.TopAlpha * strand.Alpha * num2);
                    verts[vId++] = new Vector3(preNode.Position.x, preNode.Position.y - preNode.Height + preOffsetY);
                    colors[vId] = curNode.Color * (curNode.TopAlpha * strand.Alpha * num3);
                    verts[vId++] = new Vector3(curNode.Position.x, curNode.Position.y - curNode.Height + curOffsetY);

                    colors[vId] = preNode.Color * (preNode.BottomAlpha * strand.Alpha * num2);
                    verts[vId++] = new Vector3(preNode.Position.x, preNode.Position.y + preOffsetY);
                    colors[vId] = curNode.Color * (curNode.TopAlpha * strand.Alpha * num3);
                    verts[vId++] = new Vector3(curNode.Position.x, curNode.Position.y - curNode.Height + curOffsetY);
                    colors[vId] = curNode.Color * (curNode.BottomAlpha * strand.Alpha * num3);
                    verts[vId++] = new Vector3(curNode.Position.x, curNode.Position.y + curOffsetY);
                    preNode = curNode;
                }
            }

            for (int j = 0; j < particles.Length; j++)
            {
                this.DrawRect(new Vector2
                {
                    x = Calc.Mod(particles[j].Position.x - camera.transform.position.x * 0.2f, 320f),
                    y = Calc.Mod(particles[j].Position.y - camera.transform.position.y * 0.2f, 180f)
                }, 1f, 1f, particles[j].Color);
            }

            // this.DrawTriangle3By3WithRandomColor(verts);
            this.DrawTriangle3By3(verts, colors);
        }


        private class Strand
        {
            public List<Node> Nodes = new();
            public float Duration;
            public float Percent;
            public float Alpha;

            public Strand()
            {
                Reset(Calc.Random.NextFloat());
            }

            public void Reset(float startPercent)
            {
                Percent = startPercent;
                Duration = Calc.Random.Range(12f, 32f);
                Alpha = 0f;
                Nodes.Clear();
                Vector2 curPos = new Vector2(Calc.Random.Range(-40, 60), Calc.Random.Range(40, 90));
                Color color = Calc.Random.Choose(particleColors);
                for (int i = 0; i < 40; i++)
                {
                    Node node = new Node
                    {
                        Position = curPos,
                        Height = Calc.Random.Range(10, 80),
                        SineOffset = Calc.Random.NextFloat() * PI(2),
                        TopAlpha = Calc.Random.Range(0.3f, 0.8f),
                        BottomAlpha = Calc.Random.Range(0.5f, 1f),
                        Color = Color.Lerp(color, Calc.Random.Choose(particleColors), Calc.Random.Range(0f, 0.3f))
                    };
                    curPos += new Vector2(Calc.Random.Range(4, 20), Calc.Random.Range(-15, 15));
                    Nodes.Add(node);
                }
            }
        }

        private class Node
        {
            public Vector2 Position;
            public float Height;
            public float SineOffset;
            public float TopAlpha;
            public float BottomAlpha;
            public Color Color;
        }

        // 往下掉就完事了
        private struct Particle
        {
            public Vector2 Position;
            public float Speed;
            public Color Color;
        }
    }
}