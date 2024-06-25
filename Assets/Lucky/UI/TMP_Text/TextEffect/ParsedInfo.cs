using System.Collections.Generic;

namespace Lucky.UI.TMP_Text.TextEffect
{
    /// <summary>
    /// 调用take effect前的parsed info中的start是原字符串中的位置，为了后续方便排序和消除
    /// 调用take effect时拿到的parsed info中的start是解析后字符串中的位置，是realIndex
    /// </summary>
    public struct ParsedInfo
    {
        public TextEffectType textEffectType;
        public int start;
        public int length;
        public Dictionary<string, string> args;

        public ParsedInfo(TextEffectType textEffectType, int start, int length, Dictionary<string, string> args)
        {
            this.textEffectType = textEffectType;
            this.start = start;
            this.length = length;
            this.args = args;
        }
    }
}