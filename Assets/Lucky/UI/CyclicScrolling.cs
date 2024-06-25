using System.Collections.Generic;
using Lucky.Extensions;
using UnityEngine;

namespace Lucky.Utilities.UI
{
    public class CyclicScrolling : MonoBehaviour  // 如果要搞暂停什么的，调用事件就行了，看具体情况
    {
        public Vector2 offset = Vector2.left;
        public float scrollingSpeed = 400; // 因为是以像素为单位的
        public List<RectTransform> groups; // 默认第1个为起点, 一般来说两张图片就够用了
        private Vector2 startPos;
        private Vector2 thresholdPos; // 超过这个临界就往回跳

        private bool IsExceedThreshold(Vector2 pos) =>
            (thresholdPos - startPos).Sign() != (thresholdPos - pos).Sign(); // 感觉直接换成单位向量距离的比较也行，但感觉这样好理解一点

        private void Start()
        {
            RectTransform rect = groups[0];
            startPos = rect.position;
            thresholdPos = startPos + offset * rect.sizeDelta;
            for (var i = 0; i < groups.Count; i++)  // 刚开始先摆好位置
            {
                var trans = groups[i];
                trans.position = startPos - i * offset * rect.sizeDelta;
            }
        }

        private void Update()
        {
            foreach (var trans in groups)
            {  // offset.normalized == dir
                trans.position += (Vector3)(scrollingSpeed * UnityEngine.Time.deltaTime * offset.normalized);
                if (IsExceedThreshold(trans.position))
                {
                    trans.position -= (Vector3)(offset * groups[0].sizeDelta * groups.Count);
                }
            }
        }
    }
}