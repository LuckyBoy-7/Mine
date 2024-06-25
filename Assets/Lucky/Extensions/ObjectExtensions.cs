using System;
using System.Reflection;
using UnityEngine;

namespace Lucky.Extensions
{
    public static class ObjectExtensions
    {
        public static object DeepClone(this object orig)
        {
            if (orig == null)
                return null;
            Type T = orig.GetType();
            object o = Activator.CreateInstance(T);
            PropertyInfo[] PI = T.GetProperties();
            for (int i = 0; i < PI.Length; i++)
            {
                PropertyInfo P = PI[i];
                P.SetValue(o, P.GetValue(orig));
            }
            
            FieldInfo[] FI = T.GetFields();
            for (int i = 0; i < FI.Length; i++)
            {
                FieldInfo F = FI[i];
                F.SetValue(o, F.GetValue(orig));
            }
            return o;
        }
    }
}