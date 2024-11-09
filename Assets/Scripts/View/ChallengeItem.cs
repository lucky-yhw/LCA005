using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeItem : MonoBehaviour
{
    [SerializeField] private Text _textName;
    [SerializeField] private Text _textDescription;
    [SerializeField] private Text _textTime;
    [SerializeField] private Text _textScore;
    [SerializeField] private Image _headImg;
    [SerializeField] private Button _challengeButton;
    [SerializeField] private Button _reportButton;
    
    public void UpdateData(PersonConfig config)
    {
        _textName.text = config.name;
        _textDescription.text = config.description;
        _textTime.text = Utils.FormatTime(config.challengeTime);
        _textScore.text = config.score.ToString();
        _headImg.sprite = Utils.GetUserHead(config.head);
        _challengeButton.onClick.AddListener(() =>
        {
            //挑战
            GameView.Open(config.id);
        });
        _reportButton.onClick.AddListener(() =>
        {
            //举报
        });
    }
}