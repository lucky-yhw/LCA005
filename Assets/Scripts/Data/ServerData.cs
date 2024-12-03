using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Networking;

public delegate void UserLoginDelegate();

public class ServerData : MonoBehaviour
{
    private static ServerData _instance;

    public static ServerData Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject();
                go.AddComponent<ServerData>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void Login(Action successCallback = null, Action failCallback = null)
    {
        LoginToken(successCallback,failCallback);
    }

    public void ModifyUserData(Dictionary<string,string> param, Action successCallback = null, Action failCallback = null)
    {
        Post("https://api.wdtw.site/api/user/updateProfile", param, (jsonData) =>
        {
            if ((int)jsonData["code"] == 1)
            {
                successCallback?.Invoke();
            }
            else
            {
                failCallback?.Invoke();
            }
        }, failCallback);
    }
    
    private void LoginToken(Action successCallback = null, Action failCallback = null)
    {
        if (string.IsNullOrEmpty(UserData.Instance.Token))
        {
            var uuid = Application.identifier;
            Post("https://api.wdtw.site/api/quickLogin",
                new Dictionary<string, string>()
                {
                    {
                        "uuid", uuid
                    }
                },
                (data) =>
                {
                    UserData.Instance.Token = data["data"]["access_token"].ToString();
                    GetUserInfo(successCallback, failCallback);
                },
                () => { failCallback?.Invoke(); });   
        }
        else
        {
            GetUserInfo(successCallback, failCallback);
        }
    }

    private void GetUserInfo(Action successCallback = null, Action failCallback = null)
    {
        Post("https://api.wdtw.site/api/user/profile",
            new Dictionary<string, string>(),
            (jsonData) =>
            {
                if ((int)jsonData["code"] == 1)
                {                var data = jsonData["data"];
                    UserData.Instance.InitByServerData(data);
                    successCallback?.Invoke();
                }
                else
                {
                    failCallback?.Invoke();
                }
            },
            () => { failCallback?.Invoke(); });
    }

    private void Post(string url, Dictionary<string, string> param, Action<JsonData> successCallback = null,
        Action failCallback = null)
    {
        StartCoroutine(HttpPost(url, param, successCallback, failCallback));
    }

    IEnumerator HttpPost(string url, Dictionary<string, string> param, Action<JsonData> successCallback = null,
        Action failCallback = null)
    {
        WWWForm form = new WWWForm();
        foreach (var kv in param)
        {
            form.AddField(kv.Key,kv.Value);
        }
        Debug.Log(url);
        UnityWebRequest request =  UnityWebRequest.Post(url,form);
        // var paramStr = "";
        // foreach (var kv in param)
        // {
        //     paramStr += kv.Key + "=" + kv.Value + "&";
        // }
        // Debug.Log(paramStr);
        if (!string.IsNullOrEmpty(UserData.Instance.Token))
        {
            request.SetRequestHeader("Authorization","Bearer" + UserData.Instance.Token);
        }
        
        // byte[] postData = System.Text.Encoding.UTF8.GetBytes(paramStr);
        // request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        var handler = request.SendWebRequest();
        yield return handler;
        if (request.result == UnityWebRequest.Result.Success)
        {
            JsonData data = JsonMapper.ToObject(request.downloadHandler.text);
            Debug.Log(data.ToJson());
            successCallback?.Invoke(data);
        }
        else
        {
            failCallback?.Invoke();
        }
    }
}