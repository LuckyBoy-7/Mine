using System;
using System.Reflection;

namespace Mine.Extensions
{
    public static class ObjectExtensions
    {
        public static object DeepClone(this object orig)
        {
            Type T = orig.GetType();
            object o = Activator.CreateInstance(T);
            PropertyInfo[] PI = T.GetProperties();
            for (int i = 0; i < PI.Length; i++)
            {
                PropertyInfo P = PI[i];
                P.SetValue(o, P.GetValue(orig));
            }
            return o;
        }
    }
}