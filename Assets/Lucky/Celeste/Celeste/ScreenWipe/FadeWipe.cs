using Lucky.Extensions;
using Lucky.GL_;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Celeste.Celeste.ScreenWipe
{
    public class FadeWipe : ScreenWipe
    {
        public bool WipeIn;
        [Range(0, 1)] public float Percent;

        private void OnRenderObject()
        {
            Color color = Color.black.WithA(WipeIn ? Ease.CubicEaseOut(Percent) : 1f - Ease.CubicEaseIn(Percent));
            this.DrawRect(Vector3.zero, 1920, 1080, color);
        }
    }
}