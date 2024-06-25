using System;
using UnityEngine;


namespace Lucky.Utilities.UI
{
    /// <summary>
    /// contentSizeFitter便携版，但是contentSizeFitter便携版好像不能适应有子物体的对象，所以我写一个丐版的
    /// 然后过长文本的本身还是要使用contentSizeFitter + layout的
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIWrapping : MonoBehaviour
    {
        public float extraWidth;
        public float extraHeight;

        private RectTransform _rectTransform;

        private RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public Vector2 SizeDelta => RectTransform.sizeDelta;

        // 当文本改变的时候再调用
        public void UpdateUI()
        {
            RectTransform.sizeDelta = GetSize();
        }

        private Vector2 GetSize()
        {
            Vector2 min = new Vector2(Single.PositiveInfinity, Single.PositiveInfinity);
            Vector2 max = new Vector2(Single.NegativeInfinity, Single.NegativeInfinity);

            void DFS(RectTransform node)
            {
                for (int i = 0; i < node.childCount; i++)
                {
                    var child = node.GetChild(i).GetComponent<RectTransform>();
                    // pivot为 （0，0）时才是对应真正的position，所以这里要转化一下
                    Vector2 pos;
                    pos.x = child.position.x - child.pivot.x * child.sizeDelta.x;
                    pos.y = child.position.y - child.pivot.y * child.sizeDelta.y;
                    min = Vector2.Min(min, pos);
                    max = Vector2.Max(max, pos + child.sizeDelta);
                    DFS(child);
                }
            }

            DFS(RectTransform);
            return max - min + new Vector2(extraWidth, extraHeight);
        }
    }
}