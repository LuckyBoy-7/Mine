using System.Collections.Generic;
using UnityEngine;

namespace Mine.Extensions
{
    public static class ListExtensions
    {
        public static T Choice<T>(this List<T> lst)
        {
            return lst[Random.Range(0, lst.Count)];
        }

        public static void Shuffle<T>(this List<T> lst)
        {
            for (int i = 0; i < lst.Count - 1; i++)
            {
                int j = Random.Range(i, lst.Count);
                (lst[i], lst[j]) = (lst[j], lst[i]);
            }
        }

        public static void Extend<T>(this List<T> lst, List<T> newList)
        {
            foreach (var item in newList)
            {
                lst.Add(item);
            }
        }
    }
}