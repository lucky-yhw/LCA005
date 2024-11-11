using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public enum IconStatus
{
    Game,
    Collect
}

public class GameIcon : MonoBehaviour
{
    [SerializeField] private Image _imgIcon;

    public int id { get; private set; }


    private IconStatus _status = IconStatus.Game;
    public IconStatus status
    {
        get => _status;
        set
        {
            _status = value;
            if (_status == IconStatus.Collect)
            {
                position = Vector2.zero;
                layer = 0;
            }
            _button.enabled = _status == IconStatus.Game;
        }
    }
    
    private Vector2 _position;

    public Vector2 position
    {
        get => _position;
        set
        {
            if (status == IconStatus.Collect)
            {
                return;
            }

            _position = value;
            SetAnchorPosition();
            Resort();
        }
    }


    private int _layer;

    public int layer
    {
        get => _layer;
        set
        {
            if (status == IconStatus.Collect)
            {
                return;
            }

            _layer = value;
            Resort();
        }
    }

    private int _type;

    public int type
    {
        get => _type;
        set
        {
            _type = value;
            _imgIcon.sprite = Resources.Load<Sprite>("Textures/GameIcon/" + type);
        }
    }

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Button _button;

    public bool Interactable
    {
        get => _button.interactable;
        set { _button.interactable = value; }
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponent<Canvas>();
        _button = GetComponent<Button>();
        _canvas.overrideSorting = true;
    }

    public void Init(JsonData jsonData)
    {
        id = Utils.GetJsonInt(jsonData["id"]);
        _position = new Vector2(Utils.GetJsonSingle(jsonData["position"][0]),
            Utils.GetJsonSingle(jsonData["position"][1]));
        _layer = Utils.GetJsonInt(jsonData["position"][2]);
        SetAnchorPosition();
        Resort();
    }

    public void SetSortingOrder(int sortingOrder)
    {
        _canvas.sortingOrder = sortingOrder;
    }

    private void SetAnchorPosition()
    {
        var rect = _rectTransform.rect;
        _rectTransform.anchoredPosition = new Vector2(position[0] * rect.width,
            -position[1] * rect.height);
    }
    
    private void Resort()
    {
        _canvas.sortingOrder = layer * 100 + (int) position.y * 10 + (int) position.x;
    }
}