using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using JsonReader = LitJson.JsonReader;

public class GameView : MonoBehaviour
{
    [SerializeField] private List<Transform> _clearBox;
    [SerializeField] private Button _buttonUndo;
    [SerializeField] private Button _buttonRemove;
    [SerializeField] private Button _buttonRefresh;
    [SerializeField] private Text _textMyScore;
    [SerializeField] private Text _textOppScore;
    [SerializeField] private Text _textMyName;
    [SerializeField] private Text _textOppName;
    [SerializeField] private Image _imgMyHead;
    [SerializeField] private Image _imgOppHead;
    [SerializeField] private Transform _gameContainer;
    [SerializeField] private GameObject _itemPrefab;

    private int _currentScore = 0;

    private List<GameIcon> _gameIcons = new List<GameIcon>();
    private List<GameIcon> _collectIcons = new List<GameIcon>();


    private void Awake()
    {
        _buttonUndo.onClick.AddListener(() => { });
        _buttonRefresh.onClick.AddListener(() => { });
        _buttonRemove.onClick.AddListener(() => { });
    }

    private void Init(int personId)
    {
        _textMyName.text = UserData.Instance.UserName;
        _textMyScore.text = _currentScore.ToString();
        _imgMyHead.sprite = Utils.GetUserHead(UserData.Instance.UserHead);
        var config = ConfigLoader.Load<PersonConfigTable>().table[personId];
        _imgOppHead.sprite = Utils.GetUserHead(config.head);
        _textOppScore.text = config.score.ToString();
        _textOppName.text = config.name;
        StartGame();
    }

    private void ChangeMyScore(int newScore)
    {
        _currentScore = newScore;
        _textMyScore.text = _currentScore.ToString();
    }
    
    
    public static void Open(int personId)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/GameView"),Main.Instance.canvas.transform);
        var script = go.GetComponent<GameView>();
        script.Init(personId);
    }


    private void StartGame()
    {
        Utils.DestroyAll(_gameContainer);
        foreach (var t in _clearBox)
        {
            Utils.DestroyAll(t);
        }
        _gameIcons.Clear();
        _collectIcons.Clear();
        var json = JsonMapper.ToObject(Resources.Load<TextAsset>("Configs/gameConfig").text);
        var cfg = json["levelConfig"][1]["iconPosition"];
        for (int i = 0; i < cfg.Count; i++)
        {
            var go = GameObject.Instantiate(_itemPrefab, _gameContainer);
            var icon = go.GetComponent<GameIcon>();
            icon.Init(cfg[i]);
            _gameIcons.Add(icon);
            icon.GetComponent<Button>().onClick.AddListener(() =>
            {
                CollectIcon(icon);
            });
        }

        ResortAll();
    }

    private void CollectIcon(GameIcon icon)
    {
        Debug.Log("Click");
        _gameIcons.Remove(icon);
        _collectIcons.Add(icon);
        // EventSystem.current.enabled = false;
        icon.GetComponent<Button>().enabled = false;
        icon.transform.DOMove(_clearBox[_collectIcons.IndexOf(icon)].position, 0.5f).OnComplete(() =>
        {
            icon.transform.SetParent(_clearBox[_collectIcons.IndexOf(icon)]);
            // EventSystem.current.enabled = true;
            if (_collectIcons.Count == _clearBox.Count)
            {
                Debug.Log("GameEnd");
                StartGame();
            }
        });
        ResortAll();
    }

    public void ResortAll()
    {
        foreach (var icon in _gameIcons)
        {
            bool interactable = true;
            foreach (var other in _gameIcons)
            {
                if (other == icon)
                {
                    continue;
                }
                //同一层级只有xy大于自己且不超过1才会遮挡
                if (other.layer==icon.layer)
                {
                    if (other.position.y >= icon.position.y && other.position.x > icon.position.x && other.position.y - icon.position.y<1 && other.position.x - icon.position.x<1)
                    {
                        interactable = false;
                        break;
                    }
                }//不同层级只要x,y相差绝对值都不大于1就形成遮挡
                else if (other.layer > icon.layer)
                {
                    if (Math.Abs(other.position.y-icon.position.y)<1 && Math.Abs(other.position.x-icon.position.x)<1)
                    {
                        interactable = false;
                        break;
                    }
                }
            }
            icon.Interactable = interactable;
        }
    }
}