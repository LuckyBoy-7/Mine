using Lucky.Extensions;
using UnityEngine;

namespace Lucky.Interactive
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Collider2D))]
    public class InteractableUI : InteractableBase
    {
        public RectTransform rectTransform;

        public long sortingLayer = 0;
        public override long SortingOrder => sortingLayer * 10000 + rectTransform.GetSiblingIndex();

        protected override void Awake()
        {
            base.Awake();
            ResetCollider();
            // 省事，不然每个都要手动调，由于很多时候ui的更新晚一点，所以这个最后改
            // 或者也可以就写个方法，然后子类调用就行
            this.DoWaitUntilEndOfFrame(ResetCollider);

            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
                rectTransform = gameObject.AddComponent<RectTransform>();
        }

        protected virtual void ResetCollider()
        {
            if (collider is BoxCollider2D)
            {
                var coll = (BoxCollider2D)collider;
                coll.size = rectTransform.rect.size;
            }
        }
    }
}