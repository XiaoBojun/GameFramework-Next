using System;
using System.Collections.Generic;

#if UNITY_2021_1_OR_NEWER
                   
#else
public static class KeyValuePairExtensions
{
    public static void Deconstruct(this KeyValuePair<string, Type> pair, out string key, out Type value)
    {
        key = pair.Key;
        value = pair.Value;
    }
}
public static class DictionaryExtensions
{
    public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out TValue value) ? value : default(TValue);
    }
}
#endif