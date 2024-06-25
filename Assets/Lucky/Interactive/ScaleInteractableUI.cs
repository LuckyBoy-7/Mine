using Lucky.Interactive;
using UnityEngine;

namespace Lucky.Interactive
{
    public class ScaleInteractableUI : InteractableUI
    {
        private Vector3 origScale;
        protected float OnCursorEnterScaleMultiplier = 1.05f;
        protected float OnCursorPressScaleMultiplier = 0.95f;
        private bool isMouseButtonKeepHolding;

        protected override void Awake()
        {
            base.Awake();
            origScale = transform.localScale;
        }

        protected override void OnCursorHover()
        {
            if (isMouseButtonKeepHolding)
            {
                transform.localScale = origScale * OnCursorPressScaleMultiplier;
                return;
            }

            transform.localScale = origScale * OnCursorEnterScaleMultiplier;
        }

        protected override void OnCursorExit()
        {
            transform.localScale = origScale;
        }

        protected override void OnCursorPress()
        {
            isMouseButtonKeepHolding = true;
        }

        protected override void OnCursorRelease()
        {
            isMouseButtonKeepHolding = false;
        }
    }
}