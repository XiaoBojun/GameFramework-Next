using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameBase
{
    public enum DictionaryOperation
    {
        Add,
        Remove,
        Update,
        Clear,
        DestroyKeyAndClear,
        DestroyValueAndClear,
    }
    public static class DictionaryExtensions
    {
        public static void Operation<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, DictionaryOperation operation, 
            TKey key= default(TKey), TValue value= default(TValue))
        {
            switch (operation)
            {
                case DictionaryOperation.Add:
                    if (dictionary.ContainsKey(key))
                    {
                        Log.Error("�ֵ�key�ظ����飺"+ dictionary+"|" +key);
                    }
                    else
                    {
                        dictionary.Add(key, value);
                    }
                    break;
                case DictionaryOperation.Remove:
                    dictionary.Remove(key);
                    break;
                case DictionaryOperation.Update:
                    {
                        if (dictionary.ContainsKey(key))
                        {
                            dictionary[key] = value;
                        }
                        else
                        {
                            dictionary.Add(key, value);
                        }
                    }
                    break;
                case DictionaryOperation.Clear:
                    dictionary.Clear();
                    break;
                case DictionaryOperation.DestroyKeyAndClear:
                    {
                        foreach (var item in dictionary)//.ToList() // ʹ�� ToList() ������ѭ��ʱ�޸��ֵ�
                        {
                            if (item.Key is Component)
                            {
                                GameObject.Destroy(item.Key as Component);
                            }
                            else
                            {
                                Log.Error("�޷����ټ����ͣ�" + item.Key.GetType());
                            }
                        }
                        dictionary.Clear();
                    }
                    break;
                case DictionaryOperation.DestroyValueAndClear:
                    {
                        foreach (var item in dictionary)
                        {
                            if (item.Value is Component)
                            {
                                GameObject.Destroy(item.Value as Component);
                            }
                            else
                            {
                                Log.Error("�޷����ټ����ͣ�" + item.Value.GetType());
                            }
                        }
                        dictionary.Clear();
                    }
                    break;
            }
        }
    }
}
