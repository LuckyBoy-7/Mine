using System;
using Mine.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Mine.Interactive
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Collider2D))]
    public class InteractableUI : InteractableBase
    {

        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public long sortingLayer = 0;
        public override long SortingOrder => sortingLayer * 10000 + RectTransform.GetSiblingIndex();

        protected virtual void Awake()
        {
            ResetCollider();
            // 省事，不然每个都要手动调，由于很多时候ui的更新晚一点，所以这个最后改
            // 或者也可以就写个方法，然后子类调用就行
            this.DoWaitUntilEndOfFrame(ResetCollider);
        }

        protected virtual void ResetCollider()
        {
            if (Collider is BoxCollider2D)
            {
                var coll = (BoxCollider2D)Collider;
                coll.size = RectTransform.rect.size;
            }
        }
    }
}