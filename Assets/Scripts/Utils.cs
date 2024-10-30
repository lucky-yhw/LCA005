using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
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
}
