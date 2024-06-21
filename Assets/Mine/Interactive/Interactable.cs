using System;
using UnityEngine;

namespace Mine.Interactive
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Interactable : InteractableBase
    {
        private Collider2D _collider;

        public new Collider2D Collider
        {
            get
            {
                if (_collider == null)
                    _collider = GetComponent<Collider2D>();
                return _collider;
            }
        }

        private SpriteRenderer _renderer;

        public SpriteRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                    _renderer = GetComponent<SpriteRenderer>();
                return _renderer;
            }
        }

        // SortingLayer.GetLayerValueFromID，通过id返回对应的层序号，越高的层序号越大从0开始
        public override long SortingOrder => SortingLayer.GetLayerValueFromID(Renderer.sortingLayerID) * Int32.MaxValue + Renderer.sortingOrder;


    }
}