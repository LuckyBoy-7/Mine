using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Lucky.UI.TMP_Text.TextEffect
{
    public class ShakeEffect : TextEffectBase
    {
        private float shakeAmount = 2f;


        public ShakeEffect()
        {
            textEffectType = TextEffectType.Shake;
        }

        public override void ParseAndCover(StringBuilder s, List<ParsedInfo> ranges)
        {
            // <selector prop1=val1, prop2 = val2> content </selector>
            // 虽然这么写可能会有bug，就是选择器+属性跟某个选择器前缀重复了
            var pattern = "<shake(.*?)>(.*?)</shake>";
            foreach (Match match in DialogueParser.ParseTag(s, pattern, true, placeholder))
            {
                int start = match.Groups[2].Index;
                int length = match.Groups[2].Length;
                Dictionary<string, string> args = DialogueParser.ParseSelector(match.Groups[1].Value);
                ranges.Add(new ParsedInfo(textEffectType, start, length, args));
            }
        }

        public override void TakeEffect(ParsedInfo parsedInfo)
        {
            float shakeAmount = parsedInfo.args.ContainsKey("shakeAmount") ? float.Parse(parsedInfo.args["shakeAmount"]) : this.shakeAmount;

            Vector3 GetRandomShakeOffset()
            {
                float mult = 100;
                float xOffset = Random.Range((int)-shakeAmount * mult, (int)shakeAmount * mult);
                float yOffset = Random.Range((int)-shakeAmount * mult, (int)shakeAmount * mult);
                return new Vector3(xOffset, yOffset) / 100f;
            }

            Vector3 preShakeOffset = GetRandomShakeOffset();
            for (var j = parsedInfo.start; j < parsedInfo.start + parsedInfo.length; j++) // 遍历每个字符
            {
                TMP_CharacterInfo charInfo = TextInfo.characterInfo[j]; // 拿到单个字符信息
                if (charInfo.character == ' ')
                    continue;


                int vertexIndex = charInfo.vertexIndex;
                Vector3 verticesOffset = parsedInfo.args.ContainsKey("share") && parsedInfo.args["share"] == "true"
                    ? preShakeOffset
                    : GetRandomShakeOffset();
                SetVerticesOffset(vertexIndex, verticesOffset);
            }
        }
    }
}