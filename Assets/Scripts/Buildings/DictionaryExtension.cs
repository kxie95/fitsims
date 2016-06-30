using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class DictionaryExtension  {

    public static int GetValueOrInit(this IDictionary<string, int> dictionary, string key)
    {
        int value;
        if( dictionary.TryGetValue(key, out value))
        {
            return value;
        }
        else
        {
            dictionary[key] = 0;
            return dictionary[key];
        }
    }

    public static int GetValueOrInitAndIncrement(this IDictionary<string, int> dictionary, string key)
    {
        int value;
        if (dictionary.TryGetValue(key, out value))
        {
            return dictionary[key]++;
        }
        else
        {
            dictionary[key] = 1;
            return dictionary[key];
        }
    }

    public static int GetValueAndDecrement(this IDictionary<string, int> dictionary, string key)
    {
        int value;
        if (dictionary.TryGetValue(key, out value))
        {
            return dictionary[key]--;
        }
        else
        {   
            dictionary[key] = -1;
            return dictionary[key];
        }
    }
}
