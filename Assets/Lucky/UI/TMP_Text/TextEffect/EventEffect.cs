using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lucky.UI.TMP_Text.TextEffect
{
    public class EventEffect : TextEffectBase
    {
        public EventEffect()
        {
            textEffectType = TextEffectType.Event;
        }

        public override void ParseAndCover(StringBuilder s, List<ParsedInfo> ranges)
        {
            string[] patterns =
            {
                "<speed=.*?>",
                "<delay=.*?>",
            };
            foreach (var pattern in patterns)
            {
                foreach (Match match in DialogueParser.ParseTag(s, pattern, true, placeholder))
                {
                    var pair = match.Groups[0].Value;
                    Dictionary<string, string> args = DialogueParser.ParseSelector(pair.Substring(1, pair.Length - 2));
                    // start还是很有必要的，因为要排序，但又因为不需要写入，所以这个标签中随便一个位置都行
                    ranges.Add(new(textEffectType, match.Groups[0].Index, -1, args));
                }
            }
        }
    }
}