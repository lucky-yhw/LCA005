using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public delegate void UserDataChangeDalegate();

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
                var save = PlayerPrefs.GetString("UserData", "");
                if (!string.IsNullOrEmpty(save))
                {
                    _instance = JsonUtility.FromJson<UserData>(save);
                }
                else
                {
                    _instance = new UserData {_userName = "Guest_" + Random.Range(100000, 999999)};
                }
            }
            return _instance;
        }
    }
    
    [SerializeField] private string _userName = "";
    [SerializeField] private int _userAge = 18;
    [SerializeField] private string _userDescription = "empty description";
    [SerializeField] private int _userHead = 1;
    [SerializeField] private SerializableTexture _customerHead;
    [SerializeField] private SerializableTexture _background;
    [SerializeField] private long _gold;
    [SerializeField] private List<ChatData> _chatDataList = new List<ChatData>();
    [SerializeField] private List<int> _blockList = new List<int>();

    public UserDataChangeDalegate OnDataChanged;
    
    public string UserName
    {
        get => _userName;
        set
        {
            _userName = value;
            OnDataChanged?.Invoke();
            Save();
        }
    }
    
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
    
    public SerializableTexture CustomerHead
    {
        get => _customerHead;
        set
        {
            _customerHead = value;
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
            OnDataChanged?.Invoke();
            Save();
        }
    }
    
    public IReadOnlyList<ChatData> ChatDataList => _chatDataList;

    public IReadOnlyList<int> BlockList => _blockList;
    
    public void Save()
    {
        PlayerPrefs.SetString("UserData",JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }

    public ChatData GetChatData(int personId)
    {
        foreach (var chatData in _chatDataList)
        {
            if (chatData.personId == personId)
            {
                return chatData;
            }
        }

        var data = new ChatData {personId = personId, chatLines = new List<ChatLine>()};
        return data;
    }

    public void SaveChat(int personId,bool isMyChat,string content,SerializableTexture texture)
    {
        var chatLine = new ChatLine
        {
            timeStamp = Utils.DateTime2Timestamp(DateTime.Now), content = content, isMyContent = isMyChat,
            texture = texture
        };
        ChatData data = null;
        foreach (var chatData in _chatDataList)
        {
            if (chatData.personId == personId)
            {
                data = chatData;
            }
        }

        if (data == null)
        {
            data = new ChatData {personId = personId, chatLines = new List<ChatLine>()};   
            _chatDataList.Add(data);
        }
        data.chatLines.Add(chatLine);
        OnDataChanged?.Invoke();
        Save();
    }

    public void Delete()
    {
        _instance = null;
        PlayerPrefs.SetString("UserData","");
        PlayerPrefs.Save();
    }

    public void Block(int personId)
    {
        if (!_blockList.Contains(personId))
        {
            _blockList.Add(personId);
            OnDataChanged?.Invoke();
            Save();
        }
    }

    public void RemoveBlock(int personId)
    {
        if (_blockList.Contains(personId))
        {
            _blockList.Remove(personId);
            OnDataChanged?.Invoke();
            Save();
        }
    }
}
