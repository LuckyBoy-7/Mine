using System;
using static Lucky.Utilities.MathUtils;
using Lucky.Utilities;
using UnityEngine;
using Random = System.Random;

namespace Lucky.Celeste.Monocle
{
    public class SineWave : MonoBehaviour
    {
        private bool customControl = false;
        private float Frequency = 1;
        private float Rate = 1;
        public Action<float> OnUpdate;
        public bool realtime;
        private float T => PI(2) / Frequency / Rate;

        private float W => PI(2) / T;

        // private float W => Frequency * Rate;
        private float counter = 0;

        public float Counter // 自变量
        {
            get => counter;
            set
            {
                // 不至于溢出(不过想不明白为什么是8PI)
                // counter = (value + PI(8)) % PI(8);
                counter = (value % T + T) % T;
                Value = (float)Math.Sin(counter);
                ValueOverTwo = (float)Math.Sin(counter / 2f);
                TwoValue = (float)Math.Sin(counter * 2f);
            }
        }

        public float Value { get; private set; }
        public float ValueOverTwo { get; private set; }
        public float TwoValue { get; private set; }

        public void Init(float frequency, float offset = 0f)
        {
            Frequency = frequency;
            Counter = offset;
        }

        public void Update()
        {
            if (!customControl)
                CallUpdate();
        }

        public SineWave SetCustomControl()
        {
            customControl = true;
            return this;
        }

        public void CallUpdate()
        {
            Counter += W * Timer.GetDeltaTime(realtime);
            if (OnUpdate != null)
            {
                OnUpdate(Value);
            }
        }

        // 给自变量一个偏移后对应的因变量
        public float ValueOffset(float offset)
        {
            return (float)Math.Sin(counter + offset);
        }

        // 抽一个x
        public SineWave Randomize()
        {
            // Counter = (float)(new Random().NextDouble() * PI(2) * 2.0);
            Counter = (float)(new Random().NextDouble() * W);
            return this;
        }

        // 重置
        public void Reset()
        {
            Counter = 0f;
        }

        // 从正弦波的极大值开始
        public void StartUp()
        {
            Counter = PI(0.5f);
        }

        // 从正弦波的极小值开始
        public void StartDown()
        {
            Counter = PI(1.5f);
        }
    }
}