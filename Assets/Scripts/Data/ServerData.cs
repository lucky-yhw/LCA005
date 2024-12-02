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
    private string _token = "";

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

    public UserLoginDelegate onUserLogin;

    public void Login(Action successCallback = null, Action failCallback = null)
    {
        LoginToken(() => { });
    }

    private void LoginToken(Action successCallback = null, Action failCallback = null)
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
                _token = data["data"]["access_token"].ToString();
                Debug.Log(_token);
                GetUserInfo(successCallback, failCallback);
                onUserLogin?.Invoke();
            },
            () => { failCallback?.Invoke(); });
    }

    private void GetUserInfo(Action successCallback = null, Action failCallback = null)
    {
        Post("https://api.wdtw.site/api/user/profile",
            new Dictionary<string, string>()
            {
                {"Authorization", "Bearer" + _token}
            },
            (data) =>
            {
                Debug.Log(data.ToJson());
                successCallback?.Invoke();
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
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        var paramStr = "";
        foreach (var kv in param)
        {
            paramStr += kv.Key + "=" + kv.Value + "&";
        }

        byte[] postData = System.Text.Encoding.UTF8.GetBytes(paramStr);
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        var handler = request.SendWebRequest();
        yield return handler;
        if (request.result == UnityWebRequest.Result.Success)
        {
            JsonData data = JsonMapper.ToObject(request.downloadHandler.text);
            successCallback?.Invoke(data);
        }
        else
        {
            failCallback?.Invoke();
        }
    }
}