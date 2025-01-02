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

    public Action onBlock;
    
    public static ServerData Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject {name = "ServerData"};
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
        LoginToken(successCallback, failCallback);
    }

    public void Block(PersonConfig person,int status,Action successCallback = null, Action failCallback = null)
    {
        Dictionary<string, string> param = new Dictionary<string, string>()
        {
            {"to_user_id", person.id.ToString()},
            {"status", status.ToString()}
        };
        Post("https://api.wdtw.site/api/block/commit",param, jsonData =>
        {
            if ((int) jsonData["code"] == 1)
            {
                successCallback?.Invoke();
                onBlock?.Invoke();
            }
            else
            {
                failCallback?.Invoke();
            }
        },failCallback);
    }
    
    public void GetBlockList(Action<List<PersonConfig>> successCallback = null, Action failCallback = null)
    {
        Dictionary<string, string> param = new Dictionary<string, string>()
        {
            {"page", "1"},
            {"page_size", "99"}
        };
        Post("https://api.wdtw.site/api/block/list", param, jsonData =>
        {
            if ((int) jsonData["code"] == 1)
            {
                List<PersonConfig> personList = new List<PersonConfig>();
                for (int i = 0; i < jsonData["data"].Count; i++)
                {
                    var personData = jsonData["data"][i];
                    var person = new PersonConfig();
                    if (personData.ContainsKey("user_id")&& personData["user_id"] != null)
                    {
                        person.id = (int) personData["user_id"];
                    }

                    if (personData.ContainsKey("nick_name")&& personData["nick_name"] != null)
                    {
                        person.name = (string) personData["nick_name"];
                    }

                    if (personData.ContainsKey("user_header")&& personData["user_header"] != null)
                    {
                        int.TryParse((string) personData["user_header"], out person.head);
                    }

                    if (personData.ContainsKey("user_sign") && personData["user_sign"] != null)
                    {
                        person.description = (string) personData["user_sign"];
                    }

                    if (personData.ContainsKey("score")&& personData["score"] != null)
                    {
                        person.score = (int) personData["score"];
                    }

                    if (personData.ContainsKey("game_time") && personData["game_time"] != null)
                    {
                        person.challengeTime = (int) personData["game_time"];
                    }

                    personList.Add(person);
                }

                successCallback?.Invoke(personList);
            }
            else
            {
                failCallback?.Invoke();
            }
        }, failCallback);
    }
    
    public void GetUserList(Action<List<PersonConfig>> successCallback = null, Action failCallback = null)
    {
        Dictionary<string, string> param = new Dictionary<string, string>()
        {
            {"page", "1"},
            {"page_size", "99"}
        };

        Post("https://api.wdtw.site/api/user/list", param, jsonData =>
        {
            if ((int) jsonData["code"] == 1)
            {
                List<PersonConfig> personList = new List<PersonConfig>();
                for (int i = 0; i < jsonData["data"]["data"].Count; i++)
                {
                    var personData = jsonData["data"]["data"][i];
                    var person = new PersonConfig();
                    if (personData.ContainsKey("user_id")&& personData["user_id"] != null)
                    {
                        person.id = (int) personData["user_id"];
                    }

                    if (personData.ContainsKey("nick_name")&& personData["nick_name"] != null)
                    {
                        person.name = (string) personData["nick_name"];
                    }

                    if (personData.ContainsKey("user_header") && personData["user_header"] != null)
                    {
                        if (!int.TryParse((string) personData["user_header"], out person.head))
                        {
                            person.head = 1;
                        }
                    }

                    if (personData.ContainsKey("user_sign")&& personData["user_sign"] != null)
                    {
                        person.description = (string) personData["user_sign"];
                    }

                    if (personData.ContainsKey("score")&& personData["score"] != null)
                    {
                        person.score = (int) personData["score"];
                    }

                    if (personData.ContainsKey("game_time") && personData["game_time"] != null)
                    {
                        person.challengeTime = (int) personData["game_time"];
                    }

                    if (person.id != UserData.Instance.UserId)
                    {
                        personList.Add(person);   
                    }
                }

                successCallback?.Invoke(personList);
            }
            else
            {
                failCallback?.Invoke();
            }
        }, failCallback);
    }

    public void ModifyUserData(Dictionary<string, string> param, Action successCallback = null,
        Action failCallback = null)
    {
        Post("https://api.wdtw.site/api/user/updateProfile", param, (jsonData) =>
        {
            if ((int) jsonData["code"] == 1)
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
            GetUserInfo(successCallback, () =>
            {
                UserData.Instance.Token = "";
                LoginToken(successCallback, failCallback);
            });
        }
    }

    private void GetUserInfo(Action successCallback = null, Action failCallback = null)
    {
        Post("https://api.wdtw.site/api/user/profile",
            new Dictionary<string, string>(),
            (jsonData) =>
            {
                if ((int) jsonData["code"] == 1)
                {
                    var data = jsonData["data"];
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
            form.AddField(kv.Key, kv.Value);
        }

        Debug.Log(url);
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        // var paramStr = "";
        // foreach (var kv in param)
        // {
        //     paramStr += kv.Key + "=" + kv.Value + "&";
        // }
        // Debug.Log(paramStr);
        LoadingView.Open();
        if (!string.IsNullOrEmpty(UserData.Instance.Token))
        {
            request.SetRequestHeader("Authorization", "Bearer" + UserData.Instance.Token);
        }

        // byte[] postData = System.Text.Encoding.UTF8.GetBytes(paramStr);
        // request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        var handler = request.SendWebRequest();
        yield return handler;
        LoadingView.Close();
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