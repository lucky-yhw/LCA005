using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using JsonReader = LitJson.JsonReader;
using Random = UnityEngine.Random;

public class GameView : MonoBehaviour
{
    public static readonly int ICON_CLEAR_COUNT = 3;
    public static readonly float ICON_COLLECT_DURATION = 0.2f;
    public static readonly int COLLECTION_LAYER = 5000;
    public static readonly int REMOVE_ITEM_COUNT = 3;
    public static readonly int REMOVE_ITEM_Y = 9;
    public static readonly int REMOVE_ITEM_X = 0;

    public static readonly int UNDO_GOLD = 5 * Const.GoldRate;
    public static readonly int REFRESH_GOLD = 5 * Const.GoldRate;
    public static readonly int REMOVE_GOLD = 10 * Const.GoldRate;

    [SerializeField] private List<Transform> _clearBox;
    [SerializeField] private Button _buttonUndo;
    [SerializeField] private Text _textUndoGold;
    [SerializeField] private Button _buttonRemove;
    [SerializeField] private Text _textRemoveGold;
    [SerializeField] private Button _buttonRefresh;
    [SerializeField] private Text _textRefreshGold;
    [SerializeField] private Button _buttonClose;
    [SerializeField] private Text _textMyScore;
    [SerializeField] private Text _textOppScore;
    [SerializeField] private Text _textMyName;
    [SerializeField] private Text _textOppName;
    [SerializeField] private Text _textTime;
    [SerializeField] private Image _imgMyHead;
    [SerializeField] private Image _imgOppHead;
    [SerializeField] private Transform _gameContainer;
    [SerializeField] private GameObject _itemPrefab;

    private int _currentScore = 0;
    private float _currentSeconds = 0;
    private PersonConfig _oppPerson;

    private List<GameIcon> _gameIcons = new List<GameIcon>();
    private List<GameIcon> _collectIcons = new List<GameIcon>();
    private EventSystem _currentEventSystem;

    private Vector2 _lastPos;
    private int _lastLayer;
    private GameIcon _lastIcon = null;

    private void Awake()
    {
        _buttonUndo.onClick.AddListener(Undo);
        _buttonRefresh.onClick.AddListener(Flush);
        _buttonRemove.onClick.AddListener(RemoveClearBox);
        _buttonClose.onClick.AddListener(() =>
        {
            CommonTipsView.Open("Are Your Sure To Quit?", () => { Destroy(gameObject); });
        });
        _currentEventSystem = EventSystem.current;
        _textUndoGold.text = "X" + Utils.FormatGold(UNDO_GOLD);
        _textRemoveGold.text = "X" + Utils.FormatGold(REMOVE_GOLD);
        _textRefreshGold.text = "X" + Utils.FormatGold(REFRESH_GOLD);
    }

    private void Init(PersonConfig person)
    {
        _oppPerson = person;
        if (_oppPerson != null)
        {
            _textMyName.text = UserData.Instance.UserName;
            _textMyScore.text = _currentScore.ToString();
            _imgMyHead.sprite = Utils.GetMyHead(); //Utils.GetUserHead(UserData.Instance.UserHead);
            _imgOppHead.sprite = Utils.GetUserHead(_oppPerson.head);
            _textOppScore.text = _oppPerson.score.ToString();
            _textOppName.text = _oppPerson.name;  
            _imgOppHead.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            _textMyName.text = UserData.Instance.UserName;
            _textMyScore.text = _currentScore.ToString();
            _imgMyHead.sprite = Utils.GetMyHead(); //Utils.GetUserHead(UserData.Instance.UserHead);
            _imgOppHead.transform.parent.gameObject.SetActive(false);
        }
        StartGame();
    }

    private void ChangeMyScore(int newScore)
    {
        _currentScore = newScore;
        _textMyScore.text = _currentScore.ToString();
    }


    public static void Open(PersonConfig person = null)
    {
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/GameView"),
            Main.Instance.canvas.transform);
        var script = go.GetComponent<GameView>();
        script.Init(person);
    }


    private void StartGame()
    {
        ChangeMyScore(0);
        _buttonRemove.interactable = true;
        _buttonUndo.interactable = false;
        Utils.DestroyAll(_gameContainer);
        foreach (var t in _clearBox)
        {
            Utils.DestroyAll(t);
        }

        _gameIcons.Clear();
        _collectIcons.Clear();
        _currentSeconds = 0;
        var json = JsonMapper.ToObject(Resources.Load<TextAsset>("Configs/gameConfig").text);
        var ran = Random.Range(0, json["levelConfig"].Count);
        var cfg = json["levelConfig"][ran]["iconPosition"];
        for (int i = 0; i < cfg.Count; i++)
        {
            var go = GameObject.Instantiate(_itemPrefab, _gameContainer);
            var icon = go.GetComponent<GameIcon>();
            icon.Init(cfg[i]);
            _gameIcons.Add(icon);
            icon.GetComponent<Button>().onClick.AddListener(() => { CollectIcon(icon); });
        }

        var totalItems = new List<GameIcon>(_gameIcons);
        var types = totalItems.Count / ICON_CLEAR_COUNT;
        for (int i = 0; i < types; i++)
        {
            var iconType = Random.Range(1, 7);
            for (int j = 0; j < ICON_CLEAR_COUNT; j++)
            {
                var idx = Random.Range(0, totalItems.Count);
                var icon = totalItems[idx];
                icon.type = iconType;
                totalItems.RemoveAt(idx);
            }
        }

        ResortAll();
    }

    private void CollectIcon(GameIcon icon)
    {
        if (!_gameIcons.Contains(icon))
        {
            return;
        }

        _lastIcon = icon;
        _lastLayer = icon.layer;
        _lastPos = icon.position;
        _gameIcons.Remove(icon);
        _collectIcons.Add(icon);
        _buttonUndo.interactable = true;
        _currentEventSystem.enabled = false;
        icon.transform.SetParent(_clearBox[_collectIcons.IndexOf(icon)]);
        icon.transform.DOMove(_clearBox[_collectIcons.IndexOf(icon)].position, ICON_COLLECT_DURATION);
        icon.status = IconStatus.Collect;
        icon.SetSortingOrder(COLLECTION_LAYER);
        StartCoroutine(WaitForAni(icon));
        ResortAll();
    }

    IEnumerator WaitForAni(GameIcon icon)
    {
        yield return new WaitForSeconds(ICON_COLLECT_DURATION);
        _currentEventSystem.enabled = true;
        var removal = _collectIcons.FindAll((i) => i.type == icon.type);

        if (removal.Count == ICON_CLEAR_COUNT)
        {
            _collectIcons.RemoveAll(removal.Contains);
            foreach (var removeIcon in removal)
            {
                Destroy(removeIcon.gameObject);
            }

            for (int i = 0; i < _collectIcons.Count; i++)
            {
                var leftIcon = _collectIcons[i];
                leftIcon.transform.SetParent(_clearBox[i]);
                leftIcon.transform.DOMove(_clearBox[i].position, ICON_COLLECT_DURATION);
            }

            ChangeMyScore(_currentScore + ICON_CLEAR_COUNT);
            if (removal.Contains(_lastIcon))
            {
                _buttonUndo.interactable = false;
            }
        }

        if (_gameIcons.Count == 0)
        {
            Destroy(gameObject);
            GameEndView.OpenWin(new GameResult(){score = _currentScore,totalSeconds = _currentSeconds}, _oppPerson);
            yield break;
        }

        if (_collectIcons.Count == _clearBox.Count)
        {
            Destroy(gameObject);
            GameEndView.OpenFail();
        }
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
                if (other.layer == icon.layer)
                {
                    if (other.position.y > icon.position.y && other.position.x > icon.position.x &&
                        Math.Abs(other.position.y - icon.position.y) < 1-1e-5 && Math.Abs(other.position.x - icon.position.x) < 1-1e-5)
                    {
                        interactable = false;
                        break;
                    }
                } //不同层级只要x,y相差绝对值都不大于1就形成遮挡
                else if (other.layer > icon.layer)
                {
                    if (Math.Abs(other.position.y - icon.position.y) < 1-1e-5 &&
                        Math.Abs(other.position.x - icon.position.x) < 1-1e-5)
                    {
                        interactable = false;
                        break;
                    }
                }
            }

            icon.Interactable = interactable;
        }
    }

    public void Undo()
    {
        if (_lastIcon)
        {
            if (UserData.Instance.Gold < UNDO_GOLD)
            {
                MsgView.Open("Gold Not Enough");
                return;
            }
            
            ServerData.Instance.CostGold(UNDO_GOLD);

            _buttonUndo.interactable = false;
            _collectIcons.Remove(_lastIcon);
            _gameIcons.Add(_lastIcon);
            _lastIcon.transform.SetParent(_gameContainer);
            _lastIcon.status = IconStatus.Game;
            _lastIcon.position = _lastPos;
            _lastIcon.layer = _lastLayer;
            _lastIcon = null;
            ResortAll();
        }
    }

    public void Flush()
    {
        if (UserData.Instance.Gold < REFRESH_GOLD)
        {
            MsgView.Open("Gold Not Enough");
            return;
        }
        
        ServerData.Instance.CostGold(REFRESH_GOLD);
        List<Vector2> positions = new List<Vector2>();
        List<int> layers = new List<int>();
        List<GameIcon> icons = new List<GameIcon>(_gameIcons);
        //打乱
        for (int i = 0; i < _gameIcons.Count; i++)
        {
            var icon = icons[i];
            positions.Add(icon.position);
            layers.Add(icon.layer);
        }

        //打乱
        for (int i = 0; i < _gameIcons.Count; i++)
        {
            var icon = icons[i];
            var newIdx = Random.Range(0, icons.Count);
            var tmp = icons[newIdx];
            icons[i] = tmp;
            icons[newIdx] = icon;
        }

        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].position = positions[i];
            icons[i].layer = layers[i];
        }

        ResortAll();
    }

    public void RemoveClearBox()
    {
        if (_collectIcons.Count < REMOVE_ITEM_COUNT)
        {
            MsgView.Open(string.Format("Please Collect {0} Icons First!", REMOVE_ITEM_COUNT));
            return;
        }

        if (UserData.Instance.Gold < REMOVE_GOLD)
        {
            MsgView.Open("Gold Not Enough");
            return;
        }
        
        ServerData.Instance.CostGold(REMOVE_GOLD);
        _buttonRemove.interactable = false;
        for (int i = 0; i < REMOVE_ITEM_COUNT; i++)
        {
            if (_collectIcons.Count > 0)
            {
                var icon = _collectIcons.Last();
                icon.transform.SetParent(_gameContainer);
                _collectIcons.Remove(icon);
                _gameIcons.Add(icon);
                icon.status = IconStatus.Game;
                icon.position = new Vector2(REMOVE_ITEM_X + i, REMOVE_ITEM_Y);
                icon.layer = 200;
            }
        }

        ResortAll();
    }

    private void Update()
    {
        _currentSeconds += Time.deltaTime;
        _textTime.text = Utils.FormatSecondsStr(_currentSeconds);
    }
}