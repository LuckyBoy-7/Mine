using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Lucky.UI.TMP_Text.TextEffect
{
    public class JitterEffect : TextEffectBase
    {
        private float jitterAmount = 2f;


        public JitterEffect()
        {
            textEffectType = TextEffectType.Jitter;
        }

        public override void ParseAndCover(StringBuilder s, List<ParsedInfo> ranges)
        {
            // <selector prop1=val1, prop2 = val2> content </selector>
            // 虽然这么写可能会有bug，就是选择器+属性跟某个选择器前缀重复了
            var pattern = "<jitter(.*?)>(.*?)</jitter>";
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
            float jitterAmount = parsedInfo.args.ContainsKey("jitterAmount") ? float.Parse(parsedInfo.args["jitterAmount"]) : this.jitterAmount;

            Vector3 GetRandomJitterOffset()
            {
                float mult = 100;
                float xOffset = Random.Range((int)-jitterAmount * mult, (int)jitterAmount * mult);
                float yOffset = Random.Range((int)-jitterAmount * mult, (int)jitterAmount * mult);
                return new Vector3(xOffset, yOffset) / 100f;
            }

            Vector3 preShakeOffset0 = GetRandomJitterOffset();
            Vector3 preShakeOffset1 = GetRandomJitterOffset();
            Vector3 preShakeOffset2 = GetRandomJitterOffset();
            Vector3 preShakeOffset3 = GetRandomJitterOffset();
            for (var j = parsedInfo.start; j < parsedInfo.start + parsedInfo.length; j++) // 遍历每个字符
            {
                TMP_CharacterInfo charInfo = TextInfo.characterInfo[j]; // 拿到单个字符信息
                if (charInfo.character == ' ')
                    continue;


                int vertexIndex = charInfo.vertexIndex;
                if (parsedInfo.args.ContainsKey("share") && parsedInfo.args["share"] == "true")
                {
                    TextInfo.meshInfo[0].vertices[vertexIndex + 0] += preShakeOffset0;
                    TextInfo.meshInfo[0].vertices[vertexIndex + 1] += preShakeOffset1;
                    TextInfo.meshInfo[0].vertices[vertexIndex + 2] += preShakeOffset2;
                    TextInfo.meshInfo[0].vertices[vertexIndex + 3] += preShakeOffset3;
                }
                else
                {
                    TextInfo.meshInfo[0].vertices[vertexIndex + 0] += GetRandomJitterOffset();
                    TextInfo.meshInfo[0].vertices[vertexIndex + 1] += GetRandomJitterOffset();
                    TextInfo.meshInfo[0].vertices[vertexIndex + 2] += GetRandomJitterOffset();
                    TextInfo.meshInfo[0].vertices[vertexIndex + 3] += GetRandomJitterOffset();
                }
            }
        }
    }
}