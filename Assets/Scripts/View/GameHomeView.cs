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

    public override void OnShow()
    {
        UserData.Instance.OnDataChanged += OnDataChanged;
        RefreshUserData();
        RefreshChallengePeople();
    }

    public override void OnHide()
    {
        UserData.Instance.OnDataChanged -= OnDataChanged;
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
        var ranPerson = Utils.RandomPerson(10);
        Utils.RefreshListItems(_scrollRect, _itemPrefab, ranPerson.Count,
            ((i, o) => { o.GetComponent<ChallengeItem>().UpdateData(ranPerson[i]); }));
    }
}