using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameEndView : MonoBehaviour
{
    [SerializeField] private Button _buttonSend;
    [SerializeField] private Button _buttonReturn;

    [SerializeField] private Image _winnerHeadImg;
    [SerializeField] private Text _textWinnerScore;
    [SerializeField] private Text _textWinnerName;
    
    [SerializeField] private Image _failerHeadImg;
    [SerializeField] private Text _textFailerScore;
    [SerializeField] private Text _textFailerName;

    [SerializeField] private Text _textCoins;
    
    [SerializeField] private Button _buttonBack;
    
    [SerializeField] private GameObject _goFail;
    [SerializeField] private GameObject _goWin;
    private void Awake()
    {
        _buttonSend.onClick.AddListener(() =>
        {
            
        });
        _buttonReturn.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
        _buttonBack.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
    }

    private void InitWin(int score, int personId)
    {
        _goFail.SetActive(false);
        _goWin.SetActive(true);
        var personConfig = ConfigLoader.Load<PersonConfigTable>().table[personId];
        if (score > personConfig.score)
        {
            _winnerHeadImg.sprite = Utils.GetMyHead();
            _textWinnerName.text = UserData.Instance.UserName;
            _textWinnerScore.text = score.ToString();
            _failerHeadImg.sprite = Utils.GetUserHead(personConfig.head);
            _textFailerName.text = personConfig.name;
            _textFailerScore.text = personConfig.score.ToString();
        }
        else
        {
            _failerHeadImg.sprite = Utils.GetMyHead();
            _textFailerName.text = UserData.Instance.UserName;
            _textFailerScore.text = score.ToString();
            _winnerHeadImg.sprite = Utils.GetUserHead(personConfig.head);
            _textWinnerName.text = personConfig.name;
            _textWinnerScore.text = personConfig.score.ToString();
        }

        _textCoins.text = Utils.FormatGold(Utils.WIN_GIFT_GOLD);
        UserData.Instance.Gold += Utils.WIN_GIFT_GOLD;
    }

    private void InitFail()
    {
        _goFail.SetActive(true);
        _goWin.SetActive(false);
    }
    
    public static void OpenWin(int score, int personId)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/GameEndView"),
            Main.Instance.canvas.transform);
        var script = go.GetComponent<GameEndView>();
        script.InitWin(score,personId);
    }
    
    public static void OpenFail()
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/GameEndView"),
            Main.Instance.canvas.transform);
        var script = go.GetComponent<GameEndView>();
        script.InitFail();
    }
}
