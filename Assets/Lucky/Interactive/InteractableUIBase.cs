using UnityEngine;

namespace Lucky.Interactive
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class InteractableUIBase : InteractableBase
    {
        private RectTransform rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }

        protected abstract override SortingLayerType sortingLayerType { get; }

        protected abstract override long SortingOrder { get; }

        public abstract override Vector2 BoundsCheckPos { get; }

        public override bool IsPositionInBounds(Vector2 pos, RectTransform trans = null)
        {
            if (trans == null)
                trans = RectTransform;
            float width = trans.rect.width * trans.localScale.x;
            float height = trans.rect.height * trans.localScale.y;
            Vector2 pivot = trans.pivot;
            float x = trans.position.x - pivot.x * width;
            float y = trans.position.y - pivot.y * height;
            if (pos.x <= x + width
                && pos.x >= x
                && pos.y <= y + height
                && pos.y >= y)
                return true;
            return false;
        }


        private bool hasStartRegister;


        protected virtual void Start()
        {
            GameCursor.instance.interactableUIs.Add(this);
            hasStartRegister = true;
        }

        protected virtual void OnEnable()
        {
            if (hasStartRegister)
                GameCursor.instance.interactableUIs.Add(this);
        }

        protected virtual void OnDisable()
        {
            GameCursor.instance.interactableUIs.Remove(this);
        }
    }
}