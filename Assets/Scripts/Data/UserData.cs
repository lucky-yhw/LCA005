using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;
using Random = UnityEngine.Random;

public delegate void UserDataChangeDalegate();

public delegate void BlockChangeDelegate();

[Serializable]
public class SerializableTexture
{
    public byte[] textureData;
    public int width;
    public int height;
    public TextureFormat textureFormat;

    public SerializableTexture(Texture2D texture)
    {
        textureData = texture.EncodeToPNG(); // 使用PNG编码作为示例
        width = texture.width;
        height = texture.height;
        textureFormat = texture.format;
    }

    public Texture2D ToTexture2D()
    {
        Texture2D texture = new Texture2D(width, height, textureFormat, false);
        texture.LoadImage(textureData);
        return texture;
    }
}


[Serializable]
public class ChatLine
{
    public string content = "";
    public SerializableTexture texture;
    public int timeStamp = 0;
    public bool isMyContent = true;
}

[Serializable]
public class ChatData
{
    public PersonConfig person;
    public int personId = 0;
    public List<ChatLine> chatLines = new List<ChatLine>();
}

[Serializable]
public class UserData
{
    private static UserData _instance;

    public static UserData Instance
    {
        get
        {
            if (_instance == null)
            {
                // var save = PlayerPrefs.GetString("UserData", "");
                // if (!string.IsNullOrEmpty(save))
                // {
                //     _instance = JsonUtility.FromJson<UserData>(save);
                // }
                // else
                // {
                //     _instance = new UserData {_userName = "Guest_" + Random.Range(100000, 999999)};
                // }
                _instance = new UserData();
            }

            return _instance;
        }
    }

    [SerializeField] private string _token = "";
    [SerializeField] private int _userId = 0;
    [SerializeField] private string _userName = "";
    [SerializeField] private int _userAge = 18;
    [SerializeField] private string _userDescription = "empty description";
    [SerializeField] private int _userHead = 1;
    [SerializeField] private int _sex = 1;
    [SerializeField] private SerializableTexture _customerHead;
    [SerializeField] private SerializableTexture _background;
    [SerializeField] private long _gold;
    [SerializeField] private bool _inReview = false;

    public UserDataChangeDalegate OnDataChanged;
    public BlockChangeDelegate OnBlockChanged;

    public string UserName
    {
        get => _userName;
        set
        {
            _userName = value;
            _inReview = true;
            OnDataChanged?.Invoke();
            Save();
        }
    }

    public int UserId => _userId;

    public int UserAge
    {
        get => _userAge;
        set
        {
            _userAge = value;
            OnDataChanged?.Invoke();
            Save();
        }
    }

    public string UserDescription
    {
        get => _userDescription;
        set
        {
            _userDescription = value;
            _inReview = true;
            OnDataChanged?.Invoke();
            Save();
        }
    }

    public int UserHead
    {
        get => _userHead;
        set
        {
            _userHead = value;
            OnDataChanged?.Invoke();
            Save();
        }
    }

    public long Gold
    {
        get => _gold;
        set
        {
            _gold = value;
            OnDataChanged?.Invoke();
            Save();
        }
    }
    
    public string Token
    {
        get => _token;
        set
        {
            _token = value;
        }
    }

    public int Sex
    {
        get => _sex;
        set
        {
            _sex = value;
            OnDataChanged?.Invoke();
            Save();
        }
    }

    public SerializableTexture CustomerHead
    {
        get => _customerHead;
        set
        {
            _customerHead = value;
            _inReview = true;
            OnDataChanged?.Invoke();
            Save();
        }
    }

    public SerializableTexture Background
    {
        get => _background;
        set
        {
            _background = value;
            _inReview = true;
            OnDataChanged?.Invoke();
            Save();
        }
    }

    public bool InReview => _inReview;

    public void Save()
    {
        // PlayerPrefs.SetString("UserData", JsonUtility.ToJson(this));
        // PlayerPrefs.Save();
    }

    public void Delete()
    {
        _instance = null;
        // PlayerPrefs.SetString("UserData", "");
        // PlayerPrefs.Save();
    }

    public void InitByServerData(JsonData data)
    {
        if (data.ContainsKey("user_header"))
        {
            var userHeader = data["user_header"].ToString();
            if (string.IsNullOrEmpty(userHeader))
            {
                _userHead = 1;
            }
            else
            {
                _userHead = int.Parse(userHeader);
            }   
        }

        if (data.ContainsKey("nick_name"))
        {
            _userName = data["nick_name"].ToString();
        }

        if (data.ContainsKey("user_id"))
        {
            _userId = (int) data["user_id"];
        }

        if (data.ContainsKey("age"))
        {
            _userAge = (int) data["age"];
        }

        if (data.ContainsKey("chips"))
        {
            _gold = (long) data["chips"];
        }

        if (data.ContainsKey("sex"))
        {
            if (!int.TryParse(data["sex"].ToString(),out _sex))
            {
                _sex = -1;
            }
        }

        if (data.ContainsKey("user_sign"))
        {
            _userDescription = data["user_sign"] == null ? "" : data["user_sign"].ToString();
        }

        Save();
    }
}