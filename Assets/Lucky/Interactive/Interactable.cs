using System;
using UnityEngine;

namespace Lucky.Interactive
{
    public class Interactable : InteractableBase
    {
        private Collider2D _collider;

        public Collider2D Collider
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

        protected override SortingLayerType sortingLayerType => SortingLayerType.Default;

        // SortingLayer.GetLayerValueFromID，通过id返回对应的层序号，越高的层序号越大从0开始
        protected override long SortingOrder =>
            (SortingLayer.GetLayerValueFromID(Renderer.sortingLayerID) + OffsetSortingLayer) * 10000 + Renderer.sortingOrder;

        public override bool IsPositionInBounds(Vector2 pos, RectTransform trans = null) => Collider.OverlapPoint(pos);
        public override Vector2 BoundsCheckPos => GameCursor.MouseWorldPos;
    }
}