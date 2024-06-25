using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lucky.UI.TMP_Text
{
    public static class DialogueParser
    {
        /// 解析字符串获得所有的match，并选择性替换原文本
        public static List<Match> ParseTag(StringBuilder s, string pattern, bool isReplace = true, char placeholder = '\0')
        {
            List<Match> res = new();
            foreach (Match match in Regex.Matches(s.ToString(), pattern))
            {
                if (isReplace)
                    for (int j = match.Groups[0].Index; j < match.Groups[0].Index + match.Groups[0].Length; j++)
                        s[j] = placeholder;
                res.Add(match);
            }

            return res;
        }

        /// 解析形如"prop1=val1; prop2=val2"这样的字符串，转化为{"prop1": "val1", "prop2": "val2"}
        public static Dictionary<string, string> ParseSelector(string s)
        {
            Dictionary<string, string> res = new();
            foreach (string pair in s.Split(";"))
            {
                string p = pair.Trim();
                if (string.IsNullOrEmpty(p))
                    continue;
                var pairSplit = p.Split("=");
                if (pairSplit.Length != 2) // 一般是防止debug的时候在打字过程中只有一个元素
                    continue;
                string property = pairSplit[0].Trim();
                string value = pairSplit[1].Trim();
                if (string.IsNullOrEmpty(property)) // 都是防止debug的时候报错
                    continue;
                if (string.IsNullOrEmpty(value))
                    continue;
                res[property] = value;
            }

            return res;
        }
    }
}