using System.Collections.Generic;
using System.Text;

namespace Lucky.Utilities
{
    public static class Html
    {
        public static string WrapTag(string content, string tag, string val="", Dictionary<string, string> pairs = null)
        {
            if (val != "")  // 因为有的tag是<link=213></link>这种形式
                return $"<{tag}={val}>{content}</{tag}>";
            if (pairs == null)
                return $"<{tag}>{content}</{tag}>";
            var args = new StringBuilder();
            foreach (var (key, value) in pairs)
                args.Append($" {key}={value}");
            return $"<{tag}{args}>{content}</{tag}>";
        }
    }
}