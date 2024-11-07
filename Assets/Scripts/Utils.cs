using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public static class Utils
{
    public static T ForceRebuildImmediate<T>(this T comp, bool withChildren = false) where T : Component
    {
        if (withChildren)
        {
            var rtChildren = comp.GetComponentsInChildren<RectTransform>();
            var children = new List<RectTransform>(rtChildren);
            for(int i = children.Count - 1; i >= 0; i--)
            {
                var child = children[i];
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(child);
            }
        }
        else
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(comp.GetComponent<RectTransform>());
        }
        return comp;
    }
    
    public const float GOLD_RATE = 10000f;
    
    public static Sprite GetUserHead(int id)
    {
        return Resources.Load<Sprite>("Textures/Head/head_" + id);
    }

    public static string FormatGold(long gold)
    {
        return (gold / GOLD_RATE).ToString("N2");
    }

    public static string FormatTime(int totalSeconds)
    {
        var minutes = totalSeconds / 60;
        var seconds = totalSeconds % 60;
        return $"{minutes}m:{seconds}s";
    }

    public static List<PersonConfig> RandomPerson(int count)
    {
        var personConfigTable = ConfigLoader.Load<PersonConfigTable>();
        //随机十个人 
        List<PersonConfig> ranPerson = new List<PersonConfig>();
        List<PersonConfig> configs = new List<PersonConfig>(personConfigTable.table.Values);
        for (int i = 0; i < count; i++)
        {
            var ran = Random.Range(0, configs.Count);
            ranPerson.Add(configs[ran]);
            configs.RemoveAt(ran);
            if (configs.Count == 0)
            {
                break;
            }
        }
        return ranPerson;
    }

    public static void RefreshListItems(ScrollRect scrollRect, GameObject itemPrefab,int count, Action<int,GameObject> onRefresh)
    {
        for (int i = scrollRect.content.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(scrollRect.content.GetChild(i).gameObject);
        }

        for (int i = 0; i < count; i++)
        {
            var go = GameObject.Instantiate(itemPrefab,scrollRect.content);
            onRefresh?.Invoke(i,go);
        }
    }

    public static DateTime Timestamp2DateTime(int timestamp)
    {
        return DateTime.Now;
    }

    public static int DateTime2Timestamp(DateTime dateTime)
    {
        return 0;
    }
}
