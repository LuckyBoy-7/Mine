using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = System.Random;

namespace Lucky.Celeste.Monocle
{
    public static class Calc
    {
        public static readonly Random Random = new();


        /// <summary>
        /// 将传入的16进制字符转化为十进制
        /// </summary>
        public static byte HexToByte(char c)
        {
            return (byte)"0123456789ABCDEF".IndexOf(char.ToUpper(c));
        }

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


        public static float NextFloat(this Random random)
        {
            return (float)random.NextDouble();
        }

        public static float NextFloat(this Random random, float max)
        {
            return random.NextFloat() * max;
        }

        public static int Range(this Random random, int min, int max)
        {
            return min + random.Next(max - min);
        }

        public static float Range(this Random random, float min, float max)
        {
            return min + random.NextFloat(max - min);
        }

        public static T Choose<T>(this Random random, params T[] choices)
        {
            return choices[random.Next(choices.Length)];
        }

        public static Vector2 AngleToVector(float angleRadians, float length)
        {
            return new Vector2((float)Math.Cos(angleRadians), (float)Math.Sin(angleRadians)) * length;
        }
        
        /// 根据val在oldMin和oldMax之间的比例，放置到newMin和newMax之间
        public static float Map(float val, float min, float max, float newMin = 0f, float newMax = 1f)
        {
            return (val - min) / (max - min) * (newMax - newMin) + newMin;
        }
        
        // 向量往右转90度
        public static Vector2 TurnRight(this Vector2 vec)
        {
            return new Vector2(vec.y, -vec.x);
        }
        
        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
    }
}