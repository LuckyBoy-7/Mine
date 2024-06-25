using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lucky.UI.TMP_Text.TextEffect
{
    public class NoneEffect : TextEffectBase
    {
        public NoneEffect()
        {
            textEffectType = TextEffectType.None;
        }

        public override void ParseAndCover(StringBuilder s, List<ParsedInfo> ranges)
        {
            // <selector prop1=val1, prop2 = val2> content </selector>
            // 虽然这么写可能会有bug，就是选择器+属性跟某个选择器前缀重复了
            var pattern = $"[^{placeholder}]+";
            foreach (Match match in DialogueParser.ParseTag(s, pattern, true, placeholder))
            {
                int start = match.Groups[0].Index;
                int length = match.Groups[0].Length;
                Dictionary<string, string> args = new();
                ranges.Add(new ParsedInfo(textEffectType, start, length, args));
            }
        }
    }
}