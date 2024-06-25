using System.Runtime.CompilerServices;
using UnityEngine;

namespace Lucky.Celeste.Monocle
{
    public static class Calc
    {
        /// <summary>
        /// 将传入的16进制字符转化为十进制
        /// </summary>
        public static byte HexToByte(char c)
        {
            return (byte)"0123456789ABCDEF".IndexOf(char.ToUpper(c));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Color HexToColor(string hex)
        {
            int num = 0;
            // #可省选项
            if (hex.Length >= 1 && hex[0] == '#')
            {
                num = 1;
            }
            // 剩下数字长度够6的话就parse返回
            if (hex.Length - num >= 6)
            {
                // [0, 1]
                float num2 = (HexToByte(hex[num]) * 16 + HexToByte(hex[num + 1])) / 255f;
                float num3 = (HexToByte(hex[num + 2]) * 16 + HexToByte(hex[num + 3])) / 255f;
                float num4 = (HexToByte(hex[num + 4]) * 16 + HexToByte(hex[num + 5])) / 255f;
                return new Color(num2, num3, num4);
            }

            // 字符串长度不够的话，就把它当作一个数字来解析
            int num5;
            if (int.TryParse(hex.Substring(num), out num5))
            {
                return HexToColor(num5);
            }
            // 还不行就fallback
            return Color.white;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Color HexToColor(int hex)
        {
            return new Color
            {
                a = byte.MaxValue,
                r = (byte)(hex >> 16),
                g = (byte)(hex >> 8),
                b = (byte)hex
            };
        }
    }
}