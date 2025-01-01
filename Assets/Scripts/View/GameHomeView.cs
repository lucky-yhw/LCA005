using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameHomeView : MainViewChild
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Image _headImg;
    [SerializeField] private Text _goldText;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private Button _buttonPurchase;

    private bool _challengeListShouldRefresh = true;
    
    private void Awake()
    {_buttonPurchase.onClick.AddListener(() =>
        {
            PurchaseView.Open();
        });
    }

    public override void OnShow()
    {
        UserData.Instance.OnDataChanged += OnDataChanged;
        UserData.Instance.OnBlockChanged += RefreshChallengePeople;
        RefreshUserData();
        RefreshChallengePeople();
    }

    public override void OnHide()
    {
        UserData.Instance.OnDataChanged -= OnDataChanged;
        UserData.Instance.OnBlockChanged -= RefreshChallengePeople;
    }

    private void OnDataChanged()
    {
        RefreshUserData();
    }

    private void RefreshUserData()
    {
        _nameText.text = UserData.Instance.UserName;
        _headImg.sprite = Utils.GetMyHead();//Utils.GetUserHead(UserData.Instance.UserHead);
        _goldText.text = Utils.FormatGold(UserData.Instance.Gold);
    }

    private void RefreshChallengePeople()
    {
        if (!_challengeListShouldRefresh)
        {
            return;
        }
        ServerData.Instance.GetUserList((exploreList) =>
        {
            _challengeListShouldRefresh = false;
            Utils.RefreshListItems(_scrollRect, _itemPrefab, exploreList.Count,
                ((i, o) => { o.GetComponent<ChallengeItem>().UpdateData(exploreList[i]); }));
        }, () =>
        {
            CommonTipsView.Open("Net error, please try again!", RefreshChallengePeople);
        });
    }
}