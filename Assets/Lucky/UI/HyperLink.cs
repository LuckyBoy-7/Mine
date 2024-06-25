using System.Collections.Generic;
using Lucky.Interactive;
using Lucky.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Lucky.UI
{
    [RequireComponent(typeof(TMPro.TMP_Text))]
    public class HyperLink : MonoBehaviour
    {
        public Color linkColor;
        public Color pressedColor;
        public Color visitedColor;
        private TMPro.TMP_Text tmpText;
        public Texture2D hand;
        private Dictionary<string, bool> linkVisited = new();
        private string rawContent = "";
        private string pressedId = "";
        private string hoverId = "";

        private void Awake()
        {
            tmpText = GetComponent<TMPro.TMP_Text>();
            tmpText.ForceMeshUpdate();
        }

        private void Start()
        {
            UpdateLink();
        }

        public void UpdateLink()
        {
            linkVisited.Clear();
            foreach (var info in tmpText.textInfo.linkInfo)
            {
                linkVisited[info.GetLinkID()] = false;
            }

            rawContent = tmpText.text;
        }

        private void Update()
        {
            string tmpContent = rawContent;

            // 这里mouse pos视情况而定，如果纯ui就用game cursor的，如果是overlay的canvas直接传屏幕坐标即可
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmpText, GameCursor.MouseWorldPos, null);
            if (linkIndex != -1) // 如果点击在超链接上
            {
                if (Input.GetMouseButtonDown(0))
                    pressedId = tmpText.textInfo.linkInfo[linkIndex].GetLinkID();
                hoverId = tmpText.textInfo.linkInfo[linkIndex].GetLinkID();
                Cursor.SetCursor(hand, new Vector2(10, 0), CursorMode.Auto);
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                hoverId = "";
            }

            if (Input.GetMouseButtonUp(0) && pressedId != "")
            {
                if (!linkVisited[pressedId])
                {
                    // EventManager.instance.Broadcast($"Link{pressedId}Pressed");
                    linkVisited[pressedId] = true;
                }

                pressedId = "";
            }

            foreach (var linkInfo in tmpText.textInfo.linkInfo)
            {
                string linkContent = linkInfo.GetLinkText();
                string linkId = linkInfo.GetLinkID();
                string pattern = Html.WrapTag(linkContent, "link", linkId);

                Color color;
                if (linkId == pressedId)
                    color = pressedColor;
                else if (linkVisited[linkId])
                    color = visitedColor;
                else
                    color = linkColor;
                string colorString = "#" + color.ToHexString();

                tmpContent = tmpContent.Replace(pattern, Html.WrapTag(pattern, "color", colorString));
                // 因为下划线的颜色也会被影响所以放最后
                if (linkId == hoverId)
                    tmpContent = tmpContent.Replace(pattern, Html.WrapTag(pattern, "u"));
            }

            tmpText.text = tmpContent;
        }
    }
}