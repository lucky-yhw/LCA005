using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public struct GameResult
{
    public int score;
    public float totalSeconds;
}

public class GameEndView : MonoBehaviour
{
    [SerializeField] private Button _buttonSend;
    [SerializeField] private Button _buttonReturn;

    [SerializeField] private Image _winnerHeadImg;
    [SerializeField] private Text _textWinnerScore;
    [SerializeField] private Text _textWinnerName;
    [SerializeField] private Text _textWinTime;
    
    [SerializeField] private Image _failerHeadImg;
    [SerializeField] private Text _textFailerScore;
    [SerializeField] private Text _textFailerName;
    [SerializeField] private Text _textFailTime;

    [SerializeField] private Text _textCoins;
    
    [SerializeField] private Button _buttonBack;
    
    [SerializeField] private GameObject _goFail;
    [SerializeField] private GameObject _goWin;
    
    [SerializeField] private GameObject _goWinner;
    [SerializeField] private GameObject _goFailer;
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

    private void InitWin(GameResult result, PersonConfig person)
    {
        _goFail.SetActive(false);
        _goWin.SetActive(true);
        if (person != null)
        {
            _goFailer.SetActive(true);
            _buttonSend.gameObject.SetActive(true);
            var personConfig = person;
            var isWin = result.score > personConfig.score ||
                        (result.score == personConfig.score && result.totalSeconds < personConfig.challengeTime);
            if (isWin)
            {
                _winnerHeadImg.sprite = Utils.GetMyHead();
                _textWinnerName.text = UserData.Instance.UserName;
                _textWinnerScore.text = result.score.ToString();
                _textWinTime.text = Utils.FormatSecondsStr(result.totalSeconds);
                _failerHeadImg.sprite = Utils.GetUserHead(personConfig.head);
                _textFailerName.text = personConfig.name;
                _textFailerScore.text = personConfig.score.ToString();
                _textFailTime.text = Utils.FormatSecondsStr(result.totalSeconds);
            }
            else
            {
                _failerHeadImg.sprite = Utils.GetMyHead();
                _textFailerName.text = UserData.Instance.UserName;
                _textFailerScore.text = result.score.ToString();
                _textFailTime.text = Utils.FormatSecondsStr(result.totalSeconds);
                _winnerHeadImg.sprite = Utils.GetUserHead(personConfig.head);
                _textWinnerName.text = personConfig.name;
                _textWinnerScore.text = personConfig.score.ToString();
                _textWinTime.text = Utils.FormatSecondsStr(personConfig.challengeTime);
            }   
        }
        else
        {
            _winnerHeadImg.sprite = Utils.GetMyHead();
            _textWinnerName.text = UserData.Instance.UserName;
            _textWinnerScore.text = result.score.ToString();
            _textWinTime.text = Utils.FormatSecondsStr(result.totalSeconds);
            _goFailer.SetActive(false);
            _buttonSend.gameObject.SetActive(false);
        }

        _textCoins.text = Utils.FormatGold(Const.WinGiftGold);
        ServerData.Instance.GetGold(Const.WinGiftGold);
        ServerData.Instance.UploadScore(result.score,(int)result.totalSeconds);
    }

    private void InitFail()
    {
        _goFail.SetActive(true);
        _goWin.SetActive(false);
    }
    
    public static void OpenWin(GameResult result, PersonConfig person = null)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/GameEndView"),
            Main.Instance.canvas.transform);
        var script = go.GetComponent<GameEndView>();
        script.InitWin(result,person);
    }
    
    public static void OpenFail()
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/GameEndView"),
            Main.Instance.canvas.transform);
        var script = go.GetComponent<GameEndView>();
        script.InitFail();
    }
}
