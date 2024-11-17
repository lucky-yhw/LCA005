using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
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

    public static Sprite GetUserHead(int id)
    {
        return Resources.Load<Sprite>("Textures/Head/head_" + id);
    }

    public static Sprite GetMyHead()
    {
        if (UserData.Instance.CustomerHead != null && UserData.Instance.CustomerHead.textureData != null && UserData.Instance.CustomerHead.textureData.Length > 0)
        {
            return  Sprite.Create(UserData.Instance.CustomerHead.ToTexture2D(),
                new Rect(Vector2.zero,
                    new Vector2(UserData.Instance.CustomerHead.width, UserData.Instance.CustomerHead.height)),
                new Vector2(0.5f, 0.5f));
        }
        else
        {
            return GetUserHead(UserData.Instance.UserHead);
        }
    }

    public static string FormatGold(long gold)
    {
        return (gold / (float)Const.GoldRate).ToString("N0");
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
            var p = configs[ran];
            if (!UserData.Instance.BlockList.Contains(p.id) && !UserData.Instance.HasChat(p.id))
            {
                ranPerson.Add(configs[ran]);
                configs.RemoveAt(ran);
            }
            else
            {
                configs.RemoveAt(ran);
            }
            if (configs.Count == 0)
            {
                break;
            }
        }
        return ranPerson;
    }

    public static void RefreshListItems(ScrollRect scrollRect, GameObject itemPrefab,int count, Action<int,GameObject> onRefresh)
    {
        DestroyAll(scrollRect.content);

        for (int i = 0; i < count; i++)
        {
            var go = GameObject.Instantiate(itemPrefab,scrollRect.content);
            onRefresh?.Invoke(i,go);
        }
    }

    public static void DestroyAll(Transform trans)
    {
        for (int i = trans.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(trans.GetChild(i).gameObject);
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

    public static float GetJsonSingle(JsonData jsonData)
    {
        if (jsonData.IsInt)
        {
            return Convert.ToSingle((int) jsonData);
        }
        if (jsonData.IsDouble)
        {
            return Convert.ToSingle((double) jsonData);
        }
        if (jsonData.IsLong)
        {
            return Convert.ToSingle((long) jsonData);
        }

        return 0;
    }
    
    public static int GetJsonInt(JsonData jsonData)
    {
        if (jsonData.IsInt)
        {
            return Convert.ToInt32((int) jsonData);
        }
        if (jsonData.IsDouble)
        {
            return Convert.ToInt32((double) jsonData);
        }
        if (jsonData.IsLong)
        {
            return Convert.ToInt32((long) jsonData);
        }

        return 0;
    }
    
    public static void PickImage(Action<Texture2D> onPick)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            if (!string.IsNullOrEmpty(path))
            {
                Main.Instance.StartCoroutine(LoadImage(path, onPick));
            }
            else
            {
                onPick?.Invoke(null);
            }
        }, "Select a PNG image", "image/png");
        Debug.Log("Permission result: " + permission);
    }
    
    private static IEnumerator LoadImage(string path,Action<Texture2D> onPick)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + path);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // 从不可读的Texture2D复制像素到可读的Texture2D
            Texture2D sourceTexture = DownloadHandlerTexture.GetContent(www);
            onPick?.Invoke(sourceTexture);
        }
    }

    // 将给定的 Texture2D 裁剪成圆形
    public static Texture2D CreateCircularTexture(Texture2D sourceTexture)
    {
        int width = sourceTexture.width;
        int height = sourceTexture.height;
        int side = width > height ? height : width;

        // 创建一个新的 RenderTexture 作为裁剪后的结果
        RenderTexture rt = new RenderTexture(width, height, 0);
        RenderTexture.active = rt;

        // 在 RenderTexture 上绘制圆形图案
        Graphics.Blit(sourceTexture, rt);

        // 创建一个新的 Texture2D 以存储裁剪后的结果
        Texture2D circularTexture = new Texture2D(side, side);
        circularTexture.ReadPixels(new Rect(0, 0, side, side), 0, 0);
        circularTexture.Apply();

        // 释放资源
        RenderTexture.active = null;
        GameObject.Destroy(rt);

        // 将裁剪后的 Texture2D 返回
        return circularTexture;
    }

    public static string FormatSecondsStr(float seconds)
    {
        var s = TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
        return s;
    }
}
