using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Lucky.UI.TMP_Text.TextEffect
{
    public class FloatEffect : TextEffectBase
    {
        public float frequency = 10f;
        public float magnitude = 5f;
        
        public FloatEffect()
        {
            textEffectType = TextEffectType.Float;
        }
        public override void ParseAndCover(StringBuilder s, List<ParsedInfo> ranges)
        {
            // <selector prop1=val1, prop2 = val2> content </selector>
            // 虽然这么写可能会有bug，就是选择器+属性跟某个选择器前缀重复了
            var pattern = "<float(.*?)>(.*?)</float>";
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
            float frequency = parsedInfo.args.ContainsKey("frequency") ? float.Parse(parsedInfo.args["frequency"]) : this.frequency;
            float magnitude = parsedInfo.args.ContainsKey("magnitude") ? float.Parse(parsedInfo.args["magnitude"]) : this.magnitude;
            for (var j = parsedInfo.start; j < parsedInfo.start + parsedInfo.length; j++) // 遍历每个字符
            {
                TMP_CharacterInfo charInfo = TextInfo.characterInfo[j]; // 拿到单个字符信息
                // 有个逆天的问题不知道为什么，就是好像有空格的部分施加效果是无效的，然后多的会转移到第一个字符上？
                // tmp的坑我已经受够了(╯▔皿▔)╯
                if (charInfo.character == ' ')
                    continue;

                int vertexIndex = charInfo.vertexIndex;
                // 以时间为x, 得到y, frequency相当于x前面的常数
                Vector3 verticesOffset = new Vector3(0,
                    Mathf.Sin(frequency * UnityEngine.Time.time + j) * magnitude, 0);
                SetVerticesOffset(vertexIndex, verticesOffset);
            }
        }
    }
}