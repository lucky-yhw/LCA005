using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    public override void OnShow()
    {
        ChangeTab(0);
    }

    public override void OnHide()
    {
        
    }

    private void ChangeTab(int tab)//0是Explore 1是Message
    {
        _toggleExplore.SetIsOnWithoutNotify(tab == 0);
        _toggleMessages.SetIsOnWithoutNotify(tab == 1);
        _tabExplore.SetActive(tab == 0);
        _tabMessages.SetActive(tab == 1);
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
        
    }

    private void ShowMessage()
    {
        
    }
}
