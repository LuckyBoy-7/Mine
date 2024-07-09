using UnityEngine;

namespace Lucky.Interactive
{
    public class InteractableScreenUI : InteractableUIBase
    {
        protected override SortingLayerType sortingLayerType => SortingLayerType.Screen;

        public long sortingLayer = 0;

        // warning: 这个不要忘了调，尤其是涉及到很多interactable的时候
        protected override long SortingOrder => (OffsetSortingLayer + sortingLayer) * 10000 + RectTransform.GetSiblingIndex();


        public override Vector2 BoundsCheckPos => GameCursor.MouseScreenPos;
    }
}