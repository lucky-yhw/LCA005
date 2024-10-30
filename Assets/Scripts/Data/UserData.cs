using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UserDataChangeDalegate();

[Serializable]
public class ChatLine
{
    public string content;
    public int timeStamp;
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
                    _instance = new UserData();
                }
            }
            return _instance;
        }
    }
    
    [SerializeField] private string _userName = "UserName";
    [SerializeField] private int _userAge = 18;
    [SerializeField] private string _userDescription = "I wan't to fly";
    [SerializeField] private int _userHead = 1;
    [SerializeField] private long _gold;
    [SerializeField] private List<ChatData> _chatDataList = new List<ChatData>();

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
    
    public List<ChatData> ChatDataList => _chatDataList;

    public void Save()
    {
        PlayerPrefs.SetString("UserData",JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }
}
