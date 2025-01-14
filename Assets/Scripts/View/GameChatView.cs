using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameChatView : MainViewChild
{
    [SerializeField] private Toggle _toggleExplore;
    [SerializeField] private Toggle _toggleMessages;

    [SerializeField] private GameObject _tabExplore;
    [SerializeField] private GameObject _tabMessages;

    [SerializeField] private ScrollRect _scrollRectExplore;
    [SerializeField] private ScrollRect _scrollRectMessages;

    [SerializeField] private GameObject _explorePrefab;
    [SerializeField] private GameObject _messagePrefab;

    private bool _exploreListShouldRefresh = true;
    private bool _messageListShouldRefresh = true;
    private int _curTab = 0;

    private void Awake()
    {
        _toggleExplore.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                ChangeTab(0);
            }
        });
        _toggleMessages.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                ChangeTab(1);
            }
        });
        ServerData.Instance.onBlock += OnUserListChange;
        ServerData.Instance.onReport += OnUserListChange;
    }

    private void OnDestroy()
    {
        ServerData.Instance.onBlock -= OnUserListChange;
        ServerData.Instance.onReport -= OnUserListChange;
    }

    public override void OnShow()
    {
        ChangeTab(0);
    }

    public override void OnHide()
    {
    }

    private void OnUserListChange()
    {
        _exploreListShouldRefresh = true;
        ChangeTab(_curTab);
    }
    
    private void ChangeTab(int tab) //0是Explore 1是Message
    {
        _toggleExplore.SetIsOnWithoutNotify(tab == 0);
        _toggleMessages.SetIsOnWithoutNotify(tab == 1);
        _tabExplore.SetActive(tab == 0);
        _tabMessages.SetActive(tab == 1);
        _curTab = tab;

        if (tab == 0)
        {
            ShowExplore();
        }
        else
        {
            ShowMessage();
        }
    }

    private void ShowExplore()
    {
        if (!_exploreListShouldRefresh)
        {
            return;
        }

        ServerData.Instance.GetUserList((exploreList) =>
        {
            _exploreListShouldRefresh = false;
            Utils.RefreshListItems(_scrollRectExplore, _explorePrefab, exploreList.Count, ((i, o) =>
            {
                var config = exploreList[i];
                var trans = o.transform;
                trans.Find("Head").GetComponent<Image>().sprite = Utils.GetUserHead(config.head);
                trans.Find("TextName").GetComponent<Text>().text = config.name;
                trans.Find("TextDesc").GetComponent<Text>().text = config.description;
                trans.Find("Btn_Message").GetComponent<Button>().onClick.AddListener(() =>
                {
                    //打开聊天界面
                    OpenChatView(config);
                });
                trans.Find("Btn_Video").GetComponent<Button>().onClick.AddListener(() =>
                {
                    //打开通话界面
                    OpenVideoView(config);
                });
                trans.Find("Btn_Report").GetComponent<Button>().onClick.AddListener(() => { OptionsView.Open(config); });
            }));
        }, () => { CommonTipsView.Open("Net error, please try again!", ShowExplore); });
    }

    private void ShowMessage()
    {
        // if (!_messageListShouldRefresh)
        // {
        //     return;
        // }
        ServerData.Instance.GetChatDataHistory((chatDataList) =>
        {
            _messageListShouldRefresh = false;
            chatDataList.Sort(((data, data1) => data.chatLines.Last().timeStamp - data1.chatLines.Last().timeStamp));
            Utils.RefreshListItems(_scrollRectMessages, _messagePrefab, chatDataList.Count, ((i, o) =>
            {
                var chatData = chatDataList[i];
                var config = chatData.person;
                var trans = o.transform;
                trans.Find("Head").GetComponent<Image>().sprite = Utils.GetUserHead(config.head);
                trans.Find("TextName").GetComponent<Text>().text = config.name;
                trans.Find("TextContent").GetComponent<Text>().text = chatData.chatLines.Last().content;
                trans.Find("TextTime").GetComponent<Text>().text =
                    Utils.Timestamp2DateTime(chatData.chatLines.Last().timeStamp).ToString("g");
                trans.Find("Btn_Message").GetComponent<Button>().onClick.AddListener(() => { OpenChatView(config); });
                trans.Find("Btn_Video").GetComponent<Button>().onClick.AddListener(() => { OpenVideoView(config); });
                trans.Find("Btn_Report").GetComponent<Button>().onClick.AddListener(() => { OptionsView.Open(config); });
            }));
        });
    }

    //打开聊天界面
    private void OpenVideoView(PersonConfig person)
    {
        CallView.Open(person);
    }

    //打开通话界面
    private void OpenChatView(PersonConfig person)
    {
        ChatView.Open(person);
    }
}