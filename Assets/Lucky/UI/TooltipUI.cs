using Lucky.Extensions;
using Lucky.Interactive;
using Lucky.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lucky.Utilities.UI
{
    public class TooltipUI : Singleton<TooltipUI>
    {
        public float diagonalDelta;
        public TMP_Text nameText;
        public Image image;
        public TMP_Text descriptionText;
        public RectTransform panelRectTransform;
        public UIWrapping wrapping;
        [SerializeField] private bool isShowing;

        private void Update()
        {
            if (!isShowing)
                return;
            UpdatePos();
        }

        private void UpdatePos()
        {
            Vector2 mousePos = GameCursor.MouseWorldPos;
            // 默认pivot为左上角
            panelRectTransform.pivot = new Vector2(0, 1);
            if (mousePos.x + panelRectTransform.sizeDelta.x + diagonalDelta > Screen.width / 2)
                // pivot变为右上角
                panelRectTransform.pivot += new Vector2(1, 0);
            if (mousePos.y - panelRectTransform.sizeDelta.y - diagonalDelta < -Screen.height / 2)
                panelRectTransform.pivot -= new Vector2(0, 1);

            // 0 1 -> 1 -1
            // 1 1 -> -1 -1
            // 0 0 -> 1 1
            // 1 0 -> -1 1
            // x -> -(x * 2 - 1)
            panelRectTransform.position = mousePos + new Vector2(diagonalDelta, diagonalDelta) * -(panelRectTransform.pivot * 2 - Vector2.one);
        }

        // 设置锚点为左上角（然后加上偏移），如果超出屏幕范围
        // 则把锚点设置到右上角，并施加偏移
        public void Show(string name = "", Sprite sprite = null, string description = "")
        {
            isShowing = true;
            panelRectTransform.gameObject.SetActive(true);
            nameText.text = name;
            if (sprite == null)
                image.enabled = false;
            else
            {
                image.enabled = true;
                image.sprite = sprite;
            }

            descriptionText.text = description;
            this.DoWaitUntilEndOfFrame(wrapping.UpdateUI);
            UpdatePos();
        }

        public void Hide()
        {
            isShowing = false;
            panelRectTransform.gameObject.SetActive(false);
        }
    }
}