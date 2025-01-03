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
    public Action onChat;

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

    public void UploadScore(int score,int totalSeconds)
    {
        Dictionary<string, string> param = new Dictionary<string, string>()
        {
            {"score", score.ToString()},
            {"game_time", totalSeconds.ToString()},
        };
        Post("https://api.wdtw.site/api/user/updateGameProfile", param);
    }
    
    public void CostGold(long gold)
    {
        Dictionary<string, string> param = new Dictionary<string, string>()
        {
            {"expend_name", "item"},
            {"diamonds", gold.ToString()},
        };
        Post("https://api.wdtw.site/api/expend/do", param, jsonData =>
        {
            UserData.Instance.Gold -= gold;
        });
    }
    
    public void GetGold(long gold)
    {
        Dictionary<string, string> param = new Dictionary<string, string>()
        {
            {"expend_name", "item"},
            {"diamonds", gold.ToString()},
        };
        Post("https://api.wdtw.site/api/income/do", param, jsonData =>
        {
            UserData.Instance.Gold += gold;
        });
    }
    
    public void SendChat(int personId, string msg, int type, Action successCallback = null,
        Action failCallback = null)
    {
        Dictionary<string, string> param = new Dictionary<string, string>()
        {
            {"type", type.ToString()},
            {"msg", msg},
            {"to_user_id", personId.ToString()}
        };
        Post("https://api.wdtw.site/api/message/send", param, jsonData =>
        {
            if ((int) jsonData["code"] == 1)
            {
                successCallback?.Invoke();
                onChat?.Invoke();
            }
            else
            {
                failCallback?.Invoke();
            }
        }, failCallback);
    }

    public void GetChatDataHistory(Action<List<ChatData>> successCallback = null, Action failCallback = null)
    {
        Post("https://api.wdtw.site/api/message/userList", new Dictionary<string, string>(), jsonData =>
        {
            if ((int) jsonData["code"] == 1)
            {
                var chatDataList = new List<ChatData>();
                for (int i = 0; i < jsonData["data"].Count; i++)
                {
                    var chatData = new ChatData();
                    chatData.personId = (int) jsonData["data"][i]["user_info"]["user_id"];
                    var lineData = jsonData["data"][i]["last_message"];
                    chatData.person = new PersonConfig();
                    JsonData userData = null;
                    if ((int)lineData["from_user_id"] == chatData.personId)
                    {
                        userData = lineData["from_user"];
                    }
                    else
                    {
                        userData = lineData["to_user"];
                    }
                    if (userData.ContainsKey("user_id") && userData["user_id"] != null)
                    {
                        chatData.person.id = (int) userData["user_id"];
                    }

                    if (userData.ContainsKey("nick_name") && userData["nick_name"] != null)
                    {
                        chatData.person.name = (string) userData["nick_name"];
                    }

                    if (userData.ContainsKey("user_header") && userData["user_header"] != null)
                    {
                        int.TryParse((string) userData["user_header"], out chatData.person.head);
                    }

                    if (userData.ContainsKey("user_sign") && userData["user_sign"] != null)
                    {
                        chatData.person.description = (string) userData["user_sign"];
                    }
                    var chatLine = new ChatLine
                    {
                        timeStamp = Utils.DateTime2Timestamp(DateTime.Parse((string) lineData["created_at"])),
                        isMyContent = (int) lineData["from_user"]["user_id"] != chatData.personId
                    };
                    chatLine.content = (string) lineData["msg"];
                    chatData.chatLines.Add(chatLine);
                    chatDataList.Add(chatData);
                }
                successCallback?.Invoke(chatDataList);
            }
        }, failCallback);
    }

    public void GetChatData(int personId, Action<ChatData> successCallback = null, Action failCallback = null)
    {
        Dictionary<string, string> param = new Dictionary<string, string>()
        {
            {"page", "1"},
            {"page_size", "99"},
            {"user_id", personId.ToString()}
        };
        Post("https://api.wdtw.site/api/message/list", param, jsonData =>
        {
            if ((int) jsonData["code"] == 1)
            {
                var chatData = new ChatData {personId = personId};
                for (int i = 0; i < jsonData["data"]["data"].Count; i++)
                {
                    var lineData = jsonData["data"]["data"][i];
                    var chatLine = new ChatLine
                    {
                        timeStamp = (int) lineData["created_at"],
                        isMyContent = (int) lineData["from_user"]["user_id"] != personId
                    };
                    if ((int) lineData["type"] == 0)
                    {
                        chatLine.content = (string) lineData["msg"];
                    }
                    else
                    {
                        chatLine.texture = JsonUtility.FromJson<SerializableTexture>((string) lineData["msg"]);
                    }

                    chatData.chatLines.Add(chatLine);
                }

                chatData.chatLines.Sort((a, b) => a.timeStamp - b.timeStamp);
                successCallback?.Invoke(chatData);
            }
            else
            {
                failCallback?.Invoke();
            }
        }, failCallback);
    }

    public void Block(PersonConfig person, int status, Action successCallback = null, Action failCallback = null)
    {
        Dictionary<string, string> param = new Dictionary<string, string>()
        {
            {"to_user_id", person.id.ToString()},
            {"status", status.ToString()}
        };
        Post("https://api.wdtw.site/api/block/commit", param, jsonData =>
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
        }, failCallback);
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
                    if (personData.ContainsKey("user_id") && personData["user_id"] != null)
                    {
                        person.id = (int) personData["user_id"];
                    }

                    if (personData.ContainsKey("nick_name") && personData["nick_name"] != null)
                    {
                        person.name = (string) personData["nick_name"];
                    }

                    if (personData.ContainsKey("user_header") && personData["user_header"] != null)
                    {
                        int.TryParse((string) personData["user_header"], out person.head);
                    }

                    if (personData.ContainsKey("user_sign") && personData["user_sign"] != null)
                    {
                        person.description = (string) personData["user_sign"];
                    }

                    if (personData.ContainsKey("score") && personData["score"] != null)
                    {
                        person.score = (int) personData["score"];
                    }

                    if (personData.ContainsKey("game_time") && personData["game_time"] != null)
                    {
                        int.TryParse((string) personData["game_time"], out person.challengeTime);
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
                    if (personData.ContainsKey("user_id") && personData["user_id"] != null)
                    {
                        person.id = (int) personData["user_id"];
                    }

                    if (personData.ContainsKey("nick_name") && personData["nick_name"] != null)
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

                    if (personData.ContainsKey("user_sign") && personData["user_sign"] != null)
                    {
                        person.description = (string) personData["user_sign"];
                    }

                    if (personData.ContainsKey("score") && personData["score"] != null)
                    {
                        person.score = (int) personData["score"];
                    }

                    if (personData.ContainsKey("game_time") && personData["game_time"] != null)
                    {
                        int.TryParse((string) personData["game_time"], out person.challengeTime);
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