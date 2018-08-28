using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Com.VT
{
    public static class ExtensionUtil
    {
        public static void ExClear(this IList list)
        {
            if(list != null)
            {
                list.Clear();
            }
        }

        public static void ExClear(this IDictionary dict)
        {
            if (dict != null)
            {
                dict.Clear();
            }
        }

        public static void ResetTransform(this Transform trans)
        {
            if (trans == null) return;
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        public static void Add<T1, T2>(this Dictionary<T1, T2> dict, T1 key, T2 value)
        {
            if (dict == null)
            {
                dict = new Dictionary<T1, T2>();
            }
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static void RemoveKey<T1, T2>(this Dictionary<T1, T2> dict, T1 key)
        {
            if (dict != null && dict.ContainsKey(key))
            {
                dict.Remove(key);
            }
        }
    }
}

