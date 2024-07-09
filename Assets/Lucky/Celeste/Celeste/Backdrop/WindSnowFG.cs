using System;
using System.Collections.Generic;
using Lucky.Celeste.Monocle;
using Lucky.Extensions;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Celeste.Celeste.Backdrop
{
    public class WindSnowFG : Backdrop
    {
        public Vector2 CameraOffset;
        public float Alpha;
        private Vector2[] positions;
        private SineWave[] sines;
        private Vector2 scale;
        private float rotation;
        private float visibleFade;

        // 先这么写（
        [Range(-1200, 1200)] public float windX;
        [Range(-1200, 1200)] public float windY;
        private Vector2 Wind => new(windX, windY);

        private List<SpriteRenderer> particles;

        protected override void Awake()
        {
            base.Awake();
            CameraOffset = Vector2.zero;
            Alpha = 1f;
            scale = Vector2.one;
            visibleFade = 1f;
            color = Color.white;
            positions = new Vector2[240];
            for (int i = 0; i < positions.Length; i++)
                positions[i] = Calc.Random.Range(new Vector2(0f, 0f), new Vector2(ScreenWidth, ScreenHeight));

            sines = new SineWave[16];
            for (int j = 0; j < sines.Length; j++)
            {
                sines[j] = gameObject.AddComponent<SineWave>().SetCustomControl();
                sines[j].Init(Calc.Random.Range(0.8f, 1.2f));
                sines[j].Randomize();
            }

            particles = new List<SpriteRenderer>();
            var srPrefab = Resources.Load<SpriteRenderer>("Components/SpriteRenderer");
            for (int i = 0; i < positions.Length; i++)
            {
                particles.Add(Instantiate(srPrefab, transform));
                particles[^1].sprite = Resources.Load<Sprite>("Graphics/Celeste/Gameplay/particles/snow");
            }
        }

        public void Update()
        {
            visibleFade = Calc.Approach(visibleFade, isVisible ? 1 : 0, Time.deltaTime * 2f);
            foreach (var s in sines)
                s.Update();

            bool isDirX = Wind.y == 0f;
            if (isDirX)
            {
                scale.x = Math.Max(1f, Math.Abs(Wind.x) / 100f);
                rotation = Calc.Approach(rotation, 0f, Time.deltaTime * 8f);
            }
            else
            {
                scale.x = Math.Max(1f, Math.Abs(Wind.y) / 40f);
                rotation = Calc.Approach(rotation, PI(-0.5f), Time.deltaTime * 8f);
            }

            scale.y = 1f / Math.Max(1f, scale.x * 0.25f);
            for (int j = 0; j < positions.Length; j++)
            {
                float value = sines[j % sines.Length].Value;
                // 在运动方向上有速度大小的变化，在另一个方向上大家都是同一个速度
                Vector2 speed = isDirX ? new Vector2(Wind.x + value * 10f, -20f) : new Vector2(0f, Wind.y * 3f + value * 10f);
                positions[j] += speed * Time.deltaTime;
            }
        }

        private void OnRenderObject()
        {
            if (Alpha <= 0f)
                return;

            Color color = this.color.WithA(visibleFade * Alpha);
            // 9 / 16 == 0.57，*0.6保证两个方向粒子比例差不多
            int particleNumber = (int)(Wind.y == 0f ? (float)positions.Length : positions.Length * 0.6f);
            int cur = 0;
            foreach (Vector2 vector in positions)
            {
                Vector2 vec = vector;
                vec.y -= camera.transform.position.y + CameraOffset.y;
                // +10 -5 我自己加的，不然老是感觉粒子在边缘突然消失
                vec.y = Calc.Mod(vec.y, ScreenHeight + 10) - 5;

                vec.x -= camera.transform.position.x + CameraOffset.x;
                vec.x = Calc.Mod(vec.x, ScreenWidth + 10) - 5;

                if (cur < particleNumber)
                {
                    particles[cur].gameObject.SetActive(true);
                    particles[cur].transform.localPosition = vec;
                    particles[cur].color = color;
                    particles[cur].transform.localScale = scale;
                    particles[cur].transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * rotation);
                }
                else
                    particles[cur].gameObject.SetActive(false);

                cur++;
            }
        }
    }
}