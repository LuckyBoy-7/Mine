using System.Collections.Generic;
using System.Text;
// using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Lucky.UI.TMP_Text.TextEffect
{
    public class TextEffectBase
    {
        public TMPro.TMP_Text tmpText;
        protected TMP_TextInfo TextInfo => tmpText.textInfo;
        protected const char placeholder = '\0';
        public TextEffectType textEffectType;


        protected void SetVerticesOffset(int idx, Vector3 offset)
        {
            TextInfo.meshInfo[0].vertices[idx + 0] += offset;
            TextInfo.meshInfo[0].vertices[idx + 1] += offset;
            TextInfo.meshInfo[0].vertices[idx + 2] += offset;
            TextInfo.meshInfo[0].vertices[idx + 3] += offset;
        }

        protected void SetColor(int idx, Color32 color)
        {
            TextInfo.meshInfo[0].colors32[idx + 0] = color;
            TextInfo.meshInfo[0].colors32[idx + 1] = color;
            TextInfo.meshInfo[0].colors32[idx + 2] = color;
            TextInfo.meshInfo[0].colors32[idx + 3] = color;
        }

        public virtual void TakeEffect(ParsedInfo parsedInfos)
        {
        }

        public virtual void ParseAndCover(StringBuilder s, List<ParsedInfo> ranges)
        {
        }

       

      
    }
}