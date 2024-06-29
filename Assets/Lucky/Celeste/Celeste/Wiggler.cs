using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lucky.Celeste.Celeste
{
    /// <summary>
    /// 创造一个随时间正弦摆动的值
    /// 可以调存活时间和摆动频率，至于magnitude，它就是Counter，所以这个摆动幅度是越来越小的，力的损失（
    /// </summary>
    public class Wiggler : MonoBehaviour
    {
        private float Counter { get; set; }
        private float Value { get; set; }

        public bool StartZero;
        public float duration;
        public float frequency;
        public bool removeSelfOnFinish;

        private float sineCounter;
        private float increment;
        private float sineAdd; // 就是w
        public Action<float> onChange;

        private void Awake()
        {
            Counter = sineCounter = 0f;
            increment = 1f / duration;
            sineAdd = Mathf.PI * 2 * frequency; // A * sin(w * x + f) + y，这里周期为1/frequency

            // debug
            onChange += v =>
            {
                transform.localScale = Vector2.one * (1f + v * 0.35f);
                transform.eulerAngles = new Vector3(0, 0, v * 27f);
            };
        }

        public void Start()
        {
            Counter = 1f;
            if (StartZero)
            {
                sineCounter = Mathf.PI / 2;
                Value = 0f;
                if (onChange != null)
                {
                    onChange(0f);
                }
            }
            else
            {
                sineCounter = 0f;
                Value = 1f;
                if (onChange != null)
                {
                    onChange(1f);
                }
            }
        }


        public void Update()
        {
            // 现在对应的x
            sineCounter += sineAdd * Time.deltaTime;
            Counter -= increment * Time.deltaTime;

            if (Counter <= 0f)
            {
                Counter = 0f;
                if (removeSelfOnFinish)
                {
                    Destroy(gameObject);
                }
            }

            Value = (float)Math.Cos(sineCounter) * Counter;
            if (onChange != null)
            {
                onChange(Value);
            }
        }
    }
}