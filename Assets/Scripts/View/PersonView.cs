using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonView : MainViewChild
{
    [SerializeField] private Text _textName;
    [SerializeField] private Text _textGold;
    [SerializeField] private Image _imgHead;
    
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _blockListButton;
    [SerializeField] private Button _feedbackButton;
    [SerializeField] private Button _termsButton;
    [SerializeField] private Button _policyButton;
    [SerializeField] private Button _rateButton;
    [SerializeField] private Button _logoutButton;
    
    private void Awake()
    {
        UserData.Instance.OnDataChanged += OnDataChanged;
        _logoutButton.onClick.AddListener(LogoutPopup.Open);
        _editButton.onClick.AddListener(EditProfileView.Open);
        _blockListButton.onClick.AddListener(BlockListView.Open);
        _feedbackButton.onClick.AddListener(FeedBackView.Open);
    }

    private void OnDestroy()
    {
        UserData.Instance.OnDataChanged += OnDataChanged;
    }

    
    private void OnDataChanged()
    {
        RefreshUserInfo();
    }

    private void RefreshUserInfo()
    {
        _textName.text = UserData.Instance.UserName;
        _imgHead.sprite = Utils.GetMyHead();//Utils.GetUserHead(UserData.Instance.UserHead);
        _textGold.text = Utils.FormatGold(UserData.Instance.Gold);
    }
    
    public override void OnShow()
    {
        RefreshUserInfo();
    }

    public override void OnHide()
    {
        
    }
}