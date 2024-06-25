using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky.Utilities
{
    public static class Itertools
    {
        public static IEnumerable<Tuple<int, T>> Enumerate<T>(List<T> lst, int start = 0)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                yield return new Tuple<int, T>(start++, lst[i]);
            }
        }

        public static IEnumerable<int> Range(int start, int end, int step)
        {
            // a0 = start
            // delta = step
            // an = end
            // n = (end - start - (int)Mathf.Sign(end - start)) / step
            int n = (end - start - (int)Mathf.Sign(end - start)) / step + 1;
            for (int i = 0; i < n; i++)
                yield return start + step * i;
        }

        public static IEnumerable Range(int start, int end) => Range(start, end, 1);
        public static IEnumerable Range(int end) => Range(0, end, 1);

        public static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>(ICollection<T1> collection1, ICollection<T2> collection2)
        {
            int n = Mathf.Min(collection1.Count, collection2.Count);
            IEnumerator it1 = collection1.GetEnumerator();
            IEnumerator it2 = collection2.GetEnumerator();
            for (int i = 0; i < n; i++)
            {
                it1.MoveNext();
                it2.MoveNext();
                yield return new Tuple<T1, T2>((T1)it1.Current, (T2)it2.Current);
            }
        }

        public static IEnumerable<Tuple<T, T>> Pairwise<T>(ICollection<T> collection)
        {
            IEnumerator it = collection.GetEnumerator();
            if (!it.MoveNext())
                yield break;

            T a, b;
            a = (T)it.Current;
            while (it.MoveNext())
            {
                b = (T)it.Current;
                yield return new Tuple<T, T>(a, b);
                a = b;
            }
        }

        public static IEnumerable<T> Accumulate<T>(Func<T, T, T> func, ICollection<T> collection, T initial)
        {
            yield return initial;

            IEnumerator it = collection.GetEnumerator();
            if (!it.MoveNext())
                yield break;

            T a, b;
            a = func(initial, (T)it.Current);
            yield return a;
            while (it.MoveNext())
            {
                b = (T)it.Current;
                a = func(a, b);
                yield return a;
            }
        }

        public static IEnumerable<T> Accumulate<T>(Func<T, T, T> func, ICollection<T> collection)
        {
            IEnumerator it = collection.GetEnumerator();
            if (!it.MoveNext())
                yield break;

            T a, b;
            a = (T)it.Current;
            yield return a;
            while (it.MoveNext())
            {
                b = (T)it.Current;
                a = func(a, b);
                yield return a;
            }
        }

        public static T Reduce<T>(Func<T, T, T> func, ICollection<T> collection, T initial)
        {
            IEnumerator it = collection.GetEnumerator();
            if (!it.MoveNext())
                return default;

            T a, b;
            a = func((T)it.Current, initial);
            while (it.MoveNext())
            {
                b = (T)it.Current;
                a = func(a, b);
            }

            return a;
        }

        public static T Reduce<T>(Func<T, T, T> func, ICollection<T> collection)
        {
            IEnumerator it = collection.GetEnumerator();
            if (!it.MoveNext())
                return default;

            T a, b;
            a = (T)it.Current;
            while (it.MoveNext())
            {
                b = (T)it.Current;
                a = func(a, b);
            }

            return a;
        }
    }
}