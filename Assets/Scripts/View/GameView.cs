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
    public static readonly float ICON_COLLECT_DURATION = 0.3f;
    public static readonly int COLLECTION_LAYER = 5000;
    public static readonly int REMOVE_ITEM_COUNT = 3;
    public static readonly int REMOVE_ITEM_Y = 9;
    public static readonly int REMOVE_ITEM_X = 0;

    [SerializeField] private List<Transform> _clearBox;
    [SerializeField] private Button _buttonRestart;
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
    private int _oppId;

    private List<GameIcon> _gameIcons = new List<GameIcon>();
    private List<GameIcon> _collectIcons = new List<GameIcon>();
    private EventSystem _currentEventSystem;

    private Vector2 _lastPos;
    private int _lastLayer;
    private GameIcon _lastIcon = null;

    private void Awake()
    {
        _buttonRestart.onClick.AddListener(() => { Undo(); });
        _buttonRefresh.onClick.AddListener(() => { Flush(); });
        _buttonRemove.onClick.AddListener(() => { RemoveClearBox(); });
        _currentEventSystem = EventSystem.current;
    }

    private void Init(int personId)
    {
        _oppId = personId;
        _textMyName.text = UserData.Instance.UserName;
        _textMyScore.text = _currentScore.ToString();
        _imgMyHead.sprite = Utils.GetMyHead();//Utils.GetUserHead(UserData.Instance.UserHead);
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
        var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/GameView"),
            Main.Instance.canvas.transform);
        var script = go.GetComponent<GameView>();
        script.Init(personId);
    }


    private void StartGame()
    {
        ChangeMyScore(0);
        _buttonRemove.interactable = true;
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
        }

        if (_gameIcons.Count == 0)
        {
            Destroy(gameObject);
            GameEndView.OpenWin(_currentScore,_oppId);
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
                    if (other.position.y >= icon.position.y && other.position.x > icon.position.x &&
                        other.position.y - icon.position.y < 1 && other.position.x - icon.position.x < 1)
                    {
                        interactable = false;
                        break;
                    }
                } //不同层级只要x,y相差绝对值都不大于1就形成遮挡
                else if (other.layer > icon.layer)
                {
                    if (Math.Abs(other.position.y - icon.position.y) < 1 &&
                        Math.Abs(other.position.x - icon.position.x) < 1)
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
            return;
        }

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
}