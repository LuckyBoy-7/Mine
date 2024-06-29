using UnityEngine;

namespace Lucky.Utilities
{
    public class Timer
    {
        public static bool OnInterval(float interval)
        {
            return (int)((Time.time - (double)Time.deltaTime) / interval) < (int)((double)Time.time / interval);
        }
    }
}