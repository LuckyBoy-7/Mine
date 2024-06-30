using UnityEngine;

namespace Lucky.Utilities
{
    public static class Timer
    {
        public static float GetTime(bool realtime = false) => realtime ? Time.realtimeSinceStartup : Time.time;
        public static float GetDeltaTime(bool realtime = false) => realtime ? Time.unscaledDeltaTime : Time.deltaTime;

        /// 每一个interval开始的时候触发 
        public static bool OnInterval(float interval, bool realtime = false)
        {
            return (int)((GetTime(realtime) - (double)GetDeltaTime(realtime)) / interval) < (int)((double)GetTime(realtime) / interval);
        }

        /// 获取按interval交替的bool
        public static bool BetweenInterval(float interval, bool realtime = false)
        {
            return GetTime(realtime) % (interval * 2f) > interval;
        }
    }
}