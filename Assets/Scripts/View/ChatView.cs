using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatView : MonoBehaviour
{
    [SerializeField] private Text _textName;
    [SerializeField] private Button _buttonClose;
    [SerializeField] private Button _buttonReport;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _chatContentItemPrefab;
    [SerializeField] private Button _buttonSend;
    [SerializeField] private InputField _inputField;
    [SerializeField] private Button _buttonPicture;

    private PersonConfig _person;

    private void Awake()
    {
        UserData.Instance.OnDataChanged += OnUserChanged;
        _buttonClose.onClick.AddListener(() => { Destroy(gameObject); });
        _buttonReport.onClick.AddListener(() =>
        {
            ReportView.Open(_person);
        });
        _buttonPicture.onClick.AddListener(() =>
        {
            Utils.PickImage((s) =>
            {
                ServerData.Instance.SendChat(_person.id, JsonUtility.ToJson(new SerializableTexture(s)), 1, RefreshUI);
            });
        });
        _buttonSend.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(_inputField.text))
            {
                ServerData.Instance.SendChat(_person.id, _inputField.text, 0, RefreshUI);
            }

            _inputField.text = "";
        });
    }

    private void OnUserChanged()
    {
        RefreshUI();
    }

    private void OnDestroy()
    {
        UserData.Instance.OnDataChanged -= OnUserChanged;
    }

    public void InitParams(PersonConfig person)
    {
        _person = person;
        _textName.text = person.name;
        RefreshUI();
    }

    private void RefreshUI()
    {

        ServerData.Instance.GetChatData(_person.id, (chatData) =>
        {
            Utils.RefreshListItems(_scrollRect, _chatContentItemPrefab, chatData.chatLines.Count, (index, go) =>
            {
                var tr = go.transform;
                var line = chatData.chatLines[index];
                if (!string.IsNullOrEmpty(line.content))
                {
                    var str = chatData.chatLines[index].content;
                    var split = 40;
                    if (Utils.ContainsBlockCharacter(str))
                    {
                        split = 16;
                    }
                    string content = Utils.AddNewLineEveryNCharacters(chatData.chatLines[index].content, split);
                    tr.Find("MyChat/BG/TextContent").GetComponent<Text>().text = content;
                    tr.Find("MyChat/BG/TextContent").gameObject.SetActive(true);
                }

                if (line.texture != null && line.texture.textureData.Length > 0)
                {
                    var sprite = Sprite.Create(line.texture.ToTexture2D(),
                        new Rect(Vector2.zero,
                            new Vector2(line.texture.width, line.texture.height)),
                        new Vector2(0.5f, 0.5f));
                    tr.Find("MyChat/BG/ImageContent").GetComponent<Image>().sprite = sprite;
                    tr.Find("MyChat/BG/ImageContent").GetComponent<Image>().SetNativeSize();
                    tr.Find("MyChat/BG/ImageContent").gameObject.SetActive(true);
                }
            });
            _scrollRect.content.ForceRebuildImmediate(true);
            _scrollRect.verticalNormalizedPosition = 1;
        });
    }

    public static void Open(PersonConfig person)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/ChatView"),
            Main.Instance.canvas.transform);
        go.GetComponent<ChatView>().InitParams(person);
    }
}