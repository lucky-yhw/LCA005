using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigBase
{
    public int id = 0;
}

public class ConfigTableBase : ScriptableObject
{
    protected void Init<T>(List<T> list, Dictionary<int, T> dictionary) where T : ConfigBase
    {
        foreach (var el in list)
        {
            if (!dictionary.ContainsKey(el.id))
            {
                dictionary.Add(el.id,el);   
            }
            else
            {
                Debug.LogError(GetType().Name + " add same key :" + el.id);
            }
        }
    }
}

public static class ConfigLoader
{
    public static T Load<T>() where T : ConfigTableBase
    {
        return Resources.Load<T>("Configs/" + typeof(T).Name);
    }
}
