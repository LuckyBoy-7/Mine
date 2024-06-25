using System;
using UnityEngine;

namespace Lucky.Interactive
{
    public class Interactable : InteractableBase
    {
        public new SpriteRenderer renderer;

        // SortingLayer.GetLayerValueFromID，通过id返回对应的层序号，越高的层序号越大从0开始
        public override long SortingOrder => SortingLayer.GetLayerValueFromID(renderer.sortingLayerID) * Int32.MaxValue + renderer.sortingOrder;

        protected override void Awake()
        {
            base.Awake();
            if (renderer == null)
                renderer = GetComponent<SpriteRenderer>();
            if (renderer == null)
                renderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }
}