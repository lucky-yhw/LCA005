using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class GameIcon : MonoBehaviour
{
    public Vector2 position { get; private set; }
    public int layer { get; private set; }
    private int _type;
    public int type { get; private set; }
    public int id { get; private set; }

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Button _button;

    public bool Interactable
    {
        get => _button.interactable;
        set
        {
            _button.interactable = value;
        }
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
        Debug.Log(JsonMapper.ToJson(jsonData));
        id = Utils.GetJsonInt(jsonData["id"]);
        SetPosition(new Vector2(Utils.GetJsonSingle(jsonData["position"][0]),
            Utils.GetJsonSingle(jsonData["position"][1])),false);
        SetLayer(Utils.GetJsonInt(jsonData["position"][2]));
    }


    public void SetPosition(Vector2 pos,bool isResetOrder = true)
    {
        position = pos;
        _rectTransform.anchoredPosition = new Vector2(position[0] * _rectTransform.rect.width,
            -position[1] * _rectTransform.rect.height);
        if (isResetOrder)
        {
            Resort();
        }
    }

    public void SetLayer(int newLayer)
    {
        layer = newLayer;
        Resort();
    }

    private void Resort()
    {
        _canvas.sortingOrder = layer*100+(int)position.y*10+(int)position.x;
    }
}
